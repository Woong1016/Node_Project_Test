using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
     public float[] stageLimitTimes;
    public int currentStage = 1;

    public AudioSource audioSource;
    public AudioClip readySound;  
    public AudioClip clickSound;  

    public GameObject lobbyPanel;
    public GameObject gamePanel;
    public GameObject clearPanel;
    public GameObject failPanel;
    public GameObject inputNamePopup;

    public Text signalText;
    public Text clearTimeText;
    public Text failTimeText;
    public Text currentStageText;

    public InputField rankNameInput;
    public Button rankSaveButton;

    private float limitTime;
    private bool isReadyToStart = false;  
    private bool isWaitingForSignal = false; 
    private bool isCanClick = false;     
    private float startTime;
    private float reactionTimeMs;
    private bool isFalseStart = false;

    void Start()
    {
        ShowLobby();
    }

    public void ShowLobby()
    {
        lobbyPanel.SetActive(true);
        gamePanel.SetActive(false);
        clearPanel.SetActive(false);
        failPanel.SetActive(false);
        inputNamePopup.SetActive(false);
    }

    public void OnStageSelectClick(int stageNum)
    {
        currentStage = stageNum;
        StartStage();
    }

    void StartStage()
    {
        if (currentStage > stageLimitTimes.Length)
        {
            ShowLobby();
            return;
        }

        limitTime = stageLimitTimes[currentStage - 1];

        lobbyPanel.SetActive(false);
        gamePanel.SetActive(true);
        clearPanel.SetActive(false);
        failPanel.SetActive(false);

        if (currentStageText != null)
            currentStageText.text = $"STAGE {currentStage}";

       
        signalText.text = "Are you ready?";
        signalText.color = Color.black;

        isReadyToStart = true;      
        isWaitingForSignal = false;
        isCanClick = false;
        isFalseStart = false;
    }

    IEnumerator GameRoutine()
    {
        signalText.text = "Wait.";

        if (readySound != null) audioSource.PlayOneShot(readySound);

        isWaitingForSignal = true;  

        float waitTime = Random.Range(2.0f, 5.0f);
        yield return new WaitForSeconds(waitTime);

        isWaitingForSignal = false;
        isCanClick = true;

        signalText.text = "CLICK!";
        signalText.color = Color.red;
        startTime = Time.time;
    }

    public void OnGameClick()
    {
        if (isReadyToStart)
        {
            isReadyToStart = false;  
            StartCoroutine(GameRoutine());  
            return;
        }

        if (clickSound != null) audioSource.PlayOneShot(clickSound);

        if (isWaitingForSignal)
        {
            StopAllCoroutines();
            isFalseStart = true;
            reactionTimeMs = -1;
            GameOver();
            return;
        }

        if (isCanClick)
        {
            isCanClick = false;
            float duration = Time.time - startTime;
            reactionTimeMs = duration * 1000f;

            if (duration <= limitTime)
            {
                GameClear((int)reactionTimeMs);
            }
            else
            {
                GameOver();
            }
        }
    }

    void GameClear(int ms)
    {
        gamePanel.SetActive(false);
        clearPanel.SetActive(true);
        if (clearTimeText != null)
            clearTimeText.text = $"SUCCESS!\n기록: {ms}ms\n(제한: {limitTime * 1000}ms)";
    }

    void GameOver()
    {
        gamePanel.SetActive(false);
        failPanel.SetActive(true);
        if (failTimeText != null)
        {
            if (isFalseStart) failTimeText.text = "FAIL\nToo Fast! (반칙)";
            else failTimeText.text = $"FAIL\nTime Over\n기록: {(int)reactionTimeMs}ms\n(제한: {limitTime * 1000}ms)";
        }
    }

    public void OnNextStageClick()
    {
        currentStage++;
        if (currentStage > stageLimitTimes.Length) ShowLobby();
        else StartStage();
    }

    public void OnRetryClick()
    {
        StartStage();
    }

    public void OnQuitToLobbyClick()
    {
        ShowLobby();
    }

    public void OnOpenSavePopupClick()
    {
        if (isFalseStart) return;
        inputNamePopup.SetActive(true);
        if (rankNameInput != null) { rankNameInput.text = ""; rankNameInput.interactable = true; }
        if (rankSaveButton != null) rankSaveButton.interactable = true;
    }

    public void OnRealSaveBtnClick()
    {
        string playerName = rankNameInput.text;
        if (string.IsNullOrEmpty(playerName)) return;
        int finalMs = (int)reactionTimeMs;
        RankingManager rankMgr = FindObjectOfType<RankingManager>();
        if (rankMgr != null) rankMgr.SaveRanking(playerName, currentStage, finalMs);
        rankNameInput.interactable = false;
        rankSaveButton.interactable = false;
        StartCoroutine(ClosePopupAndGoLobby());
    }

    IEnumerator ClosePopupAndGoLobby()
    {
        yield return new WaitForSeconds(1.0f);
        inputNamePopup.SetActive(false);
        ShowLobby();
    }

    public void OnOpenRankingClick()
    {
        RankingManager rankMgr = FindObjectOfType<RankingManager>();
        if (rankMgr != null) rankMgr.OpenRankingBoard();
    }

    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}