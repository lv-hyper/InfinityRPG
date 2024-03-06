using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using InGame.Data;
using InGame.Data.Character;
using System;
using System.Numerics;

namespace Main.UI.Statistics
{
    public class GeneralDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI warriorLevelText, mageLevelText, archerLevelText;

        [SerializeField] UnityEngine.UI.Image levelBar;
        [SerializeField] Sprite warriorLevelBar, mageLevelBar, archerLevelBar;

        [SerializeField] TextMeshProUGUI levelStatusText;

        [SerializeField] TextMeshProUGUI playDataText;

        [SerializeField] EnumEntityClass currentClass;

        [SerializeField] InGame.Data.SaveData.SoulExtractSaveData soulExtractSaveData;

        int itemCount = 0;
        int maxItemCount = 0;

        int itemCountWithStack = 0;
        int maxItemCountWithStack = 0;

        private void Awake()
        {
            InGame.Data.SaveData.InventorySaveDataManager.LoadInventoryData();

            var itemCollection = InGame.Data.Item.ItemCollection.GetInstance().allCollection;
            foreach(var item in itemCollection)
            {
                if (item.Value.getItem().GetType() == typeof(InGame.Data.Item.Material)) 
                    continue;

                itemCountWithStack += item.Value.getCount();
                maxItemCountWithStack += item.Value.getMaxCount();

                if (item.Value.getCount() > 0) ++itemCount;
                ++maxItemCount;
            }

            Refresh();
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            warriorLevelText.text = "Lv. " + CharacterClassData.GetCharacterClassLevel(EnumEntityClass.Warrior);
            mageLevelText.text = "Lv. " + CharacterClassData.GetCharacterClassLevel(EnumEntityClass.Mage);
            archerLevelText.text = "Lv. " + CharacterClassData.GetCharacterClassLevel(EnumEntityClass.Archer);

            int playCount = PlayerPrefs.GetInt("playCount", 0);
            
            long maxLevel = long.Parse(
                PlayerPrefs.GetString("MaxLevel", "0")
            );

            soulExtractSaveData = InGame.Data.SaveData.SoulExtractSaveDataManager.LoadSoulExtractData();
            int normalStartEnergy = 25 + soulExtractSaveData.GetTotalEnergyByDifficulty("normal");
            int normalMaxStartEnergy = 25 + soulExtractSaveData.GetMaxTotalEnergy("normal");
            int hardStartEnergy = 25 + soulExtractSaveData.GetTotalEnergyByDifficulty("hard");
            int hardMaxStartEnergy = 25 + soulExtractSaveData.GetMaxTotalEnergy("hard");

            playDataText.text = String.Format(
                "Play Count : {0:N0}\n" +
                "Maximium Level :  {1:N0}\n\n" +
                "Item Count : {2:N0} / {3:N0}\n" +
                "Item Count (With Stack) : {4:N0} / {5:N0}\n\n" +
                "Normal mode start energy : {6:N0}\n" +
                "Hard mode start energy : {7:N0}", 
                playCount, maxLevel, itemCount, maxItemCount, itemCountWithStack, maxItemCountWithStack, normalStartEnergy, /*normalMaxStartEnergy,*/ hardStartEnergy/*, hardMaxStartEnergy*/);


            SelectClass(currentClass.ToString());
        }

        public void SelectClass(string chClass)
        {
            EnumEntityClass characterClass = (EnumEntityClass)Enum.Parse(
                typeof(EnumEntityClass),
                chClass
            );

            BigInteger charLv = CharacterClassData.GetCharacterClassLevel(characterClass);

            BigInteger currentLv = CharacterClassData.GetAccumulatedCharacterLevel(characterClass);
            BigInteger maxLv = CharacterClassData.LevelUpRequirement(charLv);

            levelStatusText.text =
                string.Format("{0} Lv. {1} ({2:N0} / {3:N0})", chClass.ToString(), charLv, currentLv, maxLv);

            switch(characterClass)
            {
                case EnumEntityClass.Warrior:
                    levelBar.sprite = warriorLevelBar;
                    break;
                case EnumEntityClass.Mage:
                    levelBar.sprite = mageLevelBar;
                    break;
                case EnumEntityClass.Archer:
                    levelBar.sprite = archerLevelBar;
                    break;
                default:
                    levelBar.sprite = warriorLevelBar;
                    break;
            }

            levelBar.fillAmount = (float)((double)currentLv / (double)maxLv);
        }
    }
}
