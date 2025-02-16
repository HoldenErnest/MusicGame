using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class SpotifyResponse {
    /*
  "external_urls": {
    "spotify": "string"
  },
  "followers": {
    "href": "string",
    "total": 0
  },
  "genres": ["Prog rock", "Grunge"],
  "href": "string",
  "id": "string",
  "images": [
    {
      "url": "https://i.scdn.co/image/ab67616d00001e02ff9ca10b55ce82ae553c8228",
      "height": 300,
      "width": 300
    }
  ],
  "name": "string",
  "popularity": 0,
  "type": "artist",
  "uri": "string"
    */
    
    public string name;
    public string type;
    public string uri;
    public int popularity;
    public string[] genres;
    public string href;
    public string id;
    public ImagesStruct[] images;
    public followersStruct followers;

    public override string ToString() {
        string res = "Name: " + name;
        res += "\nType: " + type;
        res += "\nID: " + id;
        res += "\nPop: " + popularity;
        res += "\nIMG: " + images[0].url;
        return res;
    }
}
