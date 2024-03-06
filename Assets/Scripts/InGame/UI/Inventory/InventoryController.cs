using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using InGame.Data.Item;
using UnityEngine.UI;
using InGame.Data;

namespace InGame.UI
{
    public class InventoryController : MonoBehaviour
    {
        Dictionary<string, ItemCollection.ItemStatus> itemCollection;
        List<GameObject> itemList;
        Dictionary<string, float> prevScrollPosition;

        List<AbstractInventorySortingStrategy> allInventorySortingStrategies;
        List<AbstractInventoryFilteringStrategy> ElementInventoryFilteringStrategies;
        List<AbstractInventoryFilteringStrategy> WeaponInventoryFilteringStrategies;
        List<AbstractInventoryFilteringStrategy> AccessoryInventoryFilteringStrategies;
        List<AbstractInventoryFilteringStrategy> availableElementInventoryFilteringStrategies;
        List<AbstractInventorySortingStrategy> availableInventorySortingStrategies;
        List<AbstractInventoryFilteringStrategy> availableSpecificInventoryFilteringStrategies;
        List<AbstractInventoryFilteringStrategy> specificInventoryFilteringStrategies;
        List<AbstractInventoryFilteringStrategy> BaseInventoryFilteringStrategies;
        List<AbstractInventoryFilteringStrategy> InventoryShowingStrategies;
        List<AbstractInventoryFilteringStrategy> availableInventoryShowingStrategies;
        int currentElementFilteringStrategyIndex;
        int currentSortingStrategyIndex;
        int currentSpecificFilteringStrategyIndex;
        bool isAscending = true;
        int currentShowStrategyIndex = 0;

        string currentChoice;
        string lastSpecificFiltered = "";

        [SerializeField] GameObject itemPrefab;
        [SerializeField] GameObject inventoryContent;
        [SerializeField] TMPro.TextMeshProUGUI currentStatusText;
        [SerializeField] TMPro.TextMeshProUGUI elementFilteringButtonText, sortingButtonText, specificFilteringButtonText, orderingButtonText, showAllButtonText;
        [SerializeField] Button specificFilterButton;

        [SerializeField] ShortSetItemInformationController setItemInformation;
        [SerializeField] GameObject inventory;

        private void Awake() {
            
            string _choice = currentChoice;

            if (_choice.StartsWith("accessory"))
                _choice = "accessory";
            
            if(itemCollection == null)
                itemCollection = new Dictionary<string, ItemCollection.ItemStatus>();

            itemList = new List<GameObject>();
            prevScrollPosition = new Dictionary<string, float>();

            ElementInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>{ 
                new DefaultInventoryFilteringStrategy(),
                new WaterElementFilteringStrategy(),
                new FireElementFilteringStrategy(),
                new NatureElementFilteringStrategy(),
                new AirElementFilteringStrategy()
            };

            allInventorySortingStrategies = new List<AbstractInventorySortingStrategy> {
                new DefaultInventorySortingStrategy(),
                new OrderByNameSortingStrategy(),
                new OrderByZoneSortingStrategy(),
                new OrderByAbilityStrategy(_choice)
            };

            WeaponInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>{ 
                new DefaultInventoryFilteringStrategy(),
                    new SwordFilteringStrategy(),
                    new StaffFilteringStrategy(),
                    new BowFilteringStrategy()
            };

            AccessoryInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>{
                new DefaultInventoryFilteringStrategy(),
                new ExpFilteringStrategy(),
                new LifeStealFilteringStrategy(),
                new StatFilteringStrategy(),
                new DropRateFilteringStrategy(),
                new ResistFilteringStrategy(),
                new ElementFilteringStrategy(),
                new GoldFilteringStrategy(),
                new OtherFilteringStrategy(),
                new PassiveFilteringStrategy()
            };

            InventoryShowingStrategies = new List<AbstractInventoryFilteringStrategy>{
                new ShowingAllStrategy(),
                new ShowingStaredStrategy(),
                new ShowingBuyableStrategy()
            };
                

            BaseInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>
            {
                new DefaultInventoryFilteringStrategy()
            };

            

            availableElementInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>();
            availableInventorySortingStrategies = new List<AbstractInventorySortingStrategy>();
            availableSpecificInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>();
            availableInventoryShowingStrategies = new List<AbstractInventoryFilteringStrategy>();

            currentShowStrategyIndex = PlayerPrefs.GetInt("showAll");
            isAscending = PlayerPrefs.GetInt("sortingOrder") == 1;
            currentElementFilteringStrategyIndex = PlayerPrefs.GetInt("element");
            currentSortingStrategyIndex = PlayerPrefs.GetInt("sortingOptionNum");

            Debug.Log(currentShowStrategyIndex);
        }

        private void OnEnable()
        {
            if(itemCollection == null)
            {
                itemCollection = new Dictionary<string, ItemCollection.ItemStatus>();

                itemList = new List<GameObject>();
                prevScrollPosition = new Dictionary<string, float>();

                ElementInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>{
                    new DefaultInventoryFilteringStrategy(),
                    new WaterElementFilteringStrategy(),
                    new FireElementFilteringStrategy(),
                    new NatureElementFilteringStrategy(),
                    new AirElementFilteringStrategy()
                };

                allInventorySortingStrategies = new List<AbstractInventorySortingStrategy> {
                    new DefaultInventorySortingStrategy(),
                    new OrderByNameSortingStrategy(),
                    new OrderByZoneSortingStrategy()
                };

                WeaponInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>{ 
                    new DefaultInventoryFilteringStrategy(),
                    new SwordFilteringStrategy(),
                    new StaffFilteringStrategy(),
                    new BowFilteringStrategy()
                };

                AccessoryInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>{
                    new DefaultInventoryFilteringStrategy(),
                    new ExpFilteringStrategy(),
                    new LifeStealFilteringStrategy(),
                    new StatFilteringStrategy(),
                    new DropRateFilteringStrategy(),
                    new ResistFilteringStrategy(),
                    new ElementFilteringStrategy(),
                    new GoldFilteringStrategy(),
                    new OtherFilteringStrategy(),
                    new PassiveFilteringStrategy()
                };

                InventoryShowingStrategies = new List<AbstractInventoryFilteringStrategy>{
                    new ShowingAllStrategy(),
                    new ShowingStaredStrategy(),
                    new ShowingBuyableStrategy()
                };

                BaseInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>
                {
                    new DefaultInventoryFilteringStrategy()
                };

                availableElementInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>();
                availableInventorySortingStrategies = new List<AbstractInventorySortingStrategy>();
                availableSpecificInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>();
                availableInventoryShowingStrategies = new List<AbstractInventoryFilteringStrategy>();
            }


            string _choice = currentChoice;

            if (_choice.StartsWith("accessory"))
                _choice = "accessory";

            if (!(_choice.StartsWith("weapon") || _choice.StartsWith("accessory")))
                specificFilterButton.interactable = false;
            else specificFilterButton.interactable = true;


            if (prevScrollPosition.ContainsKey(_choice))
            {

                var scrollPos = inventoryContent.GetComponent<RectTransform>().anchoredPosition;
                scrollPos.y = prevScrollPosition[_choice];

                inventoryContent.GetComponent<RectTransform>().anchoredPosition = scrollPos;
            }
            else
            {
                var scrollPos = inventoryContent.GetComponent<RectTransform>().anchoredPosition;
                scrollPos.y = 0;

                inventoryContent.GetComponent<RectTransform>().anchoredPosition = scrollPos;
            }
        }

        public void OnDisable()
        {
            float additionalPos = 0;
            string _choice = currentChoice;

            if (_choice.StartsWith("accessory"))
                _choice = "accessory";

            foreach (var item in itemList)
            {
                Vector2 pos = item.GetComponent<RectTransform>().anchoredPosition;

                if (inventoryContent.GetComponent<RectTransform>().anchoredPosition.y - 300 > -pos.y)
                {
                    ItemController itemController = item.GetComponent<ItemController>();
                    if (itemController.IsDescActive())
                    {
                        additionalPos += 500;
                    }
                }
            }

            
            prevScrollPosition[_choice] = 
                inventoryContent.GetComponent<RectTransform>().anchoredPosition.y - additionalPos;
        }

        public void ActivateInventory(string choice)
        {
            currentChoice = choice;
            Debug.Log(currentChoice);

            gameObject.SetActive(true);

            availableElementInventoryFilteringStrategies =
                ElementInventoryFilteringStrategies.Where(x => x.IsApplicable(choice)).ToList();

            availableInventorySortingStrategies =
                allInventorySortingStrategies.Where(x => x.IsApplicable(choice)).ToList();

            availableInventoryShowingStrategies = 
                InventoryShowingStrategies.Where(x => x.IsApplicable(choice)).ToList();

            if (choice.StartsWith("weapon"))
            {
                if (!lastSpecificFiltered.StartsWith("weapon")) lastSpecificFiltered = "weapon";
                availableSpecificInventoryFilteringStrategies =
                    WeaponInventoryFilteringStrategies.Where(x => x.IsApplicable(choice)).ToList();
                var classString = PlayerPrefs.GetString("CharacterClassType", EnumEntityClass.Warrior.ToString());

                switch(classString)
                {
                    case "Warrior":
                        currentSpecificFilteringStrategyIndex = 1;
                        break;
                    case "Mage":
                        currentSpecificFilteringStrategyIndex = 2;
                        break;
                    case "Archer":
                        currentSpecificFilteringStrategyIndex = 3;
                        break;
                    //if error occurs, it will be base value(showing All)
                }
            }

            else if(choice.StartsWith("accessory"))
            {
                if (!lastSpecificFiltered.StartsWith("accessory"))
                {
                    lastSpecificFiltered = "";
                    currentSpecificFilteringStrategyIndex = 0;
                }
                availableSpecificInventoryFilteringStrategies =
                    AccessoryInventoryFilteringStrategies.Where(x => x.IsApplicable(choice)).ToList();
            }

            else availableSpecificInventoryFilteringStrategies = new List<AbstractInventoryFilteringStrategy>();//Not actually needed, just to avoid an error

            RefreshData();

            InitDisplay();
        }

        public void RefreshData()
        {
            itemCollection = new Dictionary<string, ItemCollection.ItemStatus>(
                   ItemCollection.GetInstance().GetCollection(currentChoice)
            );

            string _choice = currentChoice;

            if (_choice.StartsWith("accessory"))
                _choice = "accessory";            

            if (availableElementInventoryFilteringStrategies.Count > 0)
                itemCollection = availableElementInventoryFilteringStrategies[currentElementFilteringStrategyIndex]
                    .FilterItemCollection(itemCollection);

            if (availableInventorySortingStrategies.Count > 0)
                itemCollection = availableInventorySortingStrategies[currentSortingStrategyIndex]
                    .SortItemCollection(itemCollection, _choice, isAscending);

            if (availableSpecificInventoryFilteringStrategies.Count > 0)
                itemCollection = availableSpecificInventoryFilteringStrategies[currentSpecificFilteringStrategyIndex]
                    .FilterItemCollection(itemCollection);

            if (availableInventoryShowingStrategies.Count > 0)
                itemCollection = availableInventoryShowingStrategies[currentShowStrategyIndex]
                    .FilterItemCollection(itemCollection);
        }

        public void InitDisplay()
        {
            itemList.Clear();

            foreach(Transform transform in inventoryContent.transform)
            {
                Destroy(transform.gameObject);
            }

            foreach(var item in itemCollection.Values)
            {
                GameObject itemInstance = GameObject.Instantiate(itemPrefab, inventoryContent.transform);
                itemInstance.GetComponent<ItemController>().SetItem(item.getItem().itemID, this);

                itemList.Add(itemInstance);
            }

            RefreshDisplay();
        }

        public void RefreshDisplay()
        {
            int y = -30;

            long ownItemCount = 0;
            foreach(var item in itemList)
            {
                Vector2 pos = item.GetComponent<RectTransform>().anchoredPosition;
                pos.y = y;

                item.GetComponent<RectTransform>().anchoredPosition = pos;

                ItemController itemController = item.GetComponent<ItemController>();

                if(!itemController.IsDescActive())
                {
                    y-=200;
                }
                else
                {
                    y-=700;
                }

                if(itemCollection[itemController.GetItem().itemID].getCount() > 0)
                    ++ownItemCount;

                itemController.UpdateDisplay();
            }

            float xSize = inventoryContent.GetComponent<RectTransform>().sizeDelta.x;
            inventoryContent.GetComponent<RectTransform>().sizeDelta = new Vector2(xSize, -y);

            

            currentStatusText.text = string.Format(
                "Item : {0:N0}/{1:N0}, Gold : {2:N0}G", 
                ownItemCount, itemCollection.Count, Entity.Character.GetInstance().GetGold()
            );


            if (PlayerPrefs.GetString("passcode", "").ToLower() == "another")
                currentStatusText.text = "[Another] "+currentStatusText.text;

            elementFilteringButtonText.text = "Element : "+availableElementInventoryFilteringStrategies[currentElementFilteringStrategyIndex].GetName();
            sortingButtonText.text = "Sort : " + availableInventorySortingStrategies[currentSortingStrategyIndex].GetName();

            
            if (availableSpecificInventoryFilteringStrategies.Count == 0) specificFilteringButtonText.text = "Filter : All";
            else specificFilteringButtonText.text = "Filter : "+availableSpecificInventoryFilteringStrategies[currentSpecificFilteringStrategyIndex].GetName();

            orderingButtonText.text = "Order : "+ (isAscending ? "ascend" : "descend");
            showAllButtonText.text = "Show : " + availableInventoryShowingStrategies[currentShowStrategyIndex].GetName();
        }

        public void OpenInventory(
            Dictionary<string, ItemCollection.ItemStatus> itemCollection)
        {
            this.itemCollection = itemCollection;

            gameObject.SetActive(true);

            InitDisplay();
        }

        public void SelectItem(Item item)
        {
            Entity.Character character = Entity.Character.GetInstance();
            character.GetCurrentEquipmentSet().SetItem(currentChoice, item);
            Entity.Character.GetInstance().SaveEquipmentData();
            RefreshDisplay();
        }
        public void ChangeElementFilteringStrategy()
        {
            currentElementFilteringStrategyIndex++;
            currentElementFilteringStrategyIndex %= availableElementInventoryFilteringStrategies.Count;

            var scrollPos = inventoryContent.GetComponent<RectTransform>().anchoredPosition;
            scrollPos.y = 0;
            inventoryContent.GetComponent<RectTransform>().anchoredPosition = scrollPos;
            
            PlayerPrefs.SetInt("element", currentElementFilteringStrategyIndex);

            RefreshData();
            InitDisplay();
        }

        public void ChangeSortingStrategy()
        {
            currentSortingStrategyIndex++;
            currentSortingStrategyIndex %= availableInventorySortingStrategies.Count;

            var scrollPos = inventoryContent.GetComponent<RectTransform>().anchoredPosition;
            scrollPos.y = 0;
            inventoryContent.GetComponent<RectTransform>().anchoredPosition = scrollPos;

            PlayerPrefs.SetInt("sortingOptionNum", currentSortingStrategyIndex);

            RefreshData();
            InitDisplay();
        }

        public void ChangeSpecificFilteringStrategy()
        {
            lastSpecificFiltered = currentChoice;
            
            currentSpecificFilteringStrategyIndex++;
            currentSpecificFilteringStrategyIndex %= availableSpecificInventoryFilteringStrategies.Count;

            var scrollPos = inventoryContent.GetComponent<RectTransform>().anchoredPosition;
            scrollPos.y = 0;
            inventoryContent.GetComponent<RectTransform>().anchoredPosition = scrollPos;

            RefreshData();
            InitDisplay();
        }

        public void ChangeOrder()
        {
            isAscending = !isAscending;

            PlayerPrefs.SetInt("sortingOrder", isAscending ? 1 : 0);

            RefreshData();
            InitDisplay();
        }

        public void ChangeShowingStrategy()
        {
            currentShowStrategyIndex++;
            currentShowStrategyIndex %= availableInventoryShowingStrategies.Count;
            PlayerPrefs.SetInt("ShowAll", currentShowStrategyIndex);

            var scrollPos = inventoryContent.GetComponent<RectTransform>().anchoredPosition;
            scrollPos.y = 0;
            inventoryContent.GetComponent<RectTransform>().anchoredPosition = scrollPos;

            RefreshData();
            InitDisplay();
        }

        public void Close()
        {
            setItemInformation.Refresh();
            inventory.SetActive(false);
        }

        public Dictionary<string, ItemCollection.ItemStatus> GetItemCollection(){return itemCollection;}

        public string GetCurrentChoice(){return currentChoice;}
    }

}
