using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
//using Newtonsoft.Json;
using System.Text;

public class DataManager : MonoBehaviour
{
    public Dictionary<string, int> pointData;
    public Dictionary<string, int> nepotismData;
    public List<string> blackListData;
    string pointFile = "WheelPoints.txt";
    string nepotismFile = "WheelNepotism.txt";
    string blackListFile = "WheelBlackList.txt";


    public class RaffleEntryData
    {

        [Serializable]
        public struct Entry
        {
            public string Key;
            public int Value;
        }
        public List<Entry> entries;

        //public List<string> names;
        //public List<int> weights;

        public RaffleEntryData()
        {
            entries = new List<Entry>();
            //names = new List<string>();
            //weights = new List<int>();
        }

        public RaffleEntryData(Dictionary<string, int> data)
        {
            //names = data.Keys.ToList();
            //weights = data.Values.ToList();
            entries = new List<Entry>();
            foreach (KeyValuePair<string, int> entry in data)
            {
                Entry x = new Entry();
                x.Key = entry.Key;
                x.Value = entry.Value;
                entries.Add(x);
            }
        }
    }

    public class BlackList
    {
        public List<string> blackList;

        public BlackList(List<string> list)
        {
            blackList = list;
        }
    }

    public void PointsSave(Dictionary<string, int> playerData)
    {
        RaffleEntryData data = new RaffleEntryData(playerData);
        string json = JsonUtility.ToJson(data, true);
        Debug.Log(json);
        System.IO.File.WriteAllText(pointFile, json);
    }

    public void BlackListSave(List<string> blacklist)
    {
        BlackList shadowRat = new BlackList(blacklist);
        string json = JsonUtility.ToJson(shadowRat, true);
        Debug.Log(json);
        System.IO.File.WriteAllText(blackListFile, json);
    }

    public void NepotismSave(Dictionary<string, int> nepotismData)
    {
        RaffleEntryData data = new RaffleEntryData(nepotismData);
        string json = JsonUtility.ToJson(data, true);
        Debug.Log(json);
        System.IO.File.WriteAllText(nepotismFile, json);
    }

    public Dictionary<string, int> PointsLoad()
    {
        pointData = new Dictionary<string, int>();
        string json = ReadFromFile(pointFile);
        RaffleEntryData entryData = JsonUtility.FromJson<RaffleEntryData>(json);
        if (entryData != null)
            for (int i = 0; i < entryData.entries.Count; i++)
            {
                pointData.Add(entryData.entries[i].Key, entryData.entries[i].Value);
            }

        return pointData;
    }
    public Dictionary<string, int> NepotismLoad()
    {
        nepotismData = new Dictionary<string, int>();
        string json = ReadFromFile(nepotismFile);
        RaffleEntryData entryData = JsonUtility.FromJson<RaffleEntryData>(json);
        if (entryData != null)
            for (int i = 0; i < entryData.entries.Count; i++)
            {
                nepotismData.Add(entryData.entries[i].Key, entryData.entries[i].Value);
            }

        return nepotismData;
    }

    public List<String> BlackListLoad()
    {
        blackListData = new List<string>();
        string json = ReadFromFile(blackListFile);
        BlackList blacklist = JsonUtility.FromJson<BlackList>(json);
        if (blacklist != null)
            blackListData = blacklist.blackList;

        return blackListData;
    }

    private string ReadFromFile(string fileName)
    {
        if (File.Exists(fileName))
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string json = reader.ReadToEnd();
                return json;
            }
        }
        else
        {
            Debug.LogWarning("File not Found");
            return "";
        }
    }
}
