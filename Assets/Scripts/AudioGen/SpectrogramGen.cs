using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpectrogramGen : MonoBehaviour
{
    private Texture2D texture;

    private bool endofTexture = false;
    private int currentLine = 0;

    private int height = 0;
    private int width = 4096; // number of time samples

    private readonly float maxFreq = 1f;

    public SpectrogramGen() {
    }

    float max = 0.1f;

    public void drawLine(ref float[] data) { // draws a single line of the spectrogram representing a moment in time
        if (texture == null) resetTexture(data.Length/2);
        if (endofTexture) return;

        //max = Mathf.Max(data);
        
        for (int i = 0; i < height; i++) {
            float amplitude = data[i];// / max;
            amplitude = Mathf.Pow(amplitude, 0.2f); // increase the brightness of the color

            Color c = colorFromAmp2(amplitude);

            texture.SetPixel(currentLine, i, c);

            if (data[i] > max) max = data[i];
            
        }
        currentLine++;
        if (currentLine > width) {
            endofTexture = true;
            saveImage();
        }

        texture.Apply();
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
