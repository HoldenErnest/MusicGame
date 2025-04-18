using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text incorrectText;

    void Start() {
        Controller.scoreManager = this;
    }

    public void UpdateScore(int points, int totalPoints) {
        int percent = (int)(((points * 1f) / totalPoints) * 100);
        scoreText.text = "Score: " + points + "/" + totalPoints + " (" + percent + "%)";
    }
    public void UpdateIncorrect(int incorrect) {
        incorrectText.text = "Incorrect: " + incorrect;
    }
}
