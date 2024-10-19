using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InGame.UI
{
    public class BoardSectionView : MonoBehaviour
    {
        [SerializeField] protected Button button;
    
        public void Init(UnityAction action)
        {
            button.onClick.AddListener(action);
        }
    }
}
