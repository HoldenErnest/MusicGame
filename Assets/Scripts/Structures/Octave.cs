// Holden Ernest - 3/31/2025
// given an input frequency range, determine the specific notes which can be use later (octave is mostly just an intermediate grouping step for orgainization)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Octave {
    private Note[] notes; // this holds all 12 notes, including # and b
    private int ocNum = 0; // which octave is this (0-8)

    private int absoluteStartIndex = -1; // the index in rawAudioData[] at which this octaves frequencies begin
    private int absoluteEndIndex = -1;

    public static readonly Octave nullOctave = new NullOctave();
    public Octave() {
        
    }
    public Octave(int octaveNumber, ref float[] data) {
        notes = new Note[12];
        ocNum = octaveNumber;

        // the data cannot just be linearly interpolated. Ref this chart: https://mixbutton.com/music-tools/frequency-and-pitch/music-note-to-frequency-chart

        // a given note can be determined by 2^(o + (n/12)) * 16.35
        // where o is the Octave(0-8) and n is the Note(0-11). f(o,n)

        // these frequencies need a bit of an offset so each frequency isnt splitting between two notes.
        // A=2,B=4,C=8, you would want B's frequency range to be between 3 and 6 or so

        //TODO: IMPORTANT! this index info can be cached into a note array.

        float startFreq = getFreqFromNote(ocNum-1, 11.5f);
        float endFreq = getFreqFromNote(ocNum, 11.5f);

        // these represent the beginning and ending indicies for this octave
        absoluteStartIndex = indexFromFreq(startFreq, data.Length); 
        absoluteEndIndex = indexFromFreq(endFreq, data.Length);

        initNotes(ref data);
        
    }
    private void initNotes(ref float[] data) {
        // this is only called the first time any notes are made.
        for (int i = 0; i < notes.Length; i++) { // get all notes in this octave, 0-11
            float lowNoteFreq = getFreqFromNote(ocNum, i-0.5f);
            float highNoteFreq = getFreqFromNote(ocNum, i+0.5f);
            int lowNoteIndex = indexFromFreq(lowNoteFreq, data.Length);
            int highNoteIndex = indexFromFreq(highNoteFreq, data.Length);
            //TODO: CACHE the above data.
            int totalFreqs = highNoteIndex - lowNoteIndex;
            if (totalFreqs <= 0) {
                notes[i] = Note.nullNote;
                continue;
            }

            notes[i] = new Note(i, data.Skip(lowNoteIndex).Take(totalFreqs), lowNoteIndex);
        }
    }

    public void updateOctave(Octave prev, ref float[] data) { // clone an existing Octave
        processPrevOctave(prev);
        updateAllNoteData(ref data);
    }
    private void updateAllNoteData(ref float[] data) {
        // after you clone from a previous note, new data must be given to it.
        for (int i = 0; i < notes.Length; i++) {
            (int start, int total) freqRange= notes[i].getFrequencyRange();
            notes[i].updateNote(data.Skip(freqRange.start).Take(freqRange.total));
        }
    }
    private void processPrevOctave(Octave prev) {
        cloneNewNotes(prev);
    }
    public void cloneNewNotes(Octave prev) {
        // clone all the notes based on the previous octave
        for (int i = 0; i < notes.Length; i++) {
            notes[i] = new Note(prev.getNote(i));
        }
    }
    public Note getNote(int index) {
        return notes[index];
    }

    public void activateNotes(Octave lowNeighbor, Octave highNeighbor) {
        // octaves dont compare against other octaves directly, but are needed for the note neigbors

        // set each notes neigbors in this octave
        // set the edge neighbors of this octave to lowNs high and highNs low respectivly

        for (int i = 0; i < notes.Length; i++) {
            // find each notes neighbor
            if (i == 0) {
                if (lowNeighbor.isNull()) {
                    notes[i].activateNote(Note.nullNote, notes[i+1]);
                } else { // if there is a lower octave, grab its highest note
                    notes[i].activateNote(lowNeighbor.getNote(11), notes[i+1]);
                }
            } else if (i == notes.Length - 1) {
                if (highNeighbor.isNull()) {
                    notes[i].activateNote(notes[i-1],  Note.nullNote);
                } else { // if there is an upper octave, grab its lowest note
                    notes[i].activateNote(notes[i-1],  highNeighbor.getNote(0));
                }
            } else {
                notes[i].activateNote(notes[i-1], notes[i+1]);
            }
        }

    }

    private float getFreqFromNote(int octave, float note) { // octave from 0-8, note from 0-11
        // you can get inbetween 2 notes by giving it a .5 value
        float noteVal = octave + (note/12.0f);
        return Mathf.Pow(2, noteVal) * 16.35f;
    }

    private int indexFromFreq(float freq, int dataLen) { // THIS assumes the data is linear
        if (freq <= 20) return 0;
        // the whole array goes from 20-20k
        return (int)Math.Floor((freq-20)/(19980f / dataLen));
    }


    // DRAWABLE functions
    public void drawOctaveFull(ref Texture2D tex, bool drawNotes) {
        float alt = ocNum % 2 == 1 ? 1f : 0.75f;
        if (drawNotes) {
            foreach (Note n in notes) {
                n?.drawNoteFull(ref tex, alt);
            }
        } else {
            for (int y = absoluteStartIndex; y < absoluteEndIndex; y++) {
                for (int x = 0; x < tex.width; x++) {
                    Color d = tex.GetPixel(x,y);
                    d.a = alt;
                    tex.SetPixel(x,y,d);
                }
            }
        }
    }
    public void drawCurrentNotes(ref Texture2D tex, int lineNum) {
        foreach (Note n in notes) {
            n?.drawNote(ref tex, lineNum);
        }
    }
    public virtual bool isNull() {
        return false;
    }

}
