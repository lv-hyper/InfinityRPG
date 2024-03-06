using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.UI
{
    public class ElementalTreeElementView : MonoBehaviour
    {
        [SerializeField] ElementalTreeModalController controller;
        [SerializeField] Data.ElementalTreeElement element;
        [SerializeField] int tier, order;
        [SerializeField] UnityEngine.UI.Image mainImage;
        [SerializeField] GameObject unlocked;

        public void Init(ElementalTreeModalController _controller, Data.ElementalTreeElement _element, int _tier, int _order)
        {
            controller = _controller;
            element = _element;

            tier = _tier;
            order = _order;

            Refresh();
        }

        public int GetTier() { return tier; }
        public int GetOrder() { return order; }

        public bool DependencySatisfied()
        {
            bool result = true;

            var dependencies = element.GetDependency();

            foreach(var dependency in dependencies)
            {
                result = result && Data.ElementalSoulData.GetInstance().IsPerkUnlocked(
                    controller.GetCurrentElemental(),
                    new Data.SaveData.ElementalSoulSaveData.Perk(dependency.tier, dependency.order)
                );
            }
            
            return result;
        }

        public void Refresh()
        {
            mainImage.color = new Color(0.5f, 0.5f, 0.5f, 1);

            if (!DependencySatisfied())
            {
                GetComponent<UnityEngine.UI.Button>().interactable = false;
                mainImage.color = new Color(0.15f, 0.15f, 0.15f, 1);
            }
            else
            {
                GetComponent<UnityEngine.UI.Button>().interactable = true;
            }

            if (Data.ElementalSoulData.GetInstance().IsPerkUnlocked(
                controller.GetCurrentElemental(),
                new Data.SaveData.ElementalSoulSaveData.Perk(tier, order)
            )){
                unlocked.SetActive(true);
                //mainImage.color = new Color(0.75f, 0.75f, 0.75f, 1);
            }
            else
            {
                unlocked.SetActive(false);
            }
        }

        public void SelectElement()
        {
            controller.SetElement(this);
            mainImage.color = new Color(1, 1, 1, 1);
        }

        public Data.ElementalTreeElement GetElement() { return element; }


        public UnityEngine.UI.Image GetMainImage() { return mainImage; }

        public void DeselectElement()
        {
            Refresh();
        }
    }
}
