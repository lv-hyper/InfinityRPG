using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

namespace Main.UI.Statistics
{
    public class SaveDataDisplay : MonoBehaviour
    {
        [SerializeField] UnityEngine.UI.Button resetAllBtn;
        [SerializeField] TextMeshProUGUI resetAllBtnText;
        string gameDataPath;
        int countToReset = 6;

        private void Awake()
        {
            resetAllBtn.onClick.AddListener(ResetAllData);
            Refresh();
        }


        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            countToReset = 6;
            resetAllBtnText.text = "Reset All Savedata";
        }

        public void ResetAllData()
        {
            countToReset--;
            resetAllBtnText.text = string.Format("({0} times left to RESET ALL)", countToReset);
            if(countToReset == 0)
            {
                PlayerPrefs.DeleteAll();
                if(System.IO.Directory.Exists(gameDataPath))
                    System.IO.Directory.Delete(gameDataPath, true);

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

}
