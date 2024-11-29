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

    private void Awake()
    {
        moveSpeed = 0.5f;
        path = new List<Vector2>();
        currentPathIndex = 0;
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
        }
    }

    // ������������ �̵��ӵ� 0.2f�� ����
    public void UpdateMoveSpeed(int stage)
    {
        moveSpeed = 0.5f + (stage - 1) * 0.2f;
    }

    // ���� �ⱸ�� ������ ���� ���� ó��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Exit"))
        {
            GameManager.Instance.GameOver();
        }
    }


}
