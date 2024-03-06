using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InGame.UI;
using UnityEditor;

namespace InGame.Entity
{

    public class Altar : Signpost
    {
        [Serializable]
        public class ItemRequirement
        {
            [SerializeField] Data.Item.Item item;
            [SerializeField] int count;

            public Data.Item.Item GetItem() { return item; }
            public int GetCount() { return count; }
        }

        [SerializeField] List<ItemRequirement> itemRequirements;
        [SerializeField] long levelRequirement;
        [SerializeField] string customDescription;

        [SerializeField] List<GameObject> gameObjectsToActivate;
        [SerializeField] List<GameObject> gameObjectsToDeactivate;

        [SerializeField] string triggerID;

        [SerializeField] string actionDesc; // Something like : To (Summon ????), You need to sacrifice those items: 

        private void Reset()
        {
            triggerID = System.Guid.NewGuid().ToString();
        }

        private void Awake()
        {
            if(Data.SaveData.GameConditionSaveDataManager.GetCondition(triggerID))
            {
                Act();
                gameObject.SetActive(false);
            }
        }

        public override void Interact()
        {
            var modalWindowController = UI.ModalWindowController.GetInstnace();

            modalWindowController.SetContent(modal, signpostTitle);
            modalWindowController.SetSize(modalWidth, modalHeight);

            modalWindowController.GetContent().GetComponent<PentagramModalController>().SetPentagram(this);

            modalWindowController.OpenWindow();
        }

        public void Abort()
        {
            var modalWindowController = UI.ModalWindowController.GetInstnace();
            modalWindowController.CloseWindow();
        }

        public void Activate()
        {
            var modalWindowController = UI.ModalWindowController.GetInstnace();

            if (ValidateRequirement())
            {
                Sacrifice();
                Data.SaveData.GameConditionSaveDataManager.SetCondition(triggerID, true);
                modalWindowController.CloseWindow();
                gameObject.SetActive(false);
            }
        }

        public void Act()
        {
            foreach (var _gameObject in gameObjectsToDeactivate)
            {
                _gameObject.SetActive(false);
            }

            foreach (var _gameObject in gameObjectsToActivate)
            {
                _gameObject.SetActive(true);
            }
        }

        public bool ValidateRequirement()
        {
            bool isAvailable = true;

            foreach (var itemRequirement in itemRequirements)
            {
                int charItemCount =
                Data.Item.ItemCollection.GetInstance().allCollection[itemRequirement.GetItem().itemID].getCount();

                int neededCount = itemRequirement.GetCount();

                if (charItemCount < neededCount)
                {
                    isAvailable = false;
                    break;
                }
            }

            if (Entity.Character.GetInstance().GetLevel() < levelRequirement)
                isAvailable = false;

            return isAvailable;
        }

        void Sacrifice()
        {
            foreach (var itemRequirement in itemRequirements)
            {
                Data.Item.ItemCollection.GetInstance().RemoveItemCount(
                    itemRequirement.GetItem().itemID, 
                    itemRequirement.GetCount()
                );
            }
            Character.GetInstance().SaveCharacterData();
            Data.SaveData.InventorySaveDataManager.SaveInventoryData();

            Act();
        }

        public string GetActionDesc() { return actionDesc; }

        public string GetCustomDesc() { return customDescription; }

        public List<ItemRequirement> GetItemRequirements() { return itemRequirements; }

        public long GetLevelRequirement() { return levelRequirement; }

        public void SetTriggerID(string v)
        {
            triggerID = v;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Altar))]
    [CanEditMultipleObjects]
    public class AltarEditor : Editor
    {
        Altar altar;

        void OnEnable()
        {
            altar = (Altar)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Generate Random ID", GUILayout.Width(200), GUILayout.Height(20)))
            {
                altar.SetTriggerID(System.Guid.NewGuid().ToString());
                EditorUtility.SetDirty(altar);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}

