using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InGame.UI
{
    public class SimpleTextBoardSectionView : BoardSectionView
    {
        [SerializeField] private TMP_Text text;
    
        public void Init(string content, UnityAction action)
        {
            base.Init(action);
            text.text = content;
        }
    }
}
