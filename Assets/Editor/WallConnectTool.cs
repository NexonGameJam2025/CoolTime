using UnityEngine;
using UnityEditor;
using System.Linq;

public class WallConnectorTool : EditorWindow
{
    [MenuItem("Tools/Connect Walls to Nodes")]
    public static void ConnectWallsToNodes()
    {
        var allNodes = GameObject.FindObjectsOfType<TileNode>();
        var allWalls = GameObject.FindObjectsOfType<Transform>()
            .Where(t => t.name.StartsWith("horizontal") || t.name.StartsWith("vertical"))
            .ToArray();

        foreach (var node in allNodes)
        {
            var nodePos = node.transform.position;

            // node._upWall    = FindNearestWall(nodePos + Vector3.up, allWalls);
            // node._downWall  = FindNearestWall(nodePos + Vector3.down, allWalls);
            // node._leftWall  = FindNearestWall(nodePos + Vector3.left, allWalls);
            // node._rightWall = FindNearestWall(nodePos + Vector3.right, allWalls);

            EditorUtility.SetDirty(node); // 변경사항 저장
        }

        Debug.Log("✅ 모든 Node의 벽 연결 완료!");
    }

    private static Wall FindNearestWall(Vector3 targetPos, Transform[] wallTransforms)
    {
        float minDist = float.MaxValue;
        Transform closest = null;

        foreach (var wall in wallTransforms)
        {
            float dist = Vector2.Distance(wall.position, targetPos);
            if (dist < minDist)
            {
                minDist = dist;
                closest = wall;
            }
        }

        // 너무 멀면 연결하지 않음 (예: 0.6 이상)
        return minDist < 0.6f ? closest?.gameObject.GetComponent<Wall>() : null;
    }
}