using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using InGame.UI;
using System;
using Unity.VisualScripting;
using System.Numerics;

namespace InGame.Data
{
    [Serializable]
    public struct SpecialMobData
    {
        public Data.Mob.AbstractMob mob;
        public string destMap;
        public string teleportDest;
    }

    public class GameZone : MonoBehaviour, IDisplayComponent, Subject
    {
        [SerializeField] long minLevel = 1, maxLevel = 1;
        [SerializeField] List<Data.Mob.AbstractMob> mobList;
        [SerializeField] float staminaRate = 1.0f;
        [SerializeField] int energyRate = 1;
        [SerializeField] List<SpecialMobData> specialMobList;
        [SerializeField] float specialMobPercentage = 0.0f;
        [SerializeField] TileBase matchingTile;
        [SerializeField] UnityEngine.Vector2 preferredSize;

        [SerializeField] GridLayout gridLayout;

        [Tooltip("If this is enabled, zone text will automatically generate using zoneTextPrefab.")]
        [SerializeField] bool autoGenerateZoneText = true;

        [Tooltip("Prefab to use on Zone text auto generation")]
        [SerializeField] GameObject zoneTextPrefab;

        //TODO : refactor required - should have to manage on singleton scriptableobject
        [Tooltip("Zone color each status")]
        [SerializeField] Color[] zoneColor;


        bool isSpecial = false;
        string destMap = null, teleportDest = null;

        long rate
        {
            get{
                string difficulty = PlayerPrefs.GetString("difficulty", "normal");

                int rate = 1;

                switch(difficulty)
                {
                    case "normal":
                        rate = 1;
                        break;
                    case "hard":
                        rate = 100;
                        break;
                    case "void":
                        rate = 100000;
                        break;
                }

                return rate;
            }
        }

        long realMinLevel { get { return minLevel; } }
        long realMaxLevel { get { return maxLevel; } }

        public List<IDisplayComponent> displayComponents {get; set;}

        private void Awake()
        {
            StartCoroutine(RegisterToCharacter());
            displayComponents = new List<IDisplayComponent>();

            InitGameZone();
        }

        IEnumerator RegisterToCharacter()
        {
            Entity.Character character = null;

            while (true)
            {
                character = Entity.Character.GetInstance();
                if (character != null) break;
                yield return null;
            }

            character.AddDisplayComponent(this);

        }

        public void Refresh(Subject subject)
        {
            Entity.Character character;
            if (subject.GetType() == typeof(Entity.Character))
            {
                character = (Entity.Character)subject;
                SetZoneColor(character.GetLevel());
            }
        }

        public void SetZoneColor(BigInteger characterLevel)
        {
            double levelRatio = (double)(characterLevel + 25) / (double)(realMaxLevel + 25);
            Tilemap tilemap = GetComponent<Tilemap>();

            if (levelRatio < 0.4)
            {
                tilemap.color = zoneColor[0];
            }
            else if (levelRatio < 0.6)
            {
                tilemap.color = zoneColor[1];
            }
            else if (levelRatio < 0.8)
            {
                tilemap.color = zoneColor[2];
            }
            else if (levelRatio < 1.25)
            {
                tilemap.color = zoneColor[3];
            }
            else if (levelRatio < 1.6)
            {
                tilemap.color = zoneColor[4];
            }
            else if (levelRatio < 2.5)
            {
                tilemap.color = zoneColor[5];
            }
            else
            {
                tilemap.color = zoneColor[6];
            }

        }

        bool compareTileArea(UnityEngine.Vector2 sizeA, UnityEngine.Vector2 sizeB, UnityEngine.Vector2 preferredSize)
        {
            return sizeA.x * sizeA.y > sizeB.x * sizeB.y;
        }

        void GetTileArea(int sizeX, int sizeY, bool[,] tiles, out UnityEngine.Vector2 pos, out UnityEngine.Vector2 size)
        {
            int[,] tileChart = new int[sizeX, sizeY];
            for (int i = 0; i < sizeX; ++i)
            {
                tileChart[i, 0] = tiles[i, 0] ? 1 : 0;
            }
            for (int i = 0; i < sizeX; ++i)
            {
                for (int j = 1; j < sizeY; ++j)
                {
                    if (tiles[i, j]) tileChart[i, j] = tileChart[i, j - 1] + 1;
                    else tileChart[i, j] = 0;
                }
            }

            for (int i = 0; i < sizeY; ++i)
            {
                for (int j = 1; j < sizeX; ++j)
                {
                    //Debug.Log(j+","+i+" "+tileChart[j, i]);
                }
            }

            UnityEngine.Vector2 maxPos = UnityEngine.Vector2.zero;
            UnityEngine.Vector2 maxSize = UnityEngine.Vector2.zero;
            for (int y = 0; y < sizeY; ++y)
            {
                Stack<int> histogram = new Stack<int>();
                int maxX = -1;
                UnityEngine.Vector2 maxXSize = UnityEngine.Vector2.zero;
                for (int x = 0; x < sizeX; ++x)
                {
                    if (histogram.Count == 0 || tileChart[histogram.Peek(), y] <= tileChart[x, y])
                    {
                        histogram.Push(x);
                        continue;
                    }
                    while (histogram.Count > 0 && tileChart[histogram.Peek(), y] > tileChart[x, y])
                    {
                        UnityEngine.Vector2 result;
                        int index = histogram.Pop();
                        int currentXPos;
                        if (histogram.Count > 0)
                        {
                            result = new UnityEngine.Vector2(x - histogram.Peek() - 1, tileChart[index, y]);
                            currentXPos = histogram.Peek();
                        }
                        else
                        {
                            result = new UnityEngine.Vector2(x - 1, tileChart[index, y]);
                            currentXPos = 0;
                        }


                        if (compareTileArea(result, maxXSize, preferredSize))
                        // if(result.x * result.y > maxXSize.x * maxXSize.y)
                        {
                            maxXSize = result;
                            maxX = currentXPos;
                        }
                    }
                }
                while (histogram.Count > 0)
                {
                    int index = histogram.Pop();
                    int currentXPos;

                    UnityEngine.Vector2 result;
                    if (histogram.Count > 0)
                    {
                        result = new UnityEngine.Vector2(sizeX - histogram.Peek() - 1, tileChart[index, y]);
                        currentXPos = histogram.Peek();
                    }
                    else
                    {
                        result = new UnityEngine.Vector2(sizeX - 1, tileChart[index, y]);
                        currentXPos = 0;
                    }


                    if (compareTileArea(result, maxSize, preferredSize))
                    // if(result.x * result.y > maxXSize.x * maxXSize.y)
                    {
                        maxXSize = result;
                        maxX = currentXPos;
                    }
                }

                if (compareTileArea(maxXSize, maxSize, preferredSize))
                //if(maxSize.x * maxSize.y < maxXSize.x * maxXSize.y)
                {
                    maxSize = maxXSize;
                    maxPos = new UnityEngine.Vector2(maxX, y);
                }
            }
            pos = maxPos;
            size = maxSize;
        }

        UnityEngine.Vector3 CustomCellToWorld(UnityEngine.Vector2 cell)
        {
            UnityEngine.Vector3 pos;
            pos.x = cell.x * gridLayout.cellSize.x;
            pos.y = cell.y * gridLayout.cellSize.y;
            pos.z = 0;
            return pos;
        }

        void InitGameZone()
        {
            if(autoGenerateZoneText)
            {
                GenerateOptimalZoneText();
                UpdateGameZone();
            }
            else
            {
                RegisterExistingZoneText();
                UpdateGameZone();
            }
        }

        void GenerateOptimalZoneText()
        {
            Tilemap tilemap = GetComponent<Tilemap>();

            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

            UnityEngine.Vector2 bestArea = UnityEngine.Vector2.zero;

            bool[,] isTile = new bool[bounds.size.x, bounds.size.y]; // check whether tile on (x, y) is matchingTile

            for (int i = 0; i < bounds.size.x; ++i)
            {
                for (int j = 0; j < bounds.size.y; ++j)
                {
                    isTile[i, j] = (allTiles[j * bounds.size.x + i] == matchingTile);
                }
            }

            UnityEngine.Vector2 areaPos, areaSize;

            GetTileArea(bounds.size.x, bounds.size.y, isTile, out areaPos, out areaSize); // Get optimal position and size to generate zone text


            UnityEngine.Vector3 worldPos = CustomCellToWorld(
                new UnityEngine.Vector2(
                    tilemap.cellBounds.xMin + areaPos.x + (areaSize.x / 2) + 1,
                    tilemap.cellBounds.yMin + areaPos.y - (areaSize.y / 2) + 1
                )
            ); // convert tile position to world position

            if(zoneTextPrefab != null) // when zoneTextPrefab is not null
            {
                GameObject zoneText = Instantiate(zoneTextPrefab, worldPos, UnityEngine.Quaternion.identity, transform);
                AddDisplayComponent(zoneText.GetComponent<GameZoneText>());
            }
        }
        
        void RegisterExistingZoneText()
        {
            var gameZoneTexts = transform.GetComponentsInChildren<GameZoneText>();
            foreach(var gameZoneText in gameZoneTexts)
            {
                AddDisplayComponent(gameZoneText);
            }
        }

        public Data.Mob.AbstractMob GetNewMob()
        {
            Data.Mob.AbstractMob mobData = null;

            if (RollSpecial())
            {
                var specialMobIndex = UnityEngine.Random.Range(0, specialMobList.Count);

                mobData = specialMobList[specialMobIndex].mob;
                teleportDest = specialMobList[specialMobIndex].teleportDest;
                destMap = specialMobList[specialMobIndex].destMap;
            }

            else
            {
                mobData = mobList[UnityEngine.Random.Range(0, mobList.Count)];
                teleportDest = null;
                destMap = null;
            }

            return mobData;
        }

        private bool RollSpecial()
        {
            isSpecial = UnityEngine.Random.Range(1, 1000000) / 1000000.0f < specialMobPercentage;
            return isSpecial;
        }

        public bool IsSpecial() { return isSpecial; }

        public void SetMinLv(long lv)
        {
            minLevel = lv;
        }

        public void SetMaxLv(long lv)
        {
            maxLevel = lv;
            UpdateGameZone();
            Entity.Character.GetInstance().ZoneUpdate();
        }

        public long GetMinLv()
        {
            return minLevel;
        }

        public long GetMaxLv()
        {
            return maxLevel;
        }

        public long GetBaseLv()
        {
            return realMaxLevel;
        }

        public string GetDestMap()
        {
            return destMap;
        }

        public string GetTeleportDest()
        {
            return teleportDest;
        }


        public long getRandomLv()
        {
            var diff = maxLevel - minLevel;
            var result = minLevel + (long)Math.Round(UnityEngine.Random.value * (double)diff);

            return result;
            //return Utility.RandomUtility.RandomRangeLong(minLevel, maxLevel);
        }

        private void OnDestroy() {
            if(Entity.Character.GetInstance() != null)
                Entity.Character.GetInstance().RemoveDisplayComponent(this);
        }

        public float GetStaminaRate()
        {
            return staminaRate * (Mathf.Log(rate, 7) + 1);
        }

        public int GetEnergyRate() { return (int)Mathf.Floor(energyRate * (Mathf.Log(rate, 7) + 1) ); }

        public List<Data.Mob.AbstractMob> GetMobList() { return mobList; }

        public void PromoteMob(List<Data.Mob.AbstractMob> mobs)
        {
            mobList = mobs;
        }

        public void AddDisplayComponent(IDisplayComponent component)
        {
            displayComponents.Add(component);
        }

        public void UpdateGameZone()
        {
            foreach (var component in displayComponents)
            {
                if(component != null)
                    component.Refresh(this);
            }
        }
    }
}
