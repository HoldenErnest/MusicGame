using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public enum SType {track, artist, album};

[System.Serializable]
public struct followersStruct {
    public string href;
    public int total;
}
[System.Serializable]
public struct ImagesStruct {
    public string url;
    public int width;
    public int height;
}
[System.Serializable]
public struct STracks {
    public string href;
    public int total; // total results available
    public SItems[] items;

}
[System.Serializable]
public struct SArtists {
    public string href;
    public string id;
    public string name;
    public SType type;
    public string uri;
}
[System.Serializable]
public struct SAlbum {
    public string album_type;
    public int total_tracks;
    public string href;
    public string id;
    public string name;
    public string release_date;
    public SType type;
    public ImagesStruct[] images;
    public string uri;
    public SArtists[] artists;
}
[System.Serializable]
public struct SItems {
    public SArtists[] artists;
    public SAlbum album;
    public int duration_ms;
    public bool _explicit; // AHHHHHHHHHHHHHHHHHHHHHHH
    public string href;
    public string id;
    public bool is_playable;
    public string name;
    public int popularity;
    public int disc_number;
    public int track_number;
    public SType type;

}

[System.Serializable]
public class SpotifySearchResponse {
    public STracks tracks;
    // album results
    // artist results

    public override string ToString() {
        string res = "Top Track (1/" + tracks.total + "): ";
        if (tracks.total <= 0) return res;
        res += "\nTrack Name: " + tracks.items[0].name;
        res += "\nBy: " + tracks.items[0].artists[0].name;
        res += "\nLen: " + tracks.items[0].duration_ms;
        res += "\nID: " + tracks.items[0].id;
        res += "\nIMG: " + tracks.items[0].album.images[0].url;

        return res;
    }
    public string getTopID() {
        return tracks.items[0].id;
    }
}