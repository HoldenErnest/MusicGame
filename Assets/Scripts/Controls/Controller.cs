// Holden Ernest - 4/16/2025
// The controls for the game. (if on pc this will be something like the top of your keyboard (1234..0-=), 12 keys)

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Controller : MonoBehaviour
{
    public static int totalPoints = 0;
    public static int points = 0;

    void Update()
    {
        updateControls();
    }

    void updateControls() {

        // the playable tiles have to check directly if theyre being pressed and update the total score themselves

        for (int i = 0; i < PlayableTileManager.totalTiles; i++) {
            // i = 0, input = 1
            string input = (i+1).ToString();
            if (i == 9) input = "0";
            else if (i == 10) input = "-";
            else if (i == 11) input = "=";

            if (Input.GetKeyDown(input)) {
                PlayableTile pt = PlayableTileManager.tiles[i].GetComponent<PlayableTile>();
                pt.pressKey();
                // send the key press to the Tile itself. It will figure out if it missed or hit
                // and then send that back here with miss() or hit()
            }
        }

        //PlayableTileManager.tiles
    }

    public static void hit() {
        points++;
        totalPoints++;
    }
    public static void miss() {
        totalPoints++;
    }


}
