using InGame.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace InGame.UI
{

    public class ItemCraftBarController : MonoBehaviour, IDisplayComponent
    {
        [SerializeField] Data.Item.Craft craftTable;

        [SerializeField] UnityEngine.UI.Image icon;
        [SerializeField] TextMeshProUGUI text, descText;

        ItemCraftController itemCraftController;

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

        public void SetCraftTable(Data.Item.Craft craft)
        {
            craftTable = craft;

            icon.sprite = craft.GetResult().GetSprite();
            text.text = craft.GetResult().name;
            descText.text = craft.GetResult().shortDesc;
        }

        public void SetItemCraftController(ItemCraftController itemCraftController)
        {
            this.itemCraftController = itemCraftController;
        }

        public Data.Item.Craft GetCraft() { return craftTable; }

        public void Refresh(Subject subject)
        {
            if (subject.GetType() == typeof(Entity.Character))
            {

            }
        }

        public void Click() { 
            itemCraftController.SelectCraft(craftTable);
        }

        private void OnDestroy()
        {
            if (Entity.Character.GetInstance() != null)
                Entity.Character.GetInstance().RemoveDisplayComponent(this);
        }
    }

}

