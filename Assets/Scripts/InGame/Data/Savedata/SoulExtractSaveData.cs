using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.SaveData
{
    [Serializable]
    public class SoulExtractSaveData
    {
        public List<SoulExtractSaveDataElement> saveData;
        public int soulExtractVersion;

        string difficulty
        {
            get { return PlayerPrefs.GetString("difficulty", "normal"); }
        }


        public SoulExtractSaveData(List<SoulExtractSaveDataElement> saveData,
            int soulExtractVersion = 3)
        {
            this.saveData = saveData;
            this.soulExtractVersion = soulExtractVersion;
        }

        public int GetTotalEnergy()
        {
            if(saveData == null) return 0;

            int totalEnergy = 0;

            foreach (SoulExtractSaveDataElement element in saveData)
            {
                var tmp = element.GetID().Split('_');
                string bossDifficulty = tmp[tmp.Length - 1];
                string[] diffList = { "hard", "void" };

                if(!diffList.Contains(bossDifficulty))
                {
                    if (Data.SaveData.PlayerPrefsUtility.gameDifficulty != "normal")
                        continue;
                }

                else if (bossDifficulty != Data.SaveData.PlayerPrefsUtility.gameDifficulty)
                    continue;

                totalEnergy += element.GetEnergyAmount();
            }

            return totalEnergy;
        }

        public int GetTotalEnergyByDifficulty(string difficulty)
        {
            if(saveData == null) return 0;

            int totalEnergy = 0;

            foreach (SoulExtractSaveDataElement element in saveData)
            {
                var tmp = element.GetID().Split('_');
                string bossDifficulty = tmp[tmp.Length - 1];
                string[] diffList = { "hard", "void" };

                if(!diffList.Contains(bossDifficulty))
                {
                    if (difficulty != "normal")
                        continue;
                }

                else if (bossDifficulty != difficulty)
                    continue;

                totalEnergy += element.GetEnergyAmount();
            }

            return totalEnergy;
        }

        public int GetMaxTotalEnergy(string difficulty)
        {
            /* place holder
            if(saveData == null) return 0;

            int totalEnergy = 0;

            foreach (SoulExtractSaveDataElement element in saveData)
            {
                var tmp = element.GetID().Split('_');
                string bossDifficulty = tmp[tmp.Length - 1];
                string[] diffList = { "hard", "void" };

                if(!diffList.Contains(bossDifficulty))
                {
                    if (Data.SaveData.PlayerPrefsUtility.gameDifficulty != "normal")
                        continue;
                }

                else if (bossDifficulty != Data.SaveData.PlayerPrefsUtility.gameDifficulty)
                    continue;

                totalEnergy += element.GetEnergyAmount();
            }

            return totalEnergy;
            */
            return -26;            
        }

        public void AddData(string id, int amount)
        {
            if (saveData == null)
                saveData = new List<SoulExtractSaveDataElement>();

            if (difficulty != "normal")
                id = String.Format("{0}_{1}", id, difficulty);

            if(!contains(id))
                saveData.Add(new SoulExtractSaveDataElement(id, amount));

            else
            {
                var data = saveData.Find(x => x.GetID() == id);
                if(data.GetEnergyAmount() != amount)
                {
                    saveData.Remove(data);
                    saveData.Add(new SoulExtractSaveDataElement(id, amount));
                }
            }    
        }

        public bool contains(string bossId)
        {
            bool isContain = false;

            if (saveData == null) return false;

            foreach(var element in saveData)
            {
                if(element.GetID() == bossId)
                {
                    isContain = true;
                    break;
                }
            }

            return isContain;
        }

        public bool contains_ignoreCapitalAndBlank(string bossName)//very inefficient mechanism. O(n^2) mechanism and lots of string manipulations. can be O(n) mechanism by using alphabetical sort or others...
        {
            if (saveData == null) return false;
            foreach(var element in saveData)
            {
                Debug.Log(element.GetID().ToLower().Replace(" ", string.Empty) + " / " + bossName.ToLower().Replace(" ", string.Empty));
                if(element.GetID().ToLower().Replace(" ", string.Empty) == bossName.ToLower().Replace(" ", string.Empty))
                {
                    return true;
                }
            }
            return false;
        }

        public int GetVersion() { return soulExtractVersion; }
    }

    [Serializable]
    public struct SoulExtractSaveDataElement
    {
        [SerializeField] string bossID;
        [SerializeField] int energyAmount;
        public SoulExtractSaveDataElement(
            string bossID, int energyAmount
        )
        {
            this.bossID = bossID;
            this.energyAmount = energyAmount;
        }

        public string GetID() { return bossID; }

        public int GetEnergyAmount() { return energyAmount; }
    }

    public class SoulExtractSaveDataManager
    {
        public static void SaveSoulExtractData(SoulExtractSaveData data)
        {
            string jsonString = JsonUtility.ToJson(data);

            string encString = EncryptString.AESEncrypt128(jsonString);

            File.WriteAllText(
                string.Format("{0}/{1}.enc", Application.persistentDataPath, "SoulExtractSaveData"),
                encString
            );
        }

        public static SoulExtractSaveData LoadSoulExtractData()
        {
            string path = string.Format(
                "{0}/{1}.enc",
                Application.persistentDataPath, "SoulExtractSaveData"
            );

            try
            {
                string jsonString = File.ReadAllText(path);
                jsonString = EncryptString.AESDecrypt128(jsonString);
                var soulExtractSaveData = JsonUtility.FromJson<SoulExtractSaveData>(jsonString);

                if(soulExtractSaveData.GetVersion() < 3)
                {
                    return new SoulExtractSaveData(null);
                }
                else
                {
                    return soulExtractSaveData;
                }
            }
            catch
            {
                return new SoulExtractSaveData(null);
            }
        }

    }


}
