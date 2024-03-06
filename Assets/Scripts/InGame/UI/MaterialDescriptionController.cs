using System.Collections;
using System.Collections.Generic;
using InGame.Data.Item;
using InGame.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Numerics;

public class MaterialDescriptionController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemName, priceText, buyMaxButtonText, itemCount, itemShortDesc, itemPartDesc;
    InGame.Entity.Character character;
    [SerializeField] Button buyButton, buyMaxButton;
    [SerializeField] GameObject buyButtonObject, buyMaxButtonObject;
    [SerializeField] UnityEngine.UI.Image image;
    string id;
    Item item_loaded;

    private void Awake() {
        character = InGame.Entity.Character.GetInstance();
    }

    public void Reload(Item item)
    {
        item_loaded = item;
        id = item.itemID;
        var collectionItem = ItemCollection.GetInstance().allCollection[id];
        int count = collectionItem.getCount();
        int maxCount = collectionItem.getMaxCount();
        //var collectionItem = this.GetComponent<InventoryController>().GetItemCollection()[id];

        image.sprite = item.itemSprite;
        itemCount.text = string.Format("{0}/{1}", count, maxCount);
        itemName.text = item.name;
        itemPartDesc.text = item.type;
        itemShortDesc.text = item.shortDesc;
        //Basically button is set actived and interactable
        if(count >= maxCount)
        {
            priceText.text = "Price : Maxed Out";
            buyButtonObject.SetActive(false);
            buyMaxButtonObject.SetActive(false); 
        }

        else if(item.price > 0)
        {
            priceText.text = string.Format("Price : {0:N0}G", item.price);
            buyMaxButtonText.text = string.Format("Buy x {0}", GetMaxBuy(item.price, character.GetGold(), count, maxCount));
            Debug.Log(string.Format("{0} {1}", character.GetGold(), item.price));

            if(character.GetGold() < item.price)
            {                    
                buyButton.interactable = false;
                buyMaxButton.interactable = false;  
            }
        }
        else
        {
            priceText.text = "Price : Not for sale";
            buyButtonObject.SetActive(false);
            buyMaxButtonObject.SetActive(false);          
        }
    }

    public void BuyItem()
    {

        long itemPrice = GetItem().price;
        if(character.GetGold() >= itemPrice)
        {
            character.LoseGold(itemPrice);
            ItemCollection.GetInstance().AddItemCount(id);
            InGame.Data.SaveData.InventorySaveDataManager.SaveInventoryData();
            character.SaveCharacterData();
            character.ApplyItemAbilities();
        }

        Reload(item_loaded);
    }

    public void BuyMaxItem()
    {
        var item = ItemCollection.GetInstance().allCollection[id];
        long itemPrice = GetItem().price;
        BigInteger buyCount = GetMaxBuy(itemPrice, character.GetGold(), item.getCount(), item.getMaxCount());
        if(character.GetGold() >= itemPrice * buyCount)
        {
            character.LoseGold(itemPrice * buyCount);
            ItemCollection.GetInstance().AddItemCount(id, (int)buyCount);
            InGame.Data.SaveData.InventorySaveDataManager.SaveInventoryData();
            character.SaveCharacterData();
            character.ApplyItemAbilities();
        }
        Reload(item_loaded);
    }

    public BigInteger GetMaxBuy(long itemPrice, BigInteger playerGold, int currentCount, int maxCount)
    {
        if(itemPrice == 0) return 0;

        var GoldMaxBuy = playerGold / new BigInteger(itemPrice);
        var quantityMaxBuy = new BigInteger(maxCount - currentCount);
        return BigInteger.Min(GoldMaxBuy, quantityMaxBuy);
    }

    public Item GetItem(){return ItemCollection.GetInstance().allCollection[id].getItem();}
}
