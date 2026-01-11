using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class Item : MonoBehaviour
{
    public const short PIXEL_ARRAY_SIZE = 12;
    const int spriteSize = 128;

    // (0, PIXEL_ARRAY_SIZE)
    // /\
    // (0,0) --> (PIXEL_ARRAY_SIZE, 0)
    [SerializeField]
    public CoolColor[,] pixels = new CoolColor[PIXEL_ARRAY_SIZE, PIXEL_ARRAY_SIZE];

    public bool deathRow = false;
    public bool isMoving = false;

    public void Init(CoolColor color)
    {
        for (int i = 0; i < PIXEL_ARRAY_SIZE; i++)
        {
            for (int j = 0; j < PIXEL_ARRAY_SIZE; j++)
            {
                pixels[i, j] = color;
            }
        }
        if (GetComponent<SpriteRenderer>() != null)
        {
            UpdateSprite();
        }
    }

    /// <summary> Updates the sprite based on the items pixels-2d-array.
    /// </summary>
    public void UpdateSprite()
    {
        if (GetCachedSprite() == false)
        {
            SpriteRenderer spr = GetComponent<SpriteRenderer>();
            Sprite sprite = CreateNewTexture();
            spr.sprite = sprite;
            SetCachedSprite(sprite);
        }
    }

    private Sprite CreateNewTexture()
    {
        int length = pixels.GetLength(0);
        Texture2D tx = new Texture2D(spriteSize, spriteSize);
        Sprite sprite = Sprite.Create(
            tx,
            new Rect(0, 0, spriteSize, spriteSize),
            new Vector2(0.5f, 0.5f)
        );
        for (int y = 0; y < tx.height; y++)
        {
            for (int x = 0; x < tx.width; x++) //Goes through each pixel
            {
                Color pixelColor;
                // Helps fix the scaling of a small 2d-array to the output image resolution
                // Percentage and then scale to the appropriate length
                int funkyX = (int)(((double)x / tx.width) * length);
                int funkyY = (int)(((double)y / tx.height) * length);

                pixelColor = Data.Instance.colors[(int)pixels[funkyX, funkyY]];

                tx.SetPixel(x, y, pixelColor);
            }
        }
        tx.Apply();
        return sprite;
    }

    /// <summary> Compares two Items pixel-arrys. Returns true if identical.
    /// </summary>
    public bool EqualPixels(Item otherItem)
    {
        for (int x = 0; x < PIXEL_ARRAY_SIZE; x++)
        {
            for (int y = 0; y < PIXEL_ARRAY_SIZE; y++)
            {
                if (otherItem.pixels[x, y] != this.pixels[x, y])
                {
                    return false;
                }
            }
        }
        return true;
    }

    // Har side-effect att faktiskt uppdatera den aktuella spriten också, kanske är jättedålig approach. ajdå isf
    private bool GetCachedSprite()
    {
        string hash = TextureHash();
        //Debug.Log(hash);
        if (Data.Instance.generatedSprites.ContainsKey(hash) == false)
        {
            return false;
        }
        else
        {
            //Debug.Log("Skipped transform!");
            SpriteRenderer spr = GetComponent<SpriteRenderer>();
            Sprite sprite = Data.Instance.generatedSprites[hash];
            spr.sprite = sprite;
            return true;
        }
    }

    // Just nu cachas textures, det kanske borde vara hela spriten, men jag är osäker på vad som händer vid transformations då!
    // Kan vara värt att testa om det blir bättre performance.
    private bool SetCachedSprite(Sprite spr)
    {
        string hash = TextureHash();
        if (Data.Instance.generatedSprites.ContainsKey(hash) == false)
        {
            Data.Instance.generatedSprites[hash] = spr;
            return true;
        }
        else
        {
            // Texture already set, don't update (should not happen)
            return false;
        }
    }

    private string TextureHash()
    {
        /*
        N = Null
        B = Blue
        G = Green
        R = Red
        Y = Yellow
        0 = Error
        */
        StringBuilder sb = new StringBuilder("", PIXEL_ARRAY_SIZE * PIXEL_ARRAY_SIZE);
        foreach (CoolColor p in pixels)
        {
            if (p == CoolColor.Null)
            {
                sb.Append('N');
            }
            else if (p == CoolColor.Blue)
            {
                sb.Append('B');
            }
            else if (p == CoolColor.Green)
            {
                sb.Append('G');
            }
            else if (p == CoolColor.Red)
            {
                sb.Append('R');
            }
            else if (p == CoolColor.Yellow)
            {
                sb.Append('Y');
            }
            else
            {
                // Should really not happen!!!!
                sb.Append('0');
            }
        }
        return sb.ToString();
    }

    // Not tested, but seems to work
    public override bool Equals(object other)
    {
        if (other is Item == false)
            return false;
        if (this.pixels.GetLength(0) != (other as Item).pixels.GetLength(0))
            return false;
        if (this.pixels.GetLength(1) != (other as Item).pixels.GetLength(1))
            return false;

        for (int x = 0; x < PIXEL_ARRAY_SIZE; x++)
        {
            for (int y = 0; y < PIXEL_ARRAY_SIZE; y++)
            {
                if (this.pixels[x, y] != (other as Item).pixels[x, y])
                    return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        return 1;
    }

    public void TransformRotateClockwise()
    {
        int length = this.pixels.GetLength(0);
        int maxIndex = length - 1;
        CoolColor[,] tempPixels = new CoolColor[length, length];
        for (int x = 0; x < length / 2; x++)
        {
            for (int y = x; y < maxIndex - x; y++)
            {
                // https://www.geeksforgeeks.org/inplace-rotate-square-matrix-by-90-degrees/

                tempPixels[x, y] = this.pixels[maxIndex - y, x];
                tempPixels[y, maxIndex - x] = this.pixels[x, y];
                tempPixels[maxIndex - x, maxIndex - y] = this.pixels[y, maxIndex - x];
                tempPixels[maxIndex - y, x] = this.pixels[maxIndex - x, maxIndex - y];
            }
        }
        this.pixels = tempPixels;
    }

    public void TransformSplitVertically()
    {
        int length = this.pixels.GetLength(0);
        for (int i = length / 2; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                this.pixels[i, j] = CoolColor.Null;
            }
        }
    }

    /*
    Merges another item onto this item (modifies this item, and leaves 🍂 other unchanged)
    */
    public void TransformMerge(Item stampItem)
    {
        CoolColor[,] stampPixels = stampItem.pixels;

        int length = this.pixels.GetLength(0);

        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < length; y++)
            {
                if (stampPixels[x, y] != CoolColor.Null)
                {
                    this.pixels[x, y] = stampPixels[x, y];
                }
            }
        }
    }

    // ♥
    public void TransformHeart()
    {
        int length = this.pixels.GetLength(0);
        int indexLength = (length - 1);
        // made for 12x12 pixels

        for (int y = 0; y < length / 2; y++)
        {
            for (int x = 0; x < y; x++)
            {
                // Offset the top part up a bit
                int funkyY = Math.Min(y + (length / 2) + 1, indexLength); 
                this.pixels[x, funkyY] = CoolColor.Null;
                this.pixels[indexLength - x, funkyY] = CoolColor.Null;
            }
        }
        // Bottom is pretty basic
        for (int x = 0; x < length / 2; x++)
        {
            for (int y = 0; y < x; y++)
            {
                this.pixels[x + (length / 2), y] = CoolColor.Null;
                this.pixels[indexLength - (x + (length / 2)), y] = CoolColor.Null;
            }
        }
        // Get the indent of the heart
        for (int x = 0; x < (length / 4) + 1; x++)
        {
            for (int y = 0; y <= x; y++)
            {
                int funkyX = (x + (length / 4) - 1);
                this.pixels[funkyX, indexLength - y] = CoolColor.Null;
                this.pixels[indexLength - funkyX, indexLength - y] = CoolColor.Null;
            }
        }
    }

    // Funkar inte bra, blir lite jobbigt med vissa pixlar typ
    public void TransformSplitDiagonally()
    {
        int length = this.pixels.GetLength(0);

        /*

        *-------
        **------
        ***-----
        ****----
        ****----
        *****---
        ******--
        *******-

        */

        for (int y = 0; y < length / 2; y++)
        {
            for (int x = (length / 8) + y; x < length; x++)
            {
                this.pixels[x, y] = CoolColor.Null;
            }
        }
        for (int y = (length / 2); y < length; y++)
        {
            for (int x = (length / 8) + y; x < length; x++)
            {
                this.pixels[x, y] = CoolColor.Null;
            }
        }
    }
}

// [System.Serializable]
// public enum CoolColor
// {
//     Null, Green, Yellow, Blue, Red
// }
