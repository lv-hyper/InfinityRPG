using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleEffectIcon : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image effectIcon, effectTurnImage;
    [SerializeField] TextMeshProUGUI effectTurn;

    public UnityEngine.UI.Image GetImage() { return effectIcon; }
    public UnityEngine.UI.Image GetTurnImage() { return effectTurnImage; }
    public TextMeshProUGUI GetTurn() { return effectTurn; }
}
