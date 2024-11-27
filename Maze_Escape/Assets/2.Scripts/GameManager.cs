using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    // 싱글톤

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI countdownText;     // 카운트다운 텍스트
    [SerializeField] private GameObject countdownPanel;         // 카운트다운 패널

    private bool isGameActive = false;                          // 게임 진행 상태
    private PlayerController playerController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        countdownPanel.SetActive(true);

        // 카운트다운 시작
        StartCoroutine(StartCountdown());
    }

    // 플레이어 초기 설정
    public void SetPlayer(GameObject player)
    {
        playerController = player.GetComponent<PlayerController>();
        playerController.enabled = false;
    }

    // 카운트다운 코루틴
    private IEnumerator StartCountdown()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        countdownPanel.SetActive(false);

        // 게임 시작
        isGameActive = true;
        if (playerController != null) playerController.enabled = true;
    }

    // 게임 종료 처리
    public void EndGame()
    {
        isGameActive = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 출구 충돌 확인
    public void CheckExitCollision(GameObject player, GameObject exit)
    {
        if (isGameActive && player.CompareTag("Player") && exit.CompareTag("Exit"))
        {
            EndGame();
        }
    }

}
