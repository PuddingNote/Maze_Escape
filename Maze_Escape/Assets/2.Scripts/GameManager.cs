using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    // �̱���

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI countdownText;     // ī��Ʈ�ٿ� �ؽ�Ʈ
    [SerializeField] private GameObject countdownPanel;         // ī��Ʈ�ٿ� �г�

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
        countdownPanel.SetActive(true);

        // ī��Ʈ�ٿ� ����
        StartCoroutine(StartCountdown());
    }

    // �÷��̾� �ʱ� ����
    public void SetPlayer(GameObject player)
    {
        playerController = player.GetComponent<PlayerController>();
        playerController.enabled = false;
    }

    // ī��Ʈ�ٿ� �ڷ�ƾ
    private IEnumerator StartCountdown()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        countdownPanel.SetActive(false);

        // ���� ����
        isGameActive = true;
        if (playerController != null) playerController.enabled = true;
    }

    // ���� ���� ó��
    public void EndGame()
    {
        isGameActive = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
