// Holden Ernest - 3/31/2025
//This class should handle all the note computing 

using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public static class NoteComputation {
    
    public static readonly int TOTAL_BUFFERS = 5; // buffers have been moved to the 'Note' class level and are deeply rooted (each note holds its own buffer of preceding notes)
    private static Octave[] octaves = new Octave[9];
    public static float maxFrequency = 0.05f;
    public static float currentMaxFreq = 0f;

    public static int totalFreqs = -1;

    public static void init() {
        for (int i = 0; i < 9; i++) {
            octaves[i] = Octave.nullOctave;
        }
    }
    private static void initOctaveCalc(ref float[] data) {
        currentMaxFreq = 0f;
        for (int i = 0; i < totalFreqs; i++) {
            if (data[i] > maxFrequency) maxFrequency = data[i]; 
            if (data[i] > currentMaxFreq) currentMaxFreq = data[i];
        }
    }
    public static void updateOctaves(ref float[] data) { // update the current octaves, push the previous one to the queue
        initOctaveCalc(ref data);
        if (octaves[0].isNull()) {
            for (int i = 0; i < 9; i++) {
                octaves[i] = new Octave(i, ref data);
                if (i == 8){ //update max index
                    totalFreqs = octaves[i].getAbsoluteEndIndex()+1;
                }
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

    public static List<Note> getActiveNoteData() {
        List<Note> noteData = new List<Note>();
        foreach (Octave oc in octaves) {
            oc.getActiveNoteData(ref noteData);
        }
        return noteData;
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
