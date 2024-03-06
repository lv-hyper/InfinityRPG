using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

namespace InGame.Data.SaveData
{

    [Serializable]
    public class CharacterSaveData
    {
        public string level;
        public string statPoint;
        public string statStr, statInt, statDex, statVit, statEnd;

        public float currentStamina, maxStamina;
        public string energy, winCount, encounterCount;
        public string gold;

        public string expToString, maxExpToString;

        public float posX, posY, posZ;

        public string mapName;

        public string characterClass;

        public CharacterSaveData(
            Data.Character.RawCharacterStat stat,
            float currentStamina, float maxStamina,
            BigInteger energy, BigInteger winCount, BigInteger encounterCount, BigInteger gold,
            float posX, float posY, float posZ,
            string mapName, 
            string characterClass
        )
        {
            this.level = stat.GetLevel().ToString();
            this.statPoint = stat.GetStatPoint().ToString();

            this.statStr = stat.GetStatStr().ToString();
            this.statInt = stat.GetStatInt().ToString();
            this.statDex = stat.GetStatDex().ToString();
            this.statVit = stat.GetStatVit().ToString();
            this.statEnd = stat.GetStatEnd().ToString();

            this.currentStamina = currentStamina;
            this.maxStamina = maxStamina;

            this.energy = energy.ToString();
            this.winCount = winCount.ToString();
            this.encounterCount = encounterCount.ToString();
            this.gold = gold.ToString();

            expToString = stat.GetExp().ToString();
            maxExpToString = stat.GetMaxExp().ToString();

            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;

            this.mapName = mapName;
            this.characterClass = characterClass;
        }
    }

    public class CharacterSaveDataManager
    {
        public static void SaveCharacterData(CharacterSaveData data)
        {
            string jsonString = JsonUtility.ToJson(data);

            string encString = EncryptString.AESEncrypt128(jsonString);

            File.WriteAllText(
                string.Format("{0}/{1}.enc",Application.persistentDataPath,"CharacterSaveData"), 
                encString
            );
        }

        
        public static CharacterSaveData LoadCharacterData()
        {
            string jsonString;
            bool isOld = false;
            try{
                string oldPath = string.Format("{0}/{1}.json",Application.persistentDataPath,"CharacterSaveData");
                string encPath = string.Format("{0}/{1}.enc",Application.persistentDataPath,"CharacterSaveData");
                if(File.Exists(encPath))
                {
                    jsonString = File.ReadAllText(
                        string.Format("{0}/{1}.enc",Application.persistentDataPath,"CharacterSaveData")
                    );
                    jsonString = EncryptString.AESDecrypt128(jsonString);
                }
                else
                {
                    isOld = true;
                    jsonString = File.ReadAllText(
                        string.Format("{0}/{1}.json",Application.persistentDataPath,"CharacterSaveData")
                    );
                    File.Delete(oldPath);
                }
            }
            catch(Exception e)
            {
                jsonString = "";
                return null;
            }

            CharacterSaveData saveData = JsonUtility.FromJson<CharacterSaveData>(jsonString);

            if(isOld)
            {
                SaveCharacterData(saveData);
            }

            return saveData;
        }
    }
}