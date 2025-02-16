// DEPRECATED -- USE something like https://acousticbrainz.org/data instead
//https://musicbrainz.org/doc/MusicBrainz_API

[System.Serializable]
public class SpotifyTrackInfo {
    /*
    {
  "acousticness": 0.00242,
  "analysis_url": "https://api.spotify.com/v1/audio-analysis/2takcwOaAZWiXQijPHIx7B",
  "danceability": 0.585,
  "duration_ms": 237040,
  "energy": 0.842,
  "id": "2takcwOaAZWiXQijPHIx7B",
  "instrumentalness": 0.00686,
  "key": 9,
  "liveness": 0.0866,
  "loudness": -5.883,
  "mode": 0,
  "speechiness": 0.0556,
  "tempo": 118.211,
  "time_signature": 4,
  "track_href": "https://api.spotify.com/v1/tracks/2takcwOaAZWiXQijPHIx7B",
  "type": "audio_features",
  "uri": "spotify:track:2takcwOaAZWiXQijPHIx7B",
  "valence": 0.428
    }
    */
    public float acousticness;
    public string analysis_url;
    public float danceability;
    public int duration_ms;
    public float energy;
    public string id;
    public float instrumentalness;
    public int key; // -1 to 11. musical key, C# etc
    public float liveness; // is this song live (based on background claps/cheers)
    public float loudness;
    public int mode; // major = 1, minor = 0
    public float speechiness;
    public float tempo;
    public int time_signature; // Idk might be important tho, 3/4 or somehting
    public string track_href;
    public string uri;
    public float valence; // 0 - 1.0,  1 = upbeat positive. 0 = sad

    public override string ToString() {
        string res = "ID: " + id;
        res += "\nBPM: " + tempo;
        res += "\nEnergy: " + energy;
        res += "\nLiveness: " + liveness;
        res += "\nLoudness: " + loudness;
        res += "\nValence: " + valence;
        res += "\nDanceability: " + danceability;
        res += "\nSpeechiness: " + speechiness;
        return res;
    }
}
