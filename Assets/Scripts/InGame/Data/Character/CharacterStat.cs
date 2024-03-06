using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Data.Character
{
    public class RawCharacterStat
    {
        BigInteger level;
        BigInteger statPoint;
        BigInteger statStr, statInt, statDex, statVit, statEnd;

        BigInteger exp, maxExp;

        public RawCharacterStat() : this(
            BigInteger.One,
            BigInteger.Zero,
            new BigInteger(10),
            new BigInteger(10),
            new BigInteger(10),
            new BigInteger(10),
            new BigInteger(10)
        ){}

        public RawCharacterStat(
            BigInteger level,
            BigInteger statPoint,
            BigInteger statStr,
            BigInteger statInt,
            BigInteger statDex,
            BigInteger statVit,
            BigInteger statEnd
        )
        {
            this.level      = level;
            this.statPoint  = statPoint;

            this.statStr = statStr;
            this.statInt = statInt;
            this.statDex = statDex;
            this.statVit = statVit;
            this.statEnd = statEnd;

            exp = 0;
            maxExp = CharacterStat.GetMaxEXP(level);
        }

        public BigInteger SumExp(BigInteger n, BigInteger m)
        {
            BigInteger _m = m, _n = n;
            BigInteger value = _m * ((2 * _m * _m) + (_m * (6 * _n - 3)) + (6 * _n * _n) - (6 * _n) + 19);
            return BigInteger.Divide(value, 6);
        }

        public void GainEXP(BigInteger amount)
        {
            Data.Ability.AbilitySet abilitySet = CharacterStat.GetInstance().GetAbilitySet();
            double expRate = Data.Ability.AbilityCollection.GetInstance().GetAbility("EXP").GetAmount(abilitySet, 0);

            exp += (BigInteger)((double)amount * (100 + expRate) / 100);

            if (exp >= maxExp)
            {
                LvUp();
            }
        }

        public void AddLv(BigInteger value)
        {
            Data.Ability.AbilitySet abilitySet = CharacterStat.GetInstance().GetAbilitySet();
            level += value;

            RefreshMaxExp();
            BigInteger statPointGain = 10 + (BigInteger)abilitySet.GetAbilityAmount("Additional Stat Point");
            statPoint += (value * statPointGain);

            Data.Character.CharacterClassData.AccumulateLevel(Entity.Character.GetInstance().GetCurrentClass(), value);

            BigInteger maxLevel = BigInteger.Parse(
                PlayerPrefs.GetString("MaxLevel", "0")
            );

            if (maxLevel < level)
                PlayerPrefs.SetString("MaxLevel", level.ToString());

            AutoStat();
            Entity.Character.GetInstance().SaveCharacterData();
        }

        public void LvUp()
        {

            Data.Ability.AbilitySet abilitySet = CharacterStat.GetInstance().GetAbilitySet();
            long levelUpAmount = 0;

            do
            {
                long levelToUp = 0;

                if (SumExp(GetLevel(), 10000000000) <= exp)
                {
                    levelToUp = 10000000000;
                }
                else if (SumExp(GetLevel(), 10000000000) <= exp)
                {
                    levelToUp = 100000000;
                }
                else if (SumExp(GetLevel(), 1000000) <= exp)
                {
                    levelToUp = 1000000;
                }
                else if (SumExp(GetLevel(), 10000) <= exp)
                {
                    levelToUp = 10000;
                }
                else if (SumExp(GetLevel(), 100) <= exp)
                {
                    levelToUp = 100;
                }
                else if (SumExp(GetLevel(), 1) <= exp)
                {
                    levelToUp = 1;
                }
                else break;

                BigInteger sumEXP = SumExp(GetLevel(), levelToUp);// TODO

                exp -= sumEXP;
                level += levelToUp;
                levelUpAmount += levelToUp;
            } while (true);

            RefreshMaxExp();

            
            long statPointGain = 10 + (long)abilitySet.GetAbilityAmount("Additional Stat Point");
            statPoint += (levelUpAmount * statPointGain);

            Data.Character.CharacterClassData.AccumulateLevel(Entity.Character.GetInstance().GetCurrentClass(), levelUpAmount);

            long maxLevel = long.Parse(
                PlayerPrefs.GetString("MaxLevel", "0")
            );

            if (maxLevel < level)
                PlayerPrefs.SetString("MaxLevel", level.ToString());

            AutoStat();
        }

        public long GetLvUpAmount(BigInteger gainedEXP)
        {
            Data.Ability.AbilitySet abilitySet = CharacterStat.GetInstance().GetAbilitySet();
            long levelUpAmount = 0;

            var _exp = new BigInteger(exp.ToByteArray());
            exp += gainedEXP;

            var _level = level;

            do
            {
                long levelToUp = 0;

                if (SumExp(_level, 10000000000) <= _exp)
                {
                    levelToUp = 10000000000;
                }
                else if (SumExp(_level, 10000000000) <= _exp)
                {
                    levelToUp = 100000000;
                }
                else if (SumExp(_level, 1000000) <= _exp)
                {
                    levelToUp = 1000000;
                }
                else if (SumExp(_level, 10000) <= _exp)
                {
                    levelToUp = 10000;
                }
                else if (SumExp(_level, 100) <= _exp)
                {
                    levelToUp = 100;
                }
                else if (SumExp(_level, 1) <= _exp)
                {
                    levelToUp = 1;
                }
                else break;

                BigInteger sumEXP = SumExp(_level, levelToUp);// TODO

                _exp -= sumEXP;
                _level += levelToUp;
                levelUpAmount += levelToUp;
            } while (true);

            return levelUpAmount;
        }

        public void AutoStat()
        {
            var autoStatManager = UI.Menu.AutoStatManager.GetInstance();
            var autoStatInfo = autoStatManager.GetRatio();

            int sum = autoStatInfo.GetSum();

            if (sum > 0)
            {

                BigInteger partOfPoint = statPoint / sum;

                BigInteger resultSTR = autoStatInfo.strRatio * partOfPoint;
                BigInteger resultINT = autoStatInfo.intRatio * partOfPoint;
                BigInteger resultDEX = autoStatInfo.dexRatio * partOfPoint;
                BigInteger resultVIT = autoStatInfo.vitRatio * partOfPoint;
                BigInteger resultEND = autoStatInfo.endRatio * partOfPoint;

                ApplyStat(resultSTR, resultINT, resultDEX, resultVIT, resultEND);
            }
        }

        public void ApplyStat(
            BigInteger statStr, BigInteger statInt, BigInteger statDex, BigInteger statVit, BigInteger statEnd)
        {
            this.statStr += statStr;
            this.statInt += statInt;
            this.statDex += statDex;
            this.statVit += statVit;
            this.statEnd += statEnd;

            statPoint -= (statStr + statInt + statDex + statVit + statEnd);

            Entity.Character.GetInstance().SaveCharacterData();
            Entity.Character.GetInstance().CharacterStatUpdate();
        }

        

        public void SetCurrentEXP(BigInteger amount) { exp = amount; }
        public void RefreshMaxExp() { maxExp = CharacterStat.GetMaxEXP(level); }

        public BigInteger GetLevel() { return level; }
        public BigInteger GetStatStr() { return statStr; }
        public BigInteger GetStatInt() { return statInt; }
        public BigInteger GetStatDex() { return statDex; }
        public BigInteger GetStatVit() { return statVit; }
        public BigInteger GetStatEnd() { return statEnd; }

        public BigInteger GetStatPoint() { return statPoint; }

        public BigInteger GetExp(){return exp; }
        public BigInteger GetMaxExp() { return maxExp; }
    }

    public class CharacterStat
    {
        private static CharacterStat instance;
        RawCharacterStat rawCharacterStat;
        Data.Ability.AbilitySet abilitySet;
        //Dictionary<KeyValuePair<String, AbilityCalcStrategy>, BigInteger> statData;

        public CharacterStat()
        {
            abilitySet = new Data.Ability.AbilitySet();
            //statData = new Dictionary<KeyValuePair<String, AbilityCalcStrategy>, BigInteger>();
            rawCharacterStat = new RawCharacterStat();
        }
        public Data.Ability.AbilitySet GetAbilitySet()
        {
            return abilitySet;
        }
        public void SetRawCharacterStat(RawCharacterStat rawCharacterStat)
        {
            this.rawCharacterStat = rawCharacterStat;
        }
        public RawCharacterStat GetRawCharacterStat()
        {
            return rawCharacterStat;
        }
        public static CharacterStat GetInstance()
        {
            if (instance == null)
                instance = new CharacterStat();

            return instance;
        }

        public static BigInteger GetMaxEXP(BigInteger level)
        {
            BigInteger result = level;
            result *= level;
            result += 3;
            return result;
        }
        public static void UpdateStat(
            RawCharacterStat rawCharacterStat,
            List<Item.ItemAbility> itemAbilities,
            List<Skill.SkillAbility> skillAbilities
        )
        {
            Data.Ability.AbilitySet abilitySet = GetInstance().GetAbilitySet();

            List<Data.Ability.AbilityData> abilities = new List<Data.Ability.AbilityData>();

            abilities.Add(new Data.Ability.AbilityData(
                Data.Ability.AbilityCollection.GetInstance().GetAbility("Strength"),
                (double)rawCharacterStat.GetStatStr()
            ));

            abilities.Add(new Data.Ability.AbilityData(
                Data.Ability.AbilityCollection.GetInstance().GetAbility("Intelligence"),
                (double)rawCharacterStat.GetStatInt()
            ));

            abilities.Add(new Data.Ability.AbilityData(
                Data.Ability.AbilityCollection.GetInstance().GetAbility("Dexiterity"),
                (double)rawCharacterStat.GetStatDex()
            ));

            abilities.Add(new Data.Ability.AbilityData(
                Data.Ability.AbilityCollection.GetInstance().GetAbility("Vitality"),
                (double)rawCharacterStat.GetStatVit()
            ));

            abilities.Add(new Data.Ability.AbilityData(
                Data.Ability.AbilityCollection.GetInstance().GetAbility("Endurance"),
                (double)rawCharacterStat.GetStatEnd()
            ));

            abilities.Add(new Data.Ability.AbilityData(
                Data.Ability.AbilityCollection.GetInstance().GetAbility("Stamina"),
                10
            ));

            abilities.Add(new Data.Ability.AbilityData(
                Data.Ability.AbilityCollection.GetInstance().GetAbility("Movement Speed"),
                4
            ));

            abilities.Add(new Data.Ability.AbilityData(
                Data.Ability.AbilityCollection.GetInstance().GetAbility("Critical Rate"),
                10
            ));

            abilities.Add(new Data.Ability.AbilityData(
                Data.Ability.AbilityCollection.GetInstance().GetAbility("Critical Damage"),
                10
            ));

            abilities.Add(new Data.Ability.AbilityData(
                Data.Ability.AbilityCollection.GetInstance().GetAbility("Avoid Rate"),
                5
            ));




            abilities.AddRange(itemAbilities.Select(x => { return x.GetAbility(); }));
            abilities.AddRange(skillAbilities.Select(x => { return x.GetAbility(); }));

            abilitySet.SetAbility(abilities);

            var currentClass = Entity.Character.GetInstance().GetCurrentClass();

            Dictionary<EnumEntityClass, string> entityClassToResultStat = new Dictionary<EnumEntityClass, string> {
                    { EnumEntityClass.Warrior,   "Melee Defence" },
                    { EnumEntityClass.Mage,      "Magical Defence" },
                    { EnumEntityClass.Archer,    "Ranged Defence" }
            };

            Dictionary<EnumEntityClass, string> entityClassToRequiredStat = new Dictionary<EnumEntityClass, string> {
                    { EnumEntityClass.Warrior,   "Strength" },
                    { EnumEntityClass.Mage,      "Intelligence" },
                    { EnumEntityClass.Archer,    "Dexiterity" }
            };
            Dictionary<Item.Weapon.WeaponType, EnumEntityClass> weaponTypeToClassStat = new Dictionary<Item.Weapon.WeaponType, EnumEntityClass> {
                    { Item.Weapon.WeaponType.Melee,   EnumEntityClass.Warrior},
                    { Item.Weapon.WeaponType.Staff,   EnumEntityClass.Mage},
                    { Item.Weapon.WeaponType.Ranged,  EnumEntityClass.Archer}
            };


            foreach (var e in entityClassToRequiredStat)
            {
                double statPoint = 1.5;
                if (e.Key == currentClass)
                    statPoint = 0.5;

                
                abilitySet.AddAbility(new Data.Ability.AbilityData(
                    Data.Ability.AbilityCollection.GetInstance().GetAbility(entityClassToResultStat[e.Key]),
                    statPoint * abilitySet.GetAbilityAmount(e.Value)
                ));
            }

            foreach (var e in weaponTypeToClassStat)
            {
                abilities.Add(new Data.Ability.AbilityData(
                    Data.Ability.AbilityCollection.GetInstance().GetAbility("Attack"),
                    (double)GetBaseAttack(
                        (Data.Item.Weapon)Entity.Character.GetInstance().GetCurrentEquipmentSet().GetItem("weapon"),
                        currentClass
                    )
                ));
            }


            abilitySet.SetAbility(abilities);

        }

        public static BigInteger GetBaseAttack(Item.Weapon currentWeapon, EnumEntityClass currentCharacterClass)
        {
            Data.Ability.AbilitySet abilitySet = GetInstance().GetAbilitySet();
            BigInteger baseStat = 5;

            if (currentWeapon != null)
            {
                var _str = (BigInteger)abilitySet.GetAbilityAmount("Strength");
                var _int = (BigInteger)abilitySet.GetAbilityAmount("Intelligence");
                var _dex = (BigInteger)abilitySet.GetAbilityAmount("Dexiterity");
                switch (currentWeapon.GetWeaponType())
                {
                    case Item.Weapon.WeaponType.Melee:
                        if (currentCharacterClass == EnumEntityClass.Warrior)
                            baseStat = _str + 5;
                        break;
                    case Item.Weapon.WeaponType.Staff:
                        if (currentCharacterClass == EnumEntityClass.Mage)
                            baseStat = _int + 5;
                        break;
                    case Item.Weapon.WeaponType.Ranged:
                        if (currentCharacterClass == EnumEntityClass.Archer)
                            baseStat = _dex + 5;
                        break;
                    case Item.Weapon.WeaponType.Any:
                        baseStat = _str + _int + _dex + 5;
                        break;
                }
            }

            return baseStat;
        }

        public static BigInteger GetMaxMana(BigInteger level)
        {
            Data.Ability.AbilitySet abilitySet = GetInstance().GetAbilitySet();
            var _int = (BigInteger)abilitySet.GetAbilityAmount("Intelligence");

            BigInteger result = (BigInteger)(10 + Math.Sqrt((double)((level/500) + (_int / 10000))));

            return result;
        }

        public static BigInteger GetRecoverRateByDamage()
        {
            Data.Ability.AbilitySet abilitySet = GetInstance().GetAbilitySet();
            var _recoverByDamage = (BigInteger)abilitySet.GetAbilityAmount("Recover by Damage");
            return _recoverByDamage;
        }

        public static BigInteger GetRecoverRateByHP()
        {
            Data.Ability.AbilitySet abilitySet = GetInstance().GetAbilitySet();
            var _recoverByHP = (BigInteger)abilitySet.GetAbilityAmount("Recover by HP");
            return _recoverByHP;
        }

        public static float GetDamageReductionRatio()
        {
            Data.Ability.AbilitySet abilitySet = GetInstance().GetAbilitySet();
            var rawStat = 100 - (BigInteger)abilitySet.GetAbilityAmount("Total Damage Reduction");
            if (rawStat < 10) rawStat = 10;
            return (float)rawStat / 100.0f;
        }

        public static float GetDefenceRatio(Mob.AbstractMob mob)
        {
            //long mobLV, bool isBoss, List<String> mobTagList, EnumEntityClass mobClass
            Data.Ability.AbilitySet abilitySet = GetInstance().GetAbilitySet();
            float defenceRatioRatio = 1.0f;

            if (abilitySet.GetAbilityAmount("Defence Formula Exponential") != 0)
            {
                defenceRatioRatio = (100 + (long)abilitySet.GetAbilityAmount("Defence Formula Exponential")) / 100.0f;
            }

            Dictionary<EnumEntityClass, String> classToRequiredStat = new Dictionary<EnumEntityClass, String> {
                { EnumEntityClass.Warrior,  "Melee Defence" },
                { EnumEntityClass.Mage,     "Magical Defence" },
                { EnumEntityClass.Archer,   "Ranged Defence" }
            };


            BigInteger targetStat = 0;
            var mobClass = mob.GetEntityClass();
            if (classToRequiredStat.ContainsKey(mobClass))
                targetStat += (BigInteger)abilitySet.GetAbilityAmount(classToRequiredStat[mobClass]);

            targetStat += (BigInteger)abilitySet.GetAbilityAmount("Defence");

            double defenceRequirement = Math.Pow((double)mob.GetLV(), 1.29f * defenceRatioRatio);
            defenceRequirement *= mob.GetDefReqRate();

            int resistCount = Entity.Character.GetInstance().GetResistCount(mob.GetTagList());

            for (int i = 0; i < resistCount; i++)
            {
                defenceRequirement *= 0.93f;
            }
            
            
            float defRatio = (float)Math.Log(
                ( ( (double) targetStat + 1000 ) * 5.5f / (double)defenceRequirement),
                2.5f
            ) * 10.0f;
            
            

            float maxDefRatio = 100.0f / (100.0f - (float)(90.0f + abilitySet.GetAbilityAmount("Additional Max Defence")));
            
            if (defRatio < 1) defRatio = 1;
            if (defRatio > maxDefRatio) defRatio = maxDefRatio;

            return defRatio;
        }

        public static double GetAbilityAmount(string abilityStr)
        {
            var _abilitySet = GetInstance().GetAbilitySet();
            return _abilitySet.GetAbilityAmount(abilityStr);
        }
    }
}
