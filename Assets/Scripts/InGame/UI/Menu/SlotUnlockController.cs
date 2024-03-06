using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Numerics;

namespace InGame.UI.Menu
{
    public class SlotUnlockController : MonoBehaviour
    {
        public delegate void ConfirmAction();
        ConfirmAction action;
        
        [SerializeField] Button confirmButton, cancelButton;
        [SerializeField] TextMeshProUGUI text;

        public void OpenWindow(BigInteger balance, long cost, ConfirmAction action)
        {
            this.action = action;

            gameObject.SetActive(true);

            text.text = string.Format("Slot cost : {0:N0} Gold", cost);

            if(cost > balance) confirmButton.interactable = false;

        }

        public void Confirm()
        {
            action();
            Cancel();
        }

        public void Cancel()
        {
            gameObject.SetActive(false); 
            confirmButton.interactable = true;
        }
    }
}