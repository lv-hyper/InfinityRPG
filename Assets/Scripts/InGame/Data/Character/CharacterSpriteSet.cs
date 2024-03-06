using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Sprite Set")]
public class CharacterSpriteSet : ScriptableObject
{
    [SerializeField] Sprite left, right, top, bottom;
    
    public Sprite GetSpriteFollowingAngle(float angle)
    {

        if (-135.0f <= angle && angle < -45.0f)
        {
            return left;
        }
        else if (-45.0f <= angle && angle < 45.0f)
        {
            return top;
        }
        else if (45.0f <= angle && angle < 135.0f)
        {
            return right;
        }
        else
        {
            return bottom;
        }
    }
}
