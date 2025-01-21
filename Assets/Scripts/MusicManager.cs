using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    private AudioSource audioSource;

    private bool fetchRawAudio = true; //used only so that the audio listener isnt used more than once

    private float[] audioInfoRaw = new float[2048]; // info passed into this array is based on the current songs [0-2047] L[0-1023] + R[1023-2047]
    private float[] scalesInfo;
    public int totalScales = 8;

    public GameObject baseScale; // display garbage
    private GameObject[] allScales; // display garbage
    private int screenWidth = 20; // display garbage

    public GameObject viewer; // display to see random detected things

    
    private int updateTick = 0;
    public int updateRate = 20;
    public float transitionSpeed = 0.1f;
    public float scaleSize = 0.1f;

    void Start() {
        scalesInfo = new float[totalScales];
        audioSource = GetComponent<AudioSource>();
        initScales();
    }

    void initScales() {
        allScales = new GameObject[totalScales];
        for (int i = 0; i < totalScales; i++) {
            GameObject clone = Instantiate(baseScale);
            float x = ((i+0.5f) * ((float)screenWidth/totalScales)) - screenWidth/2;
            clone.transform.position = new Vector3(x,0,0);
            allScales[i] = clone;
        }
    }

    void Update() {
        //AudioListener.GetSpectrumData(audioInfoRaw, 0, FFTWindow.Rectangular);

        updateScales();
    }

    void updateScales() {
        if (audioInfoRaw == null) return;

        updateScalesInfo(); // only update the scaler values when you need, for smooth transition
        float prevScaleSize = 0;
        int numIncScales = 0;
        float totalHeight = 0;
        for (int i = 0; i < totalScales; i++) {
            // always update the scale even if its value isnt the newest.
            
            Vector3 newScale = new Vector3 (scaleSize,scalesInfo[i],0);
            allScales[i].transform.localScale = Vector3.Lerp(allScales[i].transform.localScale, newScale, transitionSpeed);
            // ^ update the scale

            float thisScaleSize = MathF.Abs(allScales[i].transform.localScale.y);
            totalHeight += thisScaleSize;

            if (thisScaleSize > 0.2f) { // if this scale is bigger than the last, its the start of a group of scales
                numIncScales++;
            } else {
                numIncScales = 0;
            }
            prevScaleSize = thisScaleSize;

            SpriteRenderer sr = allScales[i].GetComponent<SpriteRenderer>();
            sr.color = new Color(numIncScales, 0,0);
            
        }
        double totalEnergy = getRMS();
        viewer.GetComponent<SpriteRenderer>().color = new Color((float)totalEnergy, 0, 0);
        viewer.transform.localScale = new Vector3((float)totalEnergy+1,(float)totalEnergy+1,1);
    }

    void updateScalesInfo() { // update scalers based on the current raw audio info
        if (updateTick > 0) {
            updateTick--;
            return;
        }
        updateTick = updateRate;

        for (int i = 0; i < totalScales; i++) { // update all scales
            int recordNumber = i * ((audioInfoRaw.Length/2)/totalScales);
            float newScale = audioInfoRaw[recordNumber] * 10;
            scalesInfo[i] = newScale;
        }
    }

    private double getRMS() {
        double square = 0;
        double mean = 0;
        double root = 0;
        int n = audioInfoRaw.Length/2;

        for (int i = 0; i < n; i++)
        {
            square += Math.Pow(audioInfoRaw[i], 2);
        }

        mean = square / n;
        root = Math.Sqrt(mean);

        return root;
    }


    private void OnAudioFilterRead(float[] data, int channels) {
        if (!fetchRawAudio) return;
        audioInfoRaw = data;
        fetchRawAudio = false;
    }
}
