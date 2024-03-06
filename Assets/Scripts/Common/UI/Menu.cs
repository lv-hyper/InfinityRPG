using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Common.UI
{
    public class Menu : MenuComponent
    {
        protected List<MenuComponent> menus;
        [SerializeField] private Transform pagesTransform;
        
        MenuComponent openedMenu;
        
        protected void Awake()
        {
            menus = new List<MenuComponent>();
            var childTabs = pagesTransform.GetComponentsInChildren<MenuComponent>(true);
            Debug.Log(childTabs.Length);
            menus.AddRange(childTabs);
            openedMenu = menus.FirstOrDefault(tab => tab.gameObject.activeSelf);
        }

        public override void OpenChild(string menuName)
        {
            string[] args = menuName.Split('/');

            string preFix = args[0];
            string postFix = "";
            
            if (args.Length > 1)
            {
                postFix = args.Aggregate((strA, strB) => strA + "/" + strB);
            }
            
            var matchingMenu = menus.FirstOrDefault(menu => menu.Name == preFix);
            
            if (openedMenu != null)
            {
                openedMenu.Close();
            }

            if (matchingMenu != null)
            {
                matchingMenu.Open();
                if (postFix != "")
                {
                    matchingMenu.OpenChild(postFix);
                }
            }
            
            openedMenu = matchingMenu;
        }

        public override void Open()
        {
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
        
        public override string Name => gameObject.name;
    }
}