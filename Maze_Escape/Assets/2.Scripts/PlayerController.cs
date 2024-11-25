using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed;            // �ӵ�
    private Vector2 movePosition;       // ��ǥ

    private void Awake()
    {
        moveSpeed = 3f;
    }

    // ������ ��� ȣ�� (����� �Է� �� ������ �������� �۾��� ����)
    private void Update()
    {
        movePosition = Vector2.zero;

        // �밢�� �̵��� �����ϱ� ����
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            movePosition.x = Input.GetAxisRaw("Horizontal");
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            movePosition.y = Input.GetAxisRaw("Vertical");
        }
    }

    // �����Ӱ� �����ϰ� �����ϰ� ȣ�� (���� ���� �� �ð� �������� �۾��� ����)
    private void FixedUpdate()
    {
        transform.Translate(movePosition * moveSpeed * Time.fixedDeltaTime);
    }


}
