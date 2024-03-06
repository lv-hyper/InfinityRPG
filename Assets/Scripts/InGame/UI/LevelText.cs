using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGame.Data;
using TMPro;
using System.Numerics;

namespace InGame.UI
{
    public class LevelTextDataModel
    {
        public System.Numerics.BigInteger level { get; set; }
        public System.Numerics.BigInteger currentEXP { get; set; }
        public System.Numerics.BigInteger maxEXP { get; set; }

        public LevelTextDataModel(BigInteger level, BigInteger currentEXP, BigInteger maxEXP)
        {
            this.level = level;
            this.currentEXP = currentEXP;
            this.maxEXP = maxEXP;
        }
    }

    public class LevelText : MonoBehaviour, IDisplayComponent
    {
        public enum LevelTextDisplayMode
        {
            CharacterLevelMode,
            ClassLevelMode,
            DisplayModeCount
        }

        [SerializeField] LevelTextDisplayMode currentDisplayMode;
        [SerializeField] UnityEngine.UI.Image expBar;
        [SerializeField] TMPro.TextMeshProUGUI expText;

        Subject subject;

        void Awake()
        {
            currentDisplayMode = LevelTextDisplayMode.CharacterLevelMode;

            StartCoroutine(RegisterToCharacter());
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

        public void Transition()
        {
            switch(currentDisplayMode)
            {
                case LevelTextDisplayMode.CharacterLevelMode:
                    currentDisplayMode = LevelTextDisplayMode.ClassLevelMode;
                    break;
                case LevelTextDisplayMode.ClassLevelMode:
                    currentDisplayMode = LevelTextDisplayMode.CharacterLevelMode;
                    break;
            }
            Refresh(subject);
        }


        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(Entity.Character))
            {
                Entity.Character character = (Entity.Character)subject;

                LevelTextDataModel model = null;

                switch (currentDisplayMode)
                {
                    case LevelTextDisplayMode.CharacterLevelMode:
                        model = new LevelTextDataModel(
                            character.GetLevel(),
                            character.GetEXP(),
                            character.GetMaxEXP()
                        );
                        break;
                    case LevelTextDisplayMode.ClassLevelMode:
                        var _class = character.GetCurrentClass();
                        model = new LevelTextDataModel(
                            Data.Character.CharacterClassData.GetCharacterClassLevel(_class),
                            Data.Character.CharacterClassData.GetAccumulatedCharacterLevel(_class),
                            Data.Character.CharacterClassData.LevelUpRequirement(
                                Data.Character.CharacterClassData.GetCharacterClassLevel(_class)
                            )
                        );
                        break;
                }
                Display(model);
                this.subject = subject;
            }
        }

        public void Display(LevelTextDataModel model)
        {
            GetComponent<TextMeshProUGUI>().text =
            string.Format(
                "Lv. {0:N0}",
                model.level
            );

            if (PlayerPrefs.GetString("passcode", "").ToLower() == "another")
                GetComponent<TextMeshProUGUI>().text = "A" + GetComponent<TextMeshProUGUI>().text;

            expText.text = string.Format(
                "{0}/{1}",
                Utility.MinimizeNumText.Minimize(model.currentEXP),
                Utility.MinimizeNumText.Minimize(model.maxEXP)
            );
            
            expBar.fillAmount = (float)System.Numerics.BigInteger.Divide(model.currentEXP * 1000, model.maxEXP) / 1000.0f;
        }

        private void OnDestroy() {
            if(Entity.Character.GetInstance() != null)
                Entity.Character.GetInstance().RemoveDisplayComponent(this);
        }
    }
}
