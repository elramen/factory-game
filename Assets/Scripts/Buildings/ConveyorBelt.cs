using UnityEngine;
using System.Text;

public class ConveyorBelt : Movable
{
    // Args:
    // 0 - direction (Vector2Int)
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

    public override string CreateInstructions() {
        StringBuilder sb = new StringBuilder();

        sb.Append("ConveyorBelt");
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
