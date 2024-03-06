using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace InGame.UI
{
    public class GameSpeedDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        private void OnEnable()
        {
            RefreshDisplay();
        }

        public void RefreshDisplay()
        {
            text.text = string.Format(
                "Battle Speed : {0}x",
                Entity.Character.GetInstance().GetGameSpeed().ToString()
            );
        }
    }
}
