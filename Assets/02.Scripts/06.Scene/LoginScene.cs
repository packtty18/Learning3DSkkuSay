using ArtificeToolkit.Attributes;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Validator
{
    private static readonly Regex EmailRegex = new(@"^[\w.-]+@[\w.-]+\.\w+$");
    private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^A-Za-z0-9])(?!.*\d)\S{7,20}$");

    public static bool IsValidEmail(string email)
    {
        return !string.IsNullOrEmpty(email) && EmailRegex.IsMatch(email);
    }

    public static bool IsValidPassword(string password)
    {
        return !string.IsNullOrEmpty(password) && PasswordRegex.IsMatch(password);
    }
}

public static class PasswordUtility
{
    public static string Hash(string password)
    {
        using SHA256 sha = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = sha.ComputeHash(bytes);

        StringBuilder sb = new();
        foreach (byte b in hash)
            sb.Append(b.ToString("x2"));

        return sb.ToString();
    }
}

public class LoginScene : MonoBehaviour
{
    private enum SceneMode { Login, Register }
    private SceneMode _mode = SceneMode.Login;

    [Header("UI References")]
    [SerializeField] private GameObject _passwordConfirmObject;
    [SerializeField] private Button _registerButton;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private TMP_InputField _idField;
    [SerializeField] private TMP_InputField _passwordField;
    [SerializeField] private TMP_InputField _confirmField;
    [SerializeField] private TextMeshProUGUI _messageText;

    private void Start()
    {
        RefreshUI();
        AddButtonEvents();
    }

    private void AddButtonEvents()
    {
        _registerButton.onClick.AddListener(() => SwitchMode(SceneMode.Register));
        _loginButton.onClick.AddListener(LoginHandler);
        _confirmButton.onClick.AddListener(RegisterHandler);
        _cancelButton.onClick.AddListener(() => SwitchMode(SceneMode.Login));
    }

    private void SwitchMode(SceneMode mode)
    {
        _mode = mode;
        RefreshUI();
    }

    private void RefreshUI()
    {
        _registerButton.gameObject.SetActive(_mode == SceneMode.Login);
        _loginButton.gameObject.SetActive(_mode == SceneMode.Login);
        _passwordConfirmObject.SetActive(_mode == SceneMode.Register);
        _confirmButton.gameObject.SetActive(_mode == SceneMode.Register);
        _cancelButton.gameObject.SetActive(_mode == SceneMode.Register);

        _idField.text = PlayerPrefs.GetString("LastId", "");
        _passwordField.text = "";
        _confirmField.text = "";
        _messageText.text = "";
    }

    private void LoginHandler()
    {
        string id = _idField.text;
        string password = _passwordField.text;

        if (!Validator.IsValidEmail(id) || !Validator.IsValidPassword(password) || !PlayerPrefs.HasKey(id))
        {
            ShowMessage("아이디와 비밀번호를 확인해주세요");
            return;
        }

        string savedHash = PlayerPrefs.GetString(id, "");
        if (PasswordUtility.Hash(password) != savedHash)
        {
            ShowMessage("아이디와 비밀번호를 확인해주세요");
            return;
        }

        PlayerPrefs.SetString("LastId", id);
        SceneManager.LoadScene(1);
    }

    private void RegisterHandler()
    {
        string id = _idField.text;
        string password = _passwordField.text;
        string confirm = _confirmField.text;

        if (!ValidateRegisterInputs(id, password, confirm)) return;

        PlayerPrefs.SetString(id, PasswordUtility.Hash(password));
        PlayerPrefs.SetString("LastId", id);

        SwitchMode(SceneMode.Login);
        ShowMessage("생성이 완료되었습니다");
    }

    private bool ValidateRegisterInputs(string id, string password, string confirm)
    {
        if (!Validator.IsValidEmail(id))
        {
            ShowMessage("아이디가 적절하지 않습니다.");
            return false;
        }

        if (PlayerPrefs.HasKey(id))
        {
            ShowMessage("이미 아이디가 존재합니다");
            return false;
        }

        if (!Validator.IsValidPassword(password))
        {
            ShowMessage("패스워드가 적절하지 않습니다.");
            return false;
        }

        if (string.IsNullOrEmpty(confirm))
        {
            ShowMessage("패스워드 확인란을 입력해주세요");
            return false;
        }

        if (password != confirm)
        {
            ShowMessage("패스워드가 다릅니다");
            return false;
        }

        return true;
    }

    private void ShowMessage(string message)
    {
        _messageText.text = message;
    }

    [Button]
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
