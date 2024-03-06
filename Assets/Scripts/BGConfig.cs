using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGConfig : MonoBehaviour
{
    [SerializeField] Color color;
    [SerializeField] Sprite sprite;
    [SerializeField] int ppu = 256;
    private void Awake()
    {
        if(sprite != null)
        {
            Camera.main.transform.Find("BG").GetComponent<SpriteRenderer>().sprite =
            Sprite.Create(
                sprite.texture,
                new Rect(0, 0, sprite.texture.width, sprite.texture.height),
                Vector2.one / 2,
                ppu
            );
            Camera.main.transform.Find("BG").GetComponent<SpriteRenderer>().color = color;
            Camera.main.transform.Find("BG").GetComponent<SpriteRenderer>().size = new Vector2(5, 5);
        }

        Camera.main.backgroundColor = color;
    }
}
