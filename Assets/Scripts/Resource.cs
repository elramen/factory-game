using UnityEngine;

public class Resource : MonoBehaviour
{
    int resourceID; // Siffra som representerar resursen
    string resourceName; // Namnet som representerar siffran
    float resourcesLeft = -1; // int? - Om -1 = oändligt!
    public Vector2Int position;
    public CoolColor color;

    public void Init(CoolColor color)
    {
        this.color = color;
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        spr.color = Data.Instance.colors[(int)color];
    }
}
