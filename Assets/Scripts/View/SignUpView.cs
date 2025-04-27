using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Game
{
    public class SignUpView : MonoBehaviour
    {
        public TMP_InputField usernameInput;
        public TMP_InputField emailInput;
        public TMP_InputField passwordInput;
        public Button signUpButton;
        public GameObject errorPanel;
        public Button okButton;

        // Event for sign up button click
        public event Action<string, string, string> OnSignUpAttempt;
        public event Action OnErrorCleared;

        void Start()
        {
            signUpButton.onClick.AddListener(HandleSignUpButtonClicked);
            okButton.onClick.AddListener(ClearError);
            errorPanel.SetActive(false);
        }

        private void HandleSignUpButtonClicked()
        {
            string username = usernameInput.text;
            string email = emailInput.text;
            string password = passwordInput.text;
            OnSignUpAttempt?.Invoke(username, email, password);
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

        public void ClearFields()
        {
            usernameInput.text = "";
            emailInput.text = "";
            passwordInput.text = "";
        }
    }
} 