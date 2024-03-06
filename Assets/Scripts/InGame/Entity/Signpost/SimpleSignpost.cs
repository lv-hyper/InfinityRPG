using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace InGame.Entity
{

    public class SimpleSignpost : Signpost
    {
        [SerializeField] string informationText;

        public override void Interact()
        {
            GameObject modalClone = GameObject.Instantiate(modal);
            modalClone.GetComponent<TMPro.TextMeshProUGUI>().text = informationText;

            UI.ModalWindowController.GetInstnace().SetContent(modalClone, signpostTitle);
            UI.ModalWindowController.GetInstnace().SetSize(modalWidth, modalHeight);

            Destroy(modalClone);

            UI.ModalWindowController.GetInstnace().OpenWindow();
        }
    }
}

