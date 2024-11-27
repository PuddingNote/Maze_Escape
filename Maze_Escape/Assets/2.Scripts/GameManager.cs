using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    // 싱글톤

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI countdownText;     // 카운트다운 텍스트
    [SerializeField] private TextMeshProUGUI stageText;         // 스테이지 텍스트 추가
    [SerializeField] private GameObject uiPanel;                // ui 패널 (카운트다운, 스테이지 text 포함)

    private int currentStage = 1;                               // 현재 스테이지
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
        MazeGenerator.Instance.StartMazeGenerate();

        stageText.text = "Stage " + currentStage;
        StartCoroutine(StartCountdown());
    }

    // 카운트다운 코루틴
    private IEnumerator StartCountdown()
    {
        uiPanel.SetActive(true);
        isGameActive = false;
        if (playerController != null) playerController.enabled = false;

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        uiPanel.SetActive(false);

        // 게임 시작
        isGameActive = true;
        if (playerController != null) playerController.enabled = true;
    }

    // 플레이어 초기 설정
    public void SetPlayer(GameObject player)
    {
        playerController = player.GetComponent<PlayerController>();
        playerController.enabled = false;
    }

    // 게임 종료 처리
    public void EndGame()
    {
        isGameActive = false;

        // 스테이지가 10을 넘지 않으면, 다음 스테이지로 진행
        if (currentStage < 10)
        {
            currentStage++;
            StartNewStage();
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }

    // 새로운 스테이지 시작
    private void StartNewStage()
    {
        // 스테이지 UI 업데이트
        stageText.text = "Stage " + currentStage;

        // 기존 플레이어 제거
        if (playerController != null)
        {
            Destroy(playerController.gameObject);
        }

        // 미로 새로 생성 (초기 세팅 그대로 하면 됌)
        MazeGenerator.Instance.StartMazeGenerate();

        // 카운트다운 후 게임 시작
        StartCoroutine(StartCountdown());
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
