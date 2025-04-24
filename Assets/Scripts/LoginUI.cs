using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Game
{
    public class LoginUI : MonoBehaviour
    {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public Button loginButton;
        public APIManager apiManager;
        public GameObject errorPanel;
        public Button okButton;

        void Start()
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
            okButton.onClick.AddListener(ClearError);
            errorPanel.SetActive(false);
        }

        void OnLoginButtonClicked()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter both username and password");
                return;
            }

            apiManager.LoginUser(username, password);
        }

        public void ShowError(string message)
        {
            errorPanel.GetComponentInChildren<TextMeshProUGUI>().text = message;
            errorPanel.SetActive(true);
        }

        public void ClearError()
        {
            errorPanel.SetActive(false);
        }

        public void HandleLoginSuccess()
        {
            ClearError();
            SceneManager.LoadScene("Main Menu");
        }
    }
} 