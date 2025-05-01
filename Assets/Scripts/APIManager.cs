using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

namespace Game
{
    public class APIManager : MonoBehaviour
    {
        public string baseUrl = "http://localhost:3000";
        public LoginView loginView;
        public SignUpView signUpView;
        
        // Session token for authentication
        private string sessionToken;
        
        void Start()
        {
            // Try to load saved session token
            sessionToken = PlayerPrefs.GetString("SessionToken", "");
        }

        [System.Serializable]
        public class RegisterPayload { public string username, email, password; public RegisterPayload(string u, string e, string p) { username = u; email = e; password = p; } }
        [System.Serializable]
        public class LoginPayload { public string email, password; public LoginPayload(string e, string p) { email = e; password = p; } }
        [System.Serializable]
        public class EmailCheckPayload { public string email; public EmailCheckPayload(string e) { email = e; } }
        [System.Serializable]
        public class ScorePayload { public int score; public ScorePayload(int s) { score = s; } }

        [System.Serializable]
        public class ErrorDetails { public bool username, email, password; }
        [System.Serializable]
        public class ErrorInfo { public string code, message; public ErrorDetails details; }
        [System.Serializable]
        public class ErrorResponse { public ErrorInfo error; }
        
        [System.Serializable]
        public class RegisterResponse
        {
            public bool success;
            public ErrorInfo error;
            public UserData data;
        }

        [System.Serializable]
        public class LoginResponse
        {
            public bool success;
            public ErrorInfo error;
            public UserData data;
            public string token;
        }

        [System.Serializable]
        public class UserData
        {
            public int id;
            public string username;
            public string email;
        }

        // Method to add auth header to requests
        public UnityWebRequest CreateAuthorizedRequest(string url, string method)
        {
            UnityWebRequest req = new UnityWebRequest(url, method);
            if (!string.IsNullOrEmpty(sessionToken))
            {
                req.SetRequestHeader("Authorization", "Bearer " + sessionToken);
            }
            return req;
        }

        public bool IsValidEmail(string email)
        {
            // Remove strict validation, just check if not empty
            return !string.IsNullOrEmpty(email);
        }

        public bool IsValidUsername(string username)
        {
            // Remove strict validation, just check if not empty
            return !string.IsNullOrEmpty(username);
        }

        public bool IsValidPassword(string password)
        {
            // Only check for minimum length of 7 characters
            return !string.IsNullOrEmpty(password) && password.Length >= 7;
        }

        public void RegisterUser(string username, string email, string password, System.Action onSuccess, System.Action<string> onError)
        {
            // Only validate password length
            if (!IsValidPassword(password))
            {
                onError?.Invoke("Password must be at least 7 characters long");
                return;
            }

            StartCoroutine(CheckEmailAvailability(email, (isAvailable) => {
                if (isAvailable)
                {
                    StartCoroutine(RegisterUserCoroutine(username, email, password, onSuccess, onError));
                }
                else
                {
                    onError?.Invoke("Email is already in use");
                }
            }));
        }

        IEnumerator CheckEmailAvailability(string email, System.Action<bool> callback)
        {
            string json = JsonUtility.ToJson(new EmailCheckPayload(email));
            UnityWebRequest req = new UnityWebRequest(baseUrl + "/check-email", "POST");
            byte[] body = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Email check response: {req.downloadHandler.text}");
                try
                {
                    bool isAvailable = bool.Parse(req.downloadHandler.text);
                    callback(isAvailable);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to parse email check response: {e.Message}");
                    callback(false);
                }
            }
            else
            {
                Debug.LogError($"Email check failed: {req.error}");
                HandleError(req);
                callback(false);
            }
        }

        public void LoginUser(string email, string password, System.Action<int, string, string, string> onSuccess, System.Action<string> onError)
        {
            StartCoroutine(LoginUserCoroutine(email, password, onSuccess, onError));
        }

        IEnumerator LoginUserCoroutine(string email, string password, System.Action<int, string, string, string> onSuccess, System.Action<string> onError)
        {
            string json = JsonUtility.ToJson(new LoginPayload(email, password));
            UnityWebRequest req = new UnityWebRequest(baseUrl + "/login", "POST");
            byte[] body = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonUtility.FromJson<LoginResponse>(req.downloadHandler.text);
                    if (response.success)
                    {
                        // Save the session token
                        sessionToken = response.token;
                        PlayerPrefs.SetString("SessionToken", sessionToken);
                        PlayerPrefs.SetString("Username", response.data.username);
                        PlayerPrefs.SetInt("UserID", response.data.id);
                        PlayerPrefs.Save();
                        // Call success callback
                        onSuccess?.Invoke(response.data.id, response.data.username, response.data.email, response.token);
                    }
                    else
                    {
                        // Call error callback with error message
                        onError?.Invoke(response.error != null ? response.error.message : "Login failed");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to parse login response: {e.Message}");
                    onError?.Invoke("Failed to process login response");
                }
            }
            else
            {
                // Call error callback with error message
                onError?.Invoke(req.error ?? "Unable to connect to the server. Please try again later.");
            }
        }

        // Updated submit score with authorization
        public void SubmitScore(int score, System.Action<bool> callback = null)
        {
            StartCoroutine(SubmitScoreCoroutine(score, callback));
        }
        
        IEnumerator SubmitScoreCoroutine(int score, System.Action<bool> callback)
        {
            string json = JsonUtility.ToJson(new ScorePayload(score));
            
            // Use authorized request
            UnityWebRequest req = CreateAuthorizedRequest(baseUrl + "/scores", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            
            yield return req.SendWebRequest();
            
            bool success = false;
            
            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score submitted successfully: " + score);
                success = true;
            }
            else
            {
                Debug.LogError("Error submitting score: " + req.error);
                
                // Check for auth errors
                if (req.responseCode == 401 || req.responseCode == 403)
                {
                    // Session expired, redirect to login
                    HandleSessionExpired();
                }
            }
            
            if (callback != null)
            {
                callback(success);
            }
        }
        
        // Handle expired sessions
        private void HandleSessionExpired()
        {
            // Clear token
            sessionToken = "";
            PlayerPrefs.DeleteKey("SessionToken");
            PlayerPrefs.Save();
            
            // Show message and redirect to login
            Debug.Log("Session expired. Please log in again.");
            SceneManager.LoadScene("Log In");
        }
        
        // Add logout functionality
        public void Logout()
        {
            StartCoroutine(LogoutCoroutine());
        }
        
        IEnumerator LogoutCoroutine()
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                // Already logged out
                SceneManager.LoadScene("Log In");
                yield break;
            }
            
            UnityWebRequest req = CreateAuthorizedRequest(baseUrl + "/logout", "POST");
            req.downloadHandler = new DownloadHandlerBuffer();
            
            yield return req.SendWebRequest();
            
            // Clear token regardless of server response
            sessionToken = "";
            PlayerPrefs.DeleteKey("SessionToken");
            PlayerPrefs.DeleteKey("Username");
            PlayerPrefs.DeleteKey("UserID");
            PlayerPrefs.Save();
            
            SceneManager.LoadScene("Log In");
        }
        
        // Validate current session
        public IEnumerator ValidateSession(System.Action<bool> callback)
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                callback(false);
                yield break;
            }
            
            // Use a simple endpoint that just validates the token
            UnityWebRequest req = CreateAuthorizedRequest(baseUrl + "/validate-session", "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            
            yield return req.SendWebRequest();
            
            bool isValid = (req.result == UnityWebRequest.Result.Success);
            callback(isValid);
        }

        void HandleError(UnityWebRequest req)
        {
            try
            {
                ErrorResponse err = JsonUtility.FromJson<ErrorResponse>(req.downloadHandler.text);
                string errorMessage = $"Error: {err.error.message}";
                if (err.error.details != null)
                {
                    if (err.error.details.email) errorMessage += "\n- Invalid email";
                    if (err.error.details.password) errorMessage += "\n- Invalid password";
                    if (err.error.details.username) errorMessage += "\n- Invalid username";
                }
                Debug.LogError(errorMessage);
            }
            catch
            {
                Debug.LogError("An unexpected error occurred");
            }
        }

        IEnumerator RegisterUserCoroutine(string username, string email, string password, System.Action onSuccess, System.Action<string> onError)
        {
            string json = JsonUtility.ToJson(new RegisterPayload(username, email, password));
            Debug.Log($"Attempting to register with payload: {json}");
            
            UnityWebRequest req = new UnityWebRequest(baseUrl + "/register", "POST");
            byte[] body = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            Debug.Log($"Registration response status: {req.result}");
            Debug.Log($"Registration response: {req.downloadHandler.text}");

            if (req.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonUtility.FromJson<RegisterResponse>(req.downloadHandler.text);
                    if (response.success)
                    {
                        Debug.Log("Registration Success!");
                        onSuccess?.Invoke();
                    }
                    else
                    {
                        Debug.LogError($"Registration failed: {response.error.message}");
                        onError?.Invoke(response.error.message);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to parse registration response: {e.Message}");
                    onError?.Invoke("Failed to process registration response");
                }
            }
            else
            {
                Debug.LogError($"Registration request failed: {req.error}");
                onError?.Invoke(req.error ?? "Unable to connect to the server. Please try again later.");
            }
        }
    }
}