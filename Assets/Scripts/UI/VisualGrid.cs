using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Line
{
    public GameObject gameObject;
    public Transform transform;
    public SpriteRenderer spriteRenderer;

    public Line(Transform parent, Color color, Vector2 position, Vector2 size)
    {
        GameObject line = new GameObject();
        transform = line.transform;
        spriteRenderer = line.AddComponent<SpriteRenderer>();
        line.transform.SetParent(parent);

        spriteRenderer.sprite = Data.Instance.sprites["Square"];
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = -30;

        line.transform.position = position;
        line.transform.localScale = size;
    }
}

public class VisualGrid : MonoBehaviour
{

    public float lineWidthInactive;
    public float lineWidthActive;

    public List<Line> lines;

    public float animationTime;
    public AnimationCurve movement;

    void Start()
    {
        Data.Instance.Init();
        CreateGrid(Data.Instance.gridWidth, Data.Instance.gridHeight, Data.Instance.gridSize);
        Resize(lineWidthInactive);
    }

    void CreateGrid(int gridWidth, int gridHeight, float gridSize)
    {
        int min_x = -gridWidth / 2;
        int max_x = (gridWidth / 2) + 1;
        int min_y = -gridHeight / 2;
        int max_y = (gridHeight / 2) + 1;

        lines = new List<Line>();

        for (int x = min_x; x <= max_x; x++)
        {
            lines.Add(
                new Line(
                    transform,
                    Data.Instance.uiColors["Grid Color"],
                    new Vector2((x - 0.5f) * gridSize, 0),
                    new Vector2(lineWidthInactive, gridHeight * gridSize)
                )
            );
        }

        for (int y = min_y; y <= max_y; y++)
        {
            Line line = new Line(
                transform,
                Data.Instance.uiColors["Grid Color"],
                new Vector2(0, (y - 0.5f) * gridSize),
                new Vector2(lineWidthInactive, gridHeight * gridSize)
            );

            line.transform.Rotate(0, 0, 90);
            lines.Add(line);
        }
    }

    void Resize(float newWidth)
    {
        foreach (Line line in lines)
        {
            line.transform.localScale = new Vector2(newWidth, line.transform.localScale.y);
        }
    }

    void Recolor(Color newColor)
    {
        foreach (Line line in lines)
        {
            line.spriteRenderer.color = newColor;
        }
    }

    public void Show()
    {
        StartCoroutine(AnimateWidth(lineWidthInactive, lineWidthActive));
    }

    public void Hide()
    {
        StartCoroutine(AnimateWidth(lineWidthActive, lineWidthInactive));
    }

    public void DangerColor()
    {
        StartCoroutine(Recolor(Data.Instance.uiColors["Grid Color"], Data.Instance.uiColors["Grid Danger Color"]));
    }

    public void RegularColor()
    {
        StartCoroutine(Recolor(Data.Instance.uiColors["Grid Danger Color"], Data.Instance.uiColors["Grid Color"]));
    }

    IEnumerator AnimateWidth(float from, float to)
    {
        float timeElapsed = 0f;
        float change = to - from;

        while (timeElapsed < animationTime)
        {
            Resize(from + change * movement.Evaluate(timeElapsed / animationTime));
            timeElapsed += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        Resize(to);

        yield return null;
    }

    IEnumerator Recolor(Color from, Color to)
    {
        Color change = to - from;
        float timeElapsed = 0f;

        while (timeElapsed < animationTime)
        {
            Recolor(from + change * movement.Evaluate(timeElapsed / animationTime));
            timeElapsed += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        Recolor(to);

        yield return null;
    }
}
