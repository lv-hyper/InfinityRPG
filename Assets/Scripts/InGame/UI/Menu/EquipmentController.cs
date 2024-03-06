using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using InGame.Data;
using InGame.Data.Item;

namespace InGame.UI.Menu
{

    public class EquipmentController : Common.UI.Menu, IDisplayComponent
    {
        [SerializeField] UnityEngine.UI.Image weaponImage, helmetImage, robeImage, gloveImage, greaveImage;
        [SerializeField] TextMeshProUGUI weaponText, helmetText, robeText, gloveText, greaveText;

        [SerializeField] List<UnityEngine.UI.Image> accImage;
        [SerializeField] List<TextMeshProUGUI> accText;

        [SerializeField] List<UnityEngine.UI.Image> skillImage;
        [SerializeField] List<TextMeshProUGUI> skillText;
        [SerializeField] List<TextMeshProUGUI> skillPriorityText;

        [SerializeField] Sprite noItemSprite, lockedItemSprite;
        [SerializeField] InventoryController inventoryController;
        [SerializeField] SlotUnlockController slotUnlockController;

        [SerializeField] List<long> baseAccessorySlotCost;
        [SerializeField] ShortSetItemInformationController setItemInformation;

        private void Awake()
        {
            base.Awake();
            StartCoroutine(RegisterToCharacter());
            setItemInformation.Refresh();
        }

        IEnumerator RegisterToCharacter()
        {
            Entity.Character character = null;

            while (true)
            {
                character = Entity.Character.GetInstance();
                if (character != null) break;
                yield return null;
            }

            character.AddDisplayComponent(this);
        }

        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(Entity.Character))
            {
                var character = (Entity.Character)subject;

                var equipmentSet = character.GetCurrentEquipmentSet();

                SetItemSlotData(equipmentSet, "weapon", weaponImage, weaponText);
                SetItemSlotData(equipmentSet, "helmet", helmetImage, helmetText);
                SetItemSlotData(equipmentSet, "robe", robeImage, robeText);
                SetItemSlotData(equipmentSet, "glove", gloveImage, gloveText);
                SetItemSlotData(equipmentSet, "greave", greaveImage, greaveText);

                for (int i = 0; i < 10; ++i)
                {
                    SetItemSlotData(equipmentSet, String.Format("accessory{0}", i + 1), accImage[i], accText[i]);
                }

                for (int i = 0; i < 6; ++i)
                {
                    SetSkillSlotData(equipmentSet, i, skillImage[i], skillText[i]);

                    if(equipmentSet.GetSkillPriority(i) == 0)
                    {
                        skillPriorityText[i].text = "Skill Off";
                    }
                    else
                    {
                        skillPriorityText[i].text = String.Format("Priority : {0}", equipmentSet.GetSkillPriority(i));
                    }
                }
            }
        }

        public void SetItemSlotData(
            EquipmentSet equipmentSet, string slot,
            UnityEngine.UI.Image itemImage, TextMeshProUGUI itemText
        )
        {

            var item = equipmentSet.GetItem(slot);
            var character = Entity.Character.GetInstance();

            var accIndex = -1;

            if (slot.StartsWith("accessory"))
                accIndex = int.Parse(slot.Replace("accessory", "")) - 1;

            if (slot.StartsWith("accessory") && character.IsAccLocked(accIndex))
            {
                itemImage.sprite = lockedItemSprite;
                itemText.text = String.Format("{0:N0}G", baseAccessorySlotCost[accIndex]);
            }

            else if (item != null)
            {
                itemImage.sprite = item.GetSprite();
                if (itemText != null)
                    itemText.text = item.name;
            }
            else
            {
                itemImage.sprite = noItemSprite;
                if (itemText != null)
                    itemText.text = "No Item";
            }
        }

        public void SetSkillSlotData(
            EquipmentSet equipmentSet, int slot,
            UnityEngine.UI.Image skillIcon, TextMeshProUGUI skillText
        )
        {
            var skill = equipmentSet.GetSkill(slot);

            if (skill != null)
            {
                skillIcon.sprite = skill.GetSprite();
                if (skillText != null)
                    skillText.text = skill.name;
            }
            else
            {
                skillIcon.sprite = noItemSprite;
                if (skillText != null)
                    skillText.text = "No Skill";
            }

        }

        public void AddSkillPriority(int slot)
        {
            Entity.Character.GetInstance().GetCurrentEquipmentSet().AddSkillPriority(slot);
            Entity.Character.GetInstance().SaveEquipmentData();
        }
        public void RemoveSkillPriority(int slot)
        {
            Entity.Character.GetInstance().GetCurrentEquipmentSet().RemoveSkillPriority(slot);
            Entity.Character.GetInstance().SaveEquipmentData();
        }

        public void OpenAccessorySlot(int slot)
        {
            bool isLocked;

            var character = Entity.Character.GetInstance();

            isLocked = character.IsAccLocked(slot);

            Debug.Log(isLocked + " " + slot);

            if (isLocked)
            {
                slotUnlockController.OpenWindow(character.GetGold(), baseAccessorySlotCost[slot], 
                    () =>
                    {
                        character.LoseGold(baseAccessorySlotCost[slot]);
                        character.UnlockAcc(slot);
                    }
                );
            }
            else
            {
                inventoryController.ActivateInventory("accessory" + (slot + 1));
            }
        }
    }
}

