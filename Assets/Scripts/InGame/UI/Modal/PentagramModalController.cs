using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InGame.Data;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public class PentagramModalController : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] TMPro.TextMeshProUGUI descText;
        [SerializeField] Transform contentTransform;
        [SerializeField] GameObject requirementElementPrefab;

        [SerializeField] GameObject activateButton;
 
        [SerializeField] Entity.Altar pentagram;
        private Entity.Character character;

        void Awake()
        {
            StartCoroutine(RegisterToCharacter());
            UI.ModalWindowController.GetInstnace().onWindowClose += (sender, args) =>
            {
                character.RemoveDisplayComponent(this);
            };
        }

        IEnumerator RegisterToCharacter()
        {
            while (true)
            {
                character = Entity.Character.GetInstance();
                if (character != null) break;
                yield return null;
            }

            character.AddDisplayComponent(this);
        }

        public void SetPentagram(Entity.Altar _pentagram)
        {
            pentagram = _pentagram;

            Refresh();
        }

        public void ClearContent()
        {
            foreach (Transform t in contentTransform)
            {
                Destroy(t.gameObject);
            }
        }

        public void Refresh(Subject subject = null)
        {
            ClearContent();
            if (pentagram != null)
            {
                if (pentagram.GetCustomDesc() != "")
                    descText.text = pentagram.GetCustomDesc();
                else
                {

                    descText.text =
                        string.Format("This is a runic altar that would {0}. ", pentagram.GetActionDesc());

                    if (pentagram.GetLevelRequirement() != 0)
                        descText.text += string.Format("You need to be Lv. {0:N0} to sacrifice item.", pentagram.GetLevelRequirement());
                }


                int i = 0;

                Vector2 v;

                var itemRequirements = pentagram.GetItemRequirements();

                foreach (var itemRequirement in itemRequirements)
                {
                    GameObject itemElement =
                        GameObject.Instantiate(requirementElementPrefab);

                    DontDestroyOnLoad(itemElement);

                    int charItemCount =
                    Data.Item.ItemCollection.GetInstance().allCollection[itemRequirement.GetItem().itemID].getCount();

                    itemElement.transform.SetParent(contentTransform, false);

                    itemElement.GetComponent<PentagramModalElement>().InitElement(
                        itemRequirement.GetItem(), charItemCount, itemRequirement.GetCount()
                    );

                    var rt = itemElement.GetComponent<RectTransform>();

                    v = rt.anchoredPosition;
                    v.x += (i * 120);
                    ++i;
                    rt.anchoredPosition = v;
                }

                v = contentTransform.GetComponent<RectTransform>().sizeDelta;
                v.x = 120 * i + 60;
                contentTransform.GetComponent<RectTransform>().sizeDelta = v;

                activateButton.GetComponent<Button>().interactable = pentagram.ValidateRequirement();
            }
            else
            {
                descText.text = "";
            }            
        }

        public void Activate() { pentagram.Activate(); }
        public void Abort() { pentagram.Abort(); }
    }
}
