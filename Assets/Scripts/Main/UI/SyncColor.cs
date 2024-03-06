using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SyncColor : MonoBehaviour
{
    [SerializeField] CustomButtonController button;
    [SerializeField] List<Graphic> graphics;


    private void Update() {
        Color buttonColor = button.targetGraphic.color;
 
        if( button.transition == Selectable.Transition.ColorTint )
        {
            buttonColor = button.GetTintColor();
        }
        
        foreach(var graphic in graphics)
        {
            graphic.color = buttonColor;
        }
    }
}
