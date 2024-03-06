using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButtonController : Button
{
    public bool CheckIfHighlighted()
    {
        return IsHighlighted();
    }

    public bool CheckIfPressed()
    {
        return IsPressed();
    }

    public Color GetTintColor()
    {
        if (transition != Transition.ColorTint)
        {
            return targetGraphic.color;
        }

        if (!interactable)
        {
            return CalculateColor(colors.disabledColor);
        }
        else if (CheckIfPressed())
        {
            return CalculateColor(colors.pressedColor);
        }
        else if (CheckIfHighlighted())
        {
            return CalculateColor(colors.highlightedColor);
        }

        return targetGraphic.color;
    }


    private Color CalculateColor(Color color)
    {
        return targetGraphic.color * color * colors.colorMultiplier;
    }
}