using UnityEngine;
using System;
using System.Collections.Generic;

namespace InGame.Data
{
    public class ElementalSoulData : Subject
    {
        public static ElementalSoulData instance;
        Dictionary<EnumElemental, SaveData.ElementalSoulSaveData.Element> data;
        public List<UI.IDisplayComponent> displayComponents { get; set; }

        public static ElementalSoulData GetInstance()
        {
            if (instance == null)
                instance = new ElementalSoulData();

            return instance;
        }

        private ElementalSoulData()
        {
            data = SaveData.ElementalSoulSaveData.Manager.LoadElementalSoulData();

            if (data == null)
                data = new Dictionary<EnumElemental, SaveData.ElementalSoulSaveData.Element>();

            displayComponents = new List<UI.IDisplayComponent>();

            SaveData.ElementalSoulSaveData.Manager.SaveElementalSoulData(data);
        }

        public void GainSoul(EnumElemental _elemental, int amount)
        {
            if (!data.ContainsKey(_elemental))
                data.Add(_elemental, new SaveData.ElementalSoulSaveData.Element(_elemental.ToString()));

            var x = data[_elemental];
            x.GainSoul(amount);
            data[_elemental] = x;

            //data[_elemental].GainSoul(amount);
            SaveData.ElementalSoulSaveData.Manager.SaveElementalSoulData(data);
        }

        public int GetGainedSoulCount(EnumElemental _elemental)
        {
            if (data.ContainsKey(_elemental))
                return data[_elemental].GetGainedSoul();

            else return 0;
        }

        public int GetUsedSoulCount(EnumElemental _elemental)
        {
            if (data.ContainsKey(_elemental))
                return data[_elemental].GetUsedSoul();

            else return 0;
        }

        public void UnlockPerk(EnumElemental _elemental, SaveData.ElementalSoulSaveData.Perk perk)
        {
            if (!data.ContainsKey(_elemental))
                data.Add(_elemental, new SaveData.ElementalSoulSaveData.Element(_elemental.ToString()));

            var x = data[_elemental];
            x.AddPerk(perk);
            data[_elemental] = x;

            SaveData.ElementalSoulSaveData.Manager.SaveElementalSoulData(data);
        }

        public bool IsPerkUnlocked(EnumElemental _elemental, SaveData.ElementalSoulSaveData.Perk perk)
        {
            if (!data.ContainsKey(_elemental))
                return false;

            return data[_elemental].Exists(perk);
        }

        public bool IsPerkUnlockable(EnumElemental _elemental, SaveData.ElementalSoulSaveData.Perk perk)
        {
            if (!data.ContainsKey(_elemental))
                return false;

            else if (data[_elemental].GetGainedSoul() <= 0)
                return false;

            return data[_elemental].PerkUnlockable(perk);
        }

        public void CorrectElementalSkill()
        {
            for (var _elemental = EnumElemental.Air; _elemental < EnumElemental.Count; ++_elemental)
            {
                Data.Skill.SkillCollection.GetInstance().ClearElementalSkill(_elemental);
                var preset = GetCurrentPreset(_elemental);
                foreach(var perk in preset.GetPerks())
                {
                    var currentElementalTree = ElementalTreeCollection.GetInstance().GetElementalTree(_elemental);
                    var skillElement = currentElementalTree.GetTier(perk.tier).elements[perk.order];

                    var currentSkillUpgradeCount = 
                        Data.Skill.SkillCollection.GetInstance()
                            .allSkillCollection[skillElement.GetSkill().GetSkillID()].GetCount();
                    
                    var targetCount = skillElement.GetUpgradeCount();

                    if(targetCount > currentSkillUpgradeCount)
                    {
                        Data.Skill.SkillCollection.GetInstance().SetSkillUpgradeCount(
                            skillElement.GetSkill().GetSkillID(),
                            targetCount
                        );
                    }
                }
            }
            
        }

        public void SetPreset(EnumElemental _elemental, string presetName)
        {
            if (!data.ContainsKey(_elemental))
                data.Add(_elemental, new SaveData.ElementalSoulSaveData.Element(_elemental.ToString()));

            var x = data[_elemental];
            x.SetPreset(presetName);
            data[_elemental] = x;

            foreach(var displayComponent in displayComponents)
            {
                if(displayComponent.GetType() == typeof(UI.ElementalPresetButton))
                {
                    var button = (UI.ElementalPresetButton) displayComponent;

                    button.Refresh(this);
                }
            }

            SaveData.ElementalSoulSaveData.Manager.SaveElementalSoulData(data);
        }

        public void NextPreset(EnumElemental _elemental)
        {
            if (!data.ContainsKey(_elemental))
                data.Add(_elemental, new SaveData.ElementalSoulSaveData.Element(_elemental.ToString()));

            var x = data[_elemental];
            x.NextPreset();
            data[_elemental] = x;

            foreach (var displayComponent in displayComponents)
            {
                if (displayComponent.GetType() == typeof(UI.ElementalPresetButton))
                {
                    var button = (UI.ElementalPresetButton)displayComponent;

                    button.Refresh(this);
                }
            }

            SaveData.ElementalSoulSaveData.Manager.SaveElementalSoulData(data);
        }

        public SaveData.ElementalSoulSaveData.Preset GetCurrentPreset(EnumElemental _elemental)
        {
            if (!data.ContainsKey(_elemental))
                data.Add(_elemental, new SaveData.ElementalSoulSaveData.Element(_elemental.ToString()));

            var _index = data[_elemental].GetCurrentPresetIndex();

            if (_index >= data[_elemental].GetPresets().Count) _index = 0;

            return data[_elemental].GetPresets()[_index];
        }

        public void ClearElementalSoulData(EnumElemental _elemental)
        {
            if (!data.ContainsKey(_elemental))
                return;

            var x = data[_elemental];
            x.Clear();
            data[_elemental] = x;

            SaveData.ElementalSoulSaveData.Manager.SaveElementalSoulData(data);
        }


        public Dictionary<EnumElemental, SaveData.ElementalSoulSaveData.Element> GetElementalSoulData()
        {
            return data;
        }

        public void AddDisplayComponent(UI.IDisplayComponent component)
        {
            displayComponents.Add(component);
        }

        public void RemoveDisplayComponent(UI.IDisplayComponent component)
        {
            displayComponents.Remove(component);
        }
    }
}