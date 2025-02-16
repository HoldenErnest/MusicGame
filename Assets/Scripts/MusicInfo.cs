// Holden Ernest - 1/23/2025

// contains all the interesting data that will be captured from a playing song

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicInfo : MonoBehaviour {
    
    float[] freqPeaks; // an array of the "loudest" frequencies
    int[] freqPeakIndex;
    private int totalFreqPeakSamples = 16;

    public MusicInfo() {
        freqPeaks = new float[totalFreqPeakSamples];
        freqPeakIndex = new int[totalFreqPeakSamples];
        initFreqPeaks();
    }

    void initFreqPeaks() {
        for (int i = 0; i < totalFreqPeakSamples; i++) {
            freqPeaks[i] = 0;
            freqPeakIndex[i] = -1;
        }
    }

    public void updateSnapshot(float[] rawData, GameObject[] scales) { // update any info needed based on the current raw data
        updateFreqPeaksVisual(scales);
        initFreqPeaks();
    }

    private void updateFreqPeaks(float[] rawData) {
        for (int i = 0; i < rawData.Length; i++) {
            for (int j = 0; j < totalFreqPeakSamples; j++) { // compare with every highest, from highest to lowest

                if (rawData[i] > freqPeaks[j]) { // if this raw data is more than the currently selected peak
                    for (int k = totalFreqPeakSamples-2; k >= j; k--) { // push all the peaks back 1, from that j position
                        Debug.Log(k);
                        freqPeaks[k+1] = freqPeaks[k];
                        freqPeakIndex[k+1] = freqPeakIndex[k];
                    }
                    freqPeaks[j] = rawData[i];
                    freqPeakIndex[j] = i;
                    break;
                }

            }
        }
    }
    private void updateFreqPeaksVisual(GameObject[] scales) {
        for (int i = 0; i < scales.Length; i++) {
            for (int j = 0; j < totalFreqPeakSamples; j++) { // compare with every highest, from highest to lowest
                float currentSize = scales[i].transform.localScale.y;
                if (currentSize > freqPeaks[j]) { // if this raw data is more than the currently selected peak
                    for (int k = totalFreqPeakSamples-2; k >= j; k--) { // push all the peaks back 1, from that j position
                        freqPeaks[k+1] = freqPeaks[k];
                        freqPeakIndex[k+1] = freqPeakIndex[k];
                    }
                    freqPeaks[j] = currentSize;
                    freqPeakIndex[j] = i;
                    break;
                }

            }
        }
        updateScales(scales);
    }
    private void updateScales(GameObject[] scales) { // eventually this will probably be removed .. dont care about the visual part of the actual data
        for (int i = 0; i < totalFreqPeakSamples; i++) {
            int index = freqPeakIndex[i];
            if (index == -1) continue;
            SpriteRenderer sr = scales[index].GetComponent<SpriteRenderer>();
            float val = 1;//(totalFreqPeakSamples - i * 1.0f) / totalFreqPeakSamples;
            float freq = MusicManager.frequencyFromScaleIndex(index);
            if (freq >= 2000 && freq <= 5000) {
                sr.color = new Color(0, val,val);
            }
        }
    }

}
