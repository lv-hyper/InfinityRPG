using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

using InGame.Data;
using InGame.Entity;
using InGame.UI;
using UnityEngine.Animations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Numerics;
using Ingame.UI;
using UnityEditor.VersionControl;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace InGame.Action
{
    public class BattleController : MonoBehaviour
    {
#if UNITY_EDITOR
        static bool debug = true;
#else
        static bool debug = false;
#endif

        static BattleController instance;
        [SerializeField] GameObject playModeCanvas, battleModeCanvas, transitionLayer;
        [SerializeField] GameObject battleStartButton, battleRetreatButton, encounterButton;
        [SerializeField] Character character;
        BossEncounter bossData;
        Data.Mob.AbstractMob currentMob;

        string teleportDestMap, teleportTargetID;
        bool isBattleOngoing;
        
        [SerializeField] Image mobImage, characterImage;
        [SerializeField] CharacterBattleHealthBar characterHPBar;
        [SerializeField] MobBattleHealthBar mobHPBar;
        [SerializeField] BattleManaBar characterManaBar;
        [SerializeField] BattleTurnCounter characterTurnCounter;
        [SerializeField] EnemyItemDrop dropItemBar;

        [SerializeField] GameObject mobDescription;

        [SerializeField] GameObject mobElemental, characterElemental;

        [SerializeField] Image charWater, charNature, charFire, charAir;
        [SerializeField] Image mobWater, mobNature, mobFire, mobAir;

        [SerializeField] TextMeshProUGUI mobElementalValueText, characterElementalValueText;

        [SerializeField] Sprite waterSprite, fireSprite, thunderSprite, natureSprite, airSprite;
        [SerializeField] Color highDefColor, midHighDefColor, midDefColor, lowMidDefColor, lowDefColor, noDefColor;

        [SerializeField] Image characterDefenceImage;
        [SerializeField] TextMeshProUGUI characterDefenceValueText;

        [SerializeField] Image mobTypeImage;
        [SerializeField] Sprite physicalSprite, magicalSprite, rangedSprite;
        [SerializeField] Image unextractableIcon;  
        [SerializeField] Sprite unextractableSprite;
        [SerializeField] GameObject mobDamageText, characterDamageText;

        [SerializeField] GameObject resultButton, resultCanvas, winText, loseText;
        [SerializeField] TextMeshProUGUI resultEXPText, resultGoldText, resultEnergyText;


        [SerializeField] GameObject itemSlot, dropElementPrefab;

        [SerializeField] Transform dropList;

        [SerializeField] List<GameObject> displayComponentGameObjectList;

        [SerializeField] Data.BattleEffect.Debuff deathReaperEffect;

        [SerializeField] Data.BattleEffect.Debuff airDisadvantage, natureDisadvantage, fireDisadvantage, waterDisadvantage;
        
        [SerializeField] private float transitionSpeed = 5.0f;

        private AsyncOperationHandle<Sprite> mobSpriteHandle;

        Battle battle;
        UnityEngine.Vector2 mobImageOffset;
        UnityEngine.Vector2 originalImagePos;

        public static BattleController GetInstance()
        {
            return instance;
        }

        private IEnumerator WaitForLoadingSprites(UnityAction onFinish)
        {
            yield return new WaitUntil(() => currentMob.GetSprite() != null && mobImage.sprite != null);
            onFinish?.Invoke();
        }

        private void Awake() {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                battle = null;
                bossData = null;
                mobImageOffset = UnityEngine.Vector2.zero;

                teleportDestMap = null;
                teleportTargetID = null;

                isBattleOngoing = false;
            }
        }

        public void InitBattle()
        {
            var gameZone = character.GetCurrentGameZone();
            Debug.Log(gameZone);
            var entity = gameZone.GetNewMob();
            Debug.Log(gameZone.getRandomLv()+"/"+entity);

            InitBattle(entity.SummonNewMobInstance(gameZone.getRandomLv()));
        }

        public void InitBattle(BossEncounter bossData)
        {
            this.bossData = bossData;
            InitBattle(bossData.GenerateBattleInstance());
        }

        public void InitBattle(Data.Mob.AbstractMob mob)
        {
            if (isBattleOngoing) return;
            isBattleOngoing = true;

            var gameZone = character.GetCurrentGameZone();
            int energyRate = gameZone.GetEnergyRate();
            
            currentMob = mob;

            
            character.BlockMovement();
            character.ApplyItemAbilities();
            character.SetEnergyPending(true);

            if(!debug)
                character.LoseEnergy(2 * energyRate);

            if(currentMob.IsBoss() && bossData != null && bossData.GetReturnPoint() != null)
            {
                character.SaveCharacterData(bossData.GetReturnPoint().position);
            }
            else
            {
                character.SaveCharacterData();
            }

            currentMob.GetSpriteAsync(()=>
            {
                originalImagePos = mobImage.GetComponent<RectTransform>().anchoredPosition;
                
                mobImageOffset = currentMob.GetImageOffset();
                mobImage.GetComponent<RectTransform>().anchoredPosition += mobImageOffset;
                mobImage.sprite = currentMob.GetSprite();
                mobImage.color = new Color(1,1,1,1);
                
                mobImage.GetComponent<BobbingAnimation>().enabled = true;
            });

            if (mob.GetDescription() != null && mob.GetDescription() != "")
            {
                mobDescription.GetComponent<MobDescriptionController>().Open(mob);
            }

            
            character.GetComponent<Animator>().SetBool("isBattle", true);
            character.GetComponent<Animator>().Update(0);

            character.GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);

            characterImage.sprite = character.GetComponent<SpriteRenderer>().sprite;
            characterImage.color = new Color(1,1,1,1);
            
            mobTypeImage.color = new Color(1, 1, 1, 1);

            if(currentMob.GetEntityClass() == EnumEntityClass.Warrior)
            {
                mobTypeImage.sprite = physicalSprite;
            }
            else if(currentMob.GetEntityClass() == EnumEntityClass.Mage)
            {
                mobTypeImage.sprite = magicalSprite;
            }
            else if(currentMob.GetEntityClass() == EnumEntityClass.Archer)
            {
                mobTypeImage.sprite = rangedSprite;
            }
            else
            {
                mobTypeImage.sprite = null;
                mobTypeImage.color = new Color(0, 0, 0, 0);
            }

            if(!bossData || !bossData.IsUnextractable())
            {
                unextractableIcon.sprite = null;
                unextractableIcon.color = new Color(0, 0, 0, 0);
            }
            else
            {
                unextractableIcon.sprite = unextractableSprite;
                unextractableIcon.color = new Color(1, 1, 1, 1);
            }

            battle = new Battle(character, currentMob);
            characterManaBar.RegisterToBattle(battle);

            battle.AddDisplayComponent(characterTurnCounter);

            foreach(var _gameObject in displayComponentGameObjectList)
            {
                var displayComponent = _gameObject.GetComponent<IDisplayComponent>();
                battle.AddDisplayComponent(displayComponent);
            }

            battle.UpdateBattleStatus();
            dropItemBar.Refresh(currentMob);//Make enemy item drop list UI
            
            character.ApplySkillAbilities(battle);
            battle.DelayDeath();
            battle.GetCharacterInstance().SetDeathSurpass(
                (BigInteger)Data.Character.CharacterStat.GetAbilityAmount("Surpass Death"));


            RefreshUI();

            encounterButton.SetActive(false);
            
            StartCoroutine(StartBattleScreenTransition(TransitionDirection.LeftToRight, transitionSpeed));
            

            StartCoroutine(WaitForLoadingSprites(() =>
            {
                StartCoroutine(EndBattleScreenTransition(TransitionDirection.LeftToRight, transitionSpeed));

                
                playModeCanvas.SetActive(false);
                battleModeCanvas.SetActive(true);
                
                battleStartButton.SetActive(true);
                if (currentMob.GetType() == typeof(Data.Mob.Boss))
                {
                    battleRetreatButton.SetActive(true);
                }
            }));
        }

        private void RefreshUI()
        {

            Dictionary<EnumElemental, Sprite> elementalToSprite = new Dictionary<EnumElemental, Sprite> {
                { EnumElemental.None,       null},
                { EnumElemental.Water,      waterSprite},
                { EnumElemental.Fire,       fireSprite},
                { EnumElemental.Nature,     natureSprite},
                { EnumElemental.Air,        airSprite}
            }; 
            
            Dictionary<EnumElemental, Image> elementalToMobImage = new Dictionary<EnumElemental, Image> {
                { EnumElemental.None,       null},
                { EnumElemental.Water,      mobWater},
                { EnumElemental.Fire,       mobFire},
                { EnumElemental.Nature,     mobNature},
                { EnumElemental.Air,        mobAir}
            }; 
            
            Dictionary<EnumElemental, Image> elementalToCharacterImage = new Dictionary<EnumElemental, Image> {
                { EnumElemental.None,       null},
                { EnumElemental.Water,      charWater},
                { EnumElemental.Fire,       charFire},
                { EnumElemental.Nature,     charNature},
                { EnumElemental.Air,        charAir}
            };

            var characterElementalBonus = battle.GetCharacterInstance().GetTotalElementalDamageBonus(battle.GetMobInstance());
            var enemyElementalBonus = battle.GetMobInstance().GetTotalElementalDamageBonus(battle.GetCharacterInstance());

            Color transparent = Color.white * new UnityEngine.Vector4(1, 1, 1, 0.25f);

            for(var _elemental = EnumElemental.Air; _elemental <= EnumElemental.Count - 1; ++_elemental)
            {
                /*
                if (enemyElementalBonus <= 1)
                    elementalToMobImage[_elemental].color = UnityEngine.Vector4.zero;
                else 
                */
                if (battle.GetMobInstance().GetElementalDamage(_elemental) > 0)
                    elementalToMobImage[_elemental].color = Color.white;
                else
                    elementalToMobImage[_elemental].color = transparent;

                /*
                if (characterElementalBonus <= 1)
                    elementalToCharacterImage[_elemental].color = UnityEngine.Vector4.zero;
                else 
                */
                if (battle.GetCharacterInstance().GetElementalDamage(_elemental) > 0)
                    elementalToCharacterImage[_elemental].color = Color.white;
                else
                    elementalToCharacterImage[_elemental].color = transparent;
            }

            //mobElemental.SetActive(enemyElementalBonus > 1);
            //characterElemental.SetActive(characterElementalBonus > 1);
            
            mobElemental.SetActive(true);
            characterElemental.SetActive(true);

            characterElementalValueText.text = string.Format(
                "{0:0.0}%",
                (characterElementalBonus - 1) * 100.0f
            );

            mobElementalValueText.text = string.Format(
                "{0:0.0}%", 
                (enemyElementalBonus - 1) * 100.0f
            );

            float defRatio = Data.Character.CharacterStat.GetDefenceRatio(currentMob);

            if (defRatio < 1.1f)
            {
                characterDefenceImage.color = noDefColor;
            }
            else if (defRatio < 3.0f)
            {
                characterDefenceImage.color = lowDefColor;
            }
            else if (defRatio < 5.0f)
            {
                characterDefenceImage.color = lowMidDefColor;
            }
            else if (defRatio < 7.0f)
            {
                characterDefenceImage.color = midDefColor;
            }
            else if (defRatio < 9.0f)
            {
                characterDefenceImage.color = midHighDefColor;
            }
            else
            {
                characterDefenceImage.color = highDefColor;
            }

            characterDefenceValueText.text = String.Format("{0:0.00}%", 100 - 100.0f / defRatio);
        }

        public void StartBattle()
        {
            StartCoroutine(OnBattle());
        }

        public void RetreatBattle()
        {
            var gameZone = character.GetCurrentGameZone();

            int energyRate = gameZone.GetEnergyRate();

            if (!debug)
                character.GainEnergy(energyRate);
            
            
            StartCoroutine(StartBattleScreenTransition(TransitionDirection.RightToLeft, transitionSpeed));
            
            BattleDamageTextController.GetInstance().ClearDamageTexts();
            BattleBuffController.GetInstance().OnEnd();

            playModeCanvas.SetActive(true);
            battleModeCanvas.SetActive(false);

            mobImage.GetComponent<BobbingAnimation>().enabled = false;
            mobImage.GetComponent<RectTransform>().anchoredPosition = originalImagePos;

            if (bossData != null && bossData.GetReturnPoint() != null)
                character.transform.position = bossData.GetReturnPoint().position;

            character.EnableMovement();
            character.SetEnergyPending(false);

            battle = null;
            
            StartCoroutine(EndBattleScreenTransition(TransitionDirection.RightToLeft, transitionSpeed));
            
            isBattleOngoing = false;
            character.GetComponent<Animator>().SetBool("isBattle", false);
            character.GetComponent<Animator>().Update(0);
            character.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
            encounterButton.SetActive(true);
            
            Addressables.Release(mobSpriteHandle);
        }

        IEnumerator OnBattle()
        {
            BattleResult battleResult = null;

            int deathReaperCount = 0;

            Dictionary<EnumElemental, Data.BattleEffect.Debuff> elementalToDebuff = new Dictionary<EnumElemental, Data.BattleEffect.Debuff> {
                { EnumElemental.None,       null},
                { EnumElemental.Air,        airDisadvantage},
                { EnumElemental.Nature,     natureDisadvantage},
                { EnumElemental.Fire,       fireDisadvantage},
                { EnumElemental.Water,      waterDisadvantage}
            };

            for (var _elemental = EnumElemental.Air; _elemental <= EnumElemental.Count - 1; ++_elemental)
            {
                if (battle.GetCharacterInstance().GetElementalDamage(Elemental.GetWeakerElemental(_elemental)) > 0 &&
                    battle.GetMobInstance().GetElementalDamage(_elemental) > 0)
                {
                    battle.GetCharacterInstance().Debuff(elementalToDebuff[_elemental]);
                }
            }

            while (true)
            {

                do
                {
                    if(deathReaperCount != battle.GetDeathReaperDebuffCount())
                    {
                        int diff = battle.GetDeathReaperDebuffCount() - deathReaperCount;
                        deathReaperCount = battle.GetDeathReaperDebuffCount();

                        for(int i=0;i<diff;++i)
                            battle.GetCharacterInstance().Debuff(deathReaperEffect);
                    }
                    battle.CharacterTurn();
                    BattleBuffController.GetInstance().Turn();
                    BattleSkillController.GetInstance().Turn();
                    BattleSkillController.GetInstance().UpdateNextSkill();

                    RefreshUI();

                    battleResult = battle.GetBattleResult();
                    if (battleResult != null)
                        break;

                    yield return new WaitForSeconds(0.2f / Character.GetInstance().GetGameSpeed());
                } while (battle.GetCharacterInstance().isCombo());


                if (battleResult != null)
                    break;

                battle.MobTurn();
                battle.IncrementTurn();

                battleResult = battle.GetBattleResult();
                if (battleResult != null)
                    break;

                yield return new WaitForSeconds(0.2f / Character.GetInstance().GetGameSpeed());
            }

            FinishBattle(battleResult);
        }

        public void FinishBattle(BattleResult battleResult)
        {
            Data.BattleInstance.Mob mobInstance = battle.GetMobInstance();

            System.Numerics.BigInteger expToGet = mobInstance.GetExp();
           // if (currentMob.GetType() == typeof(Data.Mob.Boss) && bossData == null)
           //     expToGet = 0;

            BigInteger goldToGet = mobInstance.GetGold();

            BattleBuffController.GetInstance().ClearCharacterBattleEffect();

            character.ApplySkillAbilities(battle);

            resultButton.SetActive(true);

            if(battleResult.IsSuccess())
            {
                var gameZone = character.GetCurrentGameZone();
                int energyRate = gameZone.GetEnergyRate();

                List<Data.Item.Item> dropItems = currentMob.RollDrop();

                mobImage.color = new Color(1,1,1,0);

                
                resultGoldText.text = string.Format("+{0:N0} gold", goldToGet * energyRate * 
                    (BigInteger)(100 + Data.Character.CharacterStat.GetAbilityAmount("Base Gold Rate")) / 100);

                if(dropItems.Count >= 1)
                {
                    resultCanvas.GetComponent<RectTransform>().sizeDelta = 
                    new UnityEngine.Vector2(600, 600);

                    itemSlot.SetActive(true);

                    foreach(Transform transform in dropList)
                    {
                        Destroy(transform.gameObject);
                    }

                    int _index = 0;
                    foreach (var dropItem in dropItems)
                    {
                        GameObject dropItemInstance = Instantiate(dropElementPrefab, dropList);
                        dropItemInstance.GetComponent<RectTransform>().anchoredPosition = new UnityEngine.Vector2(_index * 80, 0);

                        dropItemInstance.GetComponent<Image>().sprite = dropItem.GetSprite();
                        Data.Item.ItemCollection.GetInstance().AddItemCount(dropItem.itemID, 1);

                        _index++;

                    }
                    Data.SaveData.InventorySaveDataManager.SaveInventoryData();

                    character.ApplyItemAbilities();
                }
                else
                {
                    itemSlot.SetActive(false);
                    resultCanvas.GetComponent<RectTransform>().sizeDelta = 
                    new UnityEngine.Vector2(600, 400);
                }   

                winText.SetActive(true);

                var prevLevel = character.GetLevel();
                Data.Character.CharacterStat.GetInstance().GetRawCharacterStat().GainEXP(expToGet * energyRate);

                var expTextNum = expToGet * energyRate * (System.Numerics.BigInteger)(100 + Data.Character.CharacterStat.GetAbilityAmount("EXP")) / 100;
                string expText = Utility.MinimizeNumText.Minimize(expTextNum);
                resultEXPText.text = string.Format(
                    "+{0} exp (+{1:N0} lv)",
                    expText,
                    character.GetLevel() - prevLevel
                );

                character.GainGold(goldToGet * energyRate);

                if (!debug)
                    character.GainEnergy(energyRate);

                else
                    character.LoseEnergy(energyRate);

                character.AddWinCount();
                character.AddEncounterCount();

                resultEnergyText.text = string.Format("-{0} energy", energyRate);

                if (currentMob.GetType() == typeof(Data.Mob.Boss))
                {
                    if(bossData == null)
                    {
                        character.GainEnergy(energyRate);
                    }
                    else
                    {
                        if (!character.isExtracted(bossData.GetBossID()))
                        {
                            var totalEnergy = bossData.GetAdditionalEnergy() - energyRate;

                            if(totalEnergy > 0)
                                resultEnergyText.text = string.Format("+{0} energy", totalEnergy);
                            else
                                resultEnergyText.text = string.Format("-{0} energy", -totalEnergy);

                            character.GainEnergy(bossData.GetAdditionalEnergy());
                        }
                        else
                        {
                            resultEnergyText.text = string.Format("+{0} energy", 0);
                            character.GainEnergy(energyRate);
                        }

                        if (
                            Data.Character.CharacterStat.GetAbilityAmount("Soul Extract") >= bossData.GetMob().GetSoulLevel() && 
                            !bossData.IsUnextractable()
                        )
                        {
                            character.ExtractEnergy(bossData.GetBossID(), bossData.GetAdditionalEnergy() - energyRate);
                        }

                        if (bossData.GetMob().GetElementalSoulDrops().Count > 0)
                        {
                            foreach(var elementalSoulDrop in bossData.GetMob().GetElementalSoulDrops())
                            {
                                if(elementalSoulDrop.elementalType != EnumElemental.None)
                                    character.GainElementalSoul(bossData.GetBossID(), elementalSoulDrop.elementalType, elementalSoulDrop.count);
                            }
                        }

                        var progressionLevel = PlayerPrefs.GetInt("ProgressionLevel", 0);
                        var bossProgressionLevel = bossData.GetMob().GetProgressionLevel();
                        if (bossProgressionLevel > progressionLevel)
                        {
                            PlayerPrefs.SetInt("ProgressionLevel", bossProgressionLevel);
                        }

                        if (bossData.GetTeleportTarget(out teleportDestMap, out teleportTargetID))
                        {
                            PlayerPrefs.SetString("SceneMovementTriggerFallback", teleportTargetID);
                            character.SaveCharacterData(teleportDestMap, teleportTargetID);
                        }
                        else if (gameZone.GetTeleportDest() != null)
                        {
                            teleportDestMap = gameZone.GetDestMap();
                            teleportTargetID = gameZone.GetTeleportDest();
                            character.SaveCharacterData(teleportDestMap, teleportTargetID);
                        }
                        else
                        {
                            character.SaveCharacterData();
                        }
                        bossData.Kill();
                    }                   

                }
                else
                {
                    if (gameZone.GetTeleportDest() != null)
                    {
                        teleportDestMap = gameZone.GetDestMap();
                        teleportTargetID = gameZone.GetTeleportDest();
                        character.SaveCharacterData(teleportDestMap, teleportTargetID);
                    }
                    else
                    {
                        character.SaveCharacterData();
                    }
                }

                character.GainStamina(1.0f);
            }
            else
            {

                characterImage.color = new Color(1,1,1,0);
                resultEXPText.text = string.Format("+{0:N0} exp", 0);
                resultGoldText.text = string.Format("+{0:N0} gold", 0);
                resultEnergyText.text = string.Format("-{0} energy", 2 * character.GetCurrentGameZone().GetEnergyRate());

                itemSlot.SetActive(false);
                resultCanvas.GetComponent<RectTransform>().sizeDelta = 
                new UnityEngine.Vector2(600, 400);
                
                loseText.SetActive(true);

                character.AddEncounterCount();
                character.GainStamina(0.4f);

                character.SaveCharacterData();
            }

            character.ApplySkillAbilities(null);
            character.CharacterStatUpdate();
            character.SaveElementalSoulDropData();
            character.SaveSoulExtractData();

        }

        public void EndBattle()
        {
            StartCoroutine(StartBattleScreenTransition(TransitionDirection.RightToLeft, transitionSpeed));
            
            BattleDamageTextController.GetInstance().ClearDamageTexts();
            BattleBuffController.GetInstance().OnEnd();

            playModeCanvas.SetActive(true);
            battleModeCanvas.SetActive(false);

            mobImage.GetComponent<BobbingAnimation>().enabled = false;
            mobImage.GetComponent<RectTransform>().anchoredPosition = originalImagePos;


            if (currentMob.GetType() == typeof(Data.Mob.Boss))
            {
                if (battle.GetBattleResult().IsSuccess() == false)
                {
                    if(bossData != null && bossData.GetReturnPoint() != null)
                        character.transform.position = bossData.GetReturnPoint().position;
                }
                else if(teleportDestMap != null && teleportTargetID != null)
                {
                    character.Teleport(teleportDestMap, teleportTargetID);
                    teleportDestMap = null;
                    teleportTargetID = null;
                }
            }

            else if(battle.GetBattleResult().IsSuccess())
            {
                if (teleportDestMap != null && teleportTargetID != null)
                {
                    character.Teleport(teleportDestMap, teleportTargetID);
                    teleportDestMap = null;
                    teleportTargetID = null;
                }
            }

            character.EnableMovement();
            character.SetEnergyPending(false);
                    
            battle = null;
            
            StartCoroutine(EndBattleScreenTransition(TransitionDirection.RightToLeft, transitionSpeed));
            
            if (PlayerPrefs.GetInt("playCount", 1) <= 1)
            {
                if (character.GetWinCount() == 1)
                {
                    ModalWindowController.GetInstnace().SetContent(
                        BoardSectionCollection.GetInstance().boardSections["tutorial001"]
                    );
                    ModalWindowController.GetInstnace().SetTitle("Tutorial");
                    ModalWindowController.GetInstnace().OpenWindow();
                }
                else if (character.GetWinCount() == 2)
                {
                    ModalWindowController.GetInstnace().SetContent(
                        BoardSectionCollection.GetInstance().boardSections["tutorial002"]
                    );
                    ModalWindowController.GetInstnace().SetTitle("Tutorial");
                    ModalWindowController.GetInstnace().OpenWindow();
                }
            }
            isBattleOngoing = false;
            character.GetComponent<Animator>().SetBool("isBattle", false);
            character.GetComponent<Animator>().Update(0);
            character.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
            encounterButton.SetActive(true);
        }

        enum TransitionDirection{
            TopToBottom,
            BottomToTop,
            LeftToRight,
            RightToLeft
        }

        IEnumerator StartBattleScreenTransition(TransitionDirection direction, float speed)
        {
            Image transitionLayerImage = transitionLayer.GetComponent<Image>();
            switch(direction)
            {
                case TransitionDirection.BottomToTop:
                    transitionLayerImage.fillOrigin = (int)Image.OriginVertical.Bottom;
                    break;
                case TransitionDirection.TopToBottom:
                    transitionLayerImage.fillOrigin = (int)Image.OriginVertical.Top;
                    break;
                case TransitionDirection.LeftToRight:
                    transitionLayerImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                    break;
                case TransitionDirection.RightToLeft:
                    transitionLayerImage.fillOrigin = (int)Image.OriginHorizontal.Right;
                    break;
            }
            for(float i=0;i<1;i+=(Time.deltaTime*speed))
            {
                if(i>1) i=1;
                transitionLayer.GetComponent<Image>().fillAmount = i;
                yield return null;
            }
            transitionLayer.GetComponent<Image>().fillAmount = 1;
        }

        IEnumerator EndBattleScreenTransition(TransitionDirection direction, float speed)
        {
            Image transitionLayerImage = transitionLayer.GetComponent<Image>();
            switch(direction)
            {
                case TransitionDirection.BottomToTop:
                    transitionLayerImage.fillOrigin = (int)Image.OriginVertical.Top;
                    break;
                case TransitionDirection.TopToBottom:
                    transitionLayerImage.fillOrigin = (int)Image.OriginVertical.Bottom;
                    break;
                case TransitionDirection.LeftToRight:
                    transitionLayerImage.fillOrigin = (int)Image.OriginHorizontal.Right;
                    break;
                case TransitionDirection.RightToLeft:
                    transitionLayerImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                    break;
            }
            for(float i=1;i>0;i-=(Time.deltaTime*speed))
            {
                if(i<0) i=0;
                transitionLayer.GetComponent<Image>().fillAmount = i;
                yield return null;
            }
            transitionLayer.GetComponent<Image>().fillAmount = 0;
        }
        
        public Battle GetBattle() { return battle; }
    }
}