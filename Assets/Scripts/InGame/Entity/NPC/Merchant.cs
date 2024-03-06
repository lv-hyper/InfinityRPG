using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Entity
{
    public class Merchant : AbstractNPC
    {
        [SerializeField] GameObject itemCraftModal;
        [SerializeField] List<Data.Item.Craft> itemCraftTable;
        
        public override void OnAction()
        {
            CancelInvoke();
            PauseMovement();

            itemCraftModal.GetComponent<UI.ItemCraftController>().SetCraftTable(itemCraftTable);
            UI.ModalWindowController.GetInstnace().SetContent(itemCraftModal, "Item Craft");
            UI.ModalWindowController.GetInstnace().onWindowClose += new EventHandler((sender, args)=>{
                OnFinishAction();
            });
            UI.ModalWindowController.GetInstnace().OpenWindow();
        }

        public override void OnFinishAction()
        {
            Invoke("UnpauseMovement", 2);
        }
    }
}
