using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Numerics;

namespace Main.Action
{
    public class NewGame : MonoBehaviour
    {
        [SerializeField] string playSceneName;
        [SerializeField] string difficulty;
        public void Execute()
        {
            InGame.Data.BossKillData.GetInstance().ClearBossKillData();
            InGame.Data.SaveData.GameConditionSaveDataManager.ClearCondition();

            var _characterData = InGame.Data.SaveData.CharacterSaveDataManager.LoadCharacterData();
            
            BigInteger gold = 0;

            if(_characterData != null)
            {
                BigInteger.TryParse(_characterData.gold, out gold);
                InGame.Data.Character.CharacterClassData.AccumulateLevel(
                   (InGame.Data.EnumEntityClass)Enum.Parse(
                       typeof(InGame.Data.EnumEntityClass),
                       _characterData.characterClass
                   ),
                   gold / 100
               );
            }

            string bossKillDataPath = string.Format("{0}/{1}.enc", Application.persistentDataPath, "BossKillSaveData");
            string characterDataPath = string.Format("{0}/{1}.enc", Application.persistentDataPath, "CharacterSaveData");

            File.Delete(bossKillDataPath);
            File.Delete(characterDataPath);

            PlayerPrefs.SetInt("isContinue", 0);
            PlayerPrefs.SetString("difficulty", difficulty);

            int playCount = PlayerPrefs.GetInt("playCount", 0);
            PlayerPrefs.SetInt("playCount", playCount + 1);

            SceneManager.LoadSceneAsync(playSceneName);
        }
    }

}