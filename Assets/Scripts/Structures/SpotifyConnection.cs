using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpotifyConnection {
    public string access_token = "";
    public string token_type = "";
    public int expires_in = 0;

    private float timeOfExpire = -1;

    public void beginExpire() {
        timeOfExpire = Time.time + expires_in;
    }

    public float getExpireTime() {
        return timeOfExpire;
    }
}
