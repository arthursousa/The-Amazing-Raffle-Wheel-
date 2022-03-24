using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
//using TwitchLib.Api.Models.Undocumented.Chatters;

public class TwitchAPI : MonoBehaviour
{
    public Api api;
    public TwitchClient twitchClient;
    public PlayerPrefsManager playerPrefs;
    bool countViewers;

    public void APIConnection()
    {
        Application.runInBackground = true;
        InitilizeAPI();

        //we will count viewers until we get raided
        countViewers = true;

        //after client connected we start making api calls
    }

    private void InitilizeAPI()
    {
        //make an api class and connect it to our tokens 
        api = new Api();
        api.Settings.AccessToken = playerPrefs.GetBotAccessToken();
        api.Settings.ClientId = playerPrefs.GetClientID();
    }

}
