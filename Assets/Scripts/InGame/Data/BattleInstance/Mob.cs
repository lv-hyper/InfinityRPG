using InGame.Data.BattleEffect;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace InGame.Data.BattleInstance
{
    public class Mob : AbstractInstance{
        Data.Mob.AbstractMob mob;

        string name;
        BigInteger currentHealth;
        BigInteger gold;

        //float defenceRatio;

        System.Numerics.BigInteger exp;

        //Utility.SerializableDictionary<Skill.EnumSkillAbility, long> skillAbilities;
        //Utility.SerializableDictionary<string, double> skillAbilities;
        Ability.AbilitySet abilitySet;

        Dictionary<string, int> pendingTurnCount;

        public Mob(Data.Mob.AbstractMob mob)
        {
            abilitySet = new Ability.AbilitySet();
            ApplySkillAbilities(null);


            this.mob = mob;
            this.currentHealth = GetMaxHealth();
            this.gold = mob.GetGold();
            this.exp = mob.GetEXP();
            this.name = mob.GetName();

            pendingTurnCount = new Dictionary<string, int>();

        }

        public override void Attack(AbstractInstance instance)
        {

            BigInteger rawDamage = RollDamage();


            float sum = 0;

            for (var _elemental = EnumElemental.Air; _elemental <= EnumElemental.Count - 1; ++_elemental)
            {
                var diff = GetElementalDamage(_elemental) * (1 - (instance.GetElementalDefence(_elemental) / 100.0f));
                if (diff < 0) diff = 0;
                sum += diff;
            }

            double elementalBonus = sum / 100.0f;
            elementalBonus += 1;

            instance.Damage(
                this, 
                (BigInteger)(GetTotalElementalDamageBonus(instance) * (double)rawDamage), 
                false, 
                ""
            );
        }   

        public override void Damage(AbstractInstance instance, BigInteger amount, bool isCritical, string additionalInfo)
        {
            BigInteger realDamage = (BigInteger)(
                (double)amount / 
                mob.getDefenceRatio(
                    (float)Data.Character.CharacterStat.GetAbilityAmount("Defence Break"),
                    instance.GetEntityClass()
                )
            );

            //Debug.Log(amount * 100000000 / realDamage);

            currentHealth -= realDamage; 
            
            string damageText = "";

            if (additionalInfo != "")
                damageText += string.Format("{0} ", additionalInfo);

            damageText = string.Format("{0}{1:N0}", damageText, realDamage);

            if (isCritical)
                damageText = string.Format("*{0}*", damageText);

            UI.BattleDamageTextController.GetInstance().GenerateMobDamageText(damageText);
        }     

        public BigInteger GetHealth(){return currentHealth;}
        public override BigInteger GetMaxHealth(){
            return mob.GetHP();
            /*
            var vitStat = Ability.AbilityCollection.GetInstance().GetAbility("Vitality");
            BigInteger vit = mob.GetHP();

            vit = (BigInteger)vitStat.GetAmount(abilitySet, (double)vit);

            if (abilitySet.GetAbilities().ContainsKey("Vitality"))
                vit = (BigInteger)abilitySet.GetAbilities()["Vitality"].GetAmount(abilitySet);

            return vit;
            */
        }
        public BigInteger GetLV(){return mob.GetLV();}
        public BigInteger GetGold(){return gold;}
        public string GetName(){return name;}
        public System.Numerics.BigInteger GetExp(){return exp;}
        public List<string> GetTagList() { return mob.GetTagList(); }
        public bool IsBoss() { return mob.IsBoss(); }

        public int GetHealthBarCount()
        {
            if (!mob.IsBoss()) return 1;

            else
            {
                var _boss = (Data.Mob.Boss)mob;

                return 2 * _boss.GetSoulLevel() + 1;
            }
        }

        public void ApplySkillAbilities(Battle battle = null)
        {
            List<Skill.SkillAbility> allSkillAbility = Skill.SkillCollection.GetInstance().GetCurrentCharacterBuff(battle);
            abilitySet.SetAbility(allSkillAbility.Select(x => x.GetAbility()).ToList());
        }

        public override void SkillAttack(AbstractInstance instance, Skill.Skill skill)
        {
            Skill.EnemyActiveSkill _skill = skill as Skill.EnemyActiveSkill;

            _skill.Attack(this, instance);
            pendingTurnCount.Add(_skill.GetSkillID(), (int)_skill.GetSkillCoolDown());
        }

        public override void Recover(BigInteger damageAmount)
        {

        }

        public override void Buff(Buff buff)
        {

        }

        public override void Debuff(Debuff debuff)
        {

        }

        public override void RegenerateHP(BigInteger amount)
        {

        }

        public override void RegenerateMana(BigInteger amount)
        {

        }

        public override BigInteger GetMaxMana()
        {
            return 0;
        }

        public override bool GetCritical()
        {
            return false;
        }

        public override float GetCriticalDamage()
        {
            return 0;
        }

        public int GetSoulLv()
        {
            if(mob.IsBoss())
            {
                var _boss = (Data.Mob.Boss)mob;
                return _boss.GetSoulLevel();
            }    
            return 0;
        }

        public override BigInteger RollDamage()
        {   
            /*
            BigInteger rawDamage = mob.GetATK();
            var atkStat = Ability.AbilityCollection.GetInstance().GetAbility("Attack");
            BigInteger finalDamage = (BigInteger)atkStat.GetAmount(abilitySet, (double)rawDamage);
            Debug.Log(rawDamage + " ==== " + finalDamage);
            
            Debug.Log((BigInteger)atkStat.GetAmount(abilitySet, (double)rawDamage) * 100000000 / rawDamage);
            */

            return mob.GetATK() * Random.Range(90000000, 100000000) / 100000000;


            /* This code made monster attack also multiplied by player attack bonus
            BigInteger rawDamage = mob.GetATK() * Random.Range(90000000, 100000000) / 100000000; 
            var atkStat = Ability.AbilityCollection.GetInstance().GetAbility("Attack");

            Debug.Log(rawDamage + "\n====\n" + (BigInteger)atkStat.GetAmount(abilitySet, (double)rawDamage));

            return (BigInteger)atkStat.GetAmount(abilitySet, (double)rawDamage);
            */
        }

        public override EnumEntityClass GetEntityClass()
        {
            return mob.GetEntityClass();
        }

        public override float GetElementalDefence(EnumElemental elemental)
        {
            return mob.GetElementalDefence(elemental);
        }

        public override float GetElementalDamage(EnumElemental elemental)
        {
            return mob.GetElementalDamage(elemental);
        }

        public override float GetTotalElementalDamageBonus(AbstractInstance instance)
        {
            float sum = 0;

            for (var _elemental = EnumElemental.Air; _elemental <= EnumElemental.Count - 1; ++_elemental)
            {
                var diff = GetElementalDamage(_elemental) * (1 - (instance.GetElementalDefence(_elemental) / 100.0f));
                if (diff < 0) diff = 0;
                sum += diff;
            }

            float elementalBonus = sum / 100.0f;
            elementalBonus += 1;

            return elementalBonus;
        }

        public override float GetDefenceRatio(AbstractInstance instance)
        {
            return mob.getDefenceRatio(
                (float)Data.Character.CharacterStat.GetAbilityAmount("Defence Break"),
                instance.GetEntityClass()
            );
        }

        public Skill.EnemyActiveSkill GetSkill()
        {
            var enemySkills = mob.GetSkills();

            if(enemySkills != null)
                enemySkills.Sort((a, b) => { return a.GetSkillCoolDown().CompareTo(b.GetSkillCoolDown()); });

            return enemySkills.Where(x => !pendingTurnCount.ContainsKey(x.GetSkillID())).DefaultIfEmpty(null).Last();
        }

        public void ReduceCooldown()
        {
            var skillIDList = pendingTurnCount.Keys.ToList();

            foreach(var skillID in skillIDList)
            {
                pendingTurnCount[skillID]--;

                if (pendingTurnCount[skillID] <= 0)
                    pendingTurnCount.Remove(skillID);
            }
        }

        public Data.Mob.AbstractMob GetMobData() { return mob; }
    }
}