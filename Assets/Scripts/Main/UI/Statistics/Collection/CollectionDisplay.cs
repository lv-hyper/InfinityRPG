using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.UI;
using InGame.Data.Mob;
using InGame.Data.SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Main.UI.Statistics.Collection
{
    public class CollectionDisplay : MonoBehaviour, Backdrop.ILockable
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
        [FormerlySerializedAs("page")] [SerializeField] int currentPage = 1;
        [SerializeField] private List<InGame.Data.Mob.AbstractMob> rawMobs, filteredMobs;
        [SerializeField] SoulExtractSaveData soulExtractSaveData;
        [SerializeField] private Backdrop backdrop;
        private HashSet<int> cachedEntryIndices;
        private int requiredSpritesToLoad = 0, loadedSpriteCount = 0;
        private bool isInitialized = false, assetLoadComplete = false;
        private ICollectionSortingStrategy sortingStrategy = new OrderByDefault();

        public string LockId => "CollectionDisplay";
        public string LockMessage => "Loading Images ...";
        public float Progress => 0.5f;
        
        private void LoadSoulExtractData()
        {
            soulExtractSaveData = SoulExtractSaveDataManager.LoadSoulExtractData();
        }

        private void Awake()
        {
            if (!isInitialized)
            {
                Initialize();
                isInitialized = true;
            }
        }

        private void Initialize()
        {
            rawMobs = new List<InGame.Data.Mob.AbstractMob>();
            cachedEntryIndices = new HashSet<int>();
            backdrop.AddLock(this);

            int count = 0;
            Addressables.LoadAssetsAsync<AbstractMob>("mob", mob =>
            {
                Debug.Log($"{++count} mobs loaded. ({mob.GetName()})");
            }).Completed += OnAssetLoadComplete;
        }

        private void OnAssetLoadComplete(AsyncOperationHandle<IList<AbstractMob>> asyncOperation)
        {
            SearchCriteria searchCriteria = new SearchCriteria();
            
            rawMobs = asyncOperation.Result
                .Where(mob => searchCriteria.IsValid(mob))
                .ToList();

            searchCriteria = new SearchCriteria().Difficulty(difficulty).SoulLevel(soulLevel);
            filteredMobs = sortingStrategy.Sort(searchCriteria.Filter(rawMobs)).ToList();
            cachedEntryIndices.Clear();
            
            string currentDifficulty = PlayerPrefs.GetString("CollectionDiff", "normal");
            PlayerPrefs.SetString("CollectionDiff", currentDifficulty);
            
            //Change difficulty. If void mode updated, this mechanism should be changed;
            //Currently alternating between normal and hard
            if (currentDifficulty != difficulty) 
                difficulty = PlayerPrefs.GetString("CollectionDiff");
            
            if (!PlayerPrefs.HasKey("CollectionPage")) PlayerPrefs.SetInt("CollectionPage", 1);

            GetMaxSoulLevel();
            LoadSoulExtractData();

            var diffValue = difficulty.ToCharArray();

            diffValue[0] = char.ToUpper(diffValue[0]);
            diffText.text = diffValue.ArrayToString();

            currentPage = PlayerPrefs.GetInt("CollectionPage"); //Loading last page viewed
            assetLoadComplete = true;
            backdrop.RemoveLock(this);
            Refresh();
        }

        public void IncrementPage()
        {
            currentPage++;
            if (currentPage >= GetMaxPageCount())
                currentPage = GetMaxPageCount();
            Refresh();
        }

        public void IncrementSkipPage()
        {
            currentPage += 10;
            if (currentPage >= GetMaxPageCount())
                currentPage = GetMaxPageCount();
            Refresh();
        }

        public void DecrementPage()
        {
            currentPage--;
            if (currentPage <= 0)
                currentPage = 1;
            Refresh();
        }

        public void DecrementSkipPage()
        {
            currentPage -= 10;
            if (currentPage <= 0)
                currentPage = 1;
            Refresh();
        }

        public int GetMaxPageCount()
        {
            return Mathf.Max(1, Mathf.CeilToInt((float)filteredMobs.Count / maxColumnCount));
        }

        public int SearchCurrentLevelPage(BigInteger level)
        {
            int i, j;
            print(filteredMobs.Count);
            for(i = 0, j = 1; i + maxColumnCount < filteredMobs.Count; i+= maxColumnCount, j++)
            {
                var mob = filteredMobs[i + maxColumnCount];
                print(i.ToString() + "page: " + mob.GetLV().ToString());
                if (mob.GetLV() >= level) break;
            }
            return j; 
            //Because pages are not many at this moment, used simple [O(n^2)] search.
            //Maybe [O(nlog(n))] algorithm later?
        }

        public void WarpToCurrentLevelPage() //Maybe change function into showing ONLY zone monsters? But later...
        {
            while (true)
            {
                character = InGame.Entity.Character.GetInstance();
                if (character != null) break;
            }
            
            //TODO : If void mode updated, this mechanism should be changed;
            //Currently alternating between normal and hard
            if (difficulty != PlayerPrefs.GetString("difficulty"))
                ToggleDifficulty(); 
            var gameZone = character.GetCurrentGameZone();

            currentPage = SearchCurrentLevelPage(gameZone.GetMaxLv());

            Refresh();
        }

        private void ClearCurrentUI()
        {
            foreach(Transform obj in contentPanel)
            {
                Destroy(obj.gameObject);
            }
        }

        private void LoadSpritesOnRange()
        {
            int minEntry = (currentPage - 1) * maxColumnCount;
            int maxEntry = Math.Min(filteredMobs.Count, currentPage * maxColumnCount) - 1;
            
            int newMinCachedEntry = Math.Max(0, minEntry - maxColumnCount);
            int newMaxCachedEntry = Math.Min(filteredMobs.Count - 1, maxEntry + maxColumnCount);

            loadedSpriteCount = 0;
            requiredSpritesToLoad = maxEntry - minEntry + 1;
            
            foreach(var index in cachedEntryIndices.ToList())
            {
                if (index < newMinCachedEntry || index > newMaxCachedEntry)
                {
                    if (filteredMobs[index].GetSprite() != null)
                    {
                        filteredMobs[index].UnloadSprite();
                        cachedEntryIndices.Remove(index);
                    }
                }
            }

            for (int i = newMinCachedEntry; i <= newMaxCachedEntry; ++i)
            {
                if (!cachedEntryIndices.Contains(i) && filteredMobs[i].GetSprite() == null)
                {
                    var handle = filteredMobs[i].GetSpriteAsync(Refresh);
                    cachedEntryIndices.Add(i);
                }
                
                else if (minEntry <= i && i <= maxEntry && filteredMobs[i].GetSprite() != null)
                {
                    ++loadedSpriteCount;
                }
            }
        }

        private void UpdateUI()
        {
            if (loadedSpriteCount >= requiredSpritesToLoad)
            {
                ClearCurrentUI();
                // Disable Backdrop
                if (backdrop.ContainsLock(this))
                {
                    backdrop.RemoveLock(this);
                }
                
                int minEntry = (currentPage - 1) * maxColumnCount;
                int maxEntry = Math.Min(filteredMobs.Count, currentPage * maxColumnCount) - 1;
                
                for (int i = minEntry, j = 0; i <= maxEntry; ++i, ++j)
                {
                    var mob = filteredMobs[i];

                    GameObject g = Instantiate(enemyDataBar, contentPanel, false);

                    String searchName = mob.GetName();
                    searchName.Replace("[", "_");

                    //Debug.Log(searchName.ToLower().Replace(" ", string.Empty));
                    if(soulExtractSaveData.contains_ignoreCapitalAndBlank(
                           searchName.Replace("[", "_").Replace("]", string.Empty)))
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

                pageIndicator.text = string.Format("{0} / {1}", currentPage, GetMaxPageCount());
                PlayerPrefs.SetInt("CollectionPage", currentPage);//Saving last page viewed
            }
            else
            {
                // Enable Backdrop
                if (backdrop.ContainsLock(this))
                {
                    backdrop.UpdateLock(this);
                }
                else
                {
                    backdrop.AddLock(this);
                }
            }
        }

        public void Refresh()
        {
            if (!assetLoadComplete)
                return;
            
            LoadSpritesOnRange();
            UpdateUI();
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

            currentPage = 1;

            SearchCriteria searchCriteria = new SearchCriteria()
                .Difficulty(difficulty)
                .SoulLevel(soulLevel);
            
            filteredMobs = sortingStrategy.Sort(searchCriteria.Filter(rawMobs)).ToList();
            cachedEntryIndices.Clear();

            Refresh();
        }

        public int GetMaxSoulLevel()
        {
            if (maxSoulLevel != -1) 
                return maxSoulLevel;

            return rawMobs.Select(mob => mob.GetSoulLevel()).Max();
        }

        public void ToggleSoulLevel()
        {
            soulLevel++;
            if(soulLevel > GetMaxSoulLevel()) soulLevel = 0;

            if(soulLevel == 0) soulLevelIndicator.text = "All";
            else soulLevelIndicator.text = string.Format("Extract Lv.{0}", soulLevel);
           
            currentPage = 1;

            SearchCriteria searchCriteria = new SearchCriteria()
                .Difficulty(difficulty)
                .SoulLevel(soulLevel);
            
            filteredMobs = sortingStrategy.Sort(searchCriteria.Filter(rawMobs)).ToList();
            cachedEntryIndices.Clear();

            Refresh();
        }
        

        private void OnDestroy()
        {
            foreach (var mob in filteredMobs)
            {
                if (mob.GetSprite() != null)
                {
                    mob.UnloadSprite();
                }
            }
        }
    }
}
