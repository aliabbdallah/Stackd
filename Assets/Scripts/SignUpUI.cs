using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Game
{
    public class SignUpUI : MonoBehaviour
    {
        public TMP_InputField usernameInput;
        public TMP_InputField emailInput;
        public TMP_InputField passwordInput;
        public Button signUpButton;
        public APIManager apiManager;
        public GameObject errorPanel;
        public Button okButton;

        void Start()
        {
            signUpButton.onClick.AddListener(OnSignUpButtonClicked);
            okButton.onClick.AddListener(ClearError);
            errorPanel.SetActive(false);
        }

        void OnSignUpButtonClicked()
        {
            string username = usernameInput.text;
            string email = emailInput.text;
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowError("All fields are required");
                return;
            }

            apiManager.RegisterUser(username, email, password);
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

        public void HandleRegistrationSuccess()
        {
            // Clear the input fields
            usernameInput.text = "";
            emailInput.text = "";
            passwordInput.text = "";
            
            // Load the login scene directly
            SceneManager.LoadScene("Log In");
        }
    }
}
