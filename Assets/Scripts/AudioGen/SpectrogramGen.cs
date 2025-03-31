// Holden Ernest - 3/28/2025
// generates a spectrogram based on raw audio frequency data (this data must be passed into drawLine each frame)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class SpectrogramGen : MonoBehaviour
{
    private Texture2D texture;

    private bool endofTexture = false;
    private int currentLine = 0;

    private int height = 0;
    private int width = 2048; // number of time samples // if you run at 60 lines a second with 1024 lines, it will take ~17 seconds

    private readonly float maxFreq = 1f;

    //TEMP----------
    Octave[] octaves = new Octave[9];

    public SpectrogramGen() {

    }

    float max = 0.1f;

    public void drawLine(ref float[] data) { // draws a single line of the spectrogram representing a moment in time
        if (texture == null) resetTexture(data.Length/2);
        if (endofTexture) return;

        //max = Mathf.Max(data);

        for (int i = 0; i < height; i++) {
            float amplitude = data[i] / max;
            amplitude = Mathf.Pow(amplitude, 0.2f); // increase the brightness of the color

            Color c = colorFromAmp2(amplitude);

            texture.SetPixel(currentLine, i, c);
            
            if (data[i] > max) max = data[i];

        }
        currentLine++;
        if (currentLine > width) {
            endofTexture = true;
            postProcessTexture(ref data);
            texture.Apply();
            Debug.Log("MAX was " + max);
            saveImage();
        }
        
    }

    private Color colorFromAmp1(float amplitude) {
        Color c;
        if (amplitude < 0.33f) {
            c = new Color(0, 0, 3 * amplitude); // from black to blue
        } else if (amplitude < 0.66f) {
            c = new Color(3 * (amplitude - 0.33f), 0, 1f - (3 * (amplitude - 0.33f))); // from blue to red
        } else {
            c = new Color(1, 3 * (amplitude - 0.66f), 3 * (amplitude - 0.66f)); // from red to white
        }
        return c;
    }
    private Color colorFromAmp2(float amplitude) {
        Color c;
        if (amplitude < 0.25f) {
            c = new Color(0, 0, 3 * amplitude); // black to 75% blue
        } else if (amplitude < 0.5f) {
            c = new Color(4 * (amplitude - 0.25f), 0, 0.75f); // 75% blue to magenta(75% red & blue)
        } else if (amplitude < 0.75f) {
            c = new Color(1f, 2 * (amplitude - 0.5f), 0.75f - (3 * (amplitude - 0.5f))); // magenta to orange
        } else {
            c = new Color(1f, 0.5f + (2 * (amplitude - 0.75f)), 4 * (amplitude - 0.75f)); // orange to white
        }
        return c;
    }

    private void postProcessTexture(ref float[] data) { // run calculations on the result / after touchups
        postOctaveViewer(true, ref data);
    }
    private void postOctaveViewer(bool drawNotes, ref float[] data) { // horizontal lines to show octaves
        //TEMP---------
        for (int i = 0; i < 9; i++) {
            octaves[i] = new Octave(i, ref data);
        }

        foreach (Octave o in octaves) {
            o.drawOctaveFull(texture, drawNotes);
        }
    }
    private void DEPRCpostOctaveViewer() {
        for (int i = 0; i < height; i++) {
            float hz = DEPRCheightInHz(i);
            Color c = Color.black;
            if (hz < 31) {
                c.a = 1f;
                texture.GetPixel(0,i);
                
            } else if (hz < 62) {
                c.a = 0.5f;
            } else if (hz < 124) {
                c.a = 1f;
            } else if (hz < 247) {
                c.a = 0.5f;
            } else if (hz < 494) {
                c.a = 1f;
            } else if (hz < 987) {
                c.a = 0.5f;
            } else if (hz < 1976) {
                c.a = 1f;
            } else if (hz < 3951) {
                c.a = 0.5f;
            } else if (hz < 7902) {
                c.a = 1f;
            } else {
                c.a = 0.1f;
            }

            for (int x = 0; x < width; x++) {
                Color d = texture.GetPixel(x,i);
                d.a = c.a;
                texture.SetPixel(x,i,d);
            }
        }
    }

    private float DEPRCheightInHz(int h) { // returns a frequency value in hz from a given height value
        // top is 20khz, bottom is 20hz
        return 20 + (9.75f * h);
    }

    private double getRMS(ref float[] data) {
        double square = 0;
        double mean = 0;
        double root = 0;
        int n = data.Length/2;

        for (int i = 0; i < n; i++)
        {
            square += Math.Pow(data[i], 2);
        }

        mean = square / n;
        root = Math.Sqrt(mean);

        return root;
    }

    void resetTexture(int h) {
        this.height = h;
        texture = new Texture2D(width, this.height, TextureFormat.RGBA32, false);
    }


    private void saveImage() {
        string path = Application.persistentDataPath + "/a.png";
        byte[] bs = texture.EncodeToPNG();
        File.WriteAllBytes(path, bs);
        Debug.Log("Saved file to " + path);
    }
}
