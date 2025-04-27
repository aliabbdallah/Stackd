using UnityEngine;

namespace Game
{
    public class SignUpController : MonoBehaviour
    {
        public SignUpView signUpView;
        public APIManager apiManager;

        void Awake()
        {
            if (signUpView != null)
            {
                signUpView.OnSignUpAttempt += HandleSignUpAttempt;
                signUpView.OnErrorCleared += HandleErrorCleared;
            }
        }

        private void HandleSignUpAttempt(string username, string email, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                signUpView.ShowError("All fields are required");
                return;
            }
            // Call APIManager for registration with callbacks
            apiManager.RegisterUser(
                username,
                email,
                password,
                () => {
                    signUpView.ClearFields();
                    // Load the login scene directly
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Log In");
                },
                (errorMsg) => {
                    signUpView.ShowError(errorMsg);
                }
            );
        }

        private void HandleErrorCleared()
        {
            // Optionally handle error cleared event
        }
    }
} 