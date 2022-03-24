using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{

    public static PlayerPrefsManager INSTANCE;

    void Awake()
    {
        INSTANCE = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetClientID(TMPro.TMP_InputField inputString)
    {
        if (inputString.text == null)
        {
            return;
        }
        string clientIDText = inputString.text;
        Debug.Log("clientID name saved");
        PlayerPrefs.SetString("ClientID", clientIDText);
    }
    public void SetClientSecret(TMPro.TMP_InputField inputString)
    {
        if (inputString.text == null)
        {
            return;
        }
        string clientSecretText = inputString.text;
        Debug.Log("client secret saved");
        PlayerPrefs.SetString("ClientSecret", clientSecretText);
    }
    public void SetBotAccessToken(TMPro.TMP_InputField inputString)
    {
        if (inputString.text == null)
        {
            return;
        }
        string botAccessText = inputString.text;
        Debug.Log("client secret saved");
        PlayerPrefs.SetString("BotAccessToken", botAccessText);
    }
    public void SetBotRefreshToken(TMPro.TMP_InputField inputString)
    {
        if (inputString.text == null)
        {
            return;
        }
        string botRefreshText = inputString.text;
        Debug.Log("client secret saved");
        PlayerPrefs.SetString("BotRefreshToken", botRefreshText);
    }
    public void SetChannelName(TMPro.TMP_InputField inputString)
    {
        string channelName = inputString.text;
        PlayerPrefs.SetString("ChannelName", channelName);
    }
    public void SetBotName(TMPro.TMP_InputField inputString)
    {
        string channelName = inputString.text;
        PlayerPrefs.SetString("BotName", channelName);
    }
    public string GetChannelName()
    {
        if (PlayerPrefs.HasKey("ChannelName"))
        {
            return PlayerPrefs.GetString("ChannelName");
        }
        else
        {
            return "";
        }
    }
    public string GetBotName()
    {
        if (PlayerPrefs.HasKey("BotName"))
        {
            return PlayerPrefs.GetString("BotName");
        }
        else
        {
            return "";
        }
    }
    public string GetClientID()
    {
        return PlayerPrefs.GetString("ClientID");
    }
    public string GetClientSecret()
    {
        return PlayerPrefs.GetString("ClientSecret");
    }
    public string GetBotAccessToken()
    {
        return PlayerPrefs.GetString("BotAccessToken");
    }
    public string GetBotRefreshToken()
    {
        return PlayerPrefs.GetString("BotRefreshToken");
    }

    public void SetRewardOneString(TMPro.TMP_InputField inputString)
    {
        string s = inputString.text;
        PlayerPrefs.SetString("RewardOneName", s);
    }

    public void SetRewardTwoString(TMPro.TMP_InputField inputString)
    {
        string s = inputString.text;
        PlayerPrefs.SetString("RewardTwoName", s);
    }

    public void SetRewardOneValue(TMPro.TMP_InputField inputString)
    {
        string s = inputString.text;
        PlayerPrefs.SetString("RewardOneValue", s);
    }

    public void SetRewardTwoValue(TMPro.TMP_InputField inputString)
    {
        string s = inputString.text;
        PlayerPrefs.SetString("RewardTwoValue", s);
    }

    public string GetRewardOneName()
    {
        return PlayerPrefs.GetString("RewardOneName");
    }

    public string GetRewardTwoName()
    {
        return PlayerPrefs.GetString("RewardTwoName");
    }

    public int GetRewardOneValue()
    {
        int aux;
        if (int.TryParse(PlayerPrefs.GetString("RewardOneValue"), out aux))
        {
            return aux;
        }
        return 0;
    }

    public int GetRewardTwoValeu()
    {
        int aux;
        if (int.TryParse(PlayerPrefs.GetString("RewardTwoValue"), out aux))
        {
            return aux;
        }
        return 0;
    }

    public void SetChannelID(TMPro.TMP_InputField inputString)
    {
        string s = inputString.text;
        PlayerPrefs.SetString("ChannelID", s);
    }

    public string GetChannelID()
    {
        return PlayerPrefs.GetString("ChannelID");
    }


}
