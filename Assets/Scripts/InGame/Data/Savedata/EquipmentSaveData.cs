using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Data.SaveData
{
    [Serializable]
    public struct EquipmentSaveData
    {
        public List<EquipmentSaveDataElement> saveData;

        public EquipmentSaveData(List<EquipmentSaveDataElement> saveData)
        {
            this.saveData = saveData;
        }
    }

    public struct AccessoryUnlockSaveData
    {
        public List<bool> accLocked;

        public AccessoryUnlockSaveData(List<bool> accLocked)
        {
            this.accLocked = accLocked;
        }
    }
    
    [Serializable]
    public struct EquipmentSaveDataElement
    {
        public string setName;
        public string weaponID, helmetID, robeID, gloveID, greaveID;
        public List<string> accessoryIDList;
        public List<string> skillIDList;
        public List<int> skillPriorityList;

        public EquipmentSaveDataElement(
            string setName, string weaponID, string helmetID, string robeID, string gloveID, string greaveID,
            List<string> accessoryIDList, List<string> skillIDList, List<int> skillPriorityList
        )
        {
            this.setName            = setName;

            this.weaponID           = weaponID;

            this.helmetID           = helmetID;
            this.robeID             = robeID;
            this.gloveID            = gloveID;
            this.greaveID           = greaveID;

            this.accessoryIDList    = accessoryIDList;
            this.skillIDList        = skillIDList;
            this.skillPriorityList  = skillPriorityList;
        }
    }
    public class EquipmentSaveDataManager
    {
        public static void SaveEquipmentData(List<Item.EquipmentSet> equipmentSets)
        {
            List<EquipmentSaveDataElement> equipmentSaveDatas = new List<EquipmentSaveDataElement>();

            foreach(var equipmentData in equipmentSets)
            {
                Item.Item weapon, helmet, robe, glove, greave;
                List<Item.Item> accessories = new List<Item.Item>();
                List<Skill.ActiveSkill> skills = new List<Skill.ActiveSkill>();
                List<int> skillPriorityList = new List<int>();

                weapon      = equipmentData.GetItem("weapon");
                helmet      = equipmentData.GetItem("helmet");
                robe        = equipmentData.GetItem("robe");
                glove       = equipmentData.GetItem("glove");
                greave      = equipmentData.GetItem("greave");

                for (int i = 0; i < 10; ++i)
                {
                    accessories.Add(equipmentData.GetItem(String.Format("accessory{0}", i + 1)));
                }

                for (int i = 0; i < 6; ++i)
                {
                    skills.Add(equipmentData.GetSkill(i));
                    skillPriorityList.Add(equipmentData.GetSkillPriority(i));
                }

                List<string> accesoryItemIDList = new List<string>();
                List<string> skillIDList = new List<string>();

                for(int i=0; i<accessories.Count; ++i)
                {
                    if (accessories[i] != null)
                        accesoryItemIDList.Add(accessories[i].itemID);
                    else
                        accesoryItemIDList.Add("");
                }


                for (int i = 0; i < skills.Count; ++i)
                {
                    if (skills[i] != null)
                        skillIDList.Add(skills[i].GetSkillID());
                    else
                        skillIDList.Add("");
                }



                equipmentSaveDatas.Add(new EquipmentSaveDataElement(
                    equipmentData.GetSetName(),
                    (weapon!=null)?     weapon    .itemID : "",    
                    (helmet!=null)?     helmet    .itemID : "",    
                    (robe!=null)?       robe      .itemID : "",      
                    (glove!=null)?      glove     .itemID : "",     
                    (greave!=null)?     greave    .itemID : "",    
                    accesoryItemIDList,
                    skillIDList,
                    skillPriorityList
                ));
            }

            string equipmentSaveDataJson = JsonUtility.ToJson(new EquipmentSaveData(equipmentSaveDatas), true);

            Debug.Log(equipmentSaveDataJson);

            string encryptedEquipmentDataJson = EncryptString.AESEncrypt128(equipmentSaveDataJson);

            Directory.CreateDirectory(
                string.Format(
                    "{0}/{1}",
                    Application.persistentDataPath,
                    Entity.Character.GetInstance().GetCurrentClass()
                )
            );

            File.WriteAllText(
                string.Format(
                    "{0}/{1}/{2}.enc",
                    Application.persistentDataPath,
                    Entity.Character.GetInstance().GetCurrentClass(),
                    "EquipmentSaveData"
                ), 
                encryptedEquipmentDataJson
            );
        }

        public static void SaveSlotData(List<bool> accLocked)
        {
            string accLockedDataJson = JsonUtility.ToJson(new AccessoryUnlockSaveData(accLocked), true);
            string encryptedAccLockedDataJson = EncryptString.AESEncrypt128(accLockedDataJson);

            File.WriteAllText(
                string.Format(
                    "{0}/{1}.enc",
                    Application.persistentDataPath,
                    "AccUnlockData"
                ),
                encryptedAccLockedDataJson
            );
        }

        public static void LoadEquipmentData(out List<Item.EquipmentSet> result)
        {
            string equipmentJsonString = "";
            
            try{
            
                string encryptedEquipmentDataPath = string.Format(
                    "{0}/{1}/{2}.enc",
                    Application.persistentDataPath,
                    Entity.Character.GetInstance().GetCurrentClass(),
                    "EquipmentSaveData"
                );

                if (File.Exists(encryptedEquipmentDataPath))
                {
                    equipmentJsonString = File.ReadAllText(encryptedEquipmentDataPath);
                    equipmentJsonString = EncryptString.AESDecrypt128(equipmentJsonString);
                }
                else throw new Exception("No such file");
            }
            catch(Exception e)
            {
                result = null;
                return;
            }

            EquipmentSaveData equipmentSaveData = JsonUtility.FromJson<EquipmentSaveData>(equipmentJsonString);

            List<Item.EquipmentSet> equipmentSets = new List<Item.EquipmentSet>();

            foreach(var equipmentData in equipmentSaveData.saveData)
            {
                var equipmentSet = new Item.EquipmentSet();
                var itemCollection = Item.ItemCollection.GetInstance().GetCollection("all");
                var skillCollection = Skill.SkillCollection.GetInstance().GetCollection("active");

                equipmentSet.SetSetName(equipmentData.setName);

                if(equipmentData.weaponID != "" && equipmentData.weaponID != null && itemCollection.ContainsKey(equipmentData.weaponID))
                    equipmentSet.SetItem("weapon",      itemCollection[equipmentData.weaponID].getItem());

                if (equipmentData.helmetID != "" && equipmentData.helmetID != null && itemCollection.ContainsKey(equipmentData.helmetID))
                    equipmentSet.SetItem("helmet",      itemCollection[equipmentData.helmetID].getItem());
                
                if(equipmentData.robeID != "" && equipmentData.robeID != null && itemCollection.ContainsKey(equipmentData.robeID))
                    equipmentSet.SetItem("robe",        itemCollection[equipmentData.robeID].getItem());
                
                if(equipmentData.gloveID != "" && equipmentData.gloveID != null && itemCollection.ContainsKey(equipmentData.gloveID))
                    equipmentSet.SetItem("glove",       itemCollection[equipmentData.gloveID].getItem());
                
                if(equipmentData.greaveID != "" && equipmentData.greaveID != null && itemCollection.ContainsKey(equipmentData.greaveID))
                    equipmentSet.SetItem("greave",      itemCollection[equipmentData.greaveID].getItem());

                for(int i=0;i<equipmentData.accessoryIDList.Count;++i)
                {
                    if(equipmentData.accessoryIDList[i] != "" && equipmentData.accessoryIDList != null && itemCollection.ContainsKey(equipmentData.accessoryIDList[i]))
                    {
                        equipmentSet.SetItem(
                            String.Format("accessory{0}", i + 1), 
                            itemCollection[equipmentData.accessoryIDList[i]].getItem()
                        );
                    }
                }


                for (int i = 0; i < equipmentData.skillIDList.Count; ++i)
                {
                    if (equipmentData.skillIDList[i] != "" && equipmentData.skillIDList != null && skillCollection.ContainsKey(equipmentData.skillIDList[i]))
                    {
                        equipmentSet.SetSkill(
                            i, 
                            (Skill.ActiveSkill)skillCollection[equipmentData.skillIDList[i]].GetSkill()
                        );
                    }

                    if(equipmentData.skillPriorityList.Count > i)
                        equipmentSet.SetSkillPriority(i, equipmentData.skillPriorityList[i]);
                }

                equipmentSet.CorrectEquipmentSet();
                equipmentSets.Add(equipmentSet);
            }

            result = equipmentSets;
        }

        public static void LoadSlotData(out List<bool> lockedInfo)
        {
            string accUnlockJsonString = "";
            try
            {
                string encryptedAccUnlockDataPath = string.Format(
                    "{0}/{1}.enc",
                    Application.persistentDataPath,
                    "AccUnlockData"
                );

                if (File.Exists(encryptedAccUnlockDataPath))
                {
                    accUnlockJsonString = File.ReadAllText(encryptedAccUnlockDataPath);
                    accUnlockJsonString = EncryptString.AESDecrypt128(accUnlockJsonString);
                }
                else throw new Exception("No such file");
            }
            catch (Exception e)
            {
                lockedInfo = null;
                return;
            }

            AccessoryUnlockSaveData accessoryUnlockSaveData = JsonUtility.FromJson<AccessoryUnlockSaveData>(accUnlockJsonString);

            if (accessoryUnlockSaveData.accLocked != null && accessoryUnlockSaveData.accLocked.Count > 0)
            {
                lockedInfo = accessoryUnlockSaveData.accLocked;
            }
            else
            {
                lockedInfo = new List<bool>();
                lockedInfo.Add(false);
                lockedInfo.Add(false);
                lockedInfo.Add(false);
                lockedInfo.Add(true);
                lockedInfo.Add(true);
                lockedInfo.Add(true);
                lockedInfo.Add(true);
                lockedInfo.Add(true);
                lockedInfo.Add(true);
                lockedInfo.Add(true);
            }
        }
    }
}