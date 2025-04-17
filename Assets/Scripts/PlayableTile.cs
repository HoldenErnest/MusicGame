// Holden Ernest - 4/10/2025
// A purely visual script for representing the active notes of the scene. These notes can represent any given octave so to differentiate they have a different color

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class PlayableTile : MonoBehaviour {
    SpriteRenderer sr;
    private bool activeNote = false; // representing if this note is currently active.

    private bool hitThis = false;

    private float noteDuration = 0.7f;
    private ParticleSystem ps;

    void Start() {
        ps = GetComponent<ParticleSystem>();
        sr = GetComponent<SpriteRenderer>();
        badHitVisuals();
    }

    public void updateTile(Note n) {
        updateColor(n.getOctaveNumber());
        StartCoroutine("clearNote");
    }

    /// <summary>
    /// Set a timer to clear the note
    /// </summary>
    /// <returns></returns>
    private IEnumerator clearNote() {
        activeNote = true;
        yield return new WaitForSeconds(noteDuration);
        if (activeNote) { // If the same note was played twice back to back this would still only play for the single noteDuration
            updateColor(-1);
            activeNote = false;
            checkHit();
        }
    }
    
    /// <summary>
    /// Check the hit AT THE END OF ITS LIFETIME. This ensures you get exactly 0 or 1 points.
    /// </summary>
    private void checkHit() {
        if (!hitThis) {
            Controller.miss();
            missVisuals();
        } else {
            Controller.hit();
        }
        hitThis = false;
    }

    /// <summary>
    /// Color is based on its current Octave (for now at least)
    /// </summary>
    /// <param name="ocNum"></param>
    private void updateColor(int ocNum) {
        Color c = Color.black;
        if (ocNum >= 0 && ocNum <= 4) {
            c.b = (1.0f/5) * ocNum;
        } else if (ocNum >= 0 && ocNum <= 8) {
            c.b = 1f;
            c.r = (1.0f/9) * ocNum;
        }
        sr.color = c;
    }

    public bool isActive() {
        return activeNote;
    }

    /// <summary>
    /// Action called by the controller when this key was pressed
    /// </summary>
    /// <returns></returns>
    public void pressKey() {
        keyPressVisuals();
        if (!isActive()) return; // this note is not active, dont do anything
        hitThis = true;
    }

    /// <summary>
    /// Points are calculated separately to show each press the user is inputting.
    /// </summary>
    private void keyPressVisuals() {
        if (isActive()) {
            goodHitVisuals();
        } else {
            badHitVisuals();
        }
        ps.Play();
    }
    private void missVisuals() {
        badHitVisuals();
        ps.Play();
    }
    private void badHitVisuals() {
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(new Color(0.25f,0f,0.25f), 0.0f);
        colors[1] = new GradientColorKey(new Color(0.25f,0f,0f), 1.0f);

        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);
        
        Gradient g = new Gradient();
        g.SetKeys(colors, alphas);
        
        updateGradient(g);
    }
    private void goodHitVisuals() {
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(Color.yellow, 0.0f);
        colors[1] = new GradientColorKey(Color.green, 1.0f);

        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        Gradient g = new Gradient();
        g.SetKeys(colors, alphas);

        updateGradient(g);
    }

    private void updateGradient(Gradient g) {
        var main = ps.main;
        ParticleSystem.MinMaxGradient minMaxGradient = new ParticleSystem.MinMaxGradient(g);
        main.startColor = minMaxGradient;
    }
}
