using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Entity
{
    public class BasicNPC : AbstractNPC
    {
        [SerializeField] GameObject modal;
        
        public override void OnAction()
        {
            if(modal != null)
            {
                CancelInvoke();
                PauseMovement();

                UI.ModalWindowController.GetInstnace().SetContent(modal, gameObject.name);
                UI.ModalWindowController.GetInstnace().onWindowClose += new EventHandler((sender, args)=>{
                    OnFinishAction();
                });
                UI.ModalWindowController.GetInstnace().OpenWindow();
            }
        }

        public override void OnFinishAction()
        {
            Invoke("UnpauseMovement", 2);
        }
    }
}
