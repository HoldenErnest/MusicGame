using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicScalable : MonoBehaviour
{
    public GameObject forgroundScale;
    public GameObject backgroundScale;

    private float maxHeight = 7f;
    public static float lerpScale = 0.2f;

    void Start() {
        
    }
    /// <summary>
    /// percent can be 0.3 representing, 1/3 of the bar up to max height
    /// </summary>
    /// <param name="percent"></param>
    public void updateAmplitude(float percent) {
        Vector3 prev = forgroundScale.transform.localScale;
        float wantedHeight = percent * maxHeight;
        float newHeight = Mathf.Lerp(prev.y, wantedHeight, lerpScale);
        float ran = UnityEngine.Random.value;
        ran -= 1.0f/2;
        if (percent == 1) {
            newHeight = maxHeight + ran;
        }
        forgroundScale.transform.localScale = new Vector3(prev.x, newHeight, prev.z);

        Vector3 prevBack = backgroundScale.transform.localScale;

        backgroundScale.transform.localScale = new Vector3(prev.x + (0.1f * newHeight), newHeight + (0.4f * newHeight), prev.z);
    }

    public void updateColor(Color c) {
        forgroundScale.GetComponent<SpriteRenderer>().color = c;
        backgroundScale.GetComponent<SpriteRenderer>().color = new Color(c.r,c.g,c.b,0.5f);
    }
}
