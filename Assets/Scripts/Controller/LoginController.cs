using UnityEngine;

namespace Game
{
    public class LoginController : MonoBehaviour
    {
        public LoginView loginView;
        public APIManager apiManager;
        private UserModel userModel = new UserModel();

        void Awake()
        {
            if (loginView != null)
            {
                loginView.OnLoginAttempt += HandleLoginAttempt;
                loginView.OnErrorCleared += HandleErrorCleared;
            }
        }

        private void HandleLoginAttempt(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                loginView.ShowError("Please enter both username and password");
                return;
            }
            
            apiManager.LoginUser(
                username,
                password,
                (id, uname, email, token) => {
                    OnLoginSuccess(id, uname, email, token);
                    
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
                },
                (errorMsg) => {
                    loginView.ShowError(errorMsg);
                }
            );
        }

        private void HandleErrorCleared()
        {
            
        }

        
        public void OnLoginSuccess(int id, string username, string email, string token)
        {
            userModel.Id = id;
            userModel.Username = username;
            userModel.Email = email;
            userModel.Token = token;
            
        }
    }
} 