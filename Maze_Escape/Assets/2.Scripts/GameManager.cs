using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    // �̱���

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI countdownText;     // ī��Ʈ�ٿ� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI stageText;         // �������� �ؽ�Ʈ �߰�
    [SerializeField] private GameObject uiPanel;                // ui �г� (ī��Ʈ�ٿ�, �������� text ����)

    private int currentStage = 1;                               // ���� ��������
    private bool isGameActive = false;                          // ���� ���� ����
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

    // ī��Ʈ�ٿ� �ڷ�ƾ
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

        // ���� ����
        isGameActive = true;
        if (playerController != null) playerController.enabled = true;
    }

    // �÷��̾� �ʱ� ����
    public void SetPlayer(GameObject player)
    {
        playerController = player.GetComponent<PlayerController>();
        playerController.enabled = false;
    }

    // ���� ���� ó��
    public void EndGame()
    {
        isGameActive = false;

        // ���������� 10�� ���� ������, ���� ���������� ����
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

    // ���ο� �������� ����
    private void StartNewStage()
    {
        // �������� UI ������Ʈ
        stageText.text = "Stage " + currentStage;

        // ���� �÷��̾� ����
        if (playerController != null)
        {
            Destroy(playerController.gameObject);
        }

        // �̷� ���� ���� (�ʱ� ���� �״�� �ϸ� ��)
        MazeGenerator.Instance.StartMazeGenerate();

        // ī��Ʈ�ٿ� �� ���� ����
        StartCoroutine(StartCountdown());
    }

    // �ⱸ �浹 Ȯ��
    public void CheckExitCollision(GameObject player, GameObject exit)
    {
        if (isGameActive && player.CompareTag("Player") && exit.CompareTag("Exit"))
        {
            EndGame();
        }
    }

}
