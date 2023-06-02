using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float totalTime;
    public TextMeshProUGUI timerText;
    
    private float startTime;
    public bool counting;
    public bool gameover;

    public GameObject gameoverScreen;

    void Start() {
        Time.timeScale = 1;
        gameover = false;
        counting= false;

        StartTimer();
    }

    void StartTimer() {
        startTime = Time.time;
        counting = true;
    }

    void Update() {
        if(counting && !gameover && ((Time.time - startTime) <= totalTime)) {
            timerText.text = TimeToString();

        }

        if(!gameover && (Time.time - startTime) >= totalTime) {
            // Game over
            gameover = true;
            Time.timeScale = 0;
            Instantiate(gameoverScreen);
        }
    }

    string TimeToString() {
        float time = totalTime - (Time.time - startTime);
        float seconds = time % 60;
        float minutes = ((time-seconds) / 60);
        return Mathf.FloorToInt(time / 60) + ":" + ((int)(time % 60)).ToString("d2");
    }

}
