using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using InGame.Data;
using System.Numerics;
using Common.UI;

namespace InGame.UI.Menu
{

    public class StatDistributeMenu : MenuPage, IDisplayComponent
    {
        [SerializeField]
        TextMeshProUGUI statSpText,
        statStrText, statStrText2, statIntText, statIntText2,
        statDexText, statDexText2, statVitText, statVitText2,
        statEndText, statEndText2;

        BigInteger currentSP = 0, maxSP = 0;
        BigInteger statStrPoint = 0, statIntPoint = 0, statDexPoint = 0, statVitPoint = 0, statEndPoint = 0;
        BigInteger resultSTR, resultINT, resultDEX, resultVIT, resultEND;
        BigInteger totalPoint
        {
            get
            {
                return
                statStrPoint +
                statIntPoint +
                statDexPoint +
                statVitPoint +
                statEndPoint;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            StartCoroutine(RegisterToCharacter());
            ResetStatDistribution();
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
            if (subject.GetType() == typeof(Entity.Character))
            {
                Entity.Character character = (Entity.Character)subject;

                maxSP = character.GetSP();

                float spPercent = (float)currentSP / (float)maxSP * 100.0f;

                if (maxSP == 0) spPercent = 100.0f;

                statSpText.text = string.Format(
                    "{0:N0}/{1:N0} ({2}%)", currentSP, maxSP, spPercent
                );

                statStrText.text = string.Format("{0:N0}", character.GetSTR());
                statIntText.text = string.Format("{0:N0}", character.GetINT());
                statDexText.text = string.Format("{0:N0}", character.GetDEX());
                statVitText.text = string.Format("{0:N0}", character.GetVIT());
                statEndText.text = string.Format("{0:N0}", character.GetEND());
            }
        }

        public void ResetStatDistribution()
        {
            currentSP = 0;

            float spPercent = (float)currentSP / (float)maxSP * 100.0f;

            if (maxSP == 0) spPercent = 100.0f;

            statSpText.text = string.Format(
                "{0:N0}/{1:N0} ({2}%)", currentSP, maxSP, spPercent
            );

            statStrText2.text = "+0p (0)";
            statIntText2.text = "+0p (0)";
            statDexText2.text = "+0p (0)";
            statVitText2.text = "+0p (0)";
            statEndText2.text = "+0p (0)";

            resultSTR = 0; statStrPoint = 0;
            resultINT = 0; statIntPoint = 0;
            resultDEX = 0; statDexPoint = 0;
            resultVIT = 0; statVitPoint = 0;
            resultEND = 0; statEndPoint = 0;
        }

        public void ApplyStat()
        {
            var character = Entity.Character.GetInstance();

            Data.Character.CharacterStat.GetInstance().GetRawCharacterStat().ApplyStat(
                resultSTR, resultINT, resultDEX, resultVIT, resultEND
            );

            ResetStatDistribution();
        }

        void UpdateStatDistribution()
        {
            if(totalPoint == 0) return;

            BigInteger partOfPoint = (maxSP / totalPoint);

            resultSTR = statStrPoint * partOfPoint;
            resultINT = statIntPoint * partOfPoint;
            resultDEX = statDexPoint * partOfPoint;
            resultVIT = statVitPoint * partOfPoint;
            resultEND = statEndPoint * partOfPoint;

            currentSP = resultSTR+resultINT+resultDEX+resultVIT+resultEND;

            float spPercent = (float)currentSP / (float)maxSP * 100.0f;

            if (maxSP == 0) spPercent = 100.0f;

            statSpText.text = string.Format(
                "{0:N0}/{1:N0} ({2}%)", currentSP, maxSP, spPercent
            );

            statStrText2.text = string.Format("+{0:N0}p ({1})",resultSTR, statStrPoint);
            statIntText2.text = string.Format("+{0:N0}p ({1})",resultINT, statIntPoint);
            statDexText2.text = string.Format("+{0:N0}p ({1})",resultDEX, statDexPoint);
            statVitText2.text = string.Format("+{0:N0}p ({1})",resultVIT, statVitPoint);
            statEndText2.text = string.Format("+{0:N0}p ({1})",resultEND, statEndPoint);
        }

        public void AddSTR()
        {
            if (totalPoint < maxSP && totalPoint < 100)
                ++statStrPoint;
            UpdateStatDistribution();
        }
        public void AddINT()
        {
            if (totalPoint < maxSP && totalPoint < 100)
                ++statIntPoint;
            UpdateStatDistribution();
        }
        public void AddDEX()
        {
            if (totalPoint < maxSP && totalPoint < 100)
                ++statDexPoint;
            UpdateStatDistribution();
        }
        public void AddVIT()
        {
            if (totalPoint < maxSP && totalPoint < 100)
                ++statVitPoint;
            UpdateStatDistribution();
        }
        public void AddEND()
        {
            if (totalPoint < maxSP && totalPoint < 100)
                ++statEndPoint;
            UpdateStatDistribution();
        }


    }
}

