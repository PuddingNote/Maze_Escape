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
        moveInput = Vector2.zero;

        // �밢�� �̵� ����
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            moveInput.y = Input.GetAxisRaw("Vertical");
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
