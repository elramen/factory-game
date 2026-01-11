using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public float rps = 90;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Data.Instance.uiColors["Color 1"];
    }

    void Update()
    {
        transform.Rotate(0, 0, rps * Time.deltaTime);
    }
}
