using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InGame.Action
{
    public class GoBackToMain : MonoBehaviour
    {
        [SerializeField] string mainSceneName;
        [SerializeField] TMPro.TextMeshProUGUI descText;
        private void OnEnable()
        {
            var _character = Entity.Character.GetInstance();

            if(descText != null)
            {
                descText.text =
                    string.Format("You have {0:N0}G left.\n\n", _character.GetGold()) +
                    "gold remaining will be converted into following :\n\n" +
                    string.Format("{0} Class EXP +{1:N0}\n\n", _character.GetCurrentClass().ToString(), _character.GetGold() / 100) +
                    "Are you sure?";
            }
        }


        public void Execute()
        {
            SetPersistence.DestroyAll();
            SceneManager.LoadSceneAsync(mainSceneName);
        }

        public void ConvertGold()
        {
            var _character = Entity.Character.GetInstance();

            Data.Character.CharacterClassData.AccumulateLevel(
                _character.GetCurrentClass(), _character.GetGold() / 100
            );
        }

        public void AfterGameOver()
        {
            ConvertGold();

            File.Delete(string.Format("{0}/{1}.json",Application.persistentDataPath,"BossKillSaveData"));
            File.Delete(string.Format("{0}/{1}.json",Application.persistentDataPath,"CharacterSaveData"));

            PlayerPrefs.DeleteKey("isContinue");

            Data.SaveData.GameConditionSaveDataManager.ClearCondition();
            
            Execute();
        }
    }
}
