using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public SpriteRenderer backColor;
    public MusicScalable[] scales;

    private float lastUpdatedTime = 0f;
    private int chosenIndex = 0;
    private bool up = true;

    public Color a = Color.blue;
    public Color b = Color.green;
    private Color col;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastUpdatedTime >= 0.08f) {
            updateColor();
            updateScales();
            
            lastUpdatedTime = Time.time;
        }
        
    }


    private void updateScales() {
        backColor.color = new Color(col.r,col.g,col.b,0.1f);
        for (int i = 0; i < scales.Length; i++) {
            scales[i].updateColor(col);
            if (i == chosenIndex) {
                scales[i].updateAmplitude(1f);
            } else {
                scales[i].updateAmplitude(0);
            }
        }
        if (up) {
            if (chosenIndex < scales.Length - 1) {
                chosenIndex++;
            } else {
                up = false;
            }
        } else {
            if (chosenIndex > 0) {
                chosenIndex--;
            } else {
                up = true;
            }
        }
    }
    
    private float colLerp = 0f;
    private bool colUp = true;
    private static float colorTransition = 0.005f;
    private void updateColor() {
        col = Color.Lerp(a,b, colLerp);
        if (colUp) {
            if (colLerp < 1f) {
                colLerp += colorTransition;
            } else {
                colUp = false;
            }
        } else {
            if (colLerp > 0) {
                colLerp -= colorTransition;
            } else {
                colUp = true;
            }
        }
    }
}
