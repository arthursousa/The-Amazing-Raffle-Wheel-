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
    public Dictionary<string, int> rafflePoints = new Dictionary<string, int>();
    public Dictionary<string, int> nepotismPoints = new Dictionary<string, int>();
    public List<string> currentRaffle = new List<string>();
    public List<string> playerNames = new List<string>();
    public Dictionary<string, int> currentRaffleWeights = new Dictionary<string, int>();
    bool raffleOpen = false;

    private string helpMessage = "whelp";
    private string startRaffleMessage = "raffle";
    private string endRaffleMessage = "endraffle";
    private string spinWheelMessage = "spin";
    private string modifyChatterWeight = "influence";
    private string modifyChatterNepotism = "nepotism";
    private string modifyChatterNepotismSet = "setnepotism";
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
        rafflePoints = dataManager.PointsLoad();
        nepotismPoints = dataManager.NepotismLoad();
        blackList = dataManager.BlackListLoad();
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
        Debug.Log("Message received : " + e.Command.CommandText);
        CheckAdminCommands(e);

        if (e.Command.CommandText == checkPointsCommand)
        {
            string displayName = e.Command.ChatMessage.DisplayName;
            if (!rafflePoints.ContainsKey(displayName))
            {
                twitchClient.SendChatMessage("Player " + displayName + " does not have any raffle points!");
            }
            else
            {
                int x = GetChatterTotalPoints(displayName);
                if (x <= 0)
                {
                    twitchClient.SendChatMessage("Player " + displayName + " does not have any raffle points!");
                }
                if (x == 1)
                {
                    twitchClient.SendChatMessage("Player " + displayName + " has 1 raffle point!");
                }
                else
                {
                    twitchClient.SendChatMessage("Player " + displayName + " has " + x + " raffle points!");
                }
            }
        }

        if (e.Command.ChatMessage.UserId == "36044226")
        {
            if (e.Command.CommandText == "Hello")
            {
                twitchClient.SendChatMessage("OhMyDog Is that THE " + e.Command.ChatMessage.DisplayName + "!?!? Genius gamedev and creator of this Amazing Raffle Wheel?? PogChamp PogChamp");
            }

            if (!nepotismPoints.ContainsKey(e.Command.ChatMessage.DisplayName))
            {
                nepotismPoints.Add(e.Command.ChatMessage.DisplayName, 5);
            }
            else
            {
                if (nepotismPoints[e.Command.ChatMessage.DisplayName] < 5)
                {
                    nepotismPoints[e.Command.ChatMessage.DisplayName] = 5;
                }
            }
        }

        if (!raffleOpen) return;

        if (e.Command.CommandText == raffleKeyWord)
        {

            string displayName = e.Command.ChatMessage.DisplayName;

            if (e.Command.ChatMessage.UserId != "36044226")
            {
                if (blackList.Contains(displayName))
                {
                    twitchClient.SendChatMessage(displayName + " has already won the raffle or is blacklisted! You can't join again!");
                    return;
                }
            }

            if (!playerNames.Contains(displayName))
            {
                if (rafflePoints.ContainsKey(displayName))
                {
                    IncrimentPlayerInJSON(displayName);
                }
                else
                {
                    AddPlayerToPointsSave(displayName);
                }

                AddPlayerToRaffle(displayName);
            }
            else
            {
                twitchClient.SendChatMessage(displayName + " has already joined the raffle!");
            }
        }

    }

    public void FilterReward(OnRewardRedeemedArgs e)
    {
        Debug.Log(e.RewardTitle);
        Debug.Log(reward1name);
        Debug.Log(reward2name);
        if (e.RewardTitle == reward1name)
        {
            MofidyPointsInSave(e.DisplayName, reward1value);
        }
        if (e.RewardTitle == reward2name)
        {
            MofidyPointsInSave(e.DisplayName, reward2value);
        }

    }


    public void ONLINE()
    {
        twitchClient.SendChatMessage("The Amazing Raffle Wheel is Online!");
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

        if (e.Command.ArgumentsAsList.Count > 0)
        {

            Debug.Log("Tying to remove @");
            Debug.Log(e.Command.ArgumentsAsList[0]);
            Debug.Log(e.Command.ArgumentsAsList[0][0]);
            if (e.Command.ArgumentsAsList[0][0] == '@')
            {

                e.Command.ArgumentsAsList[0] = e.Command.ArgumentsAsList[0].Remove(0, 1);
                Debug.Log(e.Command.ArgumentsAsList[0][0]);
                Debug.Log(e.Command.ArgumentsAsList[0]);
            }

        }

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
                    MofidyPointsInSave(e.Command.ArgumentsAsList[0], Int32.Parse(e.Command.ArgumentsAsList[1]));
                }
            }
        }

        else if (e.Command.CommandText == modifyChatterNepotism)
        {
            if (e.Command.ArgumentsAsList.Count < 2)
            {
                twitchClient.SendChatMessage("Command used wrong: Not enough arguments");
            }
            if (e.Command.ArgumentsAsList[0] != "")
            {
                if (e.Command.ArgumentsAsList[1] != "")
                {
                    MofidyNepotismInSave(e.Command.ArgumentsAsList[0], Int32.Parse(e.Command.ArgumentsAsList[1]), false);
                }
            }
        }

        else if (e.Command.CommandText == modifyChatterNepotismSet)
        {
            if (e.Command.ArgumentsAsList.Count < 2)
            {
                twitchClient.SendChatMessage("Command used wrong: Not enough arguments");
            }
            if (e.Command.ArgumentsAsList[0] != "")
            {
                if (e.Command.ArgumentsAsList[1] != "")
                {
                    MofidyNepotismInSave(e.Command.ArgumentsAsList[0], Int32.Parse(e.Command.ArgumentsAsList[1]), true);
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
                    dataManager.BlackListSave(blackList);
                }
            }
        }
    }

    int aux1;
    private void AddPlayerToRaffle(string displayName)
    {

        for (int i = 0; i < rafflePoints[displayName]; i++)
        {
            currentRaffle.Add(displayName);
        }

        aux1 = GetChatterTotalPoints(displayName);
        currentRaffleWeights.Add(displayName, aux1);

        twitchClient.SendChatMessage(displayName + " has joined the raffle with " + aux1.ToString() + " point" + (aux1 > 1 ? "s" : "") + "!");

        Debug.LogFormat("adding {0} with {1} weight", displayName, currentRaffleWeights[displayName]);
        playerNames.Add(displayName);
        SelectionWheel.INSTANCE.SetUpWheel();
    }

    public int GetChatterTotalPoints(string displayName)
    {
        return rafflePoints[displayName]
            + (nepotismPoints.ContainsKey(displayName) ? (nepotismPoints[displayName] - 1) : 0);
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
        dataManager.BlackListSave(blackList);
    }

    public void ResetDrawing()
    {
        playerNames.Clear();
        currentRaffle.Clear();
    }

    public void AddPlayerToPointsSave(string displayName)
    {
        rafflePoints.Add(displayName, 1);
        dataManager.PointsSave(rafflePoints);
    }
    public void AddPlayerToNepotismSave(string displayName)
    {
        nepotismPoints.Add(displayName, 0);
        dataManager.NepotismSave(nepotismPoints);
    }
    public void IncrimentPlayerInJSON(string displayName)
    {
        rafflePoints[displayName]++;
        dataManager.PointsSave(rafflePoints);
    }
    public void MofidyPointsInSave(string displayName, int weight)
    {
        int aux = 0;
        if (!rafflePoints.ContainsKey(displayName))
        {
            AddPlayerToPointsSave(displayName);
            weight--;
            aux = 1;
        }

        if (rafflePoints[displayName] + weight <= 0)
        {
            RemovePlayer(displayName);
            twitchClient.SendChatMessage("Removing " + displayName + " from the raffle");
        }
        else
        {
            rafflePoints[displayName] += weight;

            if (weight > 0)
                twitchClient.SendChatMessage("Adding " + (weight + aux).ToString() + " to " + displayName);
            else
                twitchClient.SendChatMessage("Removing " + weight.ToString() + " from " + displayName);
        }

        dataManager.PointsSave(rafflePoints);
        if (raffleOpen && UseWheel)
        {
            if (currentRaffleWeights.ContainsKey(displayName))
            {
                currentRaffleWeights[displayName] = GetChatterTotalPoints(displayName);
                SelectionWheel.INSTANCE.SetUpWheel();
            }
        }
    }

    public void MofidyNepotismInSave(string displayName, int weight, bool isSet = false)
    {
        int aux = 0;
        if (!nepotismPoints.ContainsKey(displayName))
        {
            AddPlayerToNepotismSave(displayName);
        }

        if (isSet)
        {
            nepotismPoints[displayName] = weight;
            twitchClient.SendChatMessage("Setting " + displayName + "'s nepotism to " + weight.ToString());
        }

        else
        {
            if (nepotismPoints[displayName] + weight <= 0)
            {
                RemoveNepotism(displayName);
                twitchClient.SendChatMessage("Removing " + displayName + " from the nepotism");
            }
            else
            {
                nepotismPoints[displayName] += weight;

                if (weight > 0)
                    twitchClient.SendChatMessage("Adding " + (weight + aux).ToString() + " nepotism to " + displayName);
                else
                    twitchClient.SendChatMessage("Removing " + weight.ToString() + "nepotism from " + displayName);
            }
        }

        dataManager.NepotismSave(nepotismPoints);

        if (raffleOpen && UseWheel)
        {
            if (currentRaffleWeights.ContainsKey(displayName))
            {
                currentRaffleWeights[displayName] = GetChatterTotalPoints(displayName);
                SelectionWheel.INSTANCE.SetUpWheel();
            }
        }
    }

    private string previousRemovalName;

    private int previousRemovalWeight;
    public void RemovePlayer(string displayName)
    {
        if (!rafflePoints.ContainsKey(displayName))
            return;

        previousRemovalName = displayName;
        previousRemovalWeight = rafflePoints[displayName];

        rafflePoints.Remove(displayName);
        dataManager.PointsSave(rafflePoints);
    }

    public void RemoveNepotism(string displayName)
    {
        if (!nepotismPoints.ContainsKey(displayName))
            return;

        //previousRemovalName = displayName;
        //previousRemovalWeight = rafflePoints[displayName];

        nepotismPoints.Remove(displayName);
        dataManager.NepotismSave(nepotismPoints);
    }

    public void RevokeRemoval()
    {
        twitchClient.SendChatMessage("Revoking Removal, adding " + previousRemovalName + " back in with " + previousRemovalWeight + " points");
        if (!rafflePoints.ContainsKey(previousRemovalName))
        {
            AddPlayerToPointsSave(previousRemovalName);
        }

        rafflePoints[previousRemovalName] = previousRemovalWeight;
        dataManager.PointsSave(rafflePoints);

        if (blackList.Contains(previousRemovalName))
        {
            blackList.Remove(previousRemovalName);
            dataManager.BlackListSave(blackList);
        }

    }
    public void OpenRaffle()
    {
        if (SelectionWheel.INSTANCE.IS_TESTING)
            return;

        if (UseWheel)
        {
            SelectionWheel.INSTANCE.allGroup.alpha = 1;

            SelectionWheel.INSTANCE.wheelgroup.alpha = 0;
        }
        currentRaffleWeights = new Dictionary<string, int>();
        SelectionWheel.INSTANCE.SetUpWheel();
        SelectionWheel.INSTANCE.StartRaffle();
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
