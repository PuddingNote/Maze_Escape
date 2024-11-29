using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed;       // 적의 이동속도
    private Vector2 targetPosition;                 // 적의 목표 위치 (출구)
    private List<Vector2> path;                     // 적의 경로
    private int currentPathIndex;                   // 현재 경로 인덱스

    private void Awake()
    {
        moveSpeed = 0.5f;
        path = new List<Vector2>();
        currentPathIndex = 0;
    }

    private void Start()
    {
        // 출구의 위치를 목표로 설정
        targetPosition = MazeGenerator.Instance.GetGridPosition(MazeGenerator.Instance.GetExitPosition());

        // A* 알고리즘으로 출구까지의 경로 계산
        Vector2 startGridPosition = MazeGenerator.Instance.GetGridPosition(transform.position);
        path = EnemyPathfinding.Instance.FindPath(startGridPosition, targetPosition);

        if (path.Count > 0)
        {
            StartCoroutine(FollowPath());
        }
        else
        {
            Debug.LogError("경로가 없음");
        }
    }

    // 경로를 따라 이동하는 코루틴
    private IEnumerator FollowPath()
    {
        while (currentPathIndex < path.Count)
        {
            Vector2 currentTarget = path[currentPathIndex]; // 목표 지점
            Vector3 worldTarget = MazeGenerator.Instance.GetWorldPosition((int)currentTarget.x, (int)currentTarget.y);  // 월드 좌표로 변환

            // 목표 지점까지 이동
            while ((Vector2)transform.position != (Vector2)worldTarget)
            {
                transform.position = Vector2.MoveTowards(transform.position, worldTarget, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentPathIndex++;
        }
    }


}
