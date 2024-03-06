using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace InGame.UI
{

    public class BoardModalController : MonoBehaviour
    {
        [SerializeField] Transform contentTransform;
        [SerializeField] GameObject boardSectionIconPrefab;
        [SerializeField] BoardSection selectedBoardSection;
        [SerializeField] List<BoardSectionIcon> boardSectionIcons;
        [SerializeField] GameObject defaultBoardSectionModalPrefab;

        [SerializeField] TMPro.TextMeshProUGUI titleText, descText;
        [SerializeField] UnityEngine.UI.Button openButton;

        private void Awake()
        {
            boardSectionIcons = new List<BoardSectionIcon>();
            selectedBoardSection = null;

            InitBoard(BoardSectionCollection.GetInstance().GetBoardSections());
        }

        public void InitBoard(List<BoardSection> boardSections)
        {
            foreach (var boardSection in boardSections.Select((value,index)=>(value,index)))
            {
                int gridX = 100 * (boardSection.index % 6);
                int gridY = -150 * (boardSection.index / 6);

                GameObject boardSectionInstance = 
                    Instantiate(boardSectionIconPrefab, contentTransform, false);

                boardSectionInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2 (gridX, gridY);

                boardSectionIcons.Add(boardSectionInstance.GetComponent<BoardSectionIcon>());
                boardSectionInstance.GetComponent<BoardSectionIcon>().Initialize(boardSection.value, this);
            }
            contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 150 * ((boardSections.Count + 6) / 6));

            Refresh();
        }        

        public void Refresh()
        {
            foreach(var boardSectionIcon in boardSectionIcons)
            {
                boardSectionIcon.Refresh();
            }

            if(selectedBoardSection != null)
            {
                titleText.text = selectedBoardSection.GetSectionTitle();
                descText.text = selectedBoardSection.GetDescription();
                openButton.gameObject.SetActive(true);
            }
            else
            {
                titleText.text = "";
                descText.text = "";
                openButton.gameObject.SetActive(false);
            }
        }

        public void SelectBoardSection(BoardSection _boardSection)
        {
            selectedBoardSection = _boardSection;
            Refresh();
        }

        public BoardSection GetSelectedBoardSection() { return selectedBoardSection; }

        public void OpenSelectedBoardSection()
        {
            GameObject boardSectionContentInstance = null;

            var boardSectionContentPrefab = selectedBoardSection.GetBoardSectionContent();

            if (boardSectionContentPrefab == null)
            {
                boardSectionContentInstance = Instantiate(defaultBoardSectionModalPrefab, transform.parent, false);
                boardSectionContentInstance.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = selectedBoardSection.GetContent();
                boardSectionContentInstance.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => { 
                    gameObject.SetActive(true); 
                    boardSectionContentInstance.SetActive(false);
                });
                gameObject.SetActive(false);
            }
            else
            {
                boardSectionContentInstance = Instantiate(boardSectionContentPrefab, transform.parent, false);
                // TODO : Init Modal
                boardSectionContentInstance.transform.Find("ConfirmButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(
                    () => {
                        gameObject.SetActive(true);
                        boardSectionContentInstance.SetActive(false);
                    }
                );
                gameObject.SetActive(false);
            }
        }

    }

}
