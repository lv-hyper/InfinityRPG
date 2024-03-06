using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.SaveData
{
    [Serializable]
    public class GameConditionSaveData
    {
        public List<GameConditionSaveDataElement> saveData;


        public GameConditionSaveData(List<GameConditionSaveDataElement> saveData)
        {
            this.saveData = saveData;
        }

        public void AddData(GameConditionSaveDataElement element)
        {
            saveData.Add(element);
        }

        public bool contains(string componentId)
        {
            return saveData.Any(x => { return x.GetId() == componentId; });
        }
    }

    [Serializable]
    public struct GameConditionSaveDataElement
    {
        [SerializeField] string conditionComponentId;
        [SerializeField] bool value;

        public GameConditionSaveDataElement(string _id, bool _value)
        {
            this.conditionComponentId = _id;
            this.value = _value;
        }

        public string GetId()
        {
            return conditionComponentId;
        }

        public bool GetValue()
        {
            return value;
        }
    }

    public class GameConditionSaveDataManager
    {
        static GameConditionSaveDataManager instance;

        GameConditionSaveData saveData;

        bool dataChanged;

        private GameConditionSaveDataManager()
        {
            dataChanged = true;
            saveData = new GameConditionSaveData(new List<GameConditionSaveDataElement>());
        }
        public static GameConditionSaveDataManager GetInstance()
        {
            if (instance == null)
            {
                instance = new GameConditionSaveDataManager();
                LoadGameConditionData();
            }

            return instance;
        }

        public static void SetCondition(string componentId, bool value)
        {
            GetInstance().saveData.AddData(new GameConditionSaveDataElement(componentId, value));
            SaveGameConditionData(GetInstance().saveData, false);
        }

        public static bool GetCondition(string componentId)
        {
            if(GetInstance().dataChanged)
            {
                LoadGameConditionData();
            }

            if(ContainsCondition(componentId))
            {
                return GetInstance().saveData.saveData.Find(x => x.GetId() == componentId).GetValue();
            }
            else
            {
                return false;
            }
        }

        public static bool ContainsCondition(string componentId)
        {
            if (GetInstance().dataChanged)
            {
                LoadGameConditionData();
            }

            return GetInstance().saveData.contains(componentId);
        }

        public static void ClearCondition()
        {
            GetInstance().saveData.saveData.Clear();
            SaveGameConditionData(GetInstance().saveData, false);
        }

        public static void SaveGameConditionData(GameConditionSaveData data, bool dataChanged = true)
        {
            string jsonString = JsonUtility.ToJson(data);

            string encString = EncryptString.AESEncrypt128(jsonString);

            File.WriteAllText(
                string.Format("{0}/{1}.enc", Application.persistentDataPath, "GameConditionSaveData"),
                encString
            );

            GetInstance().dataChanged = dataChanged;
        }

        public static GameConditionSaveData LoadGameConditionData()
        {
            string path = string.Format(
                "{0}/{1}.enc",
                Application.persistentDataPath, "GameConditionSaveData"
            );

            if (!GetInstance().dataChanged)
                return GetInstance().saveData;

            try
            {
                string jsonString = File.ReadAllText(path);
                jsonString = EncryptString.AESDecrypt128(jsonString);
                var gameConditionSaveData = JsonUtility.FromJson<GameConditionSaveData>(jsonString);

                GetInstance().saveData = gameConditionSaveData;
                GetInstance().dataChanged = false;

                return gameConditionSaveData;
            }
            catch
            {
                var _saveData = new GameConditionSaveData(new List<GameConditionSaveDataElement>());
                GetInstance().saveData = _saveData;
                GetInstance().dataChanged = false;
                return _saveData;
            }
        }

        public static void RemoveCondition(string triggerID)
        {
            GetInstance().saveData.saveData.RemoveAll(x=>x.GetId()==triggerID);
        }
    }


}
