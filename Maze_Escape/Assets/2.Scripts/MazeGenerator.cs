using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    private int mazeWidth;              // �̷��� ���� ũ�� (Ȧ���� ����) ���� : ��, ��-��-�� �̾�� �ϴµ� ��-�� �̸� ���� �߻�
    private int mazeHeight;             // �̷��� ���� ũ��
    private int[,] maze;                // �̷� �����͸� �����ϴ� 2D �迭 (0:��, 1:��)

    [Header("UI Settings")]
    public Transform mazeBoard;         // �̷θ� �׸� �θ� ������Ʈ (Maze Board)
    public GameObject cellPrefab;       // ���� ��Ÿ�� ������ (UI Image)

    private void Awake()
    {
        mazeWidth = 45;
        mazeHeight = 25;
    }

    private void Start()
    {
        InitializeMaze();
        GenerateMaze();
        DrawMaze();
    }

    // �̷� �ʱ�ȭ
    private void InitializeMaze()
    {
        // 2D �迭 ���� (��� ���� ��(0)���� �ʱ�ȭ)
        maze = new int[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = 0;
            }
        }
    }

    // DFS �˰����� ����Ͽ� �̷� ����
    private void GenerateMaze()
    {
        Stack<Vector2> stack = new Stack<Vector2>();

        // ������ ���� (Ȧ�� ��ǥ ����) ���� : ���� �����ϰ� ���� �����ϴ� �� �ʿ��� ��Ģ���� ������ ������ �� �ֱ� ����
        Vector2 currentCell = new Vector2(1, 1);
        maze[(int)currentCell.x, (int)currentCell.y] = 1; // �������� ��� ����
        stack.Push(currentCell);

        while (stack.Count > 0)
        {
            Vector2[] neighbors = GetUnvisitedNeighbors(currentCell);
            if (neighbors.Length > 0)
            {
                // ������ �湮���� ���� ���� ���� ���, �����ϰ� ����
                Vector2 chosenCell = neighbors[Random.Range(0, neighbors.Length)];

                // ���õ� ���� ���� ���� ���� (���� �����)
                RemoveWall(currentCell, chosenCell);

                // ���õ� ���� ��� �����ϰ� ���ÿ� �߰�
                maze[(int)chosenCell.x, (int)chosenCell.y] = 1;
                stack.Push(chosenCell);
                currentCell = chosenCell;
            }
            else
            {
                // �湮�� ���� ������ ���ÿ��� ���� ���� ���ư���
                currentCell = stack.Pop();
            }
        }
    }

    // ������ �湮���� ���� ���� ã�� �Լ�
    private Vector2[] GetUnvisitedNeighbors(Vector2 currentCell)
    {
        List<Vector2> neighbors = new List<Vector2>();

        // ��, ��, ��, �쿡 �ִ� ������ Ȯ��
        Vector2[] possibleDirections = {
            new Vector2(currentCell.x - 2, currentCell.y), // ����
            new Vector2(currentCell.x + 2, currentCell.y), // ������
            new Vector2(currentCell.x, currentCell.y - 2), // �Ʒ�
            new Vector2(currentCell.x, currentCell.y + 2)  // ��
        };

        foreach (Vector2 dir in possibleDirections)
        {
            // ��輱 ������ ������ �ʵ��� üũ
            if (dir.x > 0 && dir.x < mazeWidth && dir.y > 0 && dir.y < mazeHeight && maze[(int)dir.x, (int)dir.y] == 0)
            {
                neighbors.Add(dir);
            }
        }

        return neighbors.ToArray();
    }

    // ���� ���� ���õ� ���� �����ϴ� �Լ�
    private void RemoveWall(Vector2 currentCell, Vector2 chosenCell)
    {
        int x = (int)currentCell.x;
        int y = (int)currentCell.y;
        int chosenX = (int)chosenCell.x;
        int chosenY = (int)chosenCell.y;

        if (x == chosenX)
        {
            // ���� �������� �� ����
            maze[x, (y + chosenY) / 2] = 1;
        }
        else if (y == chosenY)
        {
            // ���� �������� �� ����
            maze[(x + chosenX) / 2, y] = 1;
        }
    }

    // �̷θ� GameObject�� �׸���
    private void DrawMaze()
    {
        // MazeBoard�� ũ�� ���� ��������
        RectTransform mazeBoardRect = mazeBoard.GetComponent<RectTransform>();
        float mazeBoardWidth = mazeBoardRect.rect.width;
        float mazeBoardHeight = mazeBoardRect.rect.height;

        // �� ���� ũ�⸦ MazeBoard ũ�⿡ �°� ����
        float cellWidth = mazeBoardWidth / mazeWidth;
        float cellHeight = mazeBoardHeight / mazeHeight;

        // ��ġ ����
        Vector3 mazeBoardPosition = mazeBoardRect.localPosition;
        float offsetX = -mazeBoardPosition.x - mazeBoardWidth / 2 + cellWidth / 2;
        float offsetY = -mazeBoardPosition.y + mazeBoardHeight / 2 - cellHeight / 2;

        // �̷θ� �׸���
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                GameObject cell = Instantiate(cellPrefab, mazeBoard);

                // ���� ��ġ ���� (Canvas ��ǥ��� ����)
                RectTransform cellRect = cell.GetComponent<RectTransform>();
                cellRect.localPosition = new Vector3(offsetX + x * cellWidth, offsetY - y * cellHeight, 0);

                // �� ���� ���� (0: �� -> ������, 1: �� -> ���)
                Image cellImage = cell.GetComponent<Image>();
                if (cellImage != null)
                {
                    cellImage.color = (maze[x, y] == 0) ? Color.black : Color.white;
                }

                // �� ũ�� ����
                cellRect.sizeDelta = new Vector2(cellWidth, cellHeight);
            }
        }
    }



}
