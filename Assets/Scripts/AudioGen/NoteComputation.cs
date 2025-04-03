// Holden Ernest - 3/31/2025
//This class should handle all the note computing 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoteComputation {
    
    public static readonly int TOTAL_BUFFERS = 3; // buffers have been moved to the 'Note' class level and are deeply rooted (each note holds its own buffer of preceding notes)
    private static Octave[] octaves = new Octave[9];
    public static float maxFrequency = 0.05f;
    public static float currentMaxFreq = 0f;

    public static void init() {
        for (int i = 0; i < 9; i++) {
            octaves[i] = Octave.nullOctave;
        }
    }

    public static void updateOctaves(ref float[] data) { // update the current octaves, push the previous one to the queue

        if (octaves[0].isNull()) {
            for (int i = 0; i < 9; i++) {
                octaves[i] = new Octave(i, ref data);
            }
        } else {
            for (int i = 0; i < 9; i++) {
                octaves[i].updateOctave(octaves[i], ref data);
            }
        }

        activateCurrentNotes();
    }

    private static void activateCurrentNotes() {
        for (int i = 0; i < octaves.Length; i++) {
            if (i == 0) {
                octaves[i].activateNotes(Octave.nullOctave, octaves[i+1]);
            } else if (i == octaves.Length - 1) {
                octaves[i].activateNotes(octaves[i-1], Octave.nullOctave);
            } else {
                octaves[i].activateNotes(octaves[i-1], octaves[i+1]);
            }

        }
    }


    //DRAW texture processing -- THESE THINGS SHOULD BE CALLED FROM SpectrogramGen, it handles all the drawing
    public static void postOctaveViewer(ref Texture2D tex, bool drawNotes) { // horizontal lines to show octaves
        foreach (Octave o in octaves) {
            o.drawOctaveFull(ref tex, drawNotes);
        }
    }
    public static void drawCurrentNotes(ref Texture2D tex, int lineNumber) {
        foreach (Octave o in octaves) {
            o.drawCurrentNotes(ref tex, lineNumber);
        }
    }
}
