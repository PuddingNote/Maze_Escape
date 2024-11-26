using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    [SerializeField] private int mazeWidth = 45;        // 미로의 가로 크기 (홀수로 설정) 이유 : 벽, 벽-길-벽 이어야 하는데 벽-길 이면 문제 발생
    [SerializeField] private int mazeHeight = 25;       // 미로의 세로 크기
    private int[,] maze;                                // 미로 데이터를 저장하는 2D 배열 (0:벽, 1:길)

    [Header("UI Settings")]
    [SerializeField] private Transform mazeBoard;       // 미로를 그릴 부모 오브젝트 (Maze Board)
    [SerializeField] private GameObject cellPrefab;     // 셀을 나타낼 프리팹 (SpriteRenderer + BoxCollider2D)
    [SerializeField] private GameObject exitPrefab;     // 출구 프리팹
    [SerializeField] private GameObject playerPrefab;   // 플레이어 프리팹

    private GameObject exitInstance;                    // 출구 인스턴스
    private GameObject playerInstance;                  // 플레이어 인스턴스

    private void Start()
    {
        InitializeMaze();
        GenerateMaze();
        DrawMaze();
        PlaceExit();
        PlacePlayer();
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
        maze[(int)currentCell.x, (int)currentCell.y] = 1;   // 시작점
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
                // 방문할 셀이 없으면 이전 셀로 돌아가기
                currentCell = stack.Pop();
            }
        }
    }

    // 인접한 방문하지 않은 셀을 찾는 함수
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
        int midX = (int)(currentCell.x + chosenCell.x) / 2;
        int midY = (int)(currentCell.y + chosenCell.y) / 2;

        maze[midX, midY] = 1;
    }

    // 미로 생성 및 배치
    private void DrawMaze()
    {
        // 화면 크기 계산
        Camera mainCamera = Camera.main;
        float screenWidthInWorld = mainCamera.orthographicSize * 2 * mainCamera.aspect;
        float screenHeightInWorld = mainCamera.orthographicSize * 2;

        float cellWidth = screenWidthInWorld / mazeWidth;
        float cellHeight = screenHeightInWorld / mazeHeight;

        // 미로의 시작 위치
        Vector3 startPosition = new Vector3
        (
            -screenWidthInWorld / 2 + cellWidth / 2,
            -screenHeightInWorld / 2 + cellHeight / 2,
            0
        );

        // 미로 그리기
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                // 벽
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

    // 출구 배치
    private void PlaceExit()
    {
        int exitX = mazeWidth / 2;
        int exitY = mazeHeight - 2; // 최상단에 출구 배치

        if (maze[exitX, exitY] == 1) // 길이 확인
        {
            Vector3 exitPosition = GetWorldPosition(exitX, exitY);
            exitPosition.y -= 0.05f; // 일단 UI상으로 딱 맞는 위치 조정

            exitInstance = Instantiate(exitPrefab, exitPosition, Quaternion.identity, mazeBoard);
        }
    }

    // 플레이어 배치
    private void PlacePlayer()
    {
        Vector2Int playerPosition;

        do
        {
            int randomX = Random.Range(1, mazeWidth - 1);
            int randomY = Random.Range(1, mazeHeight - 1);

            if (maze[randomX, randomY] == 1) // 길인지 확인
            {
                playerPosition = new Vector2Int(randomX, randomY);

                // 출구와 거리가 충분히 멀리 떨어져 있는지 확인
                if (Vector2.Distance(playerPosition, new Vector2(mazeWidth / 2, mazeHeight - 2)) > mazeWidth / 2)
                {
                    break;
                }
            }
        } while (true);

        Vector3 playerWorldPosition = GetWorldPosition(playerPosition.x, playerPosition.y);
        playerInstance = Instantiate(playerPrefab, playerWorldPosition, Quaternion.identity);
    }

    // 월드 좌표 변환
    private Vector3 GetWorldPosition(int x, int y)
    {
        Camera mainCamera = Camera.main;
        float screenWidthInWorld = mainCamera.orthographicSize * 2 * mainCamera.aspect;
        float screenHeightInWorld = mainCamera.orthographicSize * 2;

        float cellWidth = screenWidthInWorld / mazeWidth;
        float cellHeight = screenHeightInWorld / mazeHeight;

        float startX = -screenWidthInWorld / 2 + cellWidth / 2;
        float startY = -screenHeightInWorld / 2 + cellHeight / 2;

        return new Vector3(startX + x * cellWidth, startY + y * cellHeight, 0);
    }

}
