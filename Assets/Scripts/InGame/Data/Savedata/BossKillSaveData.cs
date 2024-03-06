using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Data.SaveData
{
    [Serializable]
    public struct BossKillSaveData
    {
        public List<BossKillSaveDataElement> saveData;

        public BossKillSaveData(List<BossKillSaveDataElement> saveData)
        {
            this.saveData = saveData;
        }
    }

    [Serializable]
    public struct BossKillSaveDataElement
    {
        [SerializeField] string bossID;
        [SerializeField] bool isKilled;
        public BossKillSaveDataElement(
            string bossID, bool isKilled
        )
        {
            this.bossID = bossID;
            this.isKilled = isKilled;
        }

        public string GetID() { return bossID; }

        public bool GetIsKilled() { return isKilled; }
    }

    public class BossKillSaveDataManager
    {
        public static void SaveBossKillData(Dictionary<string, bool> bossKillData)
        {
            List<BossKillSaveDataElement> saveData = new List<BossKillSaveDataElement>();

            foreach (var data in bossKillData)
            {
                saveData.Add(new BossKillSaveDataElement(data.Key, data.Value));
            }

            string jsonString = JsonUtility.ToJson(new BossKillSaveData(saveData));

            string encString = EncryptString.AESEncrypt128(jsonString);

            File.WriteAllText(
                string.Format("{0}/{1}.enc", Application.persistentDataPath, "BossKillSaveData"),
                encString
            );
        }


        public static Dictionary<string, bool> LoadBossKillData()
        {
            string jsonString;
            bool isOld = false;
            try
            {
                string oldPath = string.Format("{0}/{1}.json", Application.persistentDataPath, "BossKillSaveData");
                string encPath = string.Format("{0}/{1}.enc", Application.persistentDataPath, "BossKillSaveData");
                if (File.Exists(encPath))
                {
                    jsonString = File.ReadAllText(
                        string.Format("{0}/{1}.enc", Application.persistentDataPath, "BossKillSaveData")
                    );
                    jsonString = EncryptString.AESDecrypt128(jsonString);
                }
                else
                {
                    isOld = true;
                    jsonString = File.ReadAllText(
                        string.Format("{0}/{1}.json", Application.persistentDataPath, "BossKillSaveData")
                    );
                    File.Delete(oldPath);
                }
            }
            catch (Exception e)
            {
                jsonString = "";
                return null;
            }

            BossKillSaveData saveData = JsonUtility.FromJson<BossKillSaveData>(jsonString);

            List<BossKillSaveDataElement> elementList = saveData.saveData;

            Dictionary<string, bool> result = new Dictionary<string, bool>();

            if (isOld)
            {
                SaveBossKillData(result);
            }

            foreach (var element in elementList)
            {
                result.Add(element.GetID(), element.GetIsKilled());
            }

            return result;
        }
    }
}