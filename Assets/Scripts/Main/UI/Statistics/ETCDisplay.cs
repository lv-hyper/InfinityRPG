using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

namespace Main.UI.Statistics
{
    public class ETCDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI gameVersionText, gameDataPathText;
        string gameDataPath;

        private void Awake()
        {
            gameDataPath = Application.persistentDataPath;
            Refresh();
        }


        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            gameVersionText.text = string.Format("Game Version : {0}", Application.version);
            gameDataPathText.text = string.Format("Game Saved Path (Not recommended to modify)\n{0}", gameDataPath);
        }
    }

}
