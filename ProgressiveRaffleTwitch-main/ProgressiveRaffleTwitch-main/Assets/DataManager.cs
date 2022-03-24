using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

public class DataManager : MonoBehaviour
{
    public Dictionary<string, int> data;
    public List<string> shadowData;
    string file = "ProgressiveRaffle.txt";

    public class rats
    {
        public List<string> names;
        public List<int> weights;

        public rats()
        {
            names = new List<string>();
            weights = new List<int>();
        }

        public rats(Dictionary<string, int> data)
        {
            names = data.Keys.ToList();
            weights = data.Values.ToList();
        }
    }

    public class ShadowRat
    {
        public List<string> blackList;

        public ShadowRat(List<string> list)
        {
            blackList = list;
        }
    }

    public void NewSave(Dictionary<string, int> playerData)
    {
        rats rat = new rats(playerData);
        string json = JsonUtility.ToJson(rat, true);
        Debug.Log(json);
        System.IO.File.WriteAllText("ProgressiveRaffle.txt", json);
    }

    public void ShadowNewSave(List<string> blacklist)
    {
        ShadowRat shadowRat = new ShadowRat(blacklist);
        string json = JsonUtility.ToJson(shadowRat, true);
        Debug.Log(json);
        System.IO.File.WriteAllText("BLACKLIST.txt", json);
    }

    public Dictionary<string, int> Load()
    {
        data = new Dictionary<string, int>();
        string json = ReadFromFile(file);
        rats rat = JsonUtility.FromJson<rats>(json);
        if (rat != null)
            for (int i = 0; i < rat.names.Count; i++)
            {
                data.Add(rat.names[i], rat.weights[i]);
            }
        return data;
    }

    public List<String> ShadowLoad()
    {
        shadowData = new List<string>();
        string json = ShadownReadFromFile(file);
        ShadowRat rat = JsonUtility.FromJson<ShadowRat>(json);
        if (rat != null)
            shadowData = rat.blackList;

        return shadowData;
    }

    private string ReadFromFile(string fileName)
    {
        string path = "ProgressiveRaffle.txt";

        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
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

    private string ShadownReadFromFile(string fileName)
    {
        string path = "BLACKLIST.txt";

        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
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
