using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace InGame.Entity
{
    public class Signpost : MonoBehaviour
    {
        [SerializeField] protected GameObject modal;
        [SerializeField] protected string signpostTitle;

        [SerializeField] protected int modalWidth = 800, modalHeight = 600;

        public virtual void Interact()
        {
            UI.ModalWindowController.GetInstnace().SetContent(modal, signpostTitle);
            UI.ModalWindowController.GetInstnace().SetSize(modalWidth, modalHeight);
            UI.ModalWindowController.GetInstnace().OpenWindow();
        }
    }
}

