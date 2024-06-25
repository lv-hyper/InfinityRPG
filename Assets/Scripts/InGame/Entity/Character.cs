using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using InGame.Action;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using InGame.Data;
using InGame.Data.Item;
using InGame.Data.Item.Armor;
using InGame.Data.BattleInstance;
using InGame.UI;
using TMPro;
using InGame.Data.Skill;
using Unity.VisualScripting;

namespace InGame.Entity
{
    public class Character : MonoBehaviour, Subject
    {
        static Character instance = null;
        public List<IDisplayComponent> displayComponents { get; set; }
        [SerializeField] Action.FollowCharacter follower;
        [SerializeField] private List<CharacterFollower> characterFollowers;

        [SerializeField] LinkedList<GameZone> currentGameZone;
        [SerializeField] LinkedList<ItemThrowTrigger> currentItemTrigger;
        [SerializeField] TextMeshProUGUI zoneText, goldText;
        [SerializeField] Image zoneTextImage;
        [SerializeField] Color maxDefColor, highDefColor, midDefColor, lowDefColor, minDefColor;

        Data.Character.CharacterStat characterStat;

        [SerializeField] public List<ItemAbility> currentItemAbilities;
        [SerializeField] public List<SkillAbility> currentSkillAbilities;
        [SerializeField] public List<string> currentTagResists;
        [SerializeField] public Utility.SerializableDictionary<EnumElemental, long> elementalStatus;

        [SerializeField] Data.SaveData.SoulExtractSaveData soulExtractSaveData;
        [SerializeField] Data.SaveData.ElementalSoulDropSaveData elementalSoulDropSaveData;

        [SerializeField] float currentStamina;
        BigInteger energy = 25, winCount = 0, encounterCount = 0;
        BigInteger gold = 0;

        [SerializeField] int gameSpeed = 1;

        bool energyPending = false;

        [SerializeField] List<EquipmentSet> equipmentSets;
        [SerializeField] int currentEquipmentSetIndex = 0;
        [SerializeField] List<bool> accLocked;

        [SerializeField] Weapon warriorDefaultWeapon, mageDefaultWeapon, archerDefaultWeapon;
        [SerializeField] ActiveSkill warriorDefaultSkill, mageDefaultSkill, archerDefaultSkill;

        [SerializeField] SpriteRenderer characterImage;
        [SerializeField] Sprite leftImage, rightImage, upImage, downImage;

        [SerializeField] AnimatorOverrideController warriorAnimatorController, mageAnimatorController, archerAnimatorController;

        //[SerializeField] CharacterSpriteSet warriorSpriteSet, mageSpriteSet, archerSpriteSet;

        [SerializeField] GameObject hintButton;
        [SerializeField] ShortSetItemInformationController setItemInformation;
        [SerializeField] TextMeshProUGUI gameSpeedText;

        EnumEntityClass characterClass;

        UnityEngine.Vector2 inputPos;
        UnityEngine.Vector3 worldInputPos;

        UnityEngine.Vector3 targetDeltaPos = UnityEngine.Vector3.zero;

        bool uiTouched = false;

        bool movementBlocked = false;

        Signpost currentSignpost;

        public long collectionCount
        {
            get
            {
                var collection = ItemCollection.GetInstance().allCollection;
                int currentCount = 0;
                foreach (var item in collection)
                {
                    if (item.Value.getItem().GetType() == typeof(Data.Item.Material))
                        continue;
                    currentCount += item.Value.getCount();
                }
                return currentCount;
            }
        }

        string difficulty
        {
            get { return PlayerPrefs.GetString("difficulty", "normal"); }
        }


        public static Character GetInstance()
        {
            return instance;
        }

        public void AddDisplayComponent(IDisplayComponent component)
        {
            displayComponents.Add(component);
            CharacterStatUpdate();
        }
        
        
        public void AddCharacterFollower(CharacterFollower characterFollower)
        {
            characterFollowers.Add(characterFollower);
        }

        public void RemoveDisplayComponent(IDisplayComponent component)
        {
            displayComponents.Remove(component);
            CharacterStatUpdate();
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
                displayComponents = new List<IDisplayComponent>();
                currentItemAbilities = new List<ItemAbility>();
                currentSkillAbilities = new List<SkillAbility>();
                currentTagResists = new List<string>();
                soulExtractSaveData = new Data.SaveData.SoulExtractSaveData(null);
                elementalSoulDropSaveData = new Data.SaveData.ElementalSoulDropSaveData(null);
                currentGameZone = new LinkedList<GameZone>();
                currentItemTrigger = new LinkedList<ItemThrowTrigger>();

                characterStat = Data.Character.CharacterStat.GetInstance();
                characterStat.SetRawCharacterStat(new Data.Character.RawCharacterStat());

                characterClass = (EnumEntityClass)Enum.Parse(
                    typeof(EnumEntityClass),
                    PlayerPrefs.GetString("CharacterClassType", EnumEntityClass.Warrior.ToString())
                );

                LoadInventoryData();
                LoadSkillData();
                LoadSlotData();
                LoadEquipmentData();
                LoadElementalSoulDropData();
                LoadSoulExtractData();
                ElementalSoulData.GetInstance().CorrectElementalSkill();

                UI.Menu.AutoStatManager.GetInstance().LoadStatData();

                if (PlayerPrefs.HasKey("InGame/Character/currentEquipmentSetIndex"))
                {
                    currentEquipmentSetIndex = PlayerPrefs.GetInt("InGame/Character/currentEquipmentSetIndex");
                }

                ApplySkillAbilities();
                ApplyItemAbilities();

                int isContinue = PlayerPrefs.GetInt("isContinue", 0);

                gameSpeed = PlayerPrefs.GetInt("InGame/Character/Speed", 1);

                Data.BossKillData.GetInstance();

                Debug.Log(isContinue);

                if (isContinue != 0)
                {
                    var characterSaveData = Data.SaveData.CharacterSaveDataManager.LoadCharacterData();

                    if (characterSaveData != null)
                    {
                        characterStat.SetRawCharacterStat(new Data.Character.RawCharacterStat(
                            BigInteger.Parse(characterSaveData.level),
                            BigInteger.Parse(characterSaveData.statPoint),
                            BigInteger.Parse(characterSaveData.statStr),
                            BigInteger.Parse(characterSaveData.statInt),
                            BigInteger.Parse(characterSaveData.statDex),
                            BigInteger.Parse(characterSaveData.statVit),
                            BigInteger.Parse(characterSaveData.statEnd)
                        ));

                        currentStamina = characterSaveData.currentStamina;

                        energy = BigInteger.Parse(characterSaveData.energy);
                        winCount = BigInteger.Parse(characterSaveData.winCount);
                        encounterCount = BigInteger.Parse(characterSaveData.encounterCount);

                        gold = BigInteger.Parse(characterSaveData.gold);

                        characterStat.GetRawCharacterStat().SetCurrentEXP(BigInteger.Parse(characterSaveData.expToString));

                        characterClass = (EnumEntityClass)Enum.Parse(
                            typeof(EnumEntityClass),
                            characterSaveData.characterClass
                        );

                        GetComponent<Rigidbody2D>().position = new UnityEngine.Vector3(
                            characterSaveData.posX,
                            characterSaveData.posY,
                            characterSaveData.posZ
                        );

                        if (characterSaveData.mapName != SceneManager.GetActiveScene().name)
                        {
                            string triggerID = "";
                            if (PlayerPrefs.HasKey("SceneMovementTriggerFallback"))
                                triggerID = PlayerPrefs.GetString("SceneMovementTriggerFallback");
                            Teleport(characterSaveData.mapName, triggerID);
                        }
                    }

                    else
                    {
                        currentStamina = (float)Data.Character.CharacterStat.GetAbilityAmount("Stamina");
                        if(difficulty == "normal")
                            Teleport("Blossom_Valley_000", "test000");
                        else
                            Teleport("BlossomValleyH_000", "test000");
                    }
                }

                else
                {
                    currentStamina = (float)Data.Character.CharacterStat.GetAbilityAmount("Stamina");
                    energy += soulExtractSaveData.GetTotalEnergy();
                    if (difficulty == "normal")
                        Teleport("Blossom_Valley_000", "test000");
                    else
                        Teleport("BlossomValleyH_000", "test000");
                }

                LoadEquipmentData();
                
                switch(characterClass)
                {
                    case EnumEntityClass.Warrior:
                        GetComponent<Animator>().runtimeAnimatorController = warriorAnimatorController;
                        break;
                    case EnumEntityClass.Mage:
                        GetComponent<Animator>().runtimeAnimatorController = mageAnimatorController;
                        break;
                    case EnumEntityClass.Archer:
                        GetComponent<Animator>().runtimeAnimatorController = archerAnimatorController;
                        break;
                }
                GetComponent<Animator>().SetFloat("deltaX", 0);
                GetComponent<Animator>().SetFloat("deltaY", -1);


                //characterImage.sprite = GetCharacterSpriteSet().GetSpriteFollowingAngle(180);

                CharacterStatUpdate();
                CharacterStaminaUpdate();

                PlayerPrefs.SetInt("isContinue", 1);

                if (PlayerPrefs.GetInt("playCount", 1) <= 1 && encounterCount == 0)
                {
                    ModalWindowController.GetInstnace().SetContent(
                        BoardSectionCollection.GetInstance().boardSections["tutorial000"]
                    );
                    ModalWindowController.GetInstnace().SetTitle("Tutorial");
                    ModalWindowController.GetInstnace().OpenWindow();
                }
            }
        }

        void Update()
        {

            if
            (
                Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer
            )
            {
                if (Input.touchCount >= 1)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        {
                            uiTouched = false;
                        }
                        else
                        {
                            uiTouched = true;
                        }
                    }
                    else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        if (!uiTouched)
                        {
                            OnTouch();
                        }
                    }
                    else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                    {
                        OnTouchEnd();
                    }
                }
            }
            else
            {
                if(Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                        uiTouched = true;
                    else
                        uiTouched = false;
                }
                if (Input.GetMouseButtonUp(0) && !uiTouched)
                {
                    OnTouchEnd();
                }

                else if (Input.GetMouseButton(0) && !uiTouched)
                {
                    OnTouch();
                }
            }

            inputPos = UnityEngine.Vector2.zero;
            if (
                (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
                && Input.touchCount > 0
            )
            {
                inputPos = Input.GetTouch(0).position;
            }
            else if (Input.GetMouseButton(0))
            {
                inputPos = Input.mousePosition;
            }


            worldInputPos = Camera.main.ScreenToWorldPoint(inputPos);


        }

        void FixedUpdate()
        {
            if(targetDeltaPos != UnityEngine.Vector3.zero)
            {
                GetComponent<Rigidbody2D>().MovePosition(
                    transform.position + targetDeltaPos * Time.deltaTime * GetMovementSpeed()
                );
                foreach (var characterFollower in characterFollowers)
                {
                    characterFollower.OnMove(this);
                }
                targetDeltaPos = UnityEngine.Vector3.zero;
            }
        }



        public void CharacterStatUpdate()
        {
            foreach (var component in displayComponents)
            {
                if(component != null)
                    component.Refresh(this);
            }

            goldText.text = string.Format("{0:N0}G", gold);

            characterStat.GetRawCharacterStat().RefreshMaxExp();


            UpdateAbility();

            if (GetCurrentGameZone() != null)
                zoneTextImage.color = GetDefenceColor(GetCurrentGameZone().GetBaseLv());

            if (energy <= 0 && !energyPending)
            {
                EnergyExhausted();
            }
        }

        void CharacterStaminaUpdate()
        {
            // TO DO : Stamina Update Related
            foreach (var component in displayComponents)
            {
                if (component != null && component.GetType() == typeof(StaminaBar))
                    component.Refresh(this);
            }
        }

        public BigInteger GetLevel()
        {
            return characterStat.GetRawCharacterStat().GetLevel();
        }

        public GameZone GetCurrentGameZone()
        {
            if (currentGameZone.Count > 0)
                return currentGameZone.First.Value;
            else return null;
        }

        public void TeleportWithoutMapMovement(string triggerID)
        {
            follower.SuspendFunction();
            currentGameZone.Clear();

            GameObject teleportDests = GameObject.Find("Teleport Destination");

            UnityEngine.Vector3 teleportPos = UnityEngine.Vector3.zero;

            foreach (var teleportDest in teleportDests.transform)
            {
                GameObject dest = ((Transform)teleportDest).gameObject;
                Debug.Log(dest.name);

                if (dest.name == triggerID)
                {
                    Debug.Log(triggerID);
                    teleportPos = dest.transform.position;
                    Debug.Log(teleportPos);
                    break;
                }
            }

            if (teleportPos == UnityEngine.Vector3.zero)
            {
                teleportPos = GetComponent<Rigidbody2D>().position;
            }

            GetComponent<Rigidbody2D>().position = teleportPos;

            OnTouchEnd();

            follower.RestartFunction(
                null,
                teleportPos
            );

            SaveCharacterData();
        }

        public void Teleport(string destMap, string triggerID)
        {
            if (LoadingLayer.GetInstance() != null)
            {
                LoadingLayer.GetInstance().gameObject.SetActive(true);
            }

            follower.SuspendFunction();
            currentGameZone.Clear();

            var sceneLoad = SceneManager.LoadSceneAsync(destMap);

            StartCoroutine(WhileTeleport(sceneLoad));

            sceneLoad.completed += (handle) =>
            {
                GameObject teleportDests = GameObject.Find("Teleport Destination");

                UnityEngine.Vector3 teleportPos = UnityEngine.Vector3.zero;

                foreach (var teleportDest in teleportDests.transform)
                {
                    GameObject dest = ((Transform)teleportDest).gameObject;
                    Debug.Log(dest.name);

                    if (dest.name == triggerID)
                    {
                        teleportPos = dest.transform.position;
                        break;
                    }
                }

                if (teleportPos == UnityEngine.Vector3.zero)
                {
                    teleportPos = GetComponent<Rigidbody2D>().position;
                }

                //GetComponent<Rigidbody2D>().position = teleportPos;
                transform.position = teleportPos;

                OnTouchEnd();

                var bgTilemap = GameObject.Find("Grid").transform.Find("BGTilemap");
                UnityEngine.Tilemaps.Tilemap tilemap = null;
                if (bgTilemap != null)
                {
                    tilemap = bgTilemap.GetComponent<UnityEngine.Tilemaps.Tilemap>();
                }

                follower.RestartFunction(
                    tilemap,
                    teleportPos
                );

                /*
                if (initTeleport && SceneManager.GetActiveScene().name == "test000")
                {
                    SaveCharacterData();
                }
                else if(SceneManager.GetActiveScene().name != "test000")
                {
                    SaveCharacterData();
                }
                */
                SaveCharacterData();

                if (PlayerPrefs.HasKey("SceneMovementTriggerFallback"))
                    PlayerPrefs.DeleteKey("SceneMovementTriggerFallback");
            };
        }

        IEnumerator WhileTeleport(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                if (LoadingLayer.GetInstance() != null)
                {
                    LoadingLayer.GetInstance().SetValue(operation.progress);
                }
                yield return null;
            }
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            TeleportTrigger trigger;

            if (other.tag == "Zone")
            {
                GameZone gameZone = other.GetComponent<GameZone>();
                if (currentGameZone.Contains(gameZone))
                {
                    currentGameZone.Remove(gameZone);
                }
                currentGameZone.AddFirst(gameZone);

                zoneText.text = string.Format("{0:N0}", GetCurrentGameZone().GetBaseLv());
                zoneTextImage.color = GetDefenceColor(GetCurrentGameZone().GetBaseLv());

            }
            else if (other.tag == "ItemThrowTrigger")
            {
                var itemTrigger = other.GetComponent<ItemThrowTrigger>();
                currentItemTrigger.AddFirst(itemTrigger);
            }
            else if (other.transform.tag == "Signpost")
            {
                currentSignpost = other.transform.GetComponent<Signpost>();
                hintButton.SetActive(true);
            }
            else if ((trigger = other.GetComponent<TeleportTrigger>()) != null)
            {
                PlayerPrefs.SetString("SceneMovementTriggerFallback", trigger.GetTriggerID());
                SaveCharacterData(trigger.GetDestMap(), trigger.GetTriggerID());
                Teleport(trigger.GetDestMap(), trigger.GetTriggerID());
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log(other.gameObject.name);

            if (other.tag == "Zone")
            {
                GameZone gameZone = other.GetComponent<GameZone>();

                currentGameZone.Remove(gameZone);
                try
                {
                    zoneText.text = string.Format("{0:N0}", GetCurrentGameZone().GetBaseLv());
                    zoneTextImage.color = GetDefenceColor(GetCurrentGameZone().GetBaseLv());
                }
                catch (Exception e)
                {

                }
            }
            else if (other.transform.tag == "Signpost")
            {
                currentSignpost = null;
                hintButton.SetActive(false);
            }
            else if (other.tag == "ItemThrowTrigger")
            {
                var itemTrigger = other.GetComponent<ItemThrowTrigger>();
                currentItemTrigger.Remove(itemTrigger);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            BossEncounter boss = other.transform.GetComponent<BossEncounter>();
            TeleportTriggerWithoutMapMovement triggerWithoutMovement;

            if (boss != null)
            {
                OnTouchEnd();
                Action.BattleController.GetInstance().InitBattle(boss);
            }

            else if (other.transform.tag == "NPC")
            {
                AbstractNPC npc = other.transform.GetComponent<AbstractNPC>();
                npc.OnAction();
            }

            else if (
                (triggerWithoutMovement = other.transform.GetComponent<TeleportTriggerWithoutMapMovement>())
                != null
            )
            {
                TeleportWithoutMapMovement(triggerWithoutMovement.GetTriggerID());
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public BigInteger GetSTR() { return characterStat.GetRawCharacterStat().GetStatStr(); }
        public BigInteger GetINT() { return characterStat.GetRawCharacterStat().GetStatInt(); }
        public BigInteger GetDEX() { return characterStat.GetRawCharacterStat().GetStatDex(); }
        public BigInteger GetVIT() { return characterStat.GetRawCharacterStat().GetStatVit(); }
        public BigInteger GetEND() { return characterStat.GetRawCharacterStat().GetStatEnd(); }
        public BigInteger GetSP() { return characterStat.GetRawCharacterStat().GetStatPoint(); }

        public BigInteger GetEXP() { return characterStat.GetRawCharacterStat().GetExp(); }
        public BigInteger GetMaxEXP() { return characterStat.GetRawCharacterStat().GetMaxExp(); }

        public void GainLevel(string value) { characterStat.GetRawCharacterStat().AddLv(BigInteger.Parse(value)); }

        

        public Sprite GetDefaultImage() { return upImage; }

        public Data.BattleInstance.Character GenerateBattleInstance()
        {
            return new Data.BattleInstance.Character(this);
        }

        public void ApplyItemAbilities()
        {
            List<ItemAbility> allItemAbility = equipmentSets[currentEquipmentSetIndex].GetTotalAbility();
            List<string> allTagResists = equipmentSets[currentEquipmentSetIndex].GetAllTagResists();

            allItemAbility.AddRange(ItemCollection.GetInstance().GetPassiveAbilities());

            currentItemAbilities = allItemAbility;
            currentTagResists = allTagResists;

            CharacterStatUpdate();
        }


        public void ApplySkillAbilities(Battle battle = null)
        {

            List<SkillAbility> allSkillAbility = new List<SkillAbility>();
            allSkillAbility.AddRange(SkillCollection.GetInstance().GetCurrentCharacterBuff(battle));
            
            if(BattleBuffController.GetInstance() != null)
            {
                allSkillAbility.AddRange(BattleBuffController.GetInstance().GetCurrentCharacterBuff());
                allSkillAbility.AddRange(BattleBuffController.GetInstance().GetCurrentCharacterDebuff());
            }

            currentSkillAbilities = allSkillAbility;

            UpdateAbility();
        }

        public void UpdateAbility()
        {
            Data.Character.CharacterStat.UpdateStat(
                characterStat.GetRawCharacterStat(),
                currentItemAbilities,
                currentSkillAbilities
            );
        }

        public void GainEnergy(BigInteger energy)
        {
            this.energy += energy;
        }

        public void LoseEnergy(BigInteger energy)
        {
            this.energy -= energy;

            if (energy <= 0 && !energyPending)
            {
                EnergyExhausted();
            }

        }

        public void SetEnergyPending(bool pending)
        {
            this.energyPending = pending;
            if (!pending)
            {
                CharacterStatUpdate();
            }
        }

        public BigInteger GetEnergy() { return energy; }
        public void GainStamina(float ratio)
        {
            float maxStamina = GetMaxStamina();
            currentStamina += (maxStamina * ratio);
            if (currentStamina > maxStamina) currentStamina = maxStamina;

            CharacterStaminaUpdate();
        }
        public void LoseStamina(float amount = 1.0f)
        {
            GameZone gameZone = GetCurrentGameZone();
            if (gameZone != null)
                currentStamina -= (Time.deltaTime * amount * GetCurrentGameZone().GetStaminaRate());
            else
                currentStamina -= (Time.deltaTime * amount);

            if (currentStamina < 0)
            {
                currentStamina = 0;
            }

            CharacterStaminaUpdate();
        }
        public void EnergyExhausted()
        {
            BlockMovement();
            EnergyExhaustedDisplay.GetInstance().EnergyExhausted();
        }
        public float GetStamina() { return currentStamina; }
        public float GetMaxStamina() { return (float)Data.Character.CharacterStat.GetAbilityAmount("Stamina"); }

        public float GetMovementSpeed() { return (float)Data.Character.CharacterStat.GetAbilityAmount("Movement Speed") / 10.0f + 2.3f; }

        public BigInteger GetGold() { return gold; }

        public void ZoneUpdate()
        {
            zoneText.text = string.Format("{0:N0}", GetCurrentGameZone().GetBaseLv());
            zoneTextImage.color = GetDefenceColor(GetCurrentGameZone().GetBaseLv());
        }

        public void GainGold(BigInteger count)
        {
            gold += count * (BigInteger)(100 + Data.Character.CharacterStat.GetAbilityAmount("Base Gold Rate")) / 100;
            goldText.text = string.Format("{0:N0}G", gold);
        }
        public void LoseGold(BigInteger count)
        {
            gold -= count;
            goldText.text = string.Format("{0:N0}G", gold);
        }

        public void AddWinCount()
        {
            ++winCount;
        }

        public void AddEncounterCount()
        {
            ++encounterCount;
        }

        public BigInteger GetWinCount()
        {
            return winCount;
        }

        public BigInteger GetEncounterCount()
        {
            return encounterCount;
        }
        public EquipmentSet GetCurrentEquipmentSet() { return equipmentSets[currentEquipmentSetIndex]; }

        public List<EquipmentSet> GetEquipmentSets() { return equipmentSets; }

        public void CorrectEquipmentSet()
        {
            for (int i = 0; i < equipmentSets.Count; ++i)
            {
                equipmentSets[i].CorrectEquipmentSet();
            }
            SaveEquipmentData();
        }
        /*
        public void CorrectSkillSet()
        {
            
            for (int i = 0; i < equipmentSets.Count; ++i)
            {
                equipmentSets[i].CorrectSkillSet();
            }
            ApplySkillAbilities();
            ApplyItemAbilities();
            SaveEquipmentData();
        }
        */

        public void ResetSkillSet()
        {
            for (int i = 0; i < equipmentSets.Count; ++i)
            {
                equipmentSets[i].ResetSkill();
            }
            ApplySkillAbilities();
            ApplyItemAbilities();
            SaveEquipmentData();
        }

        public void NextEquipmentSet()
        {
            ++currentEquipmentSetIndex;
            if (currentEquipmentSetIndex >= equipmentSets.Count)
                currentEquipmentSetIndex = 0;

            ApplyItemAbilities();

            setItemInformation.Refresh();

            PlayerPrefs.SetInt("InGame/Character/currentEquipmentSetIndex", currentEquipmentSetIndex);
        }

        public void SetEquipmentSet(string setName)
        {
            int equipmentSetIndex = equipmentSets.FindIndex(x => x.GetSetName() == setName);
            if(equipmentSetIndex != -1)
            {
                currentEquipmentSetIndex = equipmentSetIndex;

                ApplyItemAbilities();

                PlayerPrefs.SetInt("InGame/Character/currentEquipmentSetIndex", currentEquipmentSetIndex);
            }
        }

        public void SaveEquipmentData()
        {
            Data.SaveData.EquipmentSaveDataManager.SaveEquipmentData(equipmentSets);
        }
        public void SaveSlotData()
        {
            Data.SaveData.EquipmentSaveDataManager.SaveSlotData(GetAccLocked());
        }
        public void SaveCharacterData()
        {
            Data.SaveData.CharacterSaveDataManager.SaveCharacterData(
                new Data.SaveData.CharacterSaveData(
                    characterStat.GetRawCharacterStat(),
                    currentStamina, GetMaxStamina(), energy, winCount, encounterCount, gold,
                    transform.position.x, transform.position.y, transform.position.z,
                    SceneManager.GetActiveScene().name,
                    characterClass.ToString()
                )
            );
        }

        public void SaveCharacterData(string mapName, string triggerName)
        {
            Data.SaveData.CharacterSaveDataManager.SaveCharacterData(
                new Data.SaveData.CharacterSaveData(
                    characterStat.GetRawCharacterStat(),
                    currentStamina, GetMaxStamina(), energy, winCount, encounterCount, gold,
                    transform.position.x, transform.position.y, transform.position.z,
                    mapName,
                    characterClass.ToString()
                )
            );

        }
        public void SaveCharacterData(UnityEngine.Vector3 pos)
        {
            Data.SaveData.CharacterSaveDataManager.SaveCharacterData(
                new Data.SaveData.CharacterSaveData(
                    characterStat.GetRawCharacterStat(),
                    currentStamina, GetMaxStamina(), energy, winCount, encounterCount, gold,
                    pos.x, pos.y, pos.z,
                    SceneManager.GetActiveScene().name,
                    characterClass.ToString()
                )
            );
        }

        public void LoadInventoryData()
        {
            bool loadResult = Data.SaveData.InventorySaveDataManager.LoadInventoryData();

            if (!loadResult)
            {
                for (int i = 0; i < equipmentSets.Count; ++i)
                {
                    equipmentSets[i].InitItemCollectionWithSet();
                }

                ItemCollection.GetInstance().SetItemCount(warriorDefaultWeapon.itemID, 1);
                ItemCollection.GetInstance().SetItemCount(mageDefaultWeapon.itemID, 1);
                ItemCollection.GetInstance().SetItemCount(archerDefaultWeapon.itemID, 1);

                Data.SaveData.InventorySaveDataManager.SaveInventoryData();
            }
        }

        public void LoadSkillData()
        {
            bool loadResult = Data.SaveData.SkillSaveDataManager.LoadSkillData();

            if (!loadResult)
            {
                Data.SaveData.SkillSaveDataManager.SaveSkillData();
            }
        }

        public void LoadEquipmentData()
        {
            List<EquipmentSet> _equipmentSets;
            Data.SaveData.EquipmentSaveDataManager.LoadEquipmentData(out _equipmentSets);

            if (_equipmentSets != null)
            {
                for(int i=0; i < _equipmentSets.Count; ++i)
                {
                    equipmentSets[i] = _equipmentSets[i];

                    switch (characterClass)
                    {
                        case EnumEntityClass.Warrior:
                            equipmentSets[i].SetSkill(0, warriorDefaultSkill);
                            break;
                        case EnumEntityClass.Mage:
                            equipmentSets[i].SetSkill(0, mageDefaultSkill);
                            break;
                        case EnumEntityClass.Archer:
                            equipmentSets[i].SetSkill(0, archerDefaultSkill);
                            break;
                        default:
                            break;
                    }
                }

                for(int i=_equipmentSets.Count; i<equipmentSets.Count; ++i)
                {
                    switch (characterClass)
                    {
                        case EnumEntityClass.Warrior:
                            equipmentSets[i].SetItem("weapon", warriorDefaultWeapon);
                            equipmentSets[i].SetSkill(0, warriorDefaultSkill);
                            break;
                        case EnumEntityClass.Mage:
                            equipmentSets[i].SetItem("weapon", mageDefaultWeapon);
                            equipmentSets[i].SetSkill(0, mageDefaultSkill);
                            break;
                        case EnumEntityClass.Archer:
                            equipmentSets[i].SetItem("weapon", archerDefaultWeapon);
                            equipmentSets[i].SetSkill(0, archerDefaultSkill);
                            break;
                        default:
                            break;
                    }
                }

                SaveEquipmentData();
                CharacterStatUpdate();
            }
            else
            {

                for (int i = 0; i < equipmentSets.Count; ++i)
                {
                    switch (characterClass)
                    {
                        case EnumEntityClass.Warrior:
                            equipmentSets[i].SetItem("weapon", warriorDefaultWeapon);
                            equipmentSets[i].SetSkill(0, warriorDefaultSkill);
                            break;
                        case EnumEntityClass.Mage:
                            equipmentSets[i].SetItem("weapon", mageDefaultWeapon);
                            equipmentSets[i].SetSkill(0, mageDefaultSkill);
                            break;
                        case EnumEntityClass.Archer:
                            equipmentSets[i].SetItem("weapon", archerDefaultWeapon);
                            equipmentSets[i].SetSkill(0, archerDefaultSkill);
                            break;
                        default:
                            break;
                    }
                }

                SaveEquipmentData();
                CharacterStatUpdate();
            }

        }

        public void LoadSlotData()
        {
            List<bool> isAccLocked;
            Data.SaveData.EquipmentSaveDataManager.LoadSlotData(out isAccLocked);

            if (isAccLocked != null)
            {
                accLocked = isAccLocked;
            }
        }


        private void LoadElementalSoulDropData()
        {
            elementalSoulDropSaveData = Data.SaveData.ElementalSoulDropSaveDataManager.LoadElementalSoulDropSaveData();
        }

        public void SaveElementalSoulDropData()
        {
            Data.SaveData.ElementalSoulDropSaveDataManager.SaveElementalSoulDropSaveData(elementalSoulDropSaveData);
        }


        private void LoadSoulExtractData()
        {
            soulExtractSaveData = Data.SaveData.SoulExtractSaveDataManager.LoadSoulExtractData();
        }

        public void SaveSoulExtractData()
        {
            Data.SaveData.SoulExtractSaveDataManager.SaveSoulExtractData(soulExtractSaveData);
        }


        public void ExtractEnergy(string bossId, int amount) {
            if (amount < 0) amount = 0;
            soulExtractSaveData.AddData(bossId, amount);
        }
        public void GainElementalSoul(string bossId, EnumElemental _elementalType, int amount)
        {
            if (amount < 0) return;
            elementalSoulDropSaveData.AddData(bossId, _elementalType, amount);
        }

        public bool isExtracted(string bossId) {

            if (difficulty != "normal")
                bossId = String.Format("{0}_{1}", bossId, difficulty);

            return soulExtractSaveData.contains(bossId);
        }
        public bool isElementalSoulGain(string bossId)
        {
            if (difficulty != "normal")
                bossId = String.Format("{0}_{1}", bossId, difficulty);

            return elementalSoulDropSaveData.contains(bossId);
        }

        public void OnTouch()
        {
            if (
                inputPos != UnityEngine.Vector2.zero &&
                !movementBlocked &&
                GetStamina() > 0
            )
            {
                GetComponent<Animator>().SetBool("isIdle",false);

                var deltaPos = worldInputPos - transform.position;
                deltaPos.z = 0;

                deltaPos *= 5;

                Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();

                float angle = UnityEngine.Vector3.Angle(UnityEngine.Vector3.up, deltaPos);

                if (deltaPos.x < 0) angle *= -1;

                //characterImage.sprite = GetCharacterSpriteSet().GetSpriteFollowingAngle(angle);

                if (deltaPos.magnitude > 1)
                    deltaPos = deltaPos.normalized;
                Debug.Log(deltaPos);

                GetComponent<Animator>().SetFloat("deltaX", deltaPos.x);
                GetComponent<Animator>().SetFloat("deltaY", deltaPos.y);
                GetComponent<Animator>().SetFloat("speed", deltaPos.magnitude * GetMovementSpeed() / 2.0f);

                targetDeltaPos = deltaPos;

                //transform.Translate(deltaPos * Time.deltaTime * currentMovementSpeed * 1.5f);
                //rigidbody2D.velocity = UnityEngine.Vector2.zero;
                //rigidbody2D.AddForce(deltaPos * GetMovementSpeed(), ForceMode2D.Impulse);

                LoseStamina();

            }
            else if (movementBlocked || GetStamina() <= 0)
            {
                OnTouchEnd();
            }

        }

        public void OnTouchEnd()
        {
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();

            rigidbody2D.velocity = UnityEngine.Vector2.zero;
            targetDeltaPos = UnityEngine.Vector3.zero;

            GetComponent<Animator>().SetFloat("speed", 0.0f);
            GetComponent<Animator>().SetBool("isIdle",true);

            uiTouched = true;

        }

        public void OnThrowItem(Data.Item.Item item)
        {
            if(currentItemTrigger.Count > 0)
                currentItemTrigger.First.Value.OnItemThrown(item);
        }

        public void BlockMovement()
        {
            movementBlocked = true;
        }

        public void EnableMovement()
        {
            movementBlocked = false;
        }

        public bool IsAccLocked(int slot)
        {
            return accLocked[slot];
        }

        public List<bool> GetAccLocked() { return accLocked; }

        public void UnlockAcc(int slot)
        {
            accLocked[slot] = false;

            CharacterStatUpdate();

            SaveSlotData();
        }

        public Color GetDefenceColor(BigInteger _lv)
        {
            return Color.black;
        }

        public int GetGameSpeed() { return gameSpeed; }
        public void SetGameSpeed(int amount) {
            gameSpeed = amount;
            PlayerPrefs.SetInt("InGame/Character/Speed", amount);
            gameSpeedText.text = string.Format("Battle Speed : {0}x", amount);
        }

        public void SwitchGameSpeed()
        {
            int currentGameSpeed = GetGameSpeed() * 2;
            if (currentGameSpeed > 16) currentGameSpeed = 1;
            SetGameSpeed(currentGameSpeed);
        }

        public void OpenHint()
        {
            if (currentSignpost != null)
                currentSignpost.Interact();
        }

        public EnumEntityClass GetCurrentClass()
        {
            return characterClass;
        }
        /*
                public CharacterSpriteSet GetCharacterSpriteSet()
                {
                    CharacterSpriteSet spriteSet;

                    switch (characterClass)
                    {
                        case EnumEntityClass.Warrior:
                            spriteSet = warriorSpriteSet;
                            break;
                        case EnumEntityClass.Mage:
                            spriteSet = mageSpriteSet;
                            break;
                        case EnumEntityClass.Archer:
                            spriteSet = archerSpriteSet;
                            break;
                        default:
                            spriteSet = warriorSpriteSet;
                            break;
                    }

                    return spriteSet;
                }
                */

                public float GetTakenDamageChanger()
                {
                    return Data.Character.CharacterStat.GetDamageReductionRatio();
                }

        public int GetResistCount(List<string> mobTagList)
        {
            return mobTagList.FindAll(e => currentTagResists.Contains(e)).Count;
        }

    }
}
