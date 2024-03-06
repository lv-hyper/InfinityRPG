using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace InGame.UI
{

    public class BoardSectionIcon : MonoBehaviour
    {
        [Serializable]
        public enum Status
        {
            Selected,
            Default,
            Disabled
        }

        [SerializeField] BoardSection boardSection;
        [SerializeField] BoardModalController boardModalController;

        [SerializeField] Color selectedColor, defaultColor, disabledColor;
        [SerializeField] Status status;

        public void Initialize(BoardSection _boardSection, BoardModalController _boardModalController)
        {
            boardSection = _boardSection;
            boardModalController = _boardModalController;
            status = Status.Default;
            Refresh();
        }

        public BoardSection GetBoardSection() { return boardSection; }

        public void Refresh()
        {
            if (boardModalController.GetSelectedBoardSection() == boardSection)
            {
                status = Status.Selected;
            }

            else
            {
                status = Status.Default;
            }

            switch(status)
            {
                case Status.Selected:
                    GetComponent<UnityEngine.UI.Image>().color = selectedColor;
                    GetComponent<UnityEngine.UI.Button>().interactable = true;
                    break;
                case Status.Default:
                    GetComponent<UnityEngine.UI.Image>().color = defaultColor;
                    GetComponent<UnityEngine.UI.Button>().interactable = true;
                    break;
                case Status.Disabled:
                    GetComponent<UnityEngine.UI.Image>().color = disabledColor;
                    GetComponent<UnityEngine.UI.Button>().interactable = false;
                    break;
            }
        }

        public void OnClick()
        {
            boardModalController.SelectBoardSection(boardSection);
        }
    }

}
