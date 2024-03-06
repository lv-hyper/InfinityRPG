using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using InGame.Data.Item;
using InGame.Data;
using System;
using Gpm.LogViewer.Internal;
using System.Numerics;
using Unity.Mathematics;
using InGame.Data.Item.Group;
using InGame.Data.Item.Armor;
using UnityEngine.UI;

namespace InGame.UI
{
    public class ItemController : MonoBehaviour
    {
#if UNITY_EDITOR
        static bool debug = true;
#else
        static bool debug = false;
#endif

        string itemID;
        InventoryController inventoryController;
        [SerializeField] UnityEngine.UI.Image image;
        [SerializeField] TextMeshProUGUI itemCount, itemTitle, itemShortDesc;

        [SerializeField] GameObject itemDesc;
        [SerializeField] TextMeshProUGUI itemDescContent;
        [SerializeField] UnityEngine.UI.Button selectButton, buyButton, buyMaxButton;
        [SerializeField] GameObject buyButtonObject, buyMaxButtonObject;
        [SerializeField] TextMeshProUGUI selectButtonText, priceText, buyButtonText, buyMaxButtonText;
        [SerializeField] Sprite noItemSprite;
        [SerializeField] Image starButton;
        [SerializeField] Sprite starSprite, unStarSprite;

        Entity.Character character;

        private void Awake() {
            character = Entity.Character.GetInstance();
        }

        public void SetItem(string itemID, InventoryController inventory)
        {
            this.itemID = itemID;
            inventoryController = inventory;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            var itemStatus = inventoryController.GetItemCollection()[itemID];
            var item = itemStatus.getItem();

            image.sprite = item.itemSprite;

            itemCount.text = string.Format("{0}/{1}",itemStatus.getCount(),itemStatus.getMaxCount());

            if(itemStatus.getCount() == itemStatus.getMaxCount())
            {
                GetComponent<UnityEngine.UI.Outline>().effectColor = Color.yellow;
            }
            else
            {
                GetComponent<UnityEngine.UI.Outline>().effectColor = Color.white;
            }

            itemTitle.text = itemStatus.name;
            itemShortDesc.text = item.shortDesc;

            string longDesc = item.longDesc;

            if(longDesc != "")
                longDesc+="\n";

            if (item.GetAdditionalInfo() != "")
                longDesc += (item.GetAdditionalInfo() + "\n");
            

            var itemSetList = ItemSetCollection.GetInstance().GetItemSetByItemID(item.itemID);

            if (itemSetList is not null && itemSetList.Count > 0)
            {
                longDesc += "This item is part of ";
                
                foreach (var itemSet in itemSetList)
                {
                    longDesc += $"Set {itemSet.name}, ";
                }

                longDesc = longDesc[..^2] + ".";
            }

            if(longDesc != "")
                longDesc += "\n";

            var itemAbility = item.GetItemAbility();

            for(int i=0;i<item.itemAbilities.Count;++i)
            {
                if (item.itemAbilities[i].GetAmount() == 0) continue;

                var ampifiedAbility = itemAbility[i];
                var originalAbility = item.itemAbilities[i];

                if (ampifiedAbility.GetAmount() != originalAbility.GetAmount())
                    longDesc += (
                        originalAbility.GetAbilityInStringExtended(
                            ampifiedAbility.GetAmount() - originalAbility.GetAmount()
                        ) + "\n"
                    );

                else
                    longDesc += (itemAbility[i].GetAbilityInString() + "\n");
            }

            foreach(var tag in item.GetTagResists())
            {
                longDesc += ("Resist : " + tag + "\n");
            }

            longDesc = longDesc.Replace("+-", "-");
            

            itemDescContent.text = longDesc;

            int wornCount = character.GetCurrentEquipmentSet().GetWornCount(itemStatus.getItem());
            int maxWornCount = itemStatus.getItem().GetMaxWornCount();

            if(maxWornCount > itemStatus.getCount()) maxWornCount = itemStatus.getCount();
            long price = itemStatus.getItem().price;

            if(itemStatus.getCount() == 0 && !(PlayerPrefs.GetString("passcode", "").ToLower() == "spoiler")) //Test purpose
            {
                selectButton.interactable = false;
                selectButtonText.text = "No Item";

                if(price <= 0)
                {
                    image.sprite = noItemSprite;
                    itemTitle.text = "????";
                    itemShortDesc.text = "????";
                    itemDescContent.text = "This item is not obtained. You probably can get this item by some action.";

                    itemCount.text = "0/?";
                }
            }
            else if(inventoryController.GetCurrentChoice() == "all")
            {
                selectButton.interactable = false;
                selectButtonText.text = "Disabled";
            }
            else if(wornCount >= maxWornCount)
            {
                selectButton.interactable = false;
                selectButtonText.text = string.Format("Select ({0}/{1})", wornCount, maxWornCount);
            }
            else
            {
                selectButton.interactable = true;
                if(wornCount > 0)
                {
                    selectButtonText.text = string.Format("Select ({0}/{1})", wornCount, maxWornCount);
                }
                else
                {
                    selectButtonText.text = "Select";
                }
            }


            //Basically button is set actived and interactable
            if(itemStatus.getMaxCount() <= itemStatus.getCount())
            {
                priceText.text = "Price : Maxed Out";
                buyButtonObject.SetActive(false);
                buyMaxButtonObject.SetActive(false); 
            }

            else if(price > 0)
            {
                priceText.text = string.Format("Price : {0:N0}G", price);
                buyMaxButtonText.text = string.Format("Buy x {0}", GetMaxBuy(price, character.GetGold(), itemStatus.getCount(), itemStatus.getMaxCount()));

                if(character.GetGold() < price)
                {
                    if (!debug)
                    {
                        buyButton.interactable = false;
                        buyMaxButton.interactable = false;  
                    }
                }
            }
            else
            {
                priceText.text = "Price : Not for sale";
                if (!debug)
                {
                    buyButtonObject.SetActive(false);
                    buyMaxButtonObject.SetActive(false);                    
                }
            }
            
            if (Data.Item.ItemCollection.GetInstance().allCollection[GetItem().itemID].isStared()) starButton.GetComponent<Image>().sprite = starSprite;
            else starButton.GetComponent<Image>().sprite = unStarSprite;
        }

        public void ToggleDesc()
        {
            if(IsDescActive())
                itemDesc.SetActive(false);
            else
                itemDesc.SetActive(true);
            
            inventoryController.RefreshDisplay();
        }

        public bool IsDescActive(){return itemDesc.activeSelf;}

        public void SelectItem()
        {
            inventoryController.SelectItem(GetItem());
        }

        public void BuyItem()
        {

            long itemPrice = GetItem().price;
            if(character.GetGold() >= itemPrice)
            {
                character.LoseGold(itemPrice);
                ItemCollection.GetInstance().AddItemCount(itemID);
                Data.SaveData.InventorySaveDataManager.SaveInventoryData();
                character.SaveCharacterData();
                character.ApplyItemAbilities();
            }

            inventoryController.RefreshDisplay();
        }

        public void BuyMaxItem()
        {
            var item = inventoryController.GetItemCollection()[itemID];
            long itemPrice = GetItem().price;
            BigInteger buyCount = GetMaxBuy(itemPrice, character.GetGold(), item.getCount(), item.getMaxCount());
            if(character.GetGold() >= itemPrice * buyCount)
            {
                character.LoseGold(itemPrice * buyCount);
                ItemCollection.GetInstance().AddItemCount(itemID, (int)buyCount);
                Data.SaveData.InventorySaveDataManager.SaveInventoryData();
                character.SaveCharacterData();
                character.ApplyItemAbilities();
            }
            inventoryController.RefreshDisplay();
        }

        public BigInteger GetMaxBuy(long itemPrice, BigInteger playerGold, int currentCount, int maxCount)
        {
            if(itemPrice == 0) return 0;

            var GoldMaxBuy = playerGold / new BigInteger(itemPrice);
            var quantityMaxBuy = new BigInteger(maxCount - currentCount);
            return BigInteger.Min(GoldMaxBuy, quantityMaxBuy);
        }

        public void ThrowItem()
        {
            if (Data.Item.ItemCollection.GetInstance().allCollection[GetItem().itemID].getCount() > 0)
            {
                Entity.Character.GetInstance().OnThrowItem(GetItem());
                Data.Item.ItemCollection.GetInstance().RemoveItemCount(GetItem().itemID, 1);
                Data.SaveData.InventorySaveDataManager.SaveInventoryData();
                Entity.Character.GetInstance().SaveEquipmentData();
                Entity.Character.GetInstance().ApplyItemAbilities();
                inventoryController.RefreshDisplay();
            }
        }

        public void ToggleStarItem()
        {
            if (Data.Item.ItemCollection.GetInstance().allCollection[GetItem().itemID].isStared()) UnStarItem();
            else StarItem();
        }

        public void StarItem()
        {
            Data.Item.ItemCollection.GetInstance().allCollection[GetItem().itemID].setStared();
            starButton.GetComponent<Image>().sprite = starSprite;
            inventoryController.RefreshDisplay();
        }

        public void UnStarItem()
        {
            Data.Item.ItemCollection.GetInstance().allCollection[GetItem().itemID].setUnStared();
            starButton.GetComponent<Image>().sprite = unStarSprite;
            inventoryController.RefreshDisplay();
        }

        public Item GetItem(){return inventoryController.GetItemCollection()[itemID].getItem();}
    }
}