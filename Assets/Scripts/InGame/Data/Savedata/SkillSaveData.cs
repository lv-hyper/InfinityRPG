using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Data.SaveData
{
    [Serializable]
    public struct SkillSaveData
    {
        public List<SkillSaveDataElement> saveData;

        public SkillSaveData(List<SkillSaveDataElement> saveData)
        {
            this.saveData = saveData;
        }
    }
    
    [Serializable]
    public struct SkillSaveDataElement
    {
        [SerializeField] string skillID;
        [SerializeField] long skillUpgradeCount;
        public SkillSaveDataElement(
            string skillID, long skillUpgradeCount
        )
        {
            this.skillID = skillID;
            this.skillUpgradeCount = skillUpgradeCount;
        }

        public string GetID(){return skillID;}
        public long GetCount(){return skillUpgradeCount;}
    }
    public class SkillSaveDataManager
    {
        public static void SaveSkillData()
        {
            var skillCollection = Data.Skill.SkillCollection.GetInstance().allSkillCollection;
            List<SkillSaveDataElement> saveData = new List<SkillSaveDataElement>();

            foreach(var skillData in skillCollection)
            {
                saveData.Add(new SkillSaveDataElement(skillData.Key, skillData.Value.GetCount()));
            }

            string jsonString = JsonUtility.ToJson(new SkillSaveData(saveData));

            string encString = EncryptString.AESEncrypt128(jsonString);

            File.WriteAllText(
                string.Format("{0}/{1}.enc",Application.persistentDataPath,"SkillSaveData"), 
                encString
            );
        }

        public static bool LoadSkillData()
        {
            string jsonString;
            bool isOld = false;
            try{
                string oldPath = string.Format("{0}/{1}.json",Application.persistentDataPath,"SkillSaveData");
                string encPath = string.Format("{0}/{1}.enc",Application.persistentDataPath,"SkillSaveData");
                
                if(File.Exists(encPath))
                {
                    jsonString = File.ReadAllText(
                        string.Format("{0}/{1}.enc",Application.persistentDataPath,"SkillSaveData")
                    );
                    jsonString = EncryptString.AESDecrypt128(jsonString);
                }
                else
                {
                    isOld = true;
                    jsonString = File.ReadAllText(
                        string.Format("{0}/{1}.json",Application.persistentDataPath,"SkillSaveData")
                    );
                    File.Delete(oldPath);
                }
            }
            catch(Exception e)
            {
                
                jsonString = "";
                return false;
            }

            SkillSaveData saveData = JsonUtility.FromJson<SkillSaveData>(jsonString);

            List<SkillSaveDataElement> elementList = saveData.saveData;

            Data.Skill.SkillCollection.GetInstance().ClearCollection();

            foreach(var element in elementList)
            {   
                Data.Skill.SkillCollection.GetInstance().SetSkillUpgradeCount(element.GetID(), element.GetCount());
            }

            if(isOld)
            {
                SaveSkillData();
            }

            return true;
        }
    }
}