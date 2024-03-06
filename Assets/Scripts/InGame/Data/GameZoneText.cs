using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using InGame.UI;
using System;
using Unity.VisualScripting;

namespace InGame.Data
{
    public class GameZoneText : MonoBehaviour, IDisplayComponent
    {        
        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(GameZone))
            {
                GameZone gameZone = subject as GameZone;                
                GetComponent<TMPro.TextMeshPro>().text = string.Format("Base Lv. {0:N0}", gameZone.GetMaxLv());
            }
        }
    }
}