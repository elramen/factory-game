using UnityEngine;

public class WorldBorder : MonoBehaviour
{
    public Game game;

    // Number of squares to extend with on every side
    private int extension = 3;
    private Vector2 extensionVec;
    private SpriteRenderer spriteRenderer;
    private float cellSize;
    private Vector2 cellSizeVec;

    public Color backgroundColor;

    public Texture2D texture;

    public void Init(Game game)
    {
        this.game = game;
        name = "WorldBorder";

        extensionVec = new Vector2(extension, extension);

        // Grid dimensions
        cellSize = game.buildingGrid.getCellSize();
        cellSizeVec = new Vector2(cellSize, cellSize);

        transform.SetParent(game.transform);
        Vector3 offset = new Vector2(0.5f, 0.5f) * cellSizeVec;
        transform.position = new Vector3(0, 0, 0);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        spriteRenderer.size = new Vector2(
            game.gridWidth + 2 * extension + 1,
            game.gridHeight + 2 * extension + 1
        );

        spriteRenderer.color = new Color(0.7f, 0.7f, 0.7f);
        spriteRenderer.sortingOrder = -5;

        CreateBackground();
    }

    public void CreateBackground()
    {
        GameObject background = new GameObject();
        SpriteRenderer spriteRenderer = background.AddComponent<SpriteRenderer>();
        background.transform.localScale = new Vector3(
            game.gridWidth,
            game.gridHeight,
            1
        );

        Data.Instance.Init();
        spriteRenderer.sprite = Data.Instance.sprites["Square"];

        spriteRenderer.color = backgroundColor;
        spriteRenderer.sortingOrder = -4;
        background.transform.SetParent(game.transform);
    }

    public Vector2 GetExtension()
    {
        return cellSizeVec * extensionVec;
    }
}
