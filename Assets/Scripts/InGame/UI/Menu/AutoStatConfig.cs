using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Common.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InGame.UI.Menu
{
    [Serializable]
    public class AutoStatInfo
    {
        [SerializeField]
        public int strRatio = 0, intRatio = 0, dexRatio = 0, vitRatio = 0, endRatio = 0;

        public int GetSum() { return strRatio + intRatio + dexRatio + vitRatio + endRatio; }
    }

    public class AutoStatManager
    {
        static AutoStatManager instance;
        AutoStatInfo statInfo;

        AutoStatManager()
        {
            LoadStatData();
        }

        public static AutoStatManager GetInstance()
        {
            if(instance == null)
            {
                instance = new AutoStatManager();
            }

            return instance;
        }

        public void LoadStatData()
        {
            string path = string.Format(
                "{0}/{1}/{2}.json",
                Application.persistentDataPath,
                Entity.Character.GetInstance().GetCurrentClass(),
                "AutoStatConfig"
            );

            string jsonString = "";
            if (File.Exists(path))
            {
                jsonString = File.ReadAllText(path);
            }

            statInfo = JsonUtility.FromJson<AutoStatInfo>(jsonString);

            if(statInfo == null) statInfo = new AutoStatInfo();
        }

        public void SaveStatData()
        {
            Directory.CreateDirectory(
                string.Format(
                    "{0}/{1}",
                    Application.persistentDataPath,
                    Entity.Character.GetInstance().GetCurrentClass()
                )
            ); 
            
            string path = string.Format(
                 "{0}/{1}/{2}.json",
                 Application.persistentDataPath,
                 Entity.Character.GetInstance().GetCurrentClass(),
                 "AutoStatConfig"
             );

            File.WriteAllText(
                path,
                JsonUtility.ToJson(statInfo)
            );
        }

        public void AdjustStat(string statName, int amount)
        {
            switch(statName)
            {
                case "STR":
                    if (statInfo.strRatio + amount >= 0)
                        statInfo.strRatio += amount;
                    break;
                case "INT":
                    if (statInfo.intRatio + amount >= 0)
                        statInfo.intRatio += amount;
                    break;
                case "DEX":
                    if (statInfo.dexRatio + amount >= 0)
                        statInfo.dexRatio += amount;
                    break;
                case "VIT":
                    if (statInfo.vitRatio + amount >= 0)
                        statInfo.vitRatio += amount;
                    break;
                case "END":
                    if (statInfo.endRatio + amount >= 0)
                        statInfo.endRatio += amount;
                    break;
            }

            SaveStatData();
        }

        public int GetRatio(string statName, int amount)
        {
            switch (statName)
            {
                case "STR":
                    return statInfo.strRatio;
                case "INT":
                    return statInfo.intRatio;
                case "DEX":
                    return statInfo.dexRatio;
                case "VIT":
                    return statInfo.vitRatio;
                case "END":
                    return statInfo.endRatio;
                default:
                    return 0;
            }
        }

        public AutoStatInfo GetRatio() { return statInfo; }
    }


    public class AutoStatConfig : MenuPage
    {
        [SerializeField] TextMeshProUGUI strText, intText, dexText, vitText, endText;

        private void Awake()
        {
            UpdateDisplay();
        }

        public void AddStat(string statName)
        {
            AutoStatManager.GetInstance().AdjustStat(statName, 1);
        }
        public void MinusStat(string statName)
        {
            AutoStatManager.GetInstance().AdjustStat(statName, -1);
        }

        public void UpdateDisplay()
        {
            AutoStatInfo info = AutoStatManager.GetInstance().GetRatio();

            strText.text = info.strRatio.ToString();
            intText.text = info.intRatio.ToString();
            dexText.text = info.dexRatio.ToString();
            vitText.text = info.vitRatio.ToString();
            endText.text = info.endRatio.ToString();
        }
    }
}
