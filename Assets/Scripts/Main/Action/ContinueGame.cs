using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.Action
{
    public class ContinueGame : MonoBehaviour
    {
        [SerializeField] string playSceneName;
        [SerializeField] Button continueButton;
        private void Awake() {
            int isContinue = PlayerPrefs.GetInt("isContinue", 0);

            if(isContinue == 1)
            {
                continueButton.interactable = true;
            }
            else
            {
                continueButton.interactable = false;
            }
        }
        public void Execute()
        {
            SceneManager.LoadSceneAsync(playSceneName);
        }
    }

}