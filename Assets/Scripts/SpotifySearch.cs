using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpotifySearch : MonoBehaviour
{
    public TMP_InputField songInput;
    public TMP_InputField artistInput;

    public MusicAPI mAPI;

    private string getSong() {
        return songInput.text;
    }
    private string getArtist() {
        return artistInput.text;
    }

    public void sendSearch() {
        mAPI.requestSong(getSong(), getArtist());
    }
}
