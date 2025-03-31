// Holden Ernest - 3/31/2025
// with the input of a frequency range that represents this single note, determine the core information about it

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Note {
    int noteNum = 0; // A B C D E F G (0-11)  WITH # and b
    private float maxAmplitude = 0;
    private float totalAmplitude = 0;
    private int totalFreqCount = 0;

    private int absoluteStartIndex = -1; // the index in rawAudioData[] at which this notes frequencies begin

    public Note(int noteNumber, IEnumerable<float> frequencyRange, int absoluteStartIndex) {
        noteNum = noteNumber;
        this.absoluteStartIndex = absoluteStartIndex;
        foreach (float amplitude in frequencyRange) {
            if (amplitude > maxAmplitude) {
                maxAmplitude = amplitude;
            }
            totalAmplitude += amplitude;
            totalFreqCount++;
        }
    }

    public float getMaxAmplitude() {
        return maxAmplitude;
    }
    public float getAvgAmplitude() {
        return totalAmplitude / totalFreqCount;
    }

    public override string ToString() {
        char note = (char)('A' + noteNum);
        return note + ": " + maxAmplitude;
    }

    // DRAWABLE functions
    public void drawNoteFull(Texture2D tex, float currentAlpha) {
        if (totalFreqCount <= 0) return;
        for (int y = absoluteStartIndex; y < absoluteStartIndex + totalFreqCount; y++) {
            float alt = noteNum % 2 == 1 ? 0.25f : 0f;
            for (int x = 0; x < tex.width; x++) {
                Color d = tex.GetPixel(x,y);
                d.a = currentAlpha - alt;
                tex.SetPixel(x,y,d);
            }
        }
    }

}
