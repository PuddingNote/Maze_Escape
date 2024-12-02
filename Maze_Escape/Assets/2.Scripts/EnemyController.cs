using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed;       // ���� �̵��ӵ�
    private Vector2 targetPosition;                 // ���� ��ǥ ��ġ (�ⱸ)
    private List<Vector2> path;                     // ���� ���
    private int currentPathIndex;                   // ���� ��� �ε���
    private LineRenderer lineRenderer;              // ���� ������

    private void Awake()
    {
        moveSpeed = 0.5f;
        path = new List<Vector2>();
        currentPathIndex = 0;

        // ���� ������ �ʱ�ȭ
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
        // �ⱸ�� ��ġ�� ��ǥ�� ����
        targetPosition = MazeGenerator.Instance.GetGridPosition(MazeGenerator.Instance.GetExitPosition());

        // A* �˰������� �ⱸ������ ��� ���
        Vector2 startGridPosition = MazeGenerator.Instance.GetGridPosition(transform.position);
        path = EnemyPathfinding.Instance.FindPath(startGridPosition, targetPosition);

        if (path.Count > 0)
        {
            StartCoroutine(FollowPath());
        }
        else
        {
            Debug.LogError("��ΰ� ����");
        }

        // ���������� �°� �̵��ӵ� ������Ʈ
        if (GameManager.Instance != null)
        {
            UpdateMoveSpeed(GameManager.Instance.GetCurrentStage());
        }
    }

    // ��θ� ���� �̵��ϴ� �ڷ�ƾ
    private IEnumerator FollowPath()
    {
        while (currentPathIndex < path.Count)
        {
            Vector2 currentTarget = path[currentPathIndex]; // ��ǥ ����
            Vector3 worldTarget = MazeGenerator.Instance.GetWorldPosition((int)currentTarget.x, (int)currentTarget.y);  // ���� ��ǥ�� ��ȯ

            // ��ǥ �������� �̵�
            while ((Vector2)transform.position != (Vector2)worldTarget)
            {
                transform.position = Vector2.MoveTowards(transform.position, worldTarget, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentPathIndex++;

            UpdatePathLine();
        }
    }

    // ������������ �̵��ӵ� 0.2f�� ����
    public void UpdateMoveSpeed(int stage)
    {
        moveSpeed = 0.5f + (stage - 1) * 0.2f;
    }

    // ��� �Ÿ� ��������
    public int GetPathDistance()
    {
        return path != null ? path.Count : 0;
    }

    // ���� ��� �Ÿ� ���
    public int GetRemainingPathDistance()
    {
        if (path == null || currentPathIndex >= path.Count)
            return 0;

        // ���� ��ġ���� �ⱸ���� ���� ��� ����
        return path.Count - currentPathIndex;
    }

    // ���� ��θ� �������� ������Ʈ
    public void UpdatePathLine()
    {
        if (path == null || path.Count == 0 || currentPathIndex >= path.Count)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        // ���� ��θ� ���� �������� ����
        int remainingCount = path.Count - currentPathIndex;
        lineRenderer.positionCount = remainingCount;

        for (int i = 0; i < remainingCount; i++)
        {
            Vector2 gridPosition = path[currentPathIndex + i];
            Vector3 worldPosition = MazeGenerator.Instance.GetWorldPosition((int)gridPosition.x, (int)gridPosition.y);
            lineRenderer.SetPosition(i, worldPosition);
        }
    }

    // ��� �����ֱ�
    public void ShowPath()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
        }
    }

    // ���� ������ �پ��� �ִϸ��̼� ����
    public void AnimatePathReduction(System.Action onPathAnimationComplete)
    {
        StartCoroutine(PathReductionCoroutine(onPathAnimationComplete));
    }

    // ������ ���� �پ��� �ڷ�ƾ
    private IEnumerator PathReductionCoroutine(System.Action onPathAnimationComplete)
    {
        while (currentPathIndex < path.Count)
        {
            currentPathIndex++;

            UpdatePathLine();

            yield return new WaitForSeconds(0.1f);
        }

        // ��� �ִϸ��̼� �Ϸ� �� �ݹ� ����
        onPathAnimationComplete?.Invoke();
    }

    // ���� �ⱸ�� ������ ���� ���� ó��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Exit"))
        {
            GameManager.Instance.GameOver();
        }
    }

    // ������ ���ߴ� �Լ� (�÷��̾ �ⱸ �������� �� ȣ��)
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
