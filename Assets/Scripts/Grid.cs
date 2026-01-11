using UnityEngine;

public class Grid<T>
{
    public int height;
    public int width;
    public T[,] grid;

    public int min_x;
    public int min_y;
    public int max_x;
    public int max_y;

    private float cellSize;
    private float left;
    private float bottom;

    public Grid(int height, int width, float cellSize)
    {
        this.height = height;
        this.width = width;
        this.cellSize = cellSize;
        this.grid = new T[width, height];

        this.left = -(this.width / 2);
        this.bottom = -(this.height / 2);

        min_x = -width / 2;
        max_x = width / 2;
        min_y = -height / 2;
        max_y = height / 2;
    }

    public Vector2 size()
    {
        return new Vector2(this.width * this.cellSize, this.height * this.cellSize);
    }

    /*  Returns the object located in the grid at gridPosition.
        Returns null if gridPosition is invalid.
        Returns null if gridPosition is empty.
    */
    public T GetObj(Vector2Int gridPosition)
    {
        if (WithinBounds(gridPosition) == false)
        {
            return default(T);
        }
        Vector2Int gridIndex = GridPositionToIndex(gridPosition);
        T obj = grid[gridIndex.x, gridIndex.y];

        return obj;
    }

    public bool AddObj(T obj, Vector2Int gridPosition)
    {
        if (WithinBounds(gridPosition) == false)
        {
            return false;
        }
        Vector2Int gridIndex = GridPositionToIndex(gridPosition);
        this.grid[gridIndex.x, gridIndex.y] = obj;
        return true;
    }

    // returns the corresponding physical coordinates to the grid coordinates
    public Vector2 GridPositionToPosition(Vector2Int gridPosition)
    {
        return new Vector2((gridPosition.x * cellSize), (gridPosition.y * cellSize));
    }

    // returns the corresponding grid coordinates to the physical coordinates
    public Vector2Int PositionToGridPosition(Vector2 position)
    {
        return new Vector2Int(
            Mathf.RoundToInt(position.x / cellSize),
            Mathf.RoundToInt(position.y / cellSize)
        );
    }

    // returns the grid indexes (in the 2d-array) of a given gridPosition
    private Vector2Int GridPositionToIndex(Vector2Int gridPosition)
    {
        int x = gridPosition.x + (int)(width / 2);
        int y = gridPosition.y + (int)(height / 2);

        return new Vector2Int(x, y);
    }

    public float getCellSize()
    {
        return cellSize;
    }

    /*  Returns true if gridPosition is within bounds of this grid */
    public bool WithinBounds(Vector2Int gridPosition)
    {
        bool legalX = gridPosition.x >= left && gridPosition.x <= -left;
        bool legalY = gridPosition.y >= bottom && gridPosition.y <= -bottom;
        return legalX && legalY;
    }
}
