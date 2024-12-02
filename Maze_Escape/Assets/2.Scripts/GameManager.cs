using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    // �̱���

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI countdownText;     // ī��Ʈ�ٿ� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI stageText;         // �������� �ؽ�Ʈ �߰�
    [SerializeField] private GameObject uiPanel;                // ui �г� (ī��Ʈ�ٿ�, �������� text ����)
    [SerializeField] private GameObject gameOverPanel;          // ���ӿ��� ui �г�
    [SerializeField] private GameObject optionPanel;            // �ɼ� ui �г�

    [Header("Game Settings")]
    private int currentStage;                                   // ���� ��������
    private bool isGameActive;                                  // ���� ���� ����
    private int score;                                          // ����

    private PlayerController playerController;
    private EnemyController enemyController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentStage = 1;
        isGameActive = false;
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
        optionPanel.SetActive(false);
        uiPanel.SetActive(true);
        gameOverPanel.SetActive(false);

        isGameActive = false;
        if (playerController != null) 
        {
            playerController.enabled = false;
        }
        if (enemyController != null)
        {
            enemyController.enabled = false;
        }

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        uiPanel.SetActive(false);

        // ���� ����
        isGameActive = true;
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        if (enemyController != null)
        {
            enemyController.enabled = true;
        }
    }

    // �÷��̾� �ʱ� ����
    public void SetPlayer(GameObject player)
    {
        playerController = player.GetComponent<PlayerController>();
        playerController.enabled = false;
    }

    // �� �ʱ� ����
    public void SetEnemy(GameObject enemy)
    {
        enemyController = enemy.GetComponent<EnemyController>();
        enemyController.enabled = false;
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
            // ������ �������� Ŭ�����

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

        // ���� �� ����
        if (enemyController != null)
        {
            Destroy(enemyController.gameObject);
        }

        MazeGenerator.Instance.NextMazeGenerate();

        // ���� �̵��ӵ� ������Ʈ
        if (enemyController != null)
        {
            enemyController.UpdateMoveSpeed(currentStage);
        }

        StartCoroutine(StartCountdown());
    }

    // �÷��̾� �ⱸ �浹 Ȯ��
    public void CheckExitCollision(GameObject player, GameObject exit)
    {
        if (isGameActive && player.CompareTag("Player") && exit.CompareTag("Exit"))
        {
            isGameActive = false;

            // ���� �÷��̾� �̵� ����
            if (playerController != null)
            {
                playerController.enabled = false;
                Rigidbody2D playerRb = playerController.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = Vector2.zero;
                    playerRb.simulated = false;
                }
            }

            // �� ��� ǥ�� �� ���� �ִϸ��̼� ����
            if (enemyController != null)
            {
                enemyController.StopMoving();
                enemyController.ShowPath();
                enemyController.AnimatePathReduction(() =>
                {
                    // ���� �ִϸ��̼��� ���� �� ���� �߰� �� ���� �������� �̵�
                    int pathDistance = enemyController.GetPathDistance();
                    AddScore(pathDistance);
                    EndGame();
                });
            }
        }
    }

    // ���� �߰� �Լ�
    public void AddScore(int value)
    {
        score += value;
        Debug.Log("Score Added: " + value + " | Total Score: " + score);
    }

    // �������� ���� ��ȯ �Լ�
    public int GetCurrentStage()
    {
        return currentStage;
    }

    // ���� ���� ó�� �Լ� �߰�
    public void GameOver()
    {
        isGameActive = false;

        // �ð� ���߰� �÷��̾�� ���� ���� ��Ȱ��ȭ
        Time.timeScale = 0;
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        if (enemyController != null)
        {
            enemyController.enabled = false;
        }

        gameOverPanel.SetActive(true);
    }

    // ��� Option ��ư
    public void OptionButton()
    {
        Time.timeScale = 0;
        isGameActive = false;

        optionPanel.SetActive(true);
    }

    // �̾��ϱ� ��ư
    public void ResumeButton()
    {
        Time.timeScale = 1;
        isGameActive = true;

        optionPanel.SetActive(false);
    }

    // Ÿ��Ʋ�� �̵� ��ư
    public void TitleButtton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
    }

    // ����� ��ư
    public void RestartButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameScene");
    }

    // ���� ���� ��ư
    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
