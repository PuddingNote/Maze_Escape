using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance { get; private set; } // 싱글톤

    [Header("Maze Settings")]
    [SerializeField] private int mazeWidth;             // 미로의 가로 크기 (홀수로 설정) 이유 : 벽, 벽-길-벽 이어야 하는데 벽-길 이면 문제 발생
    [SerializeField] private int mazeHeight;            // 미로의 세로 크기
    private int[,] maze;                                // 미로 데이터를 저장하는 2D 배열 (0:벽, 1:길)
    private float minDistanceFromExit;                  // 출구와의 최소 거리 (플레이어,적 배치를 위해)
    private float maxDistanceFromExit;                  // 출구와의 최대 거리

    [Header("UI Settings")]
    [SerializeField] private Transform mazeBoard;       // 미로를 그릴 부모 오브젝트 (Maze Board)
    [SerializeField] private GameObject cellPrefab;     // 셀을 나타낼 프리팹 (SpriteRenderer + BoxCollider2D)
    [SerializeField] private GameObject exitPrefab;     // 출구 프리팹
    [SerializeField] private GameObject playerPrefab;   // 플레이어 프리팹
    [SerializeField] private GameObject enemyPrefab;    // 적 프리팹
    private float mazeWidthSizeInWorld;                 // 미로 가로 사이즈 (World)
    private float mazeHeightSizeInWorld;                // 미로 세로 사이즈 (World)

    private GameObject exitInstance;                    // 출구 인스턴스
    private GameObject playerInstance;                  // 플레이어 인스턴스
    private GameObject enemyInstance;                   // 적 인스턴스

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        mazeWidth = 45;
        mazeHeight = 25;
        minDistanceFromExit = 23f;
        maxDistanceFromExit = 27f;
        mazeWidthSizeInWorld = 14.4f;
        mazeHeightSizeInWorld = 8.1f;
    }

    // 미로 생성
    public void StartMazeGenerate()
    {
        InitializeMaze();
        GenerateMaze();
        RenderMaze();
        SpawnExit();
        SpawnPlayer();
        SpawnEnemy();
    }

    // 다음 미로 생성
    public void NextMazeGenerate()
    {
        // 기존에 생성된 미로와 객체들 삭제
        foreach (Transform child in mazeBoard)
        {
            Destroy(child.gameObject);
        }
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }
        if (enemyInstance != null)
        {
            Destroy(enemyInstance);
        }

        StartMazeGenerate();
    }

    // 미로 초기화 (모든 셀을 벽(0)으로 초기화)
    private void InitializeMaze()
    {
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

        // 출구 위치를 시작점으로 설정
        int exitX = mazeWidth / 2 - 1;
        int exitY = mazeHeight - 2;

        // 출구위치는 무조건 길로 설정
        Vector2 currentCell = new Vector2(exitX, exitY);
        maze[(int)currentCell.x, (int)currentCell.y] = 1;

        stack.Push(currentCell);

        while (stack.Count > 0)
        {
            Vector2[] neighbors = GetUnvisitedNeighbors(currentCell);

            if (neighbors.Length > 0)
            {
                // 인접한 방문하지 않은 셀이 있을 경우, 랜덤하게 선택
                Vector2 chosenCell = neighbors[Random.Range(0, neighbors.Length)];

                // 선택된 셀과 현재 셀을 연결 (길 만들기)
                CarvePathBetweenCells(currentCell, chosenCell);

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
            if (dir.x > 0 && dir.x < mazeWidth && dir.y > 0 && dir.y < mazeHeight && maze[(int)dir.x, (int)dir.y] == 0)
            {
                neighbors.Add(dir);
            }
        }

        return neighbors.ToArray();
    }

    // 두 셀을 연결해서 길을 만드는 함수
    private void CarvePathBetweenCells(Vector2 currentCell, Vector2 chosenCell)
    {
        int midX = (int)(currentCell.x + chosenCell.x) / 2;
        int midY = (int)(currentCell.y + chosenCell.y) / 2;

        maze[midX, midY] = 1;
    }

    // 미로를 화면에 그리는 함수
    private void RenderMaze()
    {
        // 1600x900 크기 설정
        float screenWidthInWorld = mazeWidthSizeInWorld;
        float screenHeightInWorld = mazeHeightSizeInWorld;

        float cellWidth = screenWidthInWorld / mazeWidth;
        float cellHeight = screenHeightInWorld / mazeHeight;

        // 미로의 시작 위치
        Vector3 startPosition = new Vector3(-screenWidthInWorld / 2 + cellWidth / 2, -screenHeightInWorld / 2 + cellHeight / 2, 0);

        // 미로 그리기
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                // 벽일 경우만 그리기
                if (maze[x, y] == 0)
                {
                    GameObject cell = Instantiate(cellPrefab, mazeBoard);

                    cell.transform.position = new Vector3(startPosition.x + x * cellWidth, startPosition.y + y * cellHeight, 0);
                    cell.transform.localScale = new Vector3(cellWidth, cellHeight, 1);
                }
            }
        }
    }

    // 출구를 배치하는 함수
    private void SpawnExit()
    {
        int exitX = mazeWidth / 2 - 1;
        int exitY = mazeHeight - 2;

        Vector3 exitPosition = GetWorldPosition(exitX, exitY);
        exitPosition.y -= 0.05f;

        exitInstance = Instantiate(exitPrefab, exitPosition, Quaternion.identity, mazeBoard);
    }

    // 플레이어를 배치하는 함수
    private void SpawnPlayer()
    {
        Vector2Int playerPosition;

        do
        {
            int randomX = Random.Range(1, mazeWidth - 1);
            int randomY = Random.Range(1, mazeHeight - 1);

            if (maze[randomX, randomY] == 1)
            {
                playerPosition = new Vector2Int(randomX, randomY);

                // 출구와의 거리제한 조건 (출구와 거리가 너무 가까워도 안되고, 너무 멀어도 안됨)
                float distanceFromExit = Vector2.Distance(playerPosition, new Vector2(mazeWidth / 2, mazeHeight - 2));
                if (distanceFromExit > minDistanceFromExit && distanceFromExit < maxDistanceFromExit)
                {
                    break;
                }
            }
        } while (true);

        Vector3 playerWorldPosition = GetWorldPosition(playerPosition.x, playerPosition.y);
        playerInstance = Instantiate(playerPrefab, playerWorldPosition, Quaternion.identity);

        GameManager.Instance.SetPlayer(playerInstance);
    }

    // 적을 배치하는 함수
    private void SpawnEnemy()
    {
        Vector2Int enemyPosition;

        do
        {
            int randomX = Random.Range(1, mazeWidth - 1);
            int randomY = Random.Range(1, mazeHeight - 1);

            if (maze[randomX, randomY] == 1)
            {
                enemyPosition = new Vector2Int(randomX, randomY);

                // 출구와의 거리제한 조건 (출구와 거리가 너무 가까워도 안되고, 너무 멀어도 안됨)
                float distanceFromExit = Vector2.Distance(enemyPosition, new Vector2(mazeWidth / 2, mazeHeight - 2));
                if (distanceFromExit > minDistanceFromExit && distanceFromExit < maxDistanceFromExit)
                {
                    break;
                }
            }
        } while (true);

        Vector3 enemyWorldPosition = GetWorldPosition(enemyPosition.x, enemyPosition.y);
        enemyInstance = Instantiate(enemyPrefab, enemyWorldPosition, Quaternion.identity);

        GameManager.Instance.SetEnemy(enemyInstance);
    }

    // 월드 좌표로 변환하는 함수
    public Vector3 GetWorldPosition(int x, int y)
    {
        float screenWidthInWorld = mazeWidthSizeInWorld;
        float screenHeightInWorld = mazeHeightSizeInWorld;

        float cellWidth = screenWidthInWorld / mazeWidth;
        float cellHeight = screenHeightInWorld / mazeHeight;

        float startX = -screenWidthInWorld / 2 + cellWidth / 2;
        float startY = -screenHeightInWorld / 2 + cellHeight / 2;

        return new Vector3(startX + x * cellWidth, startY + y * cellHeight, 0);
    }

    // 월드 좌표를 그리드 좌표로 변환하는 함수
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        float screenWidthInWorld = mazeWidthSizeInWorld;
        float screenHeightInWorld = mazeHeightSizeInWorld;

        float cellWidth = screenWidthInWorld / mazeWidth;
        float cellHeight = screenHeightInWorld / mazeHeight;

        int gridX = Mathf.FloorToInt((worldPosition.x + screenWidthInWorld / 2) / cellWidth);
        int gridY = Mathf.FloorToInt((worldPosition.y + screenHeightInWorld / 2) / cellHeight);

        return new Vector2Int(gridX, gridY);
    }

    // 출구의 월드 좌표를 반환하는 함수
    public Vector2 GetExitPosition()
    {
        int exitX = mazeWidth / 2 - 1;
        int exitY = mazeHeight - 2;

        Vector3 exitWorldPosition = GetWorldPosition(exitX, exitY);

        return exitWorldPosition;
    }

    // 주어진 좌표의 미로 값을 반환하는 함수
    public int GetMazeValue(int x, int y)
    {
        if (x < 0 || x >= mazeWidth || y < 0 || y >= mazeHeight)
        {
            return 0; // 벽
        }

        return maze[x, y];
    }


}
