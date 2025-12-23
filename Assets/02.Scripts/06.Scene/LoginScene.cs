using ArtificeToolkit.Attributes;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScene : MonoBehaviour
{
    private enum SceneMode
    { 
        Login,
        Register
    }

    private SceneMode _mode = SceneMode.Login;

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
        Refresh();
        AddButtonEvent();
    }

    private void AddButtonEvent()
    {
        _registerButton.onClick.AddListener(RegisterButtonHanler);
        _loginButton.onClick.AddListener(LoginButtonHanler);
        _confirmButton.onClick.AddListener(ConfirmButtonHanler);
        _cancelButton.onClick.AddListener(CancelButtonHanler);
    }

    private void Refresh()
    {
        //로그인 모드
        _registerButton.gameObject.SetActive(_mode == SceneMode.Login);
        _loginButton.gameObject.SetActive(_mode == SceneMode.Login);
        //레지스터 모드
        _passwordConfirmObject.SetActive(_mode == SceneMode.Register);
        _confirmButton.gameObject.SetActive(_mode == SceneMode.Register);
        _cancelButton.gameObject.SetActive(_mode == SceneMode.Register);

        string lastId = PlayerPrefs.GetString("LastId");

        _idField.text = string.IsNullOrEmpty(lastId) ? "":lastId;
        _passwordField.text = "";
        _confirmField.text = "";

        _messageText.text = "";
    }


    private void RegisterButtonHanler()
    {
        _mode = SceneMode.Register;
        Refresh();
    }

    private void LoginButtonHanler()
    {
        string id = _idField.text;
        string password = _passwordField.text;

        bool isIdEmpty = string.IsNullOrEmpty(id);
        bool isPasswordEmpty = string.IsNullOrEmpty(password);

        bool isInvalidEmail = !Regex.IsMatch(id,
            @"^[\w.-]+ @ [\w.-]+ \. \w+ $"); 

        bool isInvalidPassword = !Regex.IsMatch(password,
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^A-Za-z0-9])(?!.*\d)\S{7,20}$");

        bool isUserNotFound = !PlayerPrefs.HasKey(id);

        if (isIdEmpty || isPasswordEmpty || isInvalidEmail || isInvalidPassword || isUserNotFound)
        {
            Debug.LogWarning($"Login Failed | Email:{id}");

            Refresh();
            _messageText.text = "아이디와 비밀번호를 확인해주세요";
            return;
        }

        string savedHash = PlayerPrefs.GetString(id);
        string inputHash = PasswordUtility.Hash(password);
        if (savedHash != inputHash)
        {
            Refresh();
            _messageText.text = "아이디와 비밀번호를 확인해주세요";
            return;
        }

        PlayerPrefs.SetString("LastId", id);
        SceneManager.LoadScene(1);
    }

    private void ConfirmButtonHanler()
    {
        string id = _idField.text;
        bool isInvalidEmail = !Regex.IsMatch(id,
            @"^[\w.-]+@[\w.-]+\.\w+$");
        if (string.IsNullOrEmpty(id) || isInvalidEmail)
        {
            Refresh();
            _messageText.text = "아이디가 적절하지 않습니다.";
            return;
        }

        if (PlayerPrefs.HasKey(id))
        {
            Refresh();
            _messageText.text = "이미 아이디가 존재합니다";
            return;
        }

        string password = _passwordField.text;
        bool isInvalidPassword = !Regex.IsMatch(password,
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^A-Za-z0-9])(?!.*\d)\S{7,20}$");
        if (string.IsNullOrEmpty(password) || isInvalidPassword)
        {
            Refresh();
            _messageText.text = "패스워드가 적절하지 않습니다.";
            return;
        }

        string checkPassword = _confirmField.text;
        if (string.IsNullOrEmpty(checkPassword))
        {
            Refresh();
            _messageText.text = "패스워드 확인란을 입력해주세요";
            return;
        }

        if (checkPassword != password)
        {
            Refresh();
            _messageText.text = "패스워드가 다릅니다";
            return;
        }

        string hashedPassword = PasswordUtility.Hash(password);
        PlayerPrefs.SetString(id, hashedPassword);
        PlayerPrefs.SetString("LastId", id);
        _mode = SceneMode.Login;
        Refresh();

        _messageText.text = "생성이 완료되었습니다";
    }

    private void CancelButtonHanler()
    {
        _mode = SceneMode.Login;
        Refresh();
    }

    [Button]
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}

public static class PasswordUtility
{
    // Simple SHA256 hash (one-way)
    public static string Hash(string password)
    {
        using SHA256 sha = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = sha.ComputeHash(bytes);

        StringBuilder sb = new StringBuilder();
        foreach (byte b in hash)
            sb.Append(b.ToString("x2"));

        return sb.ToString();
    }
}