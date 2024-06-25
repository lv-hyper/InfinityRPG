using InGame.Data.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.Data
{
    public class BattleBuffController : MonoBehaviour
    {
        List<BattleEffectStatus> characterBuffList, characterDebuffList;
        List<BattleEffectStatus> mobBuffList, mobDebuffList;

        [SerializeField] Transform characterEffectList, mobEffectList;
        [SerializeField] GameObject characterBuffPrefab, characterDebuffPrefab;
        [SerializeField] GameObject mobBuffPrefab, mobDebuffPrefab;

        static BattleBuffController instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
                characterBuffList = new List<BattleEffectStatus>();
                characterDebuffList = new List<BattleEffectStatus>();
                mobBuffList = new List<BattleEffectStatus>();
                mobDebuffList = new List<BattleEffectStatus>();
            }
        }

        public static BattleBuffController GetInstance()
        {
            return instance;
        }

        public void AddCharacterBuff(BattleEffect.Buff buff, int turnCount)
        {
            characterBuffList.Add(new BattleEffectStatus(buff, turnCount));
            RefreshUI();
        }

        public void AddCharacterDebuff(BattleEffect.Debuff debuff, int turnCount)
        {
            characterDebuffList.Add(new BattleEffectStatus(debuff, turnCount));
            RefreshUI();
        }

        public void ClearCharacterBattleEffect()
        {
            characterBuffList.Clear();
            characterDebuffList.Clear();
            RefreshUI();
        }

        public void Turn()
        {
            for(int i=0; i<characterBuffList.Count; ++i)
            {
                var buff = characterBuffList[i];
                buff.ReduceTurn();
                characterBuffList[i] = buff;

                if (buff.GetTurn() == 0)
                {
                    characterBuffList.RemoveAt(i);
                    --i;
                }
            }

            for (int i = 0; i < characterDebuffList.Count; ++i)
            {
                var debuff = characterDebuffList[i];
                debuff.ReduceTurn();
                characterDebuffList[i] = debuff;

                if (debuff.GetTurn() == 0)
                {
                    characterDebuffList.RemoveAt(i);
                    --i;
                }
            }
            RefreshUI();
        }

        public void OnEnd()
        {
            characterBuffList.Clear();
            characterDebuffList.Clear();
            RefreshUI();
        }

        public void RefreshUI()
        {
            foreach(Transform t in characterEffectList)
            {
                Destroy(t.gameObject);
            }

            int i = 0;

            foreach (var e in characterBuffList)
            {
                GameObject characterBuffIcon = Instantiate(characterBuffPrefab, characterEffectList, false);
                //DontDestroyOnLoad(characterBuffIcon);

                //characterBuffPrefab.transform.SetParent(characterEffectList, false);

                var v = characterBuffIcon.GetComponent<RectTransform>().anchoredPosition;
                v.x += 100*i;
                ++i;
                characterBuffIcon.GetComponent<RectTransform>().anchoredPosition = v;

                characterBuffIcon.GetComponent<BattleEffectIcon>().GetImage().sprite = e.effect.GetIcon();
                if(e.GetTurn() != -1)
                {
                    characterBuffIcon.GetComponent<BattleEffectIcon>().GetTurnImage().fillAmount = (float)e.GetTurn() / e.effect.duration;
                    characterBuffIcon.GetComponent<BattleEffectIcon>().GetTurn().text = e.GetTurn().ToString();
                }
                else
                {
                    characterBuffIcon.GetComponent<BattleEffectIcon>().GetTurnImage().fillAmount = 0;
                    characterBuffIcon.GetComponent<BattleEffectIcon>().GetTurn().text = "";
                }
            }
            foreach (var e in characterDebuffList)
            {
                GameObject characterDebuffIcon = Instantiate(characterDebuffPrefab, characterEffectList, false);
                //DontDestroyOnLoad(characterDebuffIcon);

                //characterBuffPrefab.transform.SetParent(characterEffectList, false);

                var v = characterDebuffIcon.GetComponent<RectTransform>().anchoredPosition;
                v.x += 100 * i;
                ++i;
                characterDebuffIcon.GetComponent<RectTransform>().anchoredPosition = v;

                characterDebuffIcon.GetComponent<BattleEffectIcon>().GetImage().sprite = e.effect.GetIcon();

                if(e.GetTurn() != -1)
                {
                    characterDebuffIcon.GetComponent<BattleEffectIcon>().GetTurnImage().fillAmount = (float)e.GetTurn() / e.effect.duration;
                    characterDebuffIcon.GetComponent<BattleEffectIcon>().GetTurn().text = e.GetTurn().ToString();
                }
                else
                {
                    characterDebuffIcon.GetComponent<BattleEffectIcon>().GetTurnImage().fillAmount = 0;
                    characterDebuffIcon.GetComponent<BattleEffectIcon>().GetTurn().text = "";
                }
            }
        }

        public List<SkillAbility> GetCurrentCharacterBuff()
        {
            List<SkillAbility> buffAbilities = new List<SkillAbility>();

            foreach (var element in characterBuffList)
            {
                buffAbilities.AddRange(element.effect.GetAbilities());
            }

            return buffAbilities;
        }

        public List<SkillAbility> GetCurrentCharacterDebuff()
        {
            List<SkillAbility> debuffAbilities = new List<SkillAbility>();

            foreach (var element in characterDebuffList)
            {
                debuffAbilities.AddRange(element.effect.GetAbilities());
            }

            return debuffAbilities;
        }
    }
    public struct BattleEffectStatus
    {
        [SerializeField] public BattleEffect.BattleEffect effect;
        [SerializeField] public int turnLeft;

        public BattleEffectStatus(BattleEffect.BattleEffect buff, int turnCount)
        {
            this.effect = buff;
            this.turnLeft = turnCount;
        }

        public void ReduceTurn() { 
            if(turnLeft != -1)
                --turnLeft; 
        }
        public int GetTurn() { return turnLeft; }
    }
}

