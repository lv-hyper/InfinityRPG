using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.SaveData
{
    public class PlayerPrefsUtility
    {
        public static string gameDifficulty
        {
            get { return PlayerPrefs.GetString("difficulty", "normal"); }
        }
    }
}
