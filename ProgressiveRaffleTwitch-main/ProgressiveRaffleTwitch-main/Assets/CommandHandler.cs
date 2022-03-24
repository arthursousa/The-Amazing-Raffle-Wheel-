using UnityEngine;
using TwitchLib.Client.Events;
using System.Collections.Generic;
using System;
using TwitchLib.PubSub.Events;

public class CommandHandler : MonoBehaviour
{
    public bool UseWheel;
    public List<string> adminNames;
    public DataManager dataManager;
    public TwitchClient twitchClient;
    public Dictionary<string, int> playerWeightedJSON = new Dictionary<string, int>();
    public List<string> currentRaffle = new List<string>();
    public List<string> playerNames = new List<string>();
    public Dictionary<string, int> currentRaffleWeights = new Dictionary<string, int>();
    bool raffleOpen = false;

    private string helpMessage = "whelp";
    private string startRaffleMessage = "raffle";
    private string endRaffleMessage = "endraffle";
    private string spinWheelMessage = "spin";
    private string modifyChatterWeight = "influence";
    private string revokeRemoval = "revoke";
    private string blackListCommand = "wheelblock";
    private string revokeBlackListCommand = "wheelallow";
    private string checkPointsCommand = "points";

    public string raffleKeyWord = "me";

    public static CommandHandler INSTANCE;
    // Start is called before the first frame update

    public List<string> blackList;
    private void Awake()
    {
        INSTANCE = this;
        playerWeightedJSON = dataManager.Load();
        blackList = dataManager.ShadowLoad();
    }

    string reward1name;
    string reward2name;
    int reward1value;
    int reward2value;
    private void Start()
    {
        reward1name = PlayerPrefsManager.INSTANCE.GetRewardOneName();
        reward2name = PlayerPrefsManager.INSTANCE.GetRewardTwoName();
        reward1value = PlayerPrefsManager.INSTANCE.GetRewardOneValue();
        reward2value = PlayerPrefsManager.INSTANCE.GetRewardTwoValeu();
    }

    public void FilterCommand(OnChatCommandReceivedArgs e)
    {
        CheckAdminCommands(e);

        if (e.Command.CommandText == checkPointsCommand)
        {
            string displayName = e.Command.ChatMessage.DisplayName;
            if (!playerWeightedJSON.ContainsKey(displayName))
            {
                twitchClient.SendChatMessage("Player " + displayName + " does not have any raffle points!");
            }
            else
            {
                if (playerWeightedJSON[displayName] <= 0)
                {
                    twitchClient.SendChatMessage("Player " + displayName + " does not have any raffle points!");
                }
                if (playerWeightedJSON[displayName] == 1)
                {
                    twitchClient.SendChatMessage("Player " + displayName + " has 1 raffle point!");
                }
                else
                {
                    twitchClient.SendChatMessage("Player " + displayName + " has " + playerWeightedJSON[displayName] + " raffle points!");
                }
            }
        }

        if (!raffleOpen) return;

        if (e.Command.CommandText == raffleKeyWord)
        {

            string displayName = e.Command.ChatMessage.DisplayName;
            if (blackList.Contains(displayName))
                return;

            if (!playerNames.Contains(displayName))
            {
                if (playerWeightedJSON.ContainsKey(displayName))
                {
                    IncrimentPlayerInJSON(displayName);
                }
                else
                {
                    AddPlayerToJSON(displayName);
                }
                AddPlayerToRaffle(displayName);
            }
        }

    }

    public void FilterReward(OnRewardRedeemedArgs e)
    {

        if (e.RewardTitle == reward1name)
        {
            MofidyPlayerInJson(e.DisplayName, reward1value);
        }
        if (e.RewardTitle == reward2name)
        {
            MofidyPlayerInJson(e.DisplayName, reward2value);
        }

    }

    public void FilterChannelPoints(OnChannelPointsRewardRedeemedArgs e)
    {
        Debug.Log(e.RewardRedeemed.Redemption.Reward.Title);
        if (e.RewardRedeemed.Redemption.Reward.Title == "One Raffle Point!")
            Debug.Log("pog");
    }

    public void CheckAdminCommands(OnChatCommandReceivedArgs e)
    {
        if (!adminNames.Contains(e.Command.ChatMessage.DisplayName))
            return;

        if (e.Command.CommandText == startRaffleMessage)
        {
            if (e.Command.ArgumentsAsList.Count > 0)
                if (e.Command.ArgumentsAsList[0] != "")
                    raffleKeyWord = e.Command.ArgumentsAsList[0];
            OpenRaffle();
        }

        else if (e.Command.CommandText == endRaffleMessage || e.Command.CommandText == spinWheelMessage)
        {
            if (UseWheel)
            {
                CloseRaffle();
            }
            else
            {
                ChooseWinner();
            }
        }

        else if (e.Command.CommandText == helpMessage)
        {

            string message = startRaffleMessage + " - start raffle submissions, add another word after it to change the enter word\n\n";
            message += endRaffleMessage + " - end raffle submissions\n\n";
            message += spinWheelMessage + " - spin the wheel\n\n";
            message += modifyChatterWeight + " - Adds/Remove the weights of a player, add name and points after\n\n";
            message += revokeRemoval + " - Revokes the previous list removal, say when someone won a raffle but you want the person back on the list with the same weights\n\n";
            message += blackListCommand + " - Adds player to blacklist\n\n";
            message += revokeBlackListCommand + " - Removes player from blacklist\n\n";

            twitchClient.SendChatMessage(string.Format(message));
        }

        else if (e.Command.CommandText == modifyChatterWeight)
        {
            if (e.Command.ArgumentsAsList[0] != "")
            {
                if (e.Command.ArgumentsAsList[1] != "")
                {
                    MofidyPlayerInJson(e.Command.ArgumentsAsList[0], Int32.Parse(e.Command.ArgumentsAsList[1]));
                }
            }
        }

        else if (e.Command.CommandText == revokeRemoval)
        {
            RevokeRemoval();
        }

        else if (e.Command.CommandText == blackListCommand)
        {
            if (e.Command.ArgumentsAsList[0] != "")
            {
                BlackListPlayer(e.Command.ArgumentsAsList[0]);
                twitchClient.SendChatMessage("Blacklisting " + e.Command.ArgumentsAsList[0]);
            }
        }

        else if (e.Command.CommandText == revokeBlackListCommand)
        {
            if (e.Command.ArgumentsAsList[0] != "")
            {
                if (blackList.Contains(e.Command.ArgumentsAsList[0]))
                {
                    blackList.Remove(e.Command.ArgumentsAsList[0]);
                    twitchClient.SendChatMessage("Removing player from blacklist: " + e.Command.ArgumentsAsList[0]);
                    dataManager.ShadowNewSave(blackList);
                }
            }
        }
    }

    private void AddPlayerToRaffle(string displayName)
    {

        for (int i = 0; i < playerWeightedJSON[displayName]; i++)
        {
            currentRaffle.Add(displayName);
        }
        currentRaffleWeights.Add(displayName, playerWeightedJSON[displayName]);
        Debug.LogFormat("adding {0} with {1} weight", displayName, currentRaffleWeights[displayName]);
        playerNames.Add(displayName);
        SelectionWheel.INSTANCE.SetUpWheel();

    }

    public void ChooseWinner()
    {
        if (UseWheel)
        {
            //use wheel here
        }
        else
            twitchClient.SendChatMessage(currentRaffle[UnityEngine.Random.Range(0, currentRaffle.Count)] + " You are the Winner!!!");

        CloseRaffle();
    }
    public void WheelWin(string winner)
    {
        twitchClient.SendChatMessage(winner + " You are the Winner!!!");

        RemovePlayer(winner);
        BlackListPlayer(winner);
    }

    public void BlackListPlayer(string displayName)
    {
        if (blackList.Contains(displayName))
            return;

        blackList.Add(displayName);
        dataManager.ShadowNewSave(blackList);
    }

    public void ResetDrawing()
    {
        playerNames.Clear();
        currentRaffle.Clear();
    }

    public void AddPlayerToJSON(string displayName)
    {
        playerWeightedJSON.Add(displayName, 1);
        dataManager.NewSave(playerWeightedJSON);
    }
    public void IncrimentPlayerInJSON(string displayName)
    {
        playerWeightedJSON[displayName]++;
        dataManager.NewSave(playerWeightedJSON);
    }
    public void MofidyPlayerInJson(string displayName, int weight)
    {
        int aux = 0;
        if (!playerWeightedJSON.ContainsKey(displayName))
        {
            AddPlayerToJSON(displayName);
            weight--;
            aux = 1;
        }

        if (playerWeightedJSON[displayName] + weight <= 0)
        {
            RemovePlayer(displayName);
            twitchClient.SendChatMessage("Removing " + displayName + " from the raffle");
        }
        else
        {
            playerWeightedJSON[displayName] += weight;

            if (weight > 0)
                twitchClient.SendChatMessage("Adding " + (weight + aux).ToString() + " to " + displayName);
            else
                twitchClient.SendChatMessage("Removing " + weight.ToString() + " from " + displayName);
        }

        dataManager.NewSave(playerWeightedJSON);
    }

    private string previousRemovalName;

    private int previousRemovalWeight;
    public void RemovePlayer(string displayName)
    {
        if (!playerWeightedJSON.ContainsKey(displayName))
            return;

        previousRemovalName = displayName;
        previousRemovalWeight = playerWeightedJSON[displayName];

        playerWeightedJSON.Remove(displayName);
        dataManager.NewSave(playerWeightedJSON);
    }

    public void RevokeRemoval()
    {
        twitchClient.SendChatMessage("Revoking Removal, adding " + previousRemovalName + " back in with " + previousRemovalWeight + " points");
        if (!playerWeightedJSON.ContainsKey(previousRemovalName))
        {
            AddPlayerToJSON(previousRemovalName);
        }

        playerWeightedJSON[previousRemovalName] = previousRemovalWeight;
        dataManager.NewSave(playerWeightedJSON);

        if (blackList.Contains(previousRemovalName))
        {
            blackList.Remove(previousRemovalName);
            dataManager.ShadowNewSave(blackList);
        }

    }
    public void OpenRaffle()
    {
        if (UseWheel)
            SelectionWheel.INSTANCE.group.alpha = 1;
        currentRaffleWeights = new Dictionary<string, int>();
        twitchClient.SendChatMessage(string.Format("Raffle is now open type !{0} to join", raffleKeyWord));
        raffleOpen = true;
    }
    public void CloseRaffle()
    {
        twitchClient.SendChatMessage("Raffle is now closed!");
        raffleOpen = false;
        if (!UseWheel)
        {
            ResetDrawing();
        }
        else
        {
            SelectionWheel.INSTANCE.SpinWheel();
        }
    }

}
