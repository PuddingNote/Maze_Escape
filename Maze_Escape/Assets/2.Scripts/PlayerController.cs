using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 2f;  // 이동속도
    private Vector2 moveInput;                      // 입력좌표
    private Rigidbody2D rb;                         // Rigidbody2D 참조

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 프레임 기반 호출 (사용자 입력 및 프레임 의존적인 작업에 적합)
    private void Update()
    {
        // 이동 입력 (대각선 이동 제한)
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));

        if (moveInput.x != 0)
        {
            moveInput.y = 0;
        }
    }

    // 프레임과 무관하게 일정하게 호출 (물리 연산 및 시간 독립적인 작업에 적합)
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
