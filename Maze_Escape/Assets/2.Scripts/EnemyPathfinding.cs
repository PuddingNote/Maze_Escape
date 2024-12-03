using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A* ��ã�� �˰���
public class EnemyPathfinding : MonoBehaviour
{
    public static EnemyPathfinding Instance { get; private set; }   // �̱���

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // A* �˰����� �� ��忡 ���� ������ ����
    private class Node
    {
        public Vector2 position;                // ����� ��ġ
        public Node parent;                     // �θ� ���
        public float gCost;                     // ���� �������κ����� ���
        public float hCost;                     // ��ǥ �������κ����� ���� ���
        public float fCost => gCost + hCost;    // �� ��� (gCost + hCost)

        public Node(Vector2 position, Node parent, float gCost, float hCost)
        {
            this.position = position;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
        }
    }

    // A* �˰������� ��� Ž��
    public List<Vector2> FindPath(Vector2 start, Vector2 target)
    {
        List<Vector2> path = new List<Vector2>();

        List<Node> openList = new List<Node>();     // ���� ��� : ���� �湮���� ���� ����� ����
        List<Node> closedList = new List<Node>();   // ���� ��� : �̹� �湮�߰ų� �� �� ���� ����� ����

        // ���� ���
        Node startNode = new Node(start, null, 0, Vector2.Distance(start, target));
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestCostNode(openList);
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // ��ǥ�� ���������� ��� ��ȯ
            if (Vector2.Distance(currentNode.position, target) < 0.1f)
            {
                while (currentNode != null)
                {
                    path.Add(currentNode.position);     // ��� ����Ʈ�� �߰�
                    currentNode = currentNode.parent;   // �θ� ���� �̵�
                }
                path.Reverse(); // ��� �������� ����

                return path;
            }

            // ���� ����� ���� ��带 �˻�
            foreach (Vector2 neighborPosition in GetNeighbors(currentNode.position))
            {
                // ���̰ų� �̹� �˻��� ���� ����
                if (IsWall(neighborPosition) || IsInList(neighborPosition, closedList))
                {
                    continue;
                }

                float gCost = currentNode.gCost + Vector2.Distance(currentNode.position, neighborPosition);
                Node neighborNode = new Node(neighborPosition, currentNode, gCost, Vector2.Distance(neighborPosition, target));

                // ���� ��Ͽ� ���� ����� ��� �߰�
                if (!IsInList(neighborPosition, openList))
                {
                    openList.Add(neighborNode);
                }
                else
                {
                    // ���� ��Ͽ� ������ �� ª�� ��ΰ� �ִٸ�, ����� ����
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

    // ������ Ȯ���ϴ� �Լ�
    private bool IsWall(Vector2 position)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        int mazeValue = MazeGenerator.Instance.GetMazeValue(gridPosition.x, gridPosition.y);
        return mazeValue == 0;
    }

    // ��Ͽ� �ش� ��ġ�� �����ϴ��� Ȯ���ϴ� �Լ�
    private bool IsInList(Vector2 position, List<Node> list)
    {
        return list.Exists(node => node.position == position);
    }

    // ������ ����� ��ġ�� ��ȯ�ϴ� �Լ�
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

    // ���� ��Ͽ��� ���(fCost)�� ���� ���� ��带 ã�� �Լ�
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
