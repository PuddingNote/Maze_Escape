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
    [SerializeField] private TextMeshProUGUI stageText;         // �������� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI scoreText;         // ���� �ؽ�Ʈ
    [SerializeField] private GameObject countdownPanel;         // ī��Ʈ�ٿ� �г� (�������� text ����)
    [SerializeField] private GameObject gameOverPanel;          // ���ӿ��� ui �г�
    [SerializeField] private GameObject optionPanel;            // �ɼ� ui �г�
    [SerializeField] private TextMeshProUGUI scorePopupText;    // ���� �˾� UI Text

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
        score = 0;
        isGameActive = false;
    }

    private void Start()
    {
        MazeGenerator.Instance.StartMazeGenerate();

        stageText.text = "Stage " + currentStage;
        scoreText.text = "Score: " + score;
        StartCoroutine(StartCountdown());
    }

    // ī��Ʈ�ٿ� �ڷ�ƾ
    private IEnumerator StartCountdown()
    {
        optionPanel.SetActive(false);
        countdownPanel.SetActive(true);
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
        countdownPanel.SetActive(false);

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
                    // ���� �ִϸ��̼��� ���� �� ���� �߰�
                    int pathDistance = enemyController.GetPathDistance();
                    AddScore(pathDistance);
                });
            }
        }
    }

    // ���� �߰� �Լ�
    public void AddScore(int value)
    {
        score += value;
        scoreText.text = "Score: " + score;

        ShowScorePopup(value);
        StartCoroutine(DelayStageTransition());
    }

    // �������� �߰� ���� �˾� ȿ��
    private void ShowScorePopup(int points)
    {
        scorePopupText.text = $"+{points}";
        scorePopupText.color = new Color(scorePopupText.color.r, scorePopupText.color.g, scorePopupText.color.b, 1);

        StartCoroutine(AnimateScorePopup());
    }

    // �˾� ȿ�� �ڷ�ƾ
    private IEnumerator AnimateScorePopup()
    {
        float duration = 1f;
        float elapsed = 0f;

        Vector3 startPosition = scorePopupText.rectTransform.anchoredPosition;
        Vector3 endPosition = startPosition + new Vector3(0, 25, 0);

        Color startColor = scorePopupText.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            scorePopupText.rectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, t);
            scorePopupText.color = Color.Lerp(startColor, endColor, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        scorePopupText.rectTransform.anchoredPosition = startPosition;
    }

    // �������� ��ȯ ���� �ڷ�ƾ
    private IEnumerator DelayStageTransition()
    {
        yield return new WaitForSeconds(1f);
        EndGame();
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
