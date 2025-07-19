using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileNodeSystem : MonoBehaviour
{
    [SerializeField] private TileNode[] _tileNodes;
    private TileNode[,] _tileNodeGrid = new TileNode[7, 7];
    public TileNode[,] TileNodeGrid => _tileNodeGrid;
    private const int GridSize = 7;
    private bool _isMoving = false;

    [Header("Animation Settings")]
    [SerializeField] private float _manaMoveDuration = 0.15f;

    private class ManaAction
    {
        public Mana Survivor;
        public TileNode StartNode;
        public Mana Sacrifice;
        public TileNode SacrificeStartNode;
        public TileNode Destination;
        public Vector3 StartPosition;
        public Vector3 SacrificeStartPosition;
    }

    private void Awake()
    {
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                _tileNodeGrid[y, x] = _tileNodes[GridSize * y + x];
            }
        }
    }

    public void Action(InputKey inputKey)
    {
        if (_isMoving) return;
        StartCoroutine(ActionCoroutine(inputKey));
    }

    private IEnumerator ActionCoroutine(InputKey inputKey)
    {
        _isMoving = true;

        List<ManaAction> finalActionPlan = CalculateActionPlan(inputKey);

        bool hasMoved = finalActionPlan.Exists(action => action.StartNode != action.Destination || action.Sacrifice != null);

        if (hasMoved)
        {
            yield return AnimateMoves(finalActionPlan);
            FinalizeState(finalActionPlan);
        }

        _isMoving = false;
    }

    private List<ManaAction> CalculateActionPlan(InputKey inputKey)
    {
        var finalPlan = new List<ManaAction>();
        if (inputKey == InputKey.Left || inputKey == InputKey.Right)
        {
            for (int y = 0; y < GridSize; y++)
            {
                List<TileNode> line = new List<TileNode>();
                for (int x = 0; x < GridSize; x++) line.Add(_tileNodeGrid[y, x]);
                finalPlan.AddRange(CalculateLineActions(line, inputKey));
            }
        }
        else if (inputKey == InputKey.Up || inputKey == InputKey.Down)
        {
            for (int x = 0; x < GridSize; x++)
            {
                List<TileNode> line = new List<TileNode>();
                for (int y = 0; y < GridSize; y++) line.Add(_tileNodeGrid[y, x]);
                finalPlan.AddRange(CalculateLineActions(line, inputKey));
            }
        }
        return finalPlan;
    }

    private List<ManaAction> CalculateLineActions(List<TileNode> line, InputKey key)
    {
        var lineActions = new List<ManaAction>();
        var currentSegment = new List<TileNode>();
        for (int i = 0; i < line.Count; i++)
        {
            TileNode currentNode = line[i];

            bool isBoundary = currentNode.TileState == ETileState.Destroy || currentNode.IsCurrentConstructing;

            if (i > 0 && !isBoundary)
            {
                TileNode previousNode = line[i - 1];
                if (key == InputKey.Left || key == InputKey.Right) { if (previousNode.IsRightWallEnable || currentNode.IsLeftWallEnable) isBoundary = true; }
                else if (key == InputKey.Up || key == InputKey.Down) { if (previousNode.IsDownWallEnable || currentNode.IsUpWallEnable) isBoundary = true; }
            }
            if (isBoundary)
            {
                if (currentSegment.Count > 0) lineActions.AddRange(CalculateSegmentActions(currentSegment, key));
                currentSegment = new List<TileNode>();
                if (currentNode.TileState == ETileState.Destroy || currentNode.IsCurrentConstructing) continue;
            }
            currentSegment.Add(currentNode);
        }
        if (currentSegment.Count > 0) lineActions.AddRange(CalculateSegmentActions(currentSegment, key));
        return lineActions;
    }

    private List<ManaAction> CalculateSegmentActions(List<TileNode> segment, InputKey key)
    {
        var segmentActions = new List<ManaAction>();
        var manaNodesInSegment = segment.Where(node => node.OnMana).ToList();

        if (manaNodesInSegment.Count == 0) return segmentActions;

        if (key == InputKey.Right || key == InputKey.Down) manaNodesInSegment.Reverse();

        List<TileNode> survivors = new List<TileNode>();
        for (int i = 0; i < manaNodesInSegment.Count; i++)
        {
            TileNode survivorNode = manaNodesInSegment[i];

            if (i + 1 < manaNodesInSegment.Count &&
                survivorNode.CurrentMana.ManaLevel == manaNodesInSegment[i + 1].CurrentMana.ManaLevel &&
                survivorNode.CurrentMana.ManaLevel < 3)
            {
                TileNode sacrificeNode = manaNodesInSegment[i + 1];
                segmentActions.Add(new ManaAction
                {
                    Survivor = survivorNode.CurrentMana,
                    StartNode = survivorNode,
                    Sacrifice = sacrificeNode.CurrentMana,
                    SacrificeStartNode = sacrificeNode
                });
                survivors.Add(survivorNode);
                i++;
            }
            else
            {
                segmentActions.Add(new ManaAction
                {
                    Survivor = survivorNode.CurrentMana,
                    StartNode = survivorNode
                });
                survivors.Add(survivorNode);
            }
        }

        if (key == InputKey.Right || key == InputKey.Down)
        {
            int placementIndex = segment.Count - 1;
            for (int i = 0; i < survivors.Count; i++)
            {
                TileNode survivorStartNode = survivors[i];
                TileNode destinationNode = segment[placementIndex - i];

                int start = segment.IndexOf(survivorStartNode);
                int end = segment.IndexOf(destinationNode);

                for (int j = start; j <= end; j++)
                {
                    if (segment[j].OnBuilding)
                    {
                        destinationNode = segment[j];
                        break;
                    }
                }

                var action = segmentActions.Find(a => a.StartNode == survivorStartNode);
                if (action != null) action.Destination = destinationNode;
            }
        }
        else
        {
            int placementIndex = 0;
            for (int i = 0; i < survivors.Count; i++)
            {
                TileNode survivorStartNode = survivors[i];
                TileNode destinationNode = segment[placementIndex + i];

                int start = segment.IndexOf(survivorStartNode);
                int end = segment.IndexOf(destinationNode);

                for (int j = start; j >= end; j--)
                {
                    if (segment[j].OnBuilding)
                    {
                        destinationNode = segment[j];
                        break;
                    }
                }
                var action = segmentActions.Find(a => a.StartNode == survivorStartNode);
                if (action != null) action.Destination = destinationNode;
            }
        }
        return segmentActions;
    }

    private IEnumerator AnimateMoves(List<ManaAction> actionPlan)
    {
        float elapsedTime = 0f;

        foreach (var action in actionPlan)
        {
            if (action.StartNode != null)
            {
                action.StartPosition = action.StartNode.transform.position;
                action.StartNode.ClearMana();
            }
            if (action.SacrificeStartNode != null)
            {
                action.SacrificeStartPosition = action.SacrificeStartNode.transform.position;
                action.SacrificeStartNode.ClearMana();
            }
        }

        while (elapsedTime < _manaMoveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _manaMoveDuration);
            foreach (var action in actionPlan)
            {
                if (action.Destination == null) continue;

                if (action.Survivor != null)
                    action.Survivor.transform.position = Vector3.Lerp(action.StartPosition, action.Destination.transform.position, t);

                if (action.Sacrifice != null)
                    action.Sacrifice.transform.position = Vector3.Lerp(action.SacrificeStartPosition, action.Destination.transform.position, t);
            }
            yield return null;
        }
    }

    private void FinalizeState(List<ManaAction> actionPlan)
    {
        foreach (var action in actionPlan)
        {
            if (action.Destination == null) continue;

            if (action.Destination.OnBuilding)
            {
                int finalLevel = action.Survivor.ManaLevel;
                if (action.Sacrifice != null) finalLevel++;

                action.Destination.CurrentBuilding.OnCollisionMana((EManaLevel)(finalLevel - 1));

                if (action.Survivor != null) Destroy(action.Survivor.gameObject);
                if (action.Sacrifice != null) Destroy(action.Sacrifice.gameObject);
            }
            else
            {
                if (action.Sacrifice != null)
                {
                    action.Survivor.SetLevel(action.Survivor.ManaLevel + 1);
                    action.Destination.ReceiveMana(action.Survivor);
                    Destroy(action.Sacrifice.gameObject);
                }
                else
                {
                    action.Destination.ReceiveMana(action.Survivor);
                }
            }
        }
    }
}