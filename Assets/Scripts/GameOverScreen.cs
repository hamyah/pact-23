using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public void MainMenu() {
        SceneManager.LoadScene(0);
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetScore(int score) {
        scoreText.text = score.ToString();
    }

    public void StartGame() {
        SceneManager.LoadScene(1);
    }

    public void LoadScene(int index) {
        SceneManager.LoadScene(index);
    }
}
