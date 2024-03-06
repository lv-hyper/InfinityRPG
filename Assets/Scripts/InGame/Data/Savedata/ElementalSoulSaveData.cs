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
    public struct ElementalSoulSaveData
    {
        static List<int> perkCost
        {
            get
            {
                return new List<int> { 2, 3, 5, 7, 11 };
            }
        }

        [Serializable]
        public struct Perk
        {
            public Perk(int _tier, int _order)
            {
                tier = _tier;
                order = _order;
            }

            public int tier;
            public int order;
        }

        [Serializable]
        public class Preset
        {
            [SerializeField] string presetName;
            [SerializeField] string minifiedName;
            [SerializeField] List<Perk> perkList;

            public Preset(string _presetName, string _minifiedName)
            {
                presetName = _presetName;
                minifiedName = _minifiedName;
                perkList = new List<Perk>();
            }

            public string GetPresetName()
            {
                return presetName;
            }

            public string GetMinifiedName()
            {
                return minifiedName;
            }

            public void AddPerk(Perk perk)
            {
                perkList.Add(perk);
            }

            public int GetUsedSoul()
            {
                return perkList.Sum(perk => perkCost[perk.tier]);
            }

            public void ClearPreset()
            {
                perkList.Clear();
            }

            public List<Perk> GetPerks() { return perkList; }

            public bool Exists(Perk _perk)
            {
                return perkList.Any(perk => perk.order == _perk.order && perk.tier == _perk.tier);
            }
        }

        [Serializable]
        public struct Element
        {
            [SerializeField] string elemental;
            [SerializeField] int gainedSoul;
            [SerializeField] List<Preset> presets;
            [SerializeField] int currentPresetIndex;

            public Element(
                string _elemental,
                int _gainedSoul = 0,
                List<Preset> _preset = null,
                int _currentPresetIndex = 0
            )
            {
                elemental = _elemental;
                gainedSoul = _gainedSoul;
                presets = _preset;
                currentPresetIndex = _currentPresetIndex;

                if (presets == null)
                {
                    presets = new List<Preset>();

                    presets.Add(new Preset("Set A", "A"));
                    presets.Add(new Preset("Set B", "B"));
                    presets.Add(new Preset("Set C", "C"));
                    presets.Add(new Preset("Set D", "D"));
                }
            }

            public void GainSoul(int amount)
            {
                gainedSoul += amount;
            }

            public bool PerkUnlockable(Perk _perk)
            {
                return !Exists(_perk) && gainedSoul - presets[currentPresetIndex].GetUsedSoul() >= perkCost[_perk.tier];
            }

            public void AddPerk(Perk _perk)
            {
                if(PerkUnlockable(_perk))
                {
                    presets[currentPresetIndex].AddPerk(_perk);
                }
            }

            public bool Exists(Perk _perk)
            {
                return presets[currentPresetIndex].Exists(_perk);
            }

            public void Clear()
            {
                presets[currentPresetIndex].ClearPreset();
            }

            public string GetElemental() { return elemental; }
            public int GetGainedSoul() { return gainedSoul; }
            public int GetUsedSoul() { return presets[currentPresetIndex].GetUsedSoul(); }
            public List<Preset> GetPresets() { return presets; }
            public void AddPreset(Preset _preset) { presets.Add(_preset); }
            public int GetCurrentPresetIndex() { return currentPresetIndex; }
            public void SetPreset(string presetName) {
                var targetIndex = GetPresets().FindIndex(preset => preset.GetPresetName() == presetName);

                if(targetIndex != -1)
                {
                    currentPresetIndex = targetIndex;
                }
            }
            public void NextPreset()
            {
                ++currentPresetIndex;
                if(currentPresetIndex >= presets.Count) currentPresetIndex = 0;
            }

            public void ClearPreset()
            {
                presets.Clear();
            }
        }

        public List<Element> saveData;
        public int version;

        public ElementalSoulSaveData(List<Element> saveData)
        {
            this.saveData = saveData;
            this.version = 2;
        }

        public class Manager
        {
            public static void SaveElementalSoulData(Dictionary<EnumElemental, Element> elementalSoulData)
            {
                List<Element> saveData = new List<Element>();

                foreach (var data in elementalSoulData)
                {
                    saveData.Add(new Element(
                        data.Key.ToString(),
                        data.Value.GetGainedSoul(),
                        data.Value.GetPresets(),
                        data.Value.GetCurrentPresetIndex()
                    ));
                }

                string jsonString = JsonUtility.ToJson(new ElementalSoulSaveData(saveData));

                string encString = EncryptString.AESEncrypt128(jsonString);

                File.WriteAllText(
                    string.Format("{0}/{1}.enc", Application.persistentDataPath, "ElementalSoulSaveData"),
                    encString
                );
            }

            public static Dictionary<EnumElemental, ElementalSoulSaveData.Element> LoadElementalSoulData()
            {
                string jsonString;
                bool isOld = false;
                try
                {
                    string oldPath = string.Format("{0}/{1}.json", Application.persistentDataPath, "ElementalSoulSaveData");
                    string encPath = string.Format("{0}/{1}.enc", Application.persistentDataPath, "ElementalSoulSaveData");
                    if (File.Exists(encPath))
                    {
                        jsonString = File.ReadAllText(
                            string.Format("{0}/{1}.enc", Application.persistentDataPath, "ElementalSoulSaveData")
                        );
                        jsonString = EncryptString.AESDecrypt128(jsonString);
                    }
                    else
                    {
                        isOld = true;
                        jsonString = File.ReadAllText(
                            string.Format("{0}/{1}.json", Application.persistentDataPath, "ElementalSoulSaveData")
                        );
                        File.Delete(oldPath);
                    }
                }
                catch (Exception e)
                {
                    jsonString = "";
                    return null;
                }

                ElementalSoulSaveData saveData = JsonUtility.FromJson<ElementalSoulSaveData>(jsonString);

                List<ElementalSoulSaveData.Element> elementList = saveData.saveData;

                Dictionary<EnumElemental, ElementalSoulSaveData.Element> result = new Dictionary<EnumElemental, ElementalSoulSaveData.Element>();

                if (isOld)
                {
                    SaveElementalSoulData(result);
                }

                foreach (var element in elementList)
                {
                    var elementalEnum = Enum.Parse(typeof(EnumElemental), element.GetElemental());

                    if (saveData.version != 2)
                    {
                        element.ClearPreset();
                    }

                    if (element.GetPresets().Count <= 0)
                    {
                        element.AddPreset(new Preset("Set A", "A"));
                        element.AddPreset(new Preset("Set B", "B"));
                        element.AddPreset(new Preset("Set C", "C"));
                        element.AddPreset(new Preset("Set D", "D"));
                    }

                    if(elementalEnum != null)
                        result.Add((EnumElemental)elementalEnum, element);
                }

                return result;
            }
        }
    }
}
