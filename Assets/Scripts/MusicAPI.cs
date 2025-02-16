using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MusicAPI : MonoBehaviour {

    void Awake() {
        StartCoroutine ("startConnection");
    }

    private SpotifyConnection connection; // holds tokens for current session -- !TODO check token exiration on calls
    private SpotifySearchResponse response; // holds responses to the last query;
    private SpotifyTrackInfo trackInfo;
    private IEnumerator startConnection() {
        UnityWebRequest www = UnityWebRequest.Post ("https://accounts.spotify.com/api/token", "grant_type=client_credentials&client_id=" + Secrets.SpotifyID + "&client_secret=" + Secrets.SpotifySecret, "application/x-www-form-urlencoded");
        yield return www.SendWebRequest();

        
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log (www.error);
        } else {
            connection = null;
            connection = JsonUtility.FromJson<SpotifyConnection>(www.downloadHandler.text);
            connection.beginExpire();
        }
    }

    public void requestSong(string song, string artist) {
        response = null;
        if (connection == null) return;
        string query = $"track:{song}%20artist:{artist}" + "&type=track";
        query = query.Replace(" ", "%20");
        Debug.Log(query);
        StartCoroutine(sendSongRequest(query));
        
    }
    private IEnumerator sendSongRequest(string query) {
        UnityWebRequest www = UnityWebRequest.Get ("https://api.spotify.com/v1/search?q=" + query);
        www.SetRequestHeader ("Authorization", connection.token_type + " " + connection.access_token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log ("Error: " + www.error);
        } else {
            response = JsonUtility.FromJson<SpotifySearchResponse>(www.downloadHandler.text);

            Debug.Log("SONG: " + response);
            StartCoroutine(sendInfoRequest(response.getTopID()));
        }
    }
    private IEnumerator sendInfoRequest(string songID) { // IM CRYING, Spotify api isnt very helpful
        Debug.Log(songID);
        songID = "6Ph8QwsRfZunN5e1GGBIqa";
        UnityWebRequest www = UnityWebRequest.Get ("https://api.spotify.com/v1/audio-features/" + songID);
        www.SetRequestHeader ("Authorization", connection.token_type + " " + connection.access_token);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log ("Error: " + www.error);
        } else {
            trackInfo = JsonUtility.FromJson<SpotifyTrackInfo>(www.downloadHandler.text);

            Debug.Log("INFO: " + trackInfo);
        }
    }
}