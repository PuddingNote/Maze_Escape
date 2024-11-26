using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    [SerializeField] private int mazeWidth = 45;        // �̷��� ���� ũ�� (Ȧ���� ����) ���� : ��, ��-��-�� �̾�� �ϴµ� ��-�� �̸� ���� �߻�
    [SerializeField] private int mazeHeight = 25;       // �̷��� ���� ũ��
    private int[,] maze;                                // �̷� �����͸� �����ϴ� 2D �迭 (0:��, 1:��)

    [Header("UI Settings")]
    [SerializeField] private Transform mazeBoard;       // �̷θ� �׸� �θ� ������Ʈ (Maze Board)
    [SerializeField] private GameObject cellPrefab;     // ���� ��Ÿ�� ������ (SpriteRenderer + BoxCollider2D)

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
        maze[(int)currentCell.x, (int)currentCell.y] = 1;   // ������
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
                // �湮�� ���� ������ ���� ���� ���ư���
                currentCell = stack.Pop();
            }
        }
    }

    // ������ �湮���� ���� ���� ã�� �Լ�
    private Vector2[] GetUnvisitedNeighbors(Vector2 currentCell)
    {
        List<Vector2> neighbors = new List<Vector2>();

        Vector2[] possibleDirections = 
        {
            new Vector2(currentCell.x - 2, currentCell.y),
            new Vector2(currentCell.x + 2, currentCell.y),
            new Vector2(currentCell.x, currentCell.y - 2),
            new Vector2(currentCell.x, currentCell.y + 2)
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
        int midX = (int)(currentCell.x + chosenCell.x) / 2;
        int midY = (int)(currentCell.y + chosenCell.y) / 2;

        maze[midX, midY] = 1;
    }

    // �̷� ���� �� ��ġ
    private void DrawMaze()
    {
        // ȭ�� ũ�� ���
        Camera mainCamera = Camera.main;
        float screenWidthInWorld = mainCamera.orthographicSize * 2 * mainCamera.aspect;
        float screenHeightInWorld = mainCamera.orthographicSize * 2;

        float cellWidth = screenWidthInWorld / mazeWidth;
        float cellHeight = screenHeightInWorld / mazeHeight;

        // �̷��� ���� ��ġ
        Vector3 startPosition = new Vector3
        (
            -screenWidthInWorld / 2 + cellWidth / 2,
            -screenHeightInWorld / 2 + cellHeight / 2,
            0
        );

        // �̷� �׸���
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                // ��
                if (maze[x, y] == 0)
                {
                    GameObject cell = Instantiate(cellPrefab, mazeBoard);

                    cell.transform.position = new Vector3
                    (
                        startPosition.x + x * cellWidth,
                        startPosition.y + y * cellHeight,
                        0
                    );

                    cell.transform.localScale = new Vector3(cellWidth, cellHeight, 1);
                }
            }
        }
    }

}
