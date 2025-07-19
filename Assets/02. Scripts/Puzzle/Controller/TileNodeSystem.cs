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

            bool isBoundary = currentNode.TileState == ETileState.Melt || currentNode.TileState == ETileState.Destroy || currentNode.IsCurrentConstructing;

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
                if (currentNode.TileState == ETileState.Melt || currentNode.TileState == ETileState.Destroy || currentNode.IsCurrentConstructing) continue;
            }
            currentSegment.Add(currentNode);
        }
        if (currentSegment.Count > 0) lineActions.AddRange(CalculateSegmentActions(currentSegment, key));
        return lineActions;
    }

    // TileNodeSystem.cs 에서 CalculateSegmentActions 함수만 교체하세요.

    private List<ManaAction> CalculateSegmentActions(List<TileNode> segment, InputKey key)
    {
        var segmentActions = new List<ManaAction>();
        var manaNodesInSegment = segment.Where(node => node.OnMana).ToList();

        if (manaNodesInSegment.Count == 0) return segmentActions;

        // 이동 방향에 맞춰 마나들을 정렬 (Right/Down이면 오른쪽/아래쪽 마나부터 처리)
        if (key == InputKey.Right || key == InputKey.Down) manaNodesInSegment.Reverse();

        // 타일 노드를 키로 사용하여, 해당 위치에 최종적으로 어떤 마나가 위치할지 계획을 저장
        var plannedPositions = new Dictionary<TileNode, Mana>();

        foreach (var startNode in manaNodesInSegment)
        {
            Mana currentMana = startNode.CurrentMana;
            TileNode destination = startNode;
            bool willMerge = false;

            int startIndex = segment.IndexOf(startNode);
            int direction = (key == InputKey.Right || key == InputKey.Down) ? 1 : -1;

            // 경로 탐색 루프
            for (int i = startIndex + direction; i >= 0 && i < segment.Count; i += direction)
            {
                TileNode pathNode = segment[i];

                // 1. 경로에서 '건설 완료' 건물을 만나면 그곳이 최종 목적지 (최우선)
                if (pathNode.OnBuilding)
                {
                    destination = pathNode;
                    break;
                }

                // 2. 경로에서 다른 마나의 예정지를 만나면 그 앞에서 멈춤
                if (plannedPositions.ContainsKey(pathNode))
                {
                    Mana occupyingMana = plannedPositions[pathNode];

                    // 합치기 조건 체크
                    if (occupyingMana.ManaLevel == currentMana.ManaLevel && occupyingMana.ManaLevel < 3 &&
                        !segmentActions.Exists(a => a.Sacrifice == occupyingMana))
                    {
                        destination = pathNode;
                        willMerge = true;
                    }

                    break; // 합칠 수 있든 없든, 더 이상 진행하지 않으므로 루프 탈출
                }

                destination = pathNode; // 이동 가능한 빈 칸이면 목적지 업데이트
            }

            // 3. 계산된 최종 목적지를 바탕으로 액션 생성
            if (willMerge)
            {
                var survivorAction = segmentActions.Find(a => a.Survivor == plannedPositions[destination]);
                if (survivorAction != null)
                {
                    survivorAction.Sacrifice = currentMana;
                    survivorAction.SacrificeStartNode = startNode;
                }
            }
            else
            {
                segmentActions.Add(new ManaAction { Survivor = currentMana, StartNode = startNode, Destination = destination });
                plannedPositions[destination] = currentMana;
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

                action.Survivor.transform.position = Vector3.Lerp(action.StartPosition, action.Destination.transform.position, t);

                if (action.Sacrifice != null)
                {
                    action.Sacrifice.transform.position = Vector3.Lerp(action.SacrificeStartPosition, action.Destination.transform.position, t);
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

            if (action.Destination.OnBuilding)
            {
                int finalLevel = action.Survivor.ManaLevel;
                if (action.Sacrifice != null) finalLevel++;

                action.Destination.CurrentBuilding.OnCollisionMana((EManaLevel)(finalLevel - 1));

                Destroy(action.Survivor.gameObject);
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