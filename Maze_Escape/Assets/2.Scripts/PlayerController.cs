using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 2f;  // �̵��ӵ�
    private Vector2 moveInput;                      // �Է���ǥ
    private Rigidbody2D rb;                         // Rigidbody2D ����

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // ������ ��� ȣ�� (����� �Է� �� ������ �������� �۾��� ����)
    private void Update()
    {
        // �̵� �Է� (�밢�� �̵� ����)
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));

        if (moveInput.x != 0)
        {
            moveInput.y = 0;
        }
    }

    // �����Ӱ� �����ϰ� �����ϰ� ȣ�� (���� ���� �� �ð� �������� �۾��� ����)
    private void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject exit = collision.gameObject;

        if (exit.CompareTag("Exit"))
        {
            GameManager.Instance.CheckExitCollision(gameObject, exit);
        }
    }

}
