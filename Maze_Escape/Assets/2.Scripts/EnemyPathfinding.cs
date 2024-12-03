using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A* 길찾기 알고리즘
public class EnemyPathfinding : MonoBehaviour
{
    public static EnemyPathfinding Instance { get; private set; }   // 싱글톤

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // A* 알고리즘의 각 노드에 대한 정보를 저장
    private class Node
    {
        public Vector2 position;                // 노드의 위치
        public Node parent;                     // 부모 노드
        public float gCost;                     // 시작 지점으로부터의 비용
        public float hCost;                     // 목표 지점으로부터의 추정 비용
        public float fCost => gCost + hCost;    // 총 비용 (gCost + hCost)

        public Node(Vector2 position, Node parent, float gCost, float hCost)
        {
            this.position = position;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
        }
    }

    // A* 알고리즘으로 경로 탐색
    public List<Vector2> FindPath(Vector2 start, Vector2 target)
    {
        List<Vector2> path = new List<Vector2>();

        List<Node> openList = new List<Node>();     // 열린 목록 : 아직 방문하지 않은 노드의 집합
        List<Node> closedList = new List<Node>();   // 닫힌 목록 : 이미 방문했거나 갈 수 없는 노드의 집합

        // 시작 노드
        Node startNode = new Node(start, null, 0, Vector2.Distance(start, target));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestCostNode(openList);
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // 목표에 도달했으면 경로 반환
            if (Vector2.Distance(currentNode.position, target) < 0.1f)
            {
                while (currentNode != null)
                {
                    path.Add(currentNode.position);     // 경로 리스트에 추가
                    currentNode = currentNode.parent;   // 부모 노드로 이동
                }
                path.Reverse(); // 경로 역순으로 정렬

                return path;
            }

            // 현재 노드의 인접 노드를 검사
            foreach (Vector2 neighborPosition in GetNeighbors(currentNode.position))
            {
                // 벽이거나 이미 검사한 노드는 제외
                if (IsWall(neighborPosition) || IsInList(neighborPosition, closedList))
                {
                    continue;
                }

                float gCost = currentNode.gCost + Vector2.Distance(currentNode.position, neighborPosition);
                Node neighborNode = new Node(neighborPosition, currentNode, gCost, Vector2.Distance(neighborPosition, target));

                // 열린 목록에 없는 노드일 경우 추가
                if (!IsInList(neighborPosition, openList))
                {
                    openList.Add(neighborNode);
                }
                else
                {
                    // 열린 목록에 있지만 더 짧은 경로가 있다면, 비용을 갱신
                    Node existingNode = openList.Find(node => node.position == neighborPosition);
                    if (existingNode != null && gCost < existingNode.gCost)
                    {
                        existingNode.gCost = gCost;
                        existingNode.parent = currentNode;
                    }
                }
            }
        }

        return path;
    }

    // 벽인지 확인하는 함수
    private bool IsWall(Vector2 position)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        int mazeValue = MazeGenerator.Instance.GetMazeValue(gridPosition.x, gridPosition.y);
        return mazeValue == 0;
    }

    // 목록에 해당 위치가 존재하는지 확인하는 함수
    private bool IsInList(Vector2 position, List<Node> list)
    {
        return list.Exists(node => node.position == position);
    }

    // 인접한 노드의 위치를 반환하는 함수
    private List<Vector2> GetNeighbors(Vector2 position)
    {
        return new List<Vector2>
        {
            new Vector2(position.x - 1, position.y),
            new Vector2(position.x + 1, position.y),
            new Vector2(position.x, position.y - 1),
            new Vector2(position.x, position.y + 1)
        };
    }

    // 열린 목록에서 비용(fCost)이 가장 낮은 노드를 찾는 함수
    private Node GetLowestCostNode(List<Node> openList)
    {
        Node lowestCostNode = openList[0];
        foreach (Node node in openList)
        {
            if (node.fCost < lowestCostNode.fCost)
                lowestCostNode = node;
        }
        return lowestCostNode;
    }


}
