using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace InGame.UI
{

    public class ModalWindowController : MonoBehaviour
    {
        private static ModalWindowController instance = null;

        [SerializeField] private GameObject modalWindow;
        [SerializeField] GameObject defaultModalPrefab;
        [SerializeField] TextMeshProUGUI titleUI;
        [SerializeField] Transform contentTransform;
        [SerializeField] RectTransform innerWindowTransform;

        Vector2 defaultSizeDelta;
        GameObject content;

        public event EventHandler onWindowOpen, onWindowClose;



        private void Awake()
        {
            if (GetInstnace() == null)
            {
                instance = this;
                defaultSizeDelta = innerWindowTransform.sizeDelta;
                content = null;
            }
        }

        public static ModalWindowController GetInstnace()
        {
            return instance;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public void SetTitle(string title)
        {
            titleUI.text = title;
        }

        public void SetSize(int width, int height)
        {
            innerWindowTransform.sizeDelta = new Vector2(width, height+400);
        }

        public void SetContent(GameObject contentPrefab, string title)
        {
            ClearContent();
            SetTitle(title);
            content = GameObject.Instantiate(contentPrefab, contentTransform);
        }

        public void SetContent(BoardSection boardSection)
        {
            ClearContent();

            var boardSectionContentPrefab = boardSection.GetBoardSectionContent();

            if (boardSectionContentPrefab == null)
            {
                content = Instantiate(defaultModalPrefab, contentTransform, false);
                content.GetComponent<SimpleTextBoardSectionView>().Init(boardSection.GetContent(), () => {
                    CloseWindow();
                });
                //content.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = boardSection.GetContent();
                /*content.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => {
                    CloseWindow();
                });*/
            }
            else
            {
                content = Instantiate(boardSectionContentPrefab, contentTransform, false);
                // TODO : Init Modal
                content.transform.Find("ConfirmButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(
                    () => {
                        CloseWindow();
                    }
                );
            }
        }

        public GameObject GetContent()
        {
            return content;
        }

        public void ClearContent()
        {
            SetTitle("Modal Window");
            content = null;
            foreach (Transform t in contentTransform)
            {
                Destroy(t.gameObject);
            }
            innerWindowTransform.sizeDelta = defaultSizeDelta;
        }

        public void OpenWindow()
        {
            if(onWindowOpen != null)
                onWindowOpen(this, EventArgs.Empty);

            Entity.Character.GetInstance().BlockMovement();
            modalWindow.SetActive(true);
        }

        public void CloseWindow() {
            if(onWindowClose != null)
                onWindowClose(this, EventArgs.Empty);

            Entity.Character.GetInstance().EnableMovement();
            modalWindow.SetActive(false);

            onWindowOpen = null;
            onWindowClose = null;
        }
    }
}