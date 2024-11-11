using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CodeColor : MonoBehaviour {
    [System.Serializable]
    public struct ColorSprite {
        [FormerlySerializedAs("colorType")] public TileColor tileColor;
        public Sprite colorsprite;
    }
    
    public ColorSprite[] Sprites;

    // 顏色字典
    public Dictionary<TileColor, Sprite> ColorDic;

    // 顏色狀態
    public TileColor Spritecolor;

    public TileColor GetTileColor {
        get { return Spritecolor; }
        set { Spritecolor = value; }
    }

    private SpriteRenderer spriteCompent;

    private void Awake()
    {
        spriteCompent = transform.Find("code").GetComponent<SpriteRenderer>();

        ColorDic = new Dictionary<TileColor, Sprite>();

        for (int i = 0; i < Sprites.Length; i++) {
            if (!ColorDic.ContainsKey(Sprites[i].tileColor)) {
                ColorDic.Add(Sprites[i].tileColor, Sprites[i].colorsprite);
            }
        }
    }

    // 指定顏色
    public void setSprite(TileColor tileColor) {

        Spritecolor = tileColor;

        if (ColorDic.ContainsKey(tileColor)) {
            spriteCompent.sprite = ColorDic[tileColor];
        }
    }

    // 隨機指定顏色
    public void setRangeSprite() {
        int ran = Random.Range(0 , Sprites.Length);
        Spritecolor = (TileColor)ran;

        if (ColorDic.ContainsKey(Spritecolor))
        {
            spriteCompent.sprite = ColorDic[Spritecolor];
        }
    }
}
