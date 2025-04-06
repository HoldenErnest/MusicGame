using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableTile : MonoBehaviour {
    SpriteRenderer sr;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
    }

    public void updateTile(Note n) {

        updateColor(n.getOctaveNumber());
        StartCoroutine("clearNote");
    }
    IEnumerator clearNote() {
        yield return new WaitForSeconds(0.5f);
        updateColor(-1);
    }

    private void updateColor(int ocNum) {
        Color c = Color.white;
        switch (ocNum) {
            case 0:
                c = Color.black;
                break;
            case 1:
                c = Color.red;
                break;
            case 2:
                c = Color.yellow;
                break;
            case 3:
                c = Color.green;
                break;
            case 4:
                c = Color.cyan;
                break;
            case 5:
                c = Color.blue;
                break;
            case 6:
                c = Color.magenta;
                break;
            case 7:
                c = new Color(0,128,255);
                break;
            case 8:
                c = new Color(0,255,255);
                break;
            default:
                break;
        }

        sr.color = c;
    }
}
