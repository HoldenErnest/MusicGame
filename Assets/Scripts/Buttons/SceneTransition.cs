using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public void loadSceneGame() { // event
        updateGlobals();
        SceneManager.LoadScene("SampleScene");
    }
    public void loadSceneMenu() { // event
        updateGlobals();
        SceneManager.LoadScene("MenuScene");
    }

    private void updateGlobals() {
        Time.timeScale = 1f;
    }
}
