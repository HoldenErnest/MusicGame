using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    private AudioSource audioSource;

    private float[] audioInfoRaw = new float[4096]; // info passed into this array is based on the current songs USE 8192 or 4096

    private float lastUpdatedTime = 0f;

    SpectrogramGen specGen;
    public PlayableTileManager ptm;

    void OnValidate() {

    }


    void Start() {

        specGen = new SpectrogramGen();

        audioSource = GetComponent<AudioSource>();

        NoteComputation.init(); // IMPORTANT
    }

    void Update() {
        updateSpectrumData();
    }

    void updateSpectrumData() {
        AudioListener.GetSpectrumData(audioInfoRaw, 0, FFTWindow.Hanning);

        if (Time.time - lastUpdatedTime >= 0.0166f) { // update the notes 60 times a second
            updateAllFromData();
            lastUpdatedTime = Time.time;
            
        }
    }
    private void updateAllFromData() {
        NoteComputation.updateOctaves(ref audioInfoRaw);
        ptm.updateTiles();
        specGen.drawLine(ref audioInfoRaw);
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

    /// <summary>
    /// PCM data, UNUSED
    /// </summary>
    /// <param name="data"></param>
    /// <param name="channels"></param>
    private void OnAudioFilterRead(float[] data, int channels) {
        // DO NOT USE, unless you want pcm data
    }
}
