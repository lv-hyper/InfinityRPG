using UnityEngine;
using System;
using System.Collections.Generic;

namespace InGame.Data
{
    public class BossKillData
    {
        public static BossKillData instance;
        Dictionary<string, bool> isKilled;

        public static BossKillData GetInstance()
        {
            if(instance == null)
                instance = new BossKillData();

            return instance;
        }
        
        private BossKillData()
        {
            int isContinue = PlayerPrefs.GetInt("isContinue", 0);
            if(isContinue != 0)
            {
                isKilled = Data.SaveData.BossKillSaveDataManager.LoadBossKillData();
            }
            else
            {
                isKilled = null;
            }

            if(isKilled == null)
                isKilled = new Dictionary<string, bool>();
        }

        public void SetBossKilled(string bossID)
        {
            isKilled[bossID] = true;

            SaveData.BossKillSaveDataManager.SaveBossKillData(isKilled);
        }

        public bool IsBossKilled(string bossID)
        {
            if(isKilled.ContainsKey(bossID))
            {
                return isKilled[bossID];
            }
            else
            {
                return false;
            }
        }

        public void ClearBossKillData(){
            isKilled.Clear();

            //SaveData.BossKillSaveDataManager.SaveBossKillData(isKilled);
        }

        public Dictionary<string, bool> GetBossKillData()
        {
            return isKilled;
        }
    }
}