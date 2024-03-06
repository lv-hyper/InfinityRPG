using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Data.SaveData
{
    [Serializable]
    public struct InventorySaveData
    {
        public List<InventorySaveDataElement> saveData;

        public InventorySaveData(List<InventorySaveDataElement> saveData)
        {
            this.saveData = saveData;
        }
    }
    
    [Serializable]
    public struct InventorySaveDataElement
    {
        [SerializeField] string itemID;
        [SerializeField] int itemCount;
        public InventorySaveDataElement(
            string itemID, int itemCount
        )
        {
            this.itemID = itemID;
            this.itemCount = itemCount;
        }

        public string GetID(){return itemID;}
        public int GetCount(){return itemCount;}
    }
    public class InventorySaveDataManager
    {
        public static void SaveInventoryData()
        {
            var itemCollection = Data.Item.ItemCollection.GetInstance().allCollection;
            List<InventorySaveDataElement> saveData = new List<InventorySaveDataElement>();

            foreach(var itemData in itemCollection)
            {
                saveData.Add(new InventorySaveDataElement(itemData.Key, itemData.Value.getCount()));
            }

            string jsonString = JsonUtility.ToJson(new InventorySaveData(saveData));

            string encString = EncryptString.AESEncrypt128(jsonString);

            File.WriteAllText(
                string.Format("{0}/{1}.enc",Application.persistentDataPath,"InventorySaveData"), 
                encString
            );
        }

        public static bool LoadInventoryData()
        {
            string jsonString;
            bool isOld = false;
            try{
                string oldPath = string.Format("{0}/{1}.json",Application.persistentDataPath,"InventorySaveData");
                string encPath = string.Format("{0}/{1}.enc",Application.persistentDataPath,"InventorySaveData");
                
                if(File.Exists(encPath))
                {
                    jsonString = File.ReadAllText(
                        string.Format("{0}/{1}.enc",Application.persistentDataPath,"InventorySaveData")
                    );
                    jsonString = EncryptString.AESDecrypt128(jsonString);
                }
                else
                {
                    isOld = true;
                    jsonString = File.ReadAllText(
                        string.Format("{0}/{1}.json",Application.persistentDataPath,"InventorySaveData")
                    );
                    File.Delete(oldPath);
                }
            }
            catch(Exception e)
            {
                
                jsonString = "";
                return false;
            }

            InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(jsonString);

            List<InventorySaveDataElement> elementList = saveData.saveData;

            Data.Item.ItemCollection.GetInstance().ClearCollection();

            foreach(var element in elementList)
            {   
                Data.Item.ItemCollection.GetInstance().SetItemCount(element.GetID(), element.GetCount());
            }

            if(isOld)
            {
                SaveInventoryData();
            }

            return true;
        }
    }
}