using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Game
{
    public class LoginView : MonoBehaviour
    {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public Button loginButton;
        public GameObject errorPanel;
        public Button okButton;

        
        public event Action<string, string> OnLoginAttempt;
        public event Action OnErrorCleared;

        void Start()
        {
            loginButton.onClick.AddListener(HandleLoginButtonClicked);
            okButton.onClick.AddListener(ClearError);
            errorPanel.SetActive(false);
        }

        private void HandleLoginButtonClicked()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;
            OnLoginAttempt?.Invoke(username, password);
        }

        public void ShowError(string message)
        {
            errorPanel.GetComponentInChildren<TextMeshProUGUI>().text = message;
            errorPanel.SetActive(true);
        }

        public void ClearError()
        {
            errorPanel.SetActive(false);
            OnErrorCleared?.Invoke();
        }
    }
} 