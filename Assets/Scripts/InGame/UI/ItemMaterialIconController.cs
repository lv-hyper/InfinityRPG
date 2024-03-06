using InGame.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace InGame.UI
{
    public class ItemMaterialIconController : MonoBehaviour, IDisplayComponent
    {
        Data.Item.CraftIngredient craftIngredient;
        [SerializeField] UnityEngine.UI.Image image;
        [SerializeField] TextMeshProUGUI status;
        [SerializeField] GameObject screenPrefab;
        [SerializeField] Transform descriptionAreaTransform;
        [SerializeField] GameObject descriptionArea;

        void Awake()
        {
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

        public Data.Item.CraftIngredient GetCraftIngredient() { return craftIngredient; }

        public void SetCraftIngredient(Data.Item.CraftIngredient ingredient)
        {
            craftIngredient = ingredient;

            image.sprite = craftIngredient.GetItem().GetSprite();

            RefreshUI();
        }

        public void ShowDescription()
        {
            descriptionAreaTransform = GameObject.Find("DescriptionArea").transform;
            
            foreach(Transform obj in descriptionAreaTransform)
            {
                Destroy(obj.gameObject);
            }
            
            GameObject descriptionScreen = Instantiate(screenPrefab);
            
            descriptionScreen.transform.SetParent(descriptionAreaTransform, false);
            descriptionScreen.transform.localScale = Vector3.one;

            descriptionScreen.GetComponent<MaterialDescriptionController>().Reload(craftIngredient.GetItem());

            descriptionScreen.SetActive(true);
        }

        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(Entity.Character))
            {
                RefreshUI();
            }
        }

        public void RefreshUI()
        {
            if(craftIngredient != null)
            {
                int charItemCount =
                Data.Item.ItemCollection.GetInstance().allCollection[craftIngredient.GetItem().itemID].getCount();
                int neededCount = craftIngredient.GetQuantity();
                status.text = String.Format("{0}/{1}", charItemCount, neededCount);

                if (charItemCount >= neededCount)
                {
                    status.color = Color.white;
                }
                else
                {
                    status.color = Color.red;
                }
            }            
        }

        private void OnDestroy()
        {
            if (Entity.Character.GetInstance() != null)
                Entity.Character.GetInstance().RemoveDisplayComponent(this);
        }
    }

}

