// Holden Ernest - 3/28/2025
// generates a spectrogram based on raw audio frequency data (this data must be passed into drawLine each frame)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.ShaderGraph;
using UnityEngine;
using System.Linq;

public class SpectrogramGen : MonoBehaviour
{
    private Texture2D texture;

    private bool endofTexture = false;
    private int currentLine = 0;

    private int height = 0;
    private readonly int width = 1024; // number of time samples // if you run at 60 lines a second with 1024 lines, it will take ~17 seconds

    public SpectrogramGen() {

    }

    // START DRAWABLE functions
    public void drawLine(ref float[] data) {
        // draws a single line of the spectrogram representing a moment in time
        if (texture == null) resetTexture(data.Length/2);
        if (endofTexture) return;

        // CORE COMPUTATIONS -- this should be moved later, not in the scope of drawing
        NoteComputation.updateOctaves(ref data);
        // CORE COMPUTATIONS -- this should be moved later

        drawCurrentLine(ref data);
        drawCurrentNotes();

        currentLine++;
        if (currentLine > width) {
            endofTexture = true;
            postProcessTexture();
            texture.Apply();
            saveImage();
        }
    }
    public void drawCurrentLine(ref float[] data) {
        for (int i = 0; i < height; i++) {
            float amplitude = data[i] / NoteComputation.maxFrequency;
            amplitude = Mathf.Pow(amplitude, 0.2f); // increase the brightness of the color

            Color c = colorFromAmp2(amplitude);

            texture.SetPixel(currentLine, i, c);
            
            if (data[i] > NoteComputation.maxFrequency) NoteComputation.maxFrequency = data[i];

        }
    }
    public void drawCurrentNotes() {
        NoteComputation.drawCurrentNotes(ref texture, currentLine);
    }

    private void postProcessTexture() { // run calculations on the result / after touchups
        NoteComputation.postOctaveViewer(ref texture, true);
    }
    // END DRAWABLE functions


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
            c = new Color(4 * (amplitude - 0.25f), 0, 0.75f); // 75% blue to magenta(red & 75% blue)
        } else if (amplitude < 0.75f) {
            c = new Color(1f, 2 * (amplitude - 0.5f), 0.75f - (3 * (amplitude - 0.5f))); // magenta to orange
        } else {
            c = new Color(1f, 0.5f + (2 * (amplitude - 0.75f)), 4 * (amplitude - 0.75f)); // orange to white
        }
        return c;
    }


    private void resetTexture(int h) {
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
