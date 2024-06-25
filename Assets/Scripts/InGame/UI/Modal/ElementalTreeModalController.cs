using InGame.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Common.UI;
using InGame.Data.SaveData;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;

namespace InGame.UI
{
    public class ElementalTreeModalController : MenuPage, IDisplayComponent
    {
        [SerializeField] Transform orbitTransform, lineTransform, elementTransform;
        [SerializeField] GameObject orbitPrefab, linePrefab, elementPrefab;

        [SerializeField] Image mainIconImage;

        [SerializeField] Image waterElementalImage, fireElementalImage, natureElementalImage, airElementalImage;

        [SerializeField] List<GameObject> orbitInstances, lineInstances, elementInstances;

        [SerializeField] ElementalTree currentElementalTree, waterTree, fireTree, natureTree, airTree;

        [SerializeField] TMPro.TextMeshProUGUI titleText, descText, soulCountText;

        [SerializeField] Button unlockButton;

        [SerializeField] ElementalTreeElementView currentElementView;

        private void Awake()
        {
            currentElementalTree = waterTree;

            orbitInstances = new List<GameObject>();
            lineInstances = new List<GameObject>();
            elementInstances = new List<GameObject>();


            var elementalSoulData = Data.ElementalSoulData.GetInstance();
            elementalSoulData.AddDisplayComponent(this);

            Refresh(elementalSoulData);
        }
        private void OnEnable()
        {
            ResetModal();
        }

        public void RemovePrefabs()
        {
            foreach(var orbit in orbitInstances)
            {
                Destroy(orbit);
            }
            foreach (var line in lineInstances)
            {
                Destroy(line);
            }
            foreach(var element in elementInstances)
            {
                Destroy(element);
            }

            orbitInstances.Clear();
            lineInstances.Clear();
            elementInstances.Clear();
        }
        public void ResetModal()
        {
            if (currentElementView != null)
            {
                currentElementView.DeselectElement();
                currentElementView = null;
            }
            RemovePrefabs();
            mainIconImage.sprite = currentElementalTree.mainIcon;

            if (!Data.ElementalSoulData.GetInstance().GetElementalSoulData().ContainsKey(currentElementalTree.GetElemental()))
                mainIconImage.color = new Color(0.5f, 0.5f, 0.5f);

            else if (Data.ElementalSoulData.GetInstance().GetElementalSoulData()[currentElementalTree.GetElemental()].GetGainedSoul() <= 0)
                mainIconImage.color = new Color(0.5f, 0.5f, 0.5f);

            else mainIconImage.color = Color.white;


            for (int i = 0; i < currentElementalTree.GetTierCount(); ++i)
            {
                var tier = currentElementalTree.GetTier(i);

                var orbitInstance = Instantiate(orbitPrefab, orbitTransform, false);
                orbitInstance.GetComponent<OrbitView>().SetRadius(tier.radius);
                orbitInstance.GetComponent<OrbitView>().SetColor(currentElementalTree.orbitColor);
                orbitInstances.Add(orbitInstance);

                float angle = Mathf.Deg2Rad * tier.angleOffset;

                for (int elementIndex = 0; elementIndex < tier.elements.Count; ++elementIndex)
                {
                    var elementInstance = Instantiate(elementPrefab, elementTransform, false);

                    elementInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        Mathf.Cos(angle) * (tier.radius),
                        Mathf.Sin(angle) * (tier.radius)
                    );

                    elementInstance.GetComponent<ElementalTreeElementView>().Init(this, tier.elements[elementIndex], i, elementIndex);

                    switch (tier.elements[elementIndex].GetCategory())
                    {
                        case ElementalTreeElement.Category.None:
                            elementInstance.GetComponent<ElementalTreeElementView>().GetMainImage().sprite = currentElementalTree.mainIcon;
                            break;
                        case ElementalTreeElement.Category.Offensive:
                            elementInstance.GetComponent<ElementalTreeElementView>().GetMainImage().sprite = currentElementalTree.offensiveSkillIcon;
                            break;
                        case ElementalTreeElement.Category.DamagePassive:
                            elementInstance.GetComponent<ElementalTreeElementView>().GetMainImage().sprite = currentElementalTree.damagePassiveIcon;
                            break;
                        case ElementalTreeElement.Category.Defensive:
                            elementInstance.GetComponent<ElementalTreeElementView>().GetMainImage().sprite = currentElementalTree.defensiveSkillIcon;
                            break;
                        case ElementalTreeElement.Category.DefencePassive:
                            elementInstance.GetComponent<ElementalTreeElementView>().GetMainImage().sprite = currentElementalTree.defencePassiveIcon;
                            break;
                        case ElementalTreeElement.Category.Supportive:
                            elementInstance.GetComponent<ElementalTreeElementView>().GetMainImage().sprite = currentElementalTree.utilitySkillIcon;
                            break;
                    }

                    elementInstances.Add(elementInstance);

                    var elementDependencies = tier.elements[elementIndex].GetDependency();

                    foreach (var elementDependency in elementDependencies)
                    {
                        var dependencyTier = currentElementalTree.GetTier(elementDependency.tier);
                        float dependencyAngle = Mathf.Deg2Rad * dependencyTier.angleOffset;
                        dependencyAngle += (2 * Mathf.PI / dependencyTier.elements.Count * elementDependency.order);

                        var lineInstance = Instantiate(linePrefab, lineTransform, false);
                        lineInstance.GetComponent<UILineRenderer>().Points = new Vector2[]{
                            elementInstance.GetComponent<RectTransform>().anchoredPosition,
                            new Vector2(
                                Mathf.Cos(dependencyAngle) * dependencyTier.radius,
                                Mathf.Sin(dependencyAngle) * dependencyTier.radius
                            )
                        };
                        lineInstance.GetComponent<UILineRenderer>().color = currentElementalTree.lineCoilor;
                        lineInstances.Add(lineInstance);
                    }

                    angle += 2 * Mathf.PI / tier.elements.Count;
                }

            }
            RefreshDescription();
        }

        public void RefreshModal()
        {
            foreach(var element in elementInstances)
            {
                element.GetComponent<ElementalTreeElementView>().Refresh();
            }
        }


        public void SetElementalTree(string elemental)
        {
            waterElementalImage.color = new Color(0.5f, 0.5f, 0.5f);
            fireElementalImage.color = new Color(0.5f, 0.5f, 0.5f);
            natureElementalImage.color = new Color(0.5f, 0.5f, 0.5f);
            airElementalImage.color = new Color(0.5f, 0.5f, 0.5f);
            switch (elemental)
            {
                case "water":
                    currentElementalTree = waterTree;
                    waterElementalImage.color = new Color(1, 1, 1);
                    break;
                case "fire":
                    currentElementalTree = fireTree;
                    fireElementalImage.color = new Color(1, 1, 1);
                    break;
                case "nature":
                    currentElementalTree = natureTree;
                    natureElementalImage.color = new Color(1, 1, 1);
                    break;
                case "air":
                    currentElementalTree = airTree;
                    airElementalImage.color = new Color(1, 1, 1);
                    break;
                default:
                    break;
            }
            ResetModal();
        }

        public void SetElement(ElementalTreeElementView elementView)
        {
            if(currentElementView != null)
                currentElementView.DeselectElement();

            currentElementView = elementView;
            RefreshDescription();
        }

        public void RefreshDescription()
        {
            if(currentElementView != null)
            {
                var element = currentElementView.GetElement();
                titleText.text = string.Format("<nobr>{0} {1}</nobr> ",
                    element.GetSkill().name,
                    ToRoman(element.GetUpgradeCount())
                );

                descText.text = currentElementView.GetElement().GetSkill().GetLongDescription(
                    currentElementView.GetElement().GetUpgradeCount()
                );
                
                unlockButton.interactable = Data.ElementalSoulData.GetInstance().IsPerkUnlockable(
                    currentElementalTree.GetElemental(),
                    new Data.SaveData.ElementalSoulSaveData.Perk(currentElementView.GetTier(), currentElementView.GetOrder())
                );
            }
            else
            {
                titleText.text = "Description";
                descText.text = "";
                unlockButton.interactable = false;
            }

            soulCountText.text = String.Format(
                "[{0}] {1:N0}/{2:N0}",
                Data.ElementalSoulData.GetInstance().GetCurrentPreset(GetCurrentElemental()).GetPresetName(),
                Data.ElementalSoulData.GetInstance().GetUsedSoulCount(GetCurrentElemental()),
                Data.ElementalSoulData.GetInstance().GetGainedSoulCount(GetCurrentElemental())
            );
        }

        public void UnlockPerk()
        {
            if(unlockButton.interactable)
            {
                Data.ElementalSoulData.GetInstance().UnlockPerk(
                    currentElementalTree.GetElemental(),
                    new Data.SaveData.ElementalSoulSaveData.Perk(currentElementView.GetTier(), currentElementView.GetOrder())
                );
                Data.Skill.SkillCollection.GetInstance().SetSkillUpgradeCount(
                    currentElementView.GetElement().GetSkill().GetSkillID(),
                    currentElementView.GetElement().GetUpgradeCount()
                );

                Data.SaveData.SkillSaveDataManager.SaveSkillData();
                Entity.Character.GetInstance().ApplySkillAbilities();

                RefreshDescription();
                RefreshModal();
            }
        }

        public void SelectPreset(string name)
        {
            var _elemental = currentElementalTree.GetElemental();
            Data.Skill.SkillCollection.GetInstance().ClearElementalSkill(_elemental);
            Data.ElementalSoulData.GetInstance().SetPreset(_elemental, name);

            var preset = Data.ElementalSoulData.GetInstance().GetCurrentPreset(_elemental);

            foreach(var perk in preset.GetPerks())
            {
                var skillElement = currentElementalTree.GetTier(perk.tier).elements[perk.order];

                var currentSkillUpgradeCount = 
                    Data.Skill.SkillCollection.GetInstance().allSkillCollection[skillElement.GetSkill().GetSkillID()].GetCount();
                var targetCount = skillElement.GetUpgradeCount();

                if(targetCount > currentSkillUpgradeCount)
                {
                    Data.Skill.SkillCollection.GetInstance().SetSkillUpgradeCount(
                        skillElement.GetSkill().GetSkillID(),
                        targetCount
                    );
                }
            }

            Data.SaveData.SkillSaveDataManager.SaveSkillData();
            Entity.Character.GetInstance().ApplySkillAbilities();

            RefreshDescription();
            ResetModal();

        }

        public void ResetPerk()
        {
            var _elemental = currentElementalTree.GetElemental();
            Data.ElementalSoulData.GetInstance().ClearElementalSoulData(_elemental);
            Data.Skill.SkillCollection.GetInstance().ClearElementalSkill(_elemental);

            Data.SaveData.SkillSaveDataManager.SaveSkillData();
            Entity.Character.GetInstance().ApplySkillAbilities();

            ResetModal();
        }

        public EnumElemental GetCurrentElemental()
        {
            return currentElementalTree.GetElemental();
        }

        private string ToRoman(int value)
        {
            if (value > 3999) return value.ToString();

            else
            {
                string result = "";

                int m = value / 1000;
                value %= 1000;

                for (int count = 0; count < m; ++count) result += "M";

                int d = value / 500;
                value %= 500;

                int c = value / 100;
                value %= 100;

                if (c == 4)
                {
                    if (d == 1)
                        result += "CM";
                    else
                        result += "CD";
                }
                else
                {
                    for (int count = 0; count < d; ++count) result += "D";
                    for (int count = 0; count < c; ++count) result += "C";
                }

                int l = value / 50;
                value %= 50;

                int x = value / 10;
                value %= 10;

                if (x == 4)
                {
                    if (l == 1)
                        result += "XC";
                    else
                        result += "XL";
                }
                else
                {
                    for (int count = 0; count < l; ++count) result += "L";
                    for (int count = 0; count < x; ++count) result += "X";
                }

                int v = value / 5;
                value %= 5;

                int i = value;

                if (i == 4)
                {
                    if (v == 1)
                        result += "IX";
                    else
                        result += "IV";
                }
                else
                {
                    for (int count = 0; count < v; ++count) result += "V";
                    for (int count = 0; count < i; ++count) result += "I";
                }

                return result;
            }
        }

        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(ElementalSoulData))
            {
                var data = (ElementalSoulData)subject;
                ResetModal();
            }

        }
    }

}
