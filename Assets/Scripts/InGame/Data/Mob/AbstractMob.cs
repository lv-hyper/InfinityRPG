using InGame.Data.BattleInstance;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor.UI;
#endif

namespace InGame.Data.Mob
{
    public class AbstractMob : ScriptableObject
    {
#if UNITY_EDITOR
        static bool debug = true;
#else
        static bool debug = false;
#endif
        public bool IsLoadingSprite { get; protected set; }
        protected Sprite mobSprite;
        [SerializeField] public AssetReferenceT<Sprite> mobSpriteRef;
        [SerializeField] protected UnityEngine.Vector2 imageOffset;

        [TextArea(5, 10)]
        [SerializeField] protected string description;
        [SerializeField] protected EnumEntityClass mobClass;
        [SerializeField] protected List<ElementalElement> elementalDamage;
        [SerializeField] protected List<ElementalElement> elementalDefence;
        [SerializeField] protected long integratedElementalDefence = -1;

        [SerializeField] protected long lv;
        [SerializeField] protected string difficulty;
        [SerializeField] protected float hpRate = 1.0f, atkRate = 1.0f;
        [SerializeField] protected float expRate = 1.0f, goldRate = 1.0f;
        [SerializeField] protected float defReqRate = 1.0f;
        [SerializeField] protected bool hidden = false;

        [SerializeField] protected List<Skill.EnemyActiveSkill> skills;

        public string GetDifficulty()
        {
            return difficulty;
        }

        [SerializeField] protected long customHP = -1, customATK = -1;
        [SerializeField] protected float maxDefenceRatio = 2.0f;
        [SerializeField] protected List<DropProperty> dropTable;
        [SerializeField] protected List<string> tagList;

        public virtual double AttackBalanceFunction(BigInteger lv)
        {
            double result;
            double _lv = (double)lv;

            if (lv <= 35000)
            {
                result = Math.Pow(_lv, 1.29f);
            }
            else if (lv <= 80000)
            {
                result =
                    Math.Pow(_lv, 1.33f) /
                    (Math.Pow(35000, 1.33f) / AttackBalanceFunction(35000));
            }
            else if (lv <= 1000000000)
            {
                result =
                    Math.Pow(_lv, 1.38f) /
                    (Math.Pow(80000, 1.38f) / AttackBalanceFunction(80000));
            }
            else
            {
                result =
                    Math.Pow(_lv, 1.41f) /
                    (Math.Pow(1000000000, 1.41f) / AttackBalanceFunction(1000000000));
            }
            return result;
        }

        public virtual double HPBalanceFunction(BigInteger lv)
        {
            double result;
            double _lv = (double)lv;
            if (lv <= 3000)
            {
                result = Math.Pow(_lv, 1.22f);
            }
            else if (lv <= 500000)
            {
                result =
                    Math.Pow(_lv, 1.31f) /
                    (Math.Pow(3000, 1.31f) / HPBalanceFunction(3000));
            }
            else if (lv <= 300000000)
            {
                result =
                    Math.Pow(_lv, 1.35f) /
                    (Math.Pow(500000, 1.35f) / HPBalanceFunction(500000));
            }
            else if(lv <= 10000000000)
            {
                result =
                    Math.Pow(_lv, 1.38f) /
                    (Math.Pow(300000000, 1.38f) / HPBalanceFunction(300000000));
            }
            else
            {
                result =
                    Math.Pow(_lv, 1.4f) /
                    (Math.Pow(10000000000, 1.38f) / HPBalanceFunction(10000000000));
            }
            return result;
        }

        public virtual float getDefenceRatio(float characterDefIgnore, EnumEntityClass attackType)
        {
            float defRatio = maxDefenceRatio;

            if (attackType == mobClass) defRatio *= 1.1f;

            defRatio = defRatio * (100.0f - characterDefIgnore) / 100.0f;

            if (defRatio < 1.0f) defRatio = 1.0f;

            return defRatio / 2.0f;
        }

        public virtual BigInteger GetHP() {
            if (customHP >= 1)
            {
                return customHP;
            }
            else
            {
                return (BigInteger)(
                    hpRate * 14.0f * HPBalanceFunction(lv)
                ) + 17;
            }
        }
        public virtual BigInteger GetATK() {
            if (customATK >= 1)
            {
                return customATK;
            }
            else
            {
                return (BigInteger)(
                    atkRate * 3.3f * AttackBalanceFunction(lv)
                ) + 21;
            }
        }

        public virtual System.Numerics.BigInteger GetEXP()
        {
            float exponent = 2.1f;

            if (lv < 1000)
            {
                exponent = 2.1f;
            }
            else if (lv < 5000)
            {
                exponent = 2.1f + 0.1f * ((lv - 1000.0f) / 4000.0f);
            }
            else if (lv < 1000000)
            {
                exponent = 2.2f;

                float logValue = (Mathf.Log10(lv / 5000) / 17);
                if (logValue < 0) logValue = 0;

                exponent += logValue;

                if (exponent > 2.33f) exponent = 2.33f;
            }
            else if (lv < 1000000000)
            {
                exponent = 2.33f;

                float logValue = (Mathf.Log10(lv / 1000000) / 25);
                if (logValue < 0) logValue = 0;

                exponent += logValue;

                if (exponent > 2.45f) exponent = 2.45f;
            }
            else
            {
                exponent = 2.45f;

                float logValue = (Mathf.Log10(lv / 1000000000) / 40);
                if (logValue < 0) logValue = 0;

                exponent += logValue;

                if (exponent > 2.5f) exponent = 2.5f;
            }

            Debug.Log(lv+"^"+exponent+"="+Math.Pow(lv, exponent));
            return ((System.Numerics.BigInteger)(
                Math.Pow(lv, exponent) * 43.0f * expRate
            ) + (System.Numerics.BigInteger)(5 * expRate));
        }

        public virtual BigInteger GetGold()
        {
            return (BigInteger)((Math.Pow(lv, 0.8) * 15 + 20) * goldRate);
        }

        public virtual BigInteger GetLV() { return lv; }
        public AsyncOperationHandle<Sprite>? GetSpriteAsync(UnityAction onComplete)
        {
            if (mobSprite == null)
            {
                var handle = mobSpriteRef.LoadAssetAsync<Sprite>();
                handle.Completed += (operationHandle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        mobSprite = handle.Result;
                        onComplete?.Invoke();
                        IsLoadingSprite = true;
                    }
                    else
                    {
                        Debug.LogError("Failed to load mob sprite");
                    }
                });
                
                return handle;
            }

            return null;
        }

        public void UnloadSprite()
        {
            mobSprite = null;
            mobSpriteRef.ReleaseAsset();
            IsLoadingSprite = false;
        }

        public Sprite GetSprite()
        {
            return mobSprite;
        }
        
        public UnityEngine.Vector2 GetImageOffset() { return imageOffset; }
        public List<DropProperty> GetDropTable() { return dropTable; }
        //temp
        public void IncreaseDroprate()
        {
            foreach (var e in dropTable)
            {
                e.probablity *= 1.5f;
            }
        }
        //
        public string GetName() { return name; }
        public float GetEXPRate() { return expRate; }
        public float GetGoldRate() { return goldRate; }
        public float GetDefReqRate() { return defReqRate; }
        public EnumEntityClass GetEntityClass() { return mobClass; }

        public float GetElementalDamage(EnumElemental elemental) {
            return elementalDamage.Where(e => e.elemental == elemental).Select(e => e.amount).Sum();
        }
        public float GetElementalDefence(EnumElemental elemental)
        {
            if(integratedElementalDefence > 0)
            {
                bool weakElementalExists = elementalDamage.Any(
                    x => Elemental.GetWeakerElemental(elemental) == x.elemental && x.amount > 0
                );
                bool strongElementalExists = elementalDamage.Any(
                   x => Elemental.GetStrongerElemental(elemental) == x.elemental && x.amount > 0
                );

                if (weakElementalExists ^ strongElementalExists)
                {
                    return integratedElementalDefence * 1.0f;
                }
                else if (strongElementalExists)
                {
                    return integratedElementalDefence * 0.8f;
                }
                else
                {
                    return integratedElementalDefence * 1.2f;
                }
            }

            else return elementalDefence.Where(x=>x.elemental == elemental).Select(x=>x.amount).Sum();
            
        }

        public List<string> GetTagList()
        {
            return tagList;
        }

        public string GetDescription() { return description; }

        public List<Skill.EnemyActiveSkill> GetSkills() { return skills; }

        public List<Item.Item> RollDrop()
        {
            List<Item.Item> itemList = new List<Item.Item>();

            var _dropTable = new List<DropProperty>(dropTable);

            float additionalDropRate = (float)(100 + Data.Character.CharacterStat.GetAbilityAmount("Base Droprate")) / 100.0f;

            if (PlayerPrefs.GetString("passcode", "").ToLower() == "another")
                additionalDropRate /= 3.0f;
/*
            if (PlayerPrefs.GetString("passcode", "").ToLower() == "lorem_ipsum_dolor_sit_amet")
                additionalDropRate *= 10000.0f;
*/

            int _index = 0;

            foreach (var dropProperty in dropTable)
            {
                var itemStatus = Data.Item.ItemCollection.GetInstance().allCollection[dropProperty.item.itemID];


                if (itemStatus.getCount() + 1 > itemStatus.getMaxCount())
                {
                    Debug.Log(string.Format("Warning : {0} is Max", itemStatus.name));
                }
                else if (
                    Random.value <= (dropProperty.probablity * additionalDropRate)
                ) {
                    itemList.Add(dropProperty.item);
                    Debug.Log(string.Format("Item : {0}, Probablity : {1}%", dropProperty.item, (dropProperty.probablity * additionalDropRate * 100)));
                    Data.SaveData.DropTryCountSaveDataManager.ClearDropTryCount(name, _index);
                }
                else
                {
                    var _dropCount = Data.SaveData.DropTryCountSaveDataManager.GetDropTryCount(name, _index);
                    if (_dropCount >= 1.6f * (1 / (dropProperty.probablity * additionalDropRate)))
                    {
                        itemList.Add(dropProperty.item);
                        Debug.Log(string.Format("Item : {0}, Drop Ceiling", dropProperty.item));
                        Data.SaveData.DropTryCountSaveDataManager.ClearDropTryCount(name, _index);
                    }
                    else
                    {
                        Data.SaveData.DropTryCountSaveDataManager.AddDropTryCount(name, _index, additionalDropRate);
                    }
                }

                _index++;
            }

            return itemList;
        }

        public AbstractMob SummonNewMobInstance(BigInteger level)
        {
            AbstractMob newMobInstance = Instantiate(this);
            newMobInstance.lv = (long)level;
            newMobInstance.name = this.name;
            return newMobInstance;
        }

        public virtual void IncreaseLv() {
        }


        public virtual bool IsBoss()
        {
            return false;
        }

        public bool IsHidden() { return hidden; }

        private void Reset()
        {
            hpRate = 1.0f;
            atkRate = 1.0f;
        }

        public void SetLV(long _lv)
        {
            lv = _lv;
        }
        
        

        public virtual int GetSoulLevel()
        {
            //if (this.GetType() == typeof(Boss)) return ((Boss)this).GetSoulLevel();
            return 0; 
        }

        public virtual bool Extracted()
        {
            if (IsBoss()) return ((Boss)this).Extracted();
            return false;
        }
    }
}
