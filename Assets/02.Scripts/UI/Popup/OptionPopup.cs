using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionPopup : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _terminateButton;
    private void Start()
    {
        Hide();
        _continueButton.onClick.AddListener(GameContinue);
        _retryButton.onClick.AddListener(GameRestart);
        _terminateButton.onClick.AddListener(GameEnd);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


    private void GameContinue()
    {
        DebugManager.Instance.Log("Continue Button Active");
        GameManager.Instance.Restart();
        Hide();
    }

    private void GameRestart()
    {
        DebugManager.Instance.Log("Retry Button Active");

        //임시.. 씬매니저 생성시 이동
        GameManager.Instance.RestartGame();
    }

    private void GameEnd()
    {
        DebugManager.Instance.Log("Terminate Button Active");
        //로비로 혹은 런타임 종료

        GameManager.Instance.QuitGame();
    }
}
