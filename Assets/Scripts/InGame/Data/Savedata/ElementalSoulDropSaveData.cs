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
    public class ElementalSoulDropSaveData
    {
        public List<ElementalSoulDropSaveDataElement> saveData;

        string difficulty
        {
            get { return PlayerPrefs.GetString("difficulty", "normal"); }
        }


        public ElementalSoulDropSaveData(List<ElementalSoulDropSaveDataElement> saveData)
        {
            this.saveData = saveData;
        }

        public int GetTotalElementalSoul(EnumElemental _elementalType)
        {
            if(saveData == null) return 0;

            return saveData.Where(x => x.GetElementalType() == _elementalType).Sum(x => x.GetElementalSoulAmount());
        }

        public void AddData(string id, EnumElemental _elementalType, int amount)
        {
            if (saveData == null)
                saveData = new List<ElementalSoulDropSaveDataElement>();

            if (difficulty != "normal")
                id = String.Format("{0}_{1}", id, difficulty);

            if(!contains(id))
            {
                Data.ElementalSoulData.GetInstance().GainSoul(_elementalType, amount);
                saveData.Add(new ElementalSoulDropSaveDataElement(id, _elementalType, amount));
            }

            else
            {
                var data = saveData.Find(x => x.GetID() == id);
                if(data.GetElementalSoulAmount() != amount)
                {
                    int soulDiff = amount - data.GetElementalSoulAmount();

                    saveData.Remove(data);
                    Data.ElementalSoulData.GetInstance().GainSoul(_elementalType, soulDiff);
                    saveData.Add(new ElementalSoulDropSaveDataElement(id, _elementalType, amount));
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
    }

    [Serializable]
    public struct ElementalSoulDropSaveDataElement
    {
        [SerializeField] string bossID;
        [SerializeField] EnumElemental elementalType;
        [SerializeField] int elementalSoulAmount;
        public ElementalSoulDropSaveDataElement(
            string bossID, EnumElemental elementalType, int elementalSoulAmount
        )
        {
            this.bossID = bossID;
            this.elementalType = elementalType;
            this.elementalSoulAmount = elementalSoulAmount;
        }

        public string GetID() { return bossID; }

        public EnumElemental GetElementalType() { return elementalType; }

        public int GetElementalSoulAmount() { return elementalSoulAmount; }
    }

    public class ElementalSoulDropSaveDataManager
    {
        public static void SaveElementalSoulDropSaveData(ElementalSoulDropSaveData data)
        {
            string jsonString = JsonUtility.ToJson(data);

            string encString = EncryptString.AESEncrypt128(jsonString);

            File.WriteAllText(
                string.Format("{0}/{1}.enc", Application.persistentDataPath, "ElementalSoulDropSaveData"),
                encString
            );
        }

        public static ElementalSoulDropSaveData LoadElementalSoulDropSaveData()
        {
            string path = string.Format(
                "{0}/{1}.enc",
                Application.persistentDataPath, "ElementalSoulDropSaveData"
            );

            try
            {
                string jsonString = File.ReadAllText(path);
                jsonString = EncryptString.AESDecrypt128(jsonString);
                var elementalSoulDropSaveData = JsonUtility.FromJson<ElementalSoulDropSaveData>(jsonString);

                return elementalSoulDropSaveData;
            }
            catch
            {
                return new ElementalSoulDropSaveData(null);
            }
        }

    }


}
