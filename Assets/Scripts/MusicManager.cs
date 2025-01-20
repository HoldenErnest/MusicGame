using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    private AudioSource audio;

    private bool fetchRawAudio = true; //used only so that the audio listener isnt used more than once

    private float[] audioInfoRaw; // info passed into this array is based on the current songs [0-2047] L[0-1023] + R[1023-2047]
    private float[] scalesInfo;
    public int totalScales = 8;

    public GameObject baseScale; // display garbage
    private GameObject[] allScales; // display garbage
    private int screenWidth = 20; // display garbage

    
    private int updateTick = 0;
    public int updateRate = 20;
    public float transitionSpeed = 0.1f;
    public float scaleSize = 0.1f;

    void Start() {
        scalesInfo = new float[totalScales];
        audio = GetComponent<AudioSource>();
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
        updateScales();
    }

    void updateScales() {
        if (audioInfoRaw == null) return;

        updateScalesInfo(); // only update the scaler values when you need, for smooth transition

        for (int i = 0; i < totalScales; i++) {
            // always update the scale even if its value isnt the newest.
            
            Vector3 newScale = new Vector3 (scaleSize,scalesInfo[i],0);
            allScales[i].transform.localScale = Vector3.Lerp(allScales[i].transform.localScale, newScale, transitionSpeed);
        }
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


    private void OnAudioFilterRead(float[] data, int channels) {
        if (!fetchRawAudio) return;
        audioInfoRaw = data;
        fetchRawAudio = false;
    }
}
