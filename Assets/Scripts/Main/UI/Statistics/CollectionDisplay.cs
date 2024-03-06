using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Gpm.LogViewer.Internal;
using InGame.Data.BattleInstance;
using System.Xml;
using System.Numerics;
using System.Collections.ObjectModel;
using System.Data.Common;
using UnityEngine.UI;
using InGame.Entity;
using InGame.Data.SaveData;
using System;

namespace Main.UI.Statistics
{
    public class CollectionDisplay : MonoBehaviour
    {
        [SerializeField] GameObject enemyDataBar;
        [SerializeField] Transform contentPanel;
        [SerializeField] string difficulty;
        [SerializeField] TextMeshProUGUI diffText;
        [SerializeField] TextMeshProUGUI pageIndicator;
        [SerializeField] TextMeshProUGUI soulLevelIndicator;
        [SerializeField] int maxColumnCount = 10;
        [SerializeField] int soulLevel = 0;
        [SerializeField] int maxSoulLevel = -1;

        [SerializeField] InGame.Entity.Character character;
        [SerializeField] int page = 1;
        [SerializeField] List<InGame.Data.Mob.AbstractMob> rawMobs, cachedMobs;
        [SerializeField] SoulExtractSaveData soulExtractSaveData;


        private void LoadSoulExtractData()
        {
            soulExtractSaveData = SoulExtractSaveDataManager.LoadSoulExtractData();
        }

        private void OnEnable()
        {
            rawMobs = InGame.Data.Mob.MobCollection.GetInstance().GetList();

            if (!PlayerPrefs.HasKey("CollectionDiff")) PlayerPrefs.SetString("CollectionDiff", "normal");//Set default difficulty to normal. It is required becuase of very-first loading
            if (PlayerPrefs.GetString("CollectionDiff") != difficulty) difficulty = PlayerPrefs.GetString("CollectionDiff"); //change difficulty. If void mode updated, this mechanism should be changed; Currently alternating between normal and hard
            if (!PlayerPrefs.HasKey("CollectionPage")) PlayerPrefs.SetInt("collectionPage", 1);

            GetMaxSoulLevel();
            LoadSoulExtractData();

            var diffValue = difficulty.ToCharArray();

            diffValue[0] = char.ToUpper(diffValue[0]);
            diffText.text = diffValue.ArrayToString();

            cachedMobs = rawMobs.Where(x => MobSearchCondition(x) == true).ToList();

            page = PlayerPrefs.GetInt("collectionPage"); //Loading last page viewed
            Refresh();
        }

        public void IncrementPage()
        {
            page++;
            if (page >= GetMaxPageCount())
                page = GetMaxPageCount();
            Refresh();
        }

        public void IncrementSkipPage()
        {
            page += 10;
            if (page >= GetMaxPageCount())
                page = GetMaxPageCount();
            Refresh();
        }

        public void DecrementPage()
        {
            page--;
            if (page <= 0)
                page = 1;
            Refresh();
        }

        public void DecrementSkipPage()
        {
            page -= 10;
            if (page <= 0)
                page = 1;
            Refresh();
        }

        public int GetMaxPageCount()
        {
            return Mathf.CeilToInt((float)cachedMobs.Count / maxColumnCount);
        }

        public bool MobSearchCondition(InGame.Data.Mob.AbstractMob mob)
        {
            if (mob.IsHidden()) return false;
            if (mob.GetDropTable().Count == 0 && !mob.IsBoss()) return false;
            if (mob.GetDifficulty() != difficulty) return false;
            if (soulLevel != 0)
            {
                Debug.Log(mob.GetSoulLevel());
                if (mob.GetSoulLevel() != soulLevel) return false;
            }
            return true;
        }

        public int SearchCurrentLevelPage(BigInteger level)
        {
            int i, j;
            print(cachedMobs.Count);
            for(i = 0, j = 1; i + maxColumnCount < cachedMobs.Count; i+= maxColumnCount, j++)
            {
                var mob = cachedMobs[i + maxColumnCount];
                print(i.ToString() + "page: " + mob.GetLV().ToString());
                if (mob.GetLV() >= level) break;
            }
            return j; //Because pages are not many at this moment, used simple [O(n^2)] search. Maybe [O(nlog(n))] algorithm later?
        }

        public void WarpToCurrentLevelPage() //Maybe change function into showing ONLY zone monsters? But later...
        {
            while (true)
            {
                character = InGame.Entity.Character.GetInstance();
                if (character != null) break;
            }
            if (difficulty != PlayerPrefs.GetString("difficulty"))
                ToggleDifficulty(); //If void mode updated, this mechanism should be changed; Currently alternating between normal and hard
            var gameZone = character.GetCurrentGameZone();

            page = SearchCurrentLevelPage(gameZone.GetMaxLv());

            Refresh();
        }

        public void Refresh()
        {
            foreach(Transform obj in contentPanel)
            {
                Destroy(obj.gameObject);
            }

            for (
                int i = (page - 1) * maxColumnCount, j = 0; 
                i < page * maxColumnCount && i < cachedMobs.Count; 
                ++i, ++j
            )
            {
                var mob = cachedMobs[i];

                GameObject g = Instantiate(enemyDataBar, contentPanel, false);

                String searchName = mob.GetName();
                searchName.Replace("[", "_");

                //Debug.Log(searchName.ToLower().Replace(" ", string.Empty));
                if(soulExtractSaveData.contains_ignoreCapitalAndBlank(searchName.Replace("[", "_").Replace("]", string.Empty)))
                {
                    g.GetComponent<Outline>().effectColor = new Color(1, 1, 0, 1);
                }

                var item = g.GetComponent<CollectionItem>();
                item.Init(mob, difficulty);

                var pos = g.GetComponent<RectTransform>().anchoredPosition;
                pos.y -= (j * 200);
                g.GetComponent<RectTransform>().anchoredPosition = pos;
            }


            var size = contentPanel.GetComponent<RectTransform>().sizeDelta;
            //size.y = i * 200 + 100;
            size.y = maxColumnCount * 200 + 100;
            contentPanel.GetComponent<RectTransform>().sizeDelta = size;

            pageIndicator.text = string.Format("{0} / {1}", page, GetMaxPageCount());
            PlayerPrefs.SetInt("collectionPage", page);//Saving last page viewed
        }

        public void ToggleDifficulty()
        {
            if (difficulty == "normal")
                difficulty = "hard";

            else
                difficulty = "normal";
            PlayerPrefs.SetString("CollectionDiff", difficulty);
            
            var diffValue = difficulty.ToCharArray();

            diffValue[0] = char.ToUpper(diffValue[0]);
            diffText.text = diffValue.ArrayToString();

            page = 1;

            cachedMobs = rawMobs.Where(x => MobSearchCondition(x) == true).ToList();

            Refresh();
        }

        public int GetMaxSoulLevel()
        {
            if (maxSoulLevel != -1) return maxSoulLevel;
            
            foreach (var mob in rawMobs)
            {
                if (mob.GetSoulLevel() > maxSoulLevel) maxSoulLevel = mob.GetSoulLevel();
            }
            return maxSoulLevel;
        }

        public void ToggleSoulLevel()
        {
            soulLevel++;
            if(soulLevel > GetMaxSoulLevel()) soulLevel = 0;

            if(soulLevel == 0) soulLevelIndicator.text = "All";
            else soulLevelIndicator.text = string.Format("Extract Lv.{0}", soulLevel);
           
            page = 1;

            cachedMobs = rawMobs.Where(x => MobSearchCondition(x) == true).ToList();

            Refresh();
        }
    }
}
