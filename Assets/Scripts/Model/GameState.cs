using UnityEngine;

namespace GameStatePattern
{
    public interface IGameState
    {
        void Enter(GameManager gameManager);
        void Update(GameManager gameManager);
        void Exit(GameManager gameManager);
    }

    public class PlayingState : IGameState
    {
        public void Enter(GameManager gameManager)
        {
            gameManager.isGameActive = true;
            if (gameManager.pausePanel != null)
                gameManager.pausePanel.SetActive(false);
        }

        public void Update(GameManager gameManager)
        {
            // Game logic while playing (could be expanded)
        }

        public void Exit(GameManager gameManager)
        {
            // Cleanup or pause logic if needed
        }
    }

    public class GameOverState : IGameState
    {
        public void Enter(GameManager gameManager)
        {
            gameManager.isGameActive = false;
            if (gameManager.gameOverView != null)
            {
                gameManager.gameOverView.Show(gameManager.score);
            }
        }

        public void Update(GameManager gameManager)
        {
            // Game over logic (wait for restart, etc.)
        }

        public void Exit(GameManager gameManager)
        {
            if (gameManager.gameOverView != null)
            {
                gameManager.gameOverView.Hide();
            }
        }
    }

    public class PauseState : IGameState
    {
        public void Enter(GameManager gameManager)
        {
            Time.timeScale = 0f;
            if (gameManager.pausePanel != null)
                gameManager.pausePanel.SetActive(true);
            Debug.Log("Game Paused");
        }

        public void Update(GameManager gameManager)
        {
            // Pause menu logic or listen for unpause
        }

        public void Exit(GameManager gameManager)
        {
            Time.timeScale = 1f;
            if (gameManager.pausePanel != null)
                gameManager.pausePanel.SetActive(false);
            Debug.Log("Game Resumed");
        }
    }

    public class GameStateContext
    {
        private IGameState currentState;
        private GameManager gameManager;

        public GameStateContext(GameManager manager)
        {
            gameManager = manager;
        }

        public void SetState(IGameState newState)
        {
            if (currentState != null)
            {
                currentState.Exit(gameManager);
            }
            currentState = newState;
            if (currentState != null)
            {
                currentState.Enter(gameManager);
            }
        }

        public void Update()
        {
            if (currentState != null)
            {
                currentState.Update(gameManager);
            }
        }
    }
} 