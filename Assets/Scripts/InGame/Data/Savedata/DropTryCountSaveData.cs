using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Data.SaveData
{
    [Serializable]
    public struct DropTryCountSaveData
    {
        public List<DropTryCountSaveDataElement> saveData;

        public DropTryCountSaveData(List<DropTryCountSaveDataElement> saveData)
        {
            this.saveData = saveData;
        }
    }

    [Serializable]
    public struct DropTryCountSaveDataElement
    {
        [SerializeField] string bossID;
        [SerializeField] List<float> dropTryCount;
        public DropTryCountSaveDataElement(
            string bossID, List<float> dropTryCount
        )
        {
            this.bossID = bossID;
            this.dropTryCount = dropTryCount;
        }

        public string GetID() { return bossID; }
        public List<float> GetDropTryCount() { return dropTryCount; }
    }

    public class DropTryCountSaveDataManager
    {
        public static void AddDropTryCount(string bossId, int _index, float count = 1)
        {
            var _dict = LoadDropTryCountSaveData();

            if (_dict == null)
            {
                _dict = new Dictionary<string, List<float>>();
            }

            if (!_dict.ContainsKey(bossId))
            {
                _dict.Add(bossId, new List<float>(new float[_index+1]));
            }

            if (_dict[bossId].Count < _index+1)
            {
                _dict[bossId].AddRange(new List<float>(new float[_index+1 - _dict[bossId].Count]));
            }

            _dict[bossId][_index]+=count;

            SaveDropTryCountSaveData(_dict);
        }
        public static void AddAllDropTryCount(string bossId, int dropsCount, float count = 1)
        {
            var _dict = LoadDropTryCountSaveData();

            if (_dict == null)
            {
                _dict = new Dictionary<string, List<float>>();
            }

            if (!_dict.ContainsKey(bossId))
            {
                _dict.Add(bossId, new List<float>(new float[dropsCount]));
            }

            if (_dict[bossId].Count < dropsCount)
            {
                _dict[bossId].AddRange(new List<float>(new float[dropsCount - _dict[bossId].Count]));
            }

            for(int i=0;i<dropsCount;i++)
            {
                _dict[bossId][i]+=count;
            }

            SaveDropTryCountSaveData(_dict);
        }
        public static void ClearDropTryCount(string bossId, int _index)
        {
            var _dict = LoadDropTryCountSaveData();
            if (_dict != null && _dict.ContainsKey(bossId) && _dict[bossId].Count > _index)
            {
                _dict[bossId][_index] = 0;
            }
            SaveDropTryCountSaveData(_dict);
        }
        public static float GetDropTryCount(string bossId, int _index)
        {
            var _dict = LoadDropTryCountSaveData();
            if (_dict != null && _dict.ContainsKey(bossId) && _dict[bossId].Count > _index)
            {
                return _dict[bossId][_index];
            }
            return 0;
        }


        public static void SaveDropTryCountSaveData(Dictionary<string, List<float>> dropTryCountSaveData)
        {
            List<DropTryCountSaveDataElement> saveData = new List<DropTryCountSaveDataElement>();

            if(dropTryCountSaveData != null)
            {
                foreach (var data in dropTryCountSaveData)
                {
                    saveData.Add(new DropTryCountSaveDataElement(data.Key, data.Value));
                }
            }

            string jsonString = JsonUtility.ToJson(new DropTryCountSaveData(saveData));

            string encString = EncryptString.AESEncrypt128(jsonString);

            File.WriteAllText(
                string.Format("{0}/{1}.enc", Application.persistentDataPath, "DropTryCountSaveData"),
                encString
            );
            File.WriteAllText(
                string.Format("{0}/{1}.json", Application.persistentDataPath, "DropTryCountSaveData"),
                jsonString
            );
        }


        public static Dictionary<string, List<float>> LoadDropTryCountSaveData()
        {
            string jsonString = "";
            bool isOld = false;
            try
            {
                string encPath = string.Format("{0}/{1}.enc", Application.persistentDataPath, "DropTryCountSaveData");
                if (File.Exists(encPath))
                {
                    jsonString = File.ReadAllText(
                        string.Format("{0}/{1}.enc", Application.persistentDataPath, "DropTryCountSaveData")
                    );
                    jsonString = EncryptString.AESDecrypt128(jsonString);
                }
            }
            catch (Exception e)
            {
                jsonString = "";
                return null;
            }

            if (jsonString == "")
                return null;

            DropTryCountSaveData saveData = JsonUtility.FromJson<DropTryCountSaveData>(jsonString);

            List<DropTryCountSaveDataElement> elementList = saveData.saveData;

            Dictionary<string, List<float>> result = new Dictionary<string, List<float>>();

            foreach (var element in elementList)
            {
                result.Add(element.GetID(), element.GetDropTryCount());
            }

            return result;
        }
    }
}
