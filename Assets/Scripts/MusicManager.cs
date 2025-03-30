using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    private AudioSource audioSource;

    public bool showAsFreqDomain = false; // show this as frequency domain or time domain audio

    private bool fetchRawAudio = true; //used only so that the audio listener isnt used more than once

    private float[] audioInfoRaw = new float[2048]; // info passed into this array is based on the current songs [0-2047] L[0-1023] + R[1023-2047]
    private float[] scalesInfo;
    public int totalScales = 8;

    public GameObject baseScale; // display garbage
    private GameObject[] allScales; // display garbage
    private float screenWidth = 19.8f; // display garbage

    public GameObject viewer; // display to see random detected things

    private MusicInfo musicOutput;

    
    private int updateTick = 0;
    public int updateRate = 20;
    public float transitionSpeed = 0.1f;
    public float scaleSize = 0.1f;

    SpectrogramGen specGen;


    void Start() {
        specGen = new SpectrogramGen();

        scalesInfo = new float[totalScales];
        audioSource = GetComponent<AudioSource>();
        initScales();
        musicOutput = new MusicInfo();
    }

    void initScales() {
        allScales = new GameObject[totalScales];
        for (int i = 0; i < totalScales; i++) {
            GameObject clone = Instantiate(baseScale);
            float x = ((i+0.5f) * ((float)screenWidth/totalScales)) - screenWidth/2.0f;
            clone.transform.position = new Vector3(x,0,0);
            allScales[i] = clone;
        }
    }

    void Update() {
        updateScales();
    }

    void updateScales() {
        if (audioInfoRaw == null) return;
        if (showAsFreqDomain) {
            updateScalesInfoFREQ(); // only update the scaler values when you need, for smooth transition
        } else {
            updateScalesInfoPCM(); // only update the scaler values when you need, for smooth transition
        }
        transitionAllScales();
    }

    void transitionAllScales() {
        int numIncScales = 0;
        float totalHeight = 0;
        for (int i = 0; i < totalScales; i++) {
            // always update the scale even if its value isnt the newest.
            
            Vector3 newScale = new Vector3 (scaleSize,Time.deltaTime * 500 * scalesInfo[i],0);
            allScales[i].transform.localScale = Vector3.Lerp(allScales[i].transform.localScale, newScale, transitionSpeed);
            // ^ update the scale

            // RANDOM EFFECTS :: COLORS whatnot

            float thisScaleSize = MathF.Abs(allScales[i].transform.localScale.y);
            totalHeight += thisScaleSize;

            if (thisScaleSize > 0.2f) { // if this scale is bigger than the last, its the start of a group of scales
                numIncScales++;
            } else {
                numIncScales = 0;
            }

            SpriteRenderer sr = allScales[i].GetComponent<SpriteRenderer>();
            sr.color = new Color(numIncScales, 0,0);

            float freq = frequencyFromScaleIndex(i);

            if (freq >= 500 && freq <= 2000) {
                sr.color = Color.green;
            }
            
        }
        renderweirdBoxThing();
        musicOutput.updateSnapshot(audioInfoRaw, allScales);
    }

    void updateScalesInfoFREQ() {
        AudioListener.GetSpectrumData(audioInfoRaw, 0, FFTWindow.Rectangular);
        specGen.drawLine(ref audioInfoRaw);
        if (updateTick > 0) {
            updateTick--;
            return;
        }
        updateTick = updateRate;
        
        for (int i = 0; i < totalScales; i++) { // go through all scales
            //Debug.Log((int)Mathf.Floor(Mathf.Pow(i,2) * (16.0f/2048))); // THIS IS A RANDOM CURVE I CAME UP WITH (20khz is much less freq than ~100hz)
            int recordNumber = recordIndexFromScaleIndex(i);
            if (recordNumber < i) recordNumber = i;
            float newScale = audioInfoRaw[recordNumber] * 20; // index: 0-2047, freq: 20-20k (tested this). Each record is about 9.75hz higher than the previous
            scalesInfo[i] = newScale;
        }
    }
    public static int recordIndexFromScaleIndex(int index) { // takes in values from 0-512 outputs from 0-2048
        int recordNumber = (int)Mathf.Floor(Mathf.Pow(index,2) * (16.0f/2048));
        return recordNumber > index? recordNumber: index;
        //index * (audioInfoRaw.Length/totalScales); // for equalized distribution
    }
    public static int scaleIndexFromRecordIndex(int index) { // inverse function of recordIndexFromScaleIndex
        return (int)Mathf.Floor(Mathf.Sqrt(index * (2048/16.0f)));
    }
    public static float frequencyFromScaleIndex(int index) { // input: 0-512, output: 20-20k
        return (recordIndexFromScaleIndex(index) * 9.755f) + 20; // 9.75 is the equalized distribution distance
    }

    void renderweirdBoxThing() {
        double totalEnergy = getRMS();
        viewer.GetComponent<SpriteRenderer>().color = new Color((float)totalEnergy, 0, 0);
        viewer.transform.localScale = new Vector3((float)totalEnergy+1,(float)totalEnergy+1,1);
    }

    void updateScalesInfoPCM() { // update scalers based on the current raw audio info
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
        if (!fetchRawAudio || showAsFreqDomain) return;
        audioInfoRaw = data;
        fetchRawAudio = false;
    }
}
