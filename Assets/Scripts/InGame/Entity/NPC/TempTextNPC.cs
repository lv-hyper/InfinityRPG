using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InGame.Entity
{
    public class TempTextNPC : AbstractNPC
    {
        [SerializeField] GameObject modal;

        [TextArea(5, 10)]
        [SerializeField] string text;
        
        public override void OnAction()
        {
            CancelInvoke();
            PauseMovement();

            modal.GetComponent<TMPro.TextMeshProUGUI>().text = text;
            UI.ModalWindowController.GetInstnace().SetContent(modal, gameObject.name);
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
