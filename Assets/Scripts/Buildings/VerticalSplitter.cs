using UnityEngine;
using System.Text;

public class VerticalSplitter : Movable
{
    /// <summary> Nulls all the pixels on the right hand side.
    /// </summary>
    public override void CustomAction()
    {
        item.TransformSplitVertically();
        item.UpdateSprite();
    }

    public void Init(
        Grid<Building> grid,
        Vector2Int position,
        Vector2Int nextPosition,
        Vector2Int prevPosition
    )
    {
        base.Init(grid, position);

        this.NextPosition = nextPosition;
        this.PrevPosition = prevPosition;
    }

    public override string CreateInstructions()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("VerticalSplitter");
        sb.Append(";");
        sb.Append(position);
        sb.Append(";");
        sb.Append(nextPosition);
        sb.Append(";");
        sb.Append(prevPosition);
        sb.Append(";");

        return sb.ToString();
    }
}
