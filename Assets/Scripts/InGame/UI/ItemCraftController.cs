using InGame.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace InGame.UI
{
    public class ItemCraftController : MonoBehaviour, IDisplayComponent
    {
        Data.Item.Craft currentCraft = null;

        [SerializeField] List<Data.Item.Craft> craftTables;

        [SerializeField] GameObject itemButtonPrefab, materialIconPrefab;
        [SerializeField] Transform craftableTransform, materialTransform;

        [SerializeField] UnityEngine.UI.Button craftButton;

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

        public void SetCraftTable(List<Data.Item.Craft> crafts)
        {
            craftTables = crafts;

        }

        public void ClearUI()
        {
            ClearCraft();
            ClearMaterial();
        }

        public void ClearCraft()
        {
            foreach (Transform t in craftableTransform.transform)
            {
                Destroy(t.gameObject);
            }
        }

        public void ClearMaterial()
        {
            foreach (Transform t in materialTransform.transform)
            {
                Destroy(t.gameObject);
            }
        }

        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(Entity.Character))
            {
            }
        }

        public bool ValidateCurrentCraft()
        {
            bool isAvailable = true;


            foreach (var e in currentCraft.GetCraftIngredients())
            {
                int charItemCount =
                Data.Item.ItemCollection.GetInstance().allCollection[e.GetItem().itemID].getCount();

                int neededCount = e.GetQuantity();

                if (charItemCount < neededCount)
                {
                    isAvailable = false;
                    break;
                }
            }

            if (Data.Item.ItemCollection.GetInstance().allCollection[currentCraft.GetResult().itemID].getCount() + 1 >
                Data.Item.ItemCollection.GetInstance().allCollection[currentCraft.GetResult().itemID].getMaxCount())
                isAvailable = false;



            return isAvailable;
        }

        public void CraftItem()
        {
            if (!ValidateCurrentCraft()) return;

            foreach(var e in currentCraft.GetCraftIngredients())
            {
                Data.Item.ItemCollection.GetInstance().RemoveItemCount(e.GetItem().itemID, e.GetQuantity());
            }

            Data.Item.ItemCollection.GetInstance().AddItemCount(currentCraft.GetResult().itemID, 1);
            Data.SaveData.InventorySaveDataManager.SaveInventoryData();
            Entity.Character.GetInstance().SaveEquipmentData();
            Entity.Character.GetInstance().ApplyItemAbilities();

            SelectCraft(currentCraft);
        }

        public void SelectCraft(Data.Item.Craft craft)
        {
            currentCraft = craft;
            ClearMaterial();

            craftButton.interactable = ValidateCurrentCraft();

            int i = 0;

            Vector2 v;

            foreach (var e in currentCraft.GetCraftIngredients())
            {
                GameObject materialIcon =
                    GameObject.Instantiate(materialIconPrefab);

                DontDestroyOnLoad(materialIcon);


                materialIcon.transform.SetParent(materialTransform, false);
                materialIcon.GetComponent<ItemMaterialIconController>().
                    SetCraftIngredient(e);

                var rt = materialIcon.GetComponent<RectTransform>();
                
                v = rt.anchoredPosition;
                v.x += (i * 120);
                ++i;
                rt.anchoredPosition = v;
            }

            v = materialTransform.GetComponent<RectTransform>().sizeDelta;
            v.x = 120 * i + 60;
            materialTransform.GetComponent<RectTransform>().sizeDelta = v;



        }

        private void OnDestroy()
        {
            if (Entity.Character.GetInstance() != null)
                Entity.Character.GetInstance().RemoveDisplayComponent(this);
        }

        private void OnEnable()
        {
            int i = 0;
            Vector2 v;

            ClearUI();

            foreach (var e in craftTables)
            {
                GameObject itemButton =
                    GameObject.Instantiate(itemButtonPrefab);

                DontDestroyOnLoad(itemButton);


                itemButton.transform.SetParent(craftableTransform, false);
                itemButton.GetComponent<ItemCraftBarController>().SetCraftTable(e);
                itemButton.GetComponent<ItemCraftBarController>().SetItemCraftController(this);

                itemButton.transform.localScale = Vector3.one;

                var rt = itemButton.GetComponent<RectTransform>();

                rt.offsetMin = new Vector2(20, rt.offsetMin.y);
                rt.offsetMax = new Vector2(-20, rt.offsetMax.y);

                v = rt.anchoredPosition;
                v.y = -20 - (120 * i);
                ++i;
                itemButton.GetComponent<RectTransform>().anchoredPosition = v;


            }
            v = craftableTransform.GetComponent<RectTransform>().sizeDelta;
            v.y = 120 * i + 20;

            craftableTransform.GetComponent<RectTransform>().sizeDelta = v;

        }
    }
}

