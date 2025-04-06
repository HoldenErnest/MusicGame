// Holden Ernest - 3/31/2025
// with the input of a frequency range that represents this single note, determine the core information about it

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class Note {
    private int noteNum = 0; // A B C D E F G (0-11)  WITH # and b
    private int ocNum = 0;
    private float maxAmplitude = 0;
    private float totalAmplitude = 0;
    private int totalFreqCount = 0;

    private int absoluteStartIndex = -1; // the index in rawAudioData[] at which this notes frequencies begin

    private bool isNoteActive = false;

    private Queue<Note> bufferedNotes = new Queue<Note>(); 

    public static readonly Note nullNote = new NullNote();

    // TEMP for testing ---------------
    public static float absMaxAmplWeight = 0.95f; // 0.7 means notes highest is within 30% of songs highest 
    public static float relMaxAmplWeight = 0.3f; // ^
    public static float minPrevPercent = 0.5f; // 0.3 means previous notes must reach 30% lower amplitudes 
    // TEMP for testing ---------------


    /// <summary>
    /// Do not use, this is just for NullNote
    /// </summary>
    public Note() {

    }
    public Note(Note prevNote) {
        // build off a previous note, it already did some "heavy" computations
        processPrevNote(prevNote);
        bufferPreviousNote(prevNote);
    }
    public Note(int noteNumber, int octaveNum, IEnumerable<float> frequencyRange, int absoluteStartIndex) {
        noteNum = noteNumber;
        ocNum = octaveNum;
        this.absoluteStartIndex = absoluteStartIndex;
        updateNote(frequencyRange);
    }
    public void updateNote(IEnumerable<float> frequencyRange) {
        totalFreqCount = 0;
        foreach (float amplitude in frequencyRange) {
            if (amplitude > maxAmplitude) {
                maxAmplitude = amplitude;
            }
            totalAmplitude += amplitude;
            totalFreqCount++;
        }
    }
    public void bufferPreviousNote(Note prev) {
        bufferedNotes.Enqueue(prev);
        if (bufferedNotes.Count > NoteComputation.TOTAL_BUFFERS) {
            bufferedNotes.Dequeue();
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
        return note + ": " + absNormalAmplitude(maxAmplitude) + ". Range: " + absoluteStartIndex + ", " + (absoluteStartIndex + totalFreqCount);
    }
    private void processPrevNote(Note prev) {
        if (prev == null || prev.isNull()) return;
        setNoteNum(prev.getNoteNum());
        setOctaveNumber(prev.getOctaveNumber());
        setFreqencyRange(prev.getFrequencyRange());
    }
    public int getNoteNum() {
        return noteNum;
    }
    public int getOctaveNumber() {
        return ocNum;
    }
    public void setOctaveNumber(int on) {
        ocNum = on;
    }
    public void setNoteNum(int nn) {
        noteNum = nn;
    }
    public void setFreqencyRange((int start, int total) range) {
        totalFreqCount = range.total;
        absoluteStartIndex = range.start;
    }
    public (int, int) getFrequencyRange() {
        // returns (total, start);
        return (absoluteStartIndex, totalFreqCount);
    }
    /// <summary>
    /// Compare two notes properties
    /// </summary>
    /// <param name="other"></param>
    /// <returns>(MaxAmplitude, AverageAmplitude)</returns>
    public (float max, float avg) compareNotes(Note other) {
        // returns this.~ - other.~
        return (maxAmplitude - other.maxAmplitude, getAvgAmplitude() - other.getAvgAmplitude());
    }

    public virtual bool isActive() {
        return isNoteActive;
    }
    public void activateNote(Note lowNeighbor, Note highNeighbor) {
        // test against itself: is this high enough difference from the previous buffered notes
        // test against neighboring notes: there could be multiple neigboring notes played but most of the time its just overflow
        // test against the last time a note on this lane was played ?? might not be needed
        // compare with the two neighbors

        //if (containsRelHighAmp() && containsAbsHighAmp() && onePrevLow()) {

        // this line immediatly ensures that there will never be 2 notes next to eachother (most of the time its just overflow from a different note)
        //* in the future I might want to make this percentage based as well. there could be 2 notes right next to eachother
        if (lowNeighbor.maxAmplitude > maxAmplitude || highNeighbor.maxAmplitude > maxAmplitude) return;

        if (onePrevLow() && containsAbsHighAmp()) {
            isNoteActive = true;
            return;
        }
        isNoteActive = false;
    }
    private bool containsAbsHighAmp() {
        // does this note contain an amplitude close to the max amp in the whole song.
        if (absNormalAmplitude(maxAmplitude) + absMaxAmplWeight >= 1) {
            isNoteActive = true;
            return true;
        }
        return false;
    }
    private bool containsRelHighAmp() {
        // does this note contain an amplitude close to the max amp in the whole song.
        if (relNormalAmplitude(maxAmplitude) + relMaxAmplWeight >= 1) {
            isNoteActive = true;
            return true;
        }
        return false;
    }
    private bool onePrevLow() {
        foreach (Note prev in bufferedNotes) {
            float thisVsPrev = compareNotes(prev).avg;
            float percentage = prev.getAvgAmplitude() / getAvgAmplitude(); // prev is x% of this
            if (percentage <= minPrevPercent) { // prev max is less than 50% of current
                return true;
            }
        }
        return false;
    }
    private float absNormalAmplitude(float amp) { // absolute max frequency
        return amp / NoteComputation.maxFrequency;
    }
    private float relNormalAmplitude(float amp) { // relative to current frame
        return amp / NoteComputation.currentMaxFreq;
    }

    // DRAWABLE functions
    public void drawNoteFull(ref Texture2D tex, float currentAlpha) {
        if (totalFreqCount <= 0) return;
        for (int y = absoluteStartIndex; y < absoluteStartIndex + totalFreqCount; y++) {
            float alt = noteNum % 2 == 1 ? 0.5f : 0f;
            for (int x = 0; x < tex.width; x++) {
                Color d = tex.GetPixel(x,y);
                d.a = currentAlpha - alt;
                tex.SetPixel(x,y,d);
            }
        }
    }
    public void drawNote(ref Texture2D tex, int x) {
        // draw the note based on its Active modifier.
        if (!isActive()) return;

        drawNote(ref tex, x, Color.green);
    }

    /// <summary>
    /// WARNING: Use with understanding, if you draw a note with a specified color it will not check if its active, it will just draw
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="x"></param>
    /// <param name="c"></param>
    public void drawNote(ref Texture2D tex, int x, Color c) {
        for (int y = absoluteStartIndex; y < absoluteStartIndex + totalFreqCount; y++) {
            tex.SetPixel(x,y,c);
        }
    }
    // END DRAWABLE functions

    public virtual bool isNull() {
        return false;
    }
}