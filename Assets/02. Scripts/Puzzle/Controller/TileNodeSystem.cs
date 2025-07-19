using System.Collections;
using System.Collections.Generic;
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
            bool isBoundary = currentNode.TileState == ETileState.Melt || currentNode.TileState == ETileState.Destroy;
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
                if (currentNode.TileState == ETileState.Melt || currentNode.TileState == ETileState.Destroy) continue;
            }
            currentSegment.Add(currentNode);
        }
        if (currentSegment.Count > 0) lineActions.AddRange(CalculateSegmentActions(currentSegment, key));
        return lineActions;
    }

    private List<ManaAction> CalculateSegmentActions(List<TileNode> segment, InputKey key)
    {
        var segmentActions = new List<ManaAction>();
        List<TileNode> manaNodesInSegment = new List<TileNode>();
        foreach (var node in segment)
        {
            if (node.OnMana) manaNodesInSegment.Add(node);
        }

        if (manaNodesInSegment.Count == 0) return segmentActions;

        if (key == InputKey.Right || key == InputKey.Down) manaNodesInSegment.Reverse();

        List<Mana> mergedSurvivors = new List<Mana>();
        for (int i = 0; i < manaNodesInSegment.Count; i++)
        {
            TileNode survivorNode = manaNodesInSegment[i];
            if (i + 1 < manaNodesInSegment.Count && survivorNode.CurrentMana.ManaLevel == manaNodesInSegment[i + 1].CurrentMana.ManaLevel && survivorNode.CurrentMana.ManaLevel < 3)
            {
                TileNode sacrificeNode = manaNodesInSegment[i + 1];
                segmentActions.Add(new ManaAction { Survivor = survivorNode.CurrentMana, StartNode = survivorNode, Sacrifice = sacrificeNode.CurrentMana, SacrificeStartNode = sacrificeNode });
                mergedSurvivors.Add(survivorNode.CurrentMana);
                i++;
            }
            else
            {
                segmentActions.Add(new ManaAction { Survivor = survivorNode.CurrentMana, StartNode = survivorNode });
                mergedSurvivors.Add(survivorNode.CurrentMana);
            }
        }

        if (key == InputKey.Right || key == InputKey.Down)
        {
            int protectedIndex = segment.Count - 1;
            for (int i = 0; i < mergedSurvivors.Count; i++)
            {
                var action = segmentActions.Find(a => a.Survivor == mergedSurvivors[i]);
                if (action != null) action.Destination = segment[protectedIndex - i];
            }
        }
        else
        {
            int protectedIndex = 0;
            for (int i = 0; i < mergedSurvivors.Count; i++)
            {
                var action = segmentActions.Find(a => a.Survivor == mergedSurvivors[i]);
                if (action != null) action.Destination = segment[protectedIndex + i];
            }
        }
        return segmentActions;
    }

    private IEnumerator AnimateMoves(List<ManaAction> actionPlan)
    {
        float elapsedTime = 0f;

        foreach (var action in actionPlan)
        {
            action.StartPosition = action.Survivor.transform.position;
            action.StartNode.ClearMana();
            if (action.Sacrifice != null)
            {
                action.SacrificeStartPosition = action.Sacrifice.transform.position;
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

                Vector3 targetPos = action.Destination.transform.position;
                action.Survivor.transform.position = Vector3.Lerp(action.StartPosition, targetPos, t);

                if (action.Sacrifice != null)
                {
                    action.Sacrifice.transform.position = Vector3.Lerp(action.SacrificeStartPosition, targetPos, t);
                }
            }
            yield return null;
        }
    }

    private void FinalizeState(List<ManaAction> actionPlan)
    {
        foreach (var action in actionPlan)
        {
            if (action.Destination == null) continue;

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