// Holden Ernest - 4/6/2025
// Holds the new visuals for the Notes called "playableTiles"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableTileManager : MonoBehaviour {

    public GameObject refTile;

    public static readonly int totalTiles = 12;
    public static GameObject[] tiles;

    private (float left, float width, float bottom, float height) playarea;

    void Start() {
        var vertExtent = Camera.main.orthographicSize;
    	var horzExtent = vertExtent * Screen.width / Screen.height;
        playarea.left = -horzExtent;
        playarea.width = horzExtent*2;
        playarea.bottom = -vertExtent;
        playarea.height = vertExtent*2;

        initTiles();
    }

    private void initTiles() {
        tiles = new GameObject[totalTiles];
        float tileSize = playarea.width / totalTiles;

        for (int i = 0; i < totalTiles; i++) {
            var clone = Instantiate(refTile, this.gameObject.transform);
            Vector3 position = new Vector3(
                (playarea.width / totalTiles) * i + playarea.left + tileSize/2,
                playarea.bottom + tileSize/2,
                0f
            );
            clone.transform.position = position;
            clone.transform.localScale = new Vector3(tileSize, tileSize, 1f);
            tiles[i] = clone;
        }
    }

    public void updateTiles() {
        // update these tiles by calling methods to NoteComputation (getActiveNoteIds)

        List<Note> activeNoteData = NoteComputation.getActiveNoteData();
        foreach (Note n in activeNoteData) { // since this goes front to back, the higher frequencies will be shown priority
            tiles[n.getNoteNum()].GetComponent<PlayableTile>().updateTile(n);
        }

    }
}
