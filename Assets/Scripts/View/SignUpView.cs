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

        
        private APIManager apiManager;

        
        public event Action<string, string, string> OnSignUpAttempt;
        public event Action OnErrorCleared;

        void Start()
        {
            apiManager = FindObjectOfType<APIManager>();
            signUpButton.onClick.AddListener(HandleSignUpButtonClicked);
            okButton.onClick.AddListener(ClearError);
            errorPanel.SetActive(false);

            
            passwordInput.onValueChanged.AddListener(ValidatePassword);

            
            signUpButton.interactable = false;
        }

        private void ValidatePassword(string password)
        {
            
            signUpButton.interactable = password.Length >= 7;
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
            signUpButton.interactable = false;
        }
    }
} 