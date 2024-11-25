using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed;            // 속도
    private Vector2 movePosition;       // 좌표

    private void Awake()
    {
        moveSpeed = 3f;
    }

    // 프레임 기반 호출 (사용자 입력 및 프레임 의존적인 작업에 적합)
    private void Update()
    {
        movePosition = Vector2.zero;

        // 대각선 이동을 제한하기 위해
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            movePosition.x = Input.GetAxisRaw("Horizontal");
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            movePosition.y = Input.GetAxisRaw("Vertical");
        }
    }

    // 프레임과 무관하게 일정하게 호출 (물리 연산 및 시간 독립적인 작업에 적합)
    private void FixedUpdate()
    {
        transform.Translate(movePosition * moveSpeed * Time.fixedDeltaTime);
    }


}
