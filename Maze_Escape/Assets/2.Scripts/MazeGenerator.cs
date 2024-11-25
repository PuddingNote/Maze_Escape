using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    private int mazeWidth;              // 미로의 가로 크기 (홀수로 설정) 이유 : 벽, 벽-길-벽 이어야 하는데 벽-길 이면 문제 발생
    private int mazeHeight;             // 미로의 세로 크기
    private int[,] maze;                // 미로 데이터를 저장하는 2D 배열 (0:벽, 1:길)

    [Header("UI Settings")]
    public Transform mazeBoard;         // 미로를 그릴 부모 오브젝트 (Maze Board)
    public GameObject cellPrefab;       // 셀을 나타낼 프리팹 (UI Image)

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

    // 미로 초기화
    private void InitializeMaze()
    {
        // 2D 배열 생성 (모든 셀을 벽(0)으로 초기화)
        maze = new int[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = 0;
            }
        }
    }

    // DFS 알고리즘을 사용하여 미로 생성
    private void GenerateMaze()
    {
        Stack<Vector2> stack = new Stack<Vector2>();

        // 시작점 설정 (홀수 좌표 시작) 이유 : 벽을 제거하고 길을 연결하는 데 필요한 규칙적인 간격을 유지할 수 있기 때문
        Vector2 currentCell = new Vector2(1, 1);
        maze[(int)currentCell.x, (int)currentCell.y] = 1; // 시작점을 길로 설정
        stack.Push(currentCell);

        while (stack.Count > 0)
        {
            Vector2[] neighbors = GetUnvisitedNeighbors(currentCell);
            if (neighbors.Length > 0)
            {
                // 인접한 방문하지 않은 셀이 있을 경우, 랜덤하게 선택
                Vector2 chosenCell = neighbors[Random.Range(0, neighbors.Length)];

                // 선택된 셀과 현재 셀을 연결 (길을 만든다)
                RemoveWall(currentCell, chosenCell);

                // 선택된 셀을 길로 설정하고 스택에 추가
                maze[(int)chosenCell.x, (int)chosenCell.y] = 1;
                stack.Push(chosenCell);
                currentCell = chosenCell;
            }
            else
            {
                // 방문할 셀이 없으면 스택에서 이전 셀로 돌아간다
                currentCell = stack.Pop();
            }
        }
    }

    // 인접한 방문하지 않은 셀을 찾는 함수
    private Vector2[] GetUnvisitedNeighbors(Vector2 currentCell)
    {
        List<Vector2> neighbors = new List<Vector2>();

        // 상, 하, 좌, 우에 있는 셀들을 확인
        Vector2[] possibleDirections = {
            new Vector2(currentCell.x - 2, currentCell.y), // 왼쪽
            new Vector2(currentCell.x + 2, currentCell.y), // 오른쪽
            new Vector2(currentCell.x, currentCell.y - 2), // 아래
            new Vector2(currentCell.x, currentCell.y + 2)  // 위
        };

        foreach (Vector2 dir in possibleDirections)
        {
            // 경계선 밖으로 나가지 않도록 체크
            if (dir.x > 0 && dir.x < mazeWidth && dir.y > 0 && dir.y < mazeHeight && maze[(int)dir.x, (int)dir.y] == 0)
            {
                neighbors.Add(dir);
            }
        }

        return neighbors.ToArray();
    }

    // 현재 셀과 선택된 셀을 연결하는 함수
    private void RemoveWall(Vector2 currentCell, Vector2 chosenCell)
    {
        int x = (int)currentCell.x;
        int y = (int)currentCell.y;
        int chosenX = (int)chosenCell.x;
        int chosenY = (int)chosenCell.y;

        if (x == chosenX)
        {
            // 세로 방향으로 벽 제거
            maze[x, (y + chosenY) / 2] = 1;
        }
        else if (y == chosenY)
        {
            // 가로 방향으로 벽 제거
            maze[(x + chosenX) / 2, y] = 1;
        }
    }

    // 미로를 GameObject로 그리기
    private void DrawMaze()
    {
        // MazeBoard의 크기 정보 가져오기
        RectTransform mazeBoardRect = mazeBoard.GetComponent<RectTransform>();
        float mazeBoardWidth = mazeBoardRect.rect.width;
        float mazeBoardHeight = mazeBoardRect.rect.height;

        // 각 셀의 크기를 MazeBoard 크기에 맞게 조정
        float cellWidth = mazeBoardWidth / mazeWidth;
        float cellHeight = mazeBoardHeight / mazeHeight;

        // 위치 조정
        Vector3 mazeBoardPosition = mazeBoardRect.localPosition;
        float offsetX = -mazeBoardPosition.x - mazeBoardWidth / 2 + cellWidth / 2;
        float offsetY = -mazeBoardPosition.y + mazeBoardHeight / 2 - cellHeight / 2;

        // 미로를 그리기
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                GameObject cell = Instantiate(cellPrefab, mazeBoard);

                // 셀의 위치 설정 (Canvas 좌표계로 조정)
                RectTransform cellRect = cell.GetComponent<RectTransform>();
                cellRect.localPosition = new Vector3(offsetX + x * cellWidth, offsetY - y * cellHeight, 0);

                // 셀 색상 설정 (0: 벽 -> 검정색, 1: 길 -> 흰색)
                Image cellImage = cell.GetComponent<Image>();
                if (cellImage != null)
                {
                    cellImage.color = (maze[x, y] == 0) ? Color.black : Color.white;
                }

                // 셀 크기 설정
                cellRect.sizeDelta = new Vector2(cellWidth, cellHeight);
            }
        }
    }



}
