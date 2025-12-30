using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Title("Game State")]
    [ReadOnly, SerializeField]
    private EGameState _state = EGameState.Ready;
    public EGameState State => _state;

    [Title("UI")]
    [Required, SerializeField]
    private TextAnimatorBase _mainTextUI;

    [Title("POP UP")]
    [Required, SerializeField]
    private OptionPopup _option;

    [Title("Player")]
    [Required, SerializeField]
    private PlayerController Player;

    public event Action OnGameStart;
    public event Action OnGameOver;

    private readonly StringBuilder _sb = new StringBuilder(64);

    public override void Init()
    {
        Player.Init();
        Player.Stat.OnDead.Subscribe(OnPlayerDead);

        StartCoroutine(GameStartRoutine());
    }

    private void OnDestroy()
    {
        if (Player != null && Player.Stat != null)
            Player.Stat.OnDead.Unsubscribe(OnPlayerDead);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
            _option.Show();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
        CursorManager.Instance.ChangeMode();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        CursorManager.Instance.ChangeMode();
    }

    private IEnumerator GameStartRoutine()
    {
        _mainTextUI.ShowText(BuildText("<wave a=2 s=1>", "Ready...", "</wave>"));
        yield return new WaitForSeconds(2f);

        _mainTextUI.ShowText(BuildText("<bounce a=15>", "Start!!", "</bounce>"));
        yield return new WaitForSeconds(1f);

        _mainTextUI.HideText();

        _state = EGameState.Playing;
        OnGameStart?.Invoke();

        DebugManager.Instance.Log("[GameManager] Playing");
    }

    private void OnPlayerDead()
    {
        if (_state == EGameState.GameOver)
            return;

        GameOver();
        OnGameOver?.Invoke();
    }

    private void GameOver()
    {
        _state = EGameState.GameOver;

        DebugManager.Instance.Log("[GameManager] GameOver");

        _mainTextUI.ShowText(BuildText("<slideh>", "Game Over...", "</slideh>"));
    }

    public void RestartGame()
    {
        Restart();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private string BuildText(string prefix, string content, string suffix)
    {
        _sb.Clear();
        _sb.Append(prefix);
        _sb.Append(content);
        _sb.Append(suffix);
        return _sb.ToString();
    }
}
