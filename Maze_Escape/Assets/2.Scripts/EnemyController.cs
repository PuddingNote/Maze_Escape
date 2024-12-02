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
    private LineRenderer lineRenderer;              // 라인 렌더러

    private void Awake()
    {
        moveSpeed = 0.5f;
        path = new List<Vector2>();
        currentPathIndex = 0;

        // 라인 렌더러 초기화
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.enabled = false;
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

        // 스테이지에 맞게 이동속도 업데이트
        if (GameManager.Instance != null)
        {
            UpdateMoveSpeed(GameManager.Instance.GetCurrentStage());
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

            UpdatePathLine();
        }
    }

    // 스테이지마다 이동속도 0.2f씩 증가
    public void UpdateMoveSpeed(int stage)
    {
        moveSpeed = 0.5f + (stage - 1) * 0.2f;
    }

    // 경로 거리 가져오기
    public int GetPathDistance()
    {
        return path != null ? path.Count : 0;
    }

    // 남은 경로 거리 계산
    public int GetRemainingPathDistance()
    {
        if (path == null || currentPathIndex >= path.Count)
            return 0;

        // 현재 위치부터 출구까지 남은 경로 길이
        return path.Count - currentPathIndex;
    }

    // 남은 경로를 라인으로 업데이트
    public void UpdatePathLine()
    {
        if (path == null || path.Count == 0 || currentPathIndex >= path.Count)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        // 남은 경로만 라인 렌더러에 적용
        int remainingCount = path.Count - currentPathIndex;
        lineRenderer.positionCount = remainingCount;

        for (int i = 0; i < remainingCount; i++)
        {
            Vector2 gridPosition = path[currentPathIndex + i];
            Vector3 worldPosition = MazeGenerator.Instance.GetWorldPosition((int)gridPosition.x, (int)gridPosition.y);
            lineRenderer.SetPosition(i, worldPosition);
        }
    }

    // 경로 보여주기
    public void ShowPath()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
        }
    }

    // 적의 라인이 줄어드는 애니메이션 시작
    public void AnimatePathReduction(System.Action onPathAnimationComplete)
    {
        StartCoroutine(PathReductionCoroutine(onPathAnimationComplete));
    }

    // 라인이 점점 줄어드는 코루틴
    private IEnumerator PathReductionCoroutine(System.Action onPathAnimationComplete)
    {
        while (currentPathIndex < path.Count)
        {
            currentPathIndex++;

            UpdatePathLine();

            yield return new WaitForSeconds(0.1f);
        }

        // 경로 애니메이션 완료 후 콜백 실행
        onPathAnimationComplete?.Invoke();
    }

    // 적이 출구에 닿으면 게임 오버 처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Exit"))
        {
            GameManager.Instance.GameOver();
        }
    }

    // 움직임 멈추는 함수 (플레이어가 출구 도달했을 시 호출)
    public void StopMoving()
    {
        StopAllCoroutines();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }
    }

}
