using UnityEngine;

public class EmptyBoi : Movable
{
    // Gör ingenting, men agerar dummy åt andra grejer, typ merger
    // Kan inte ha en nextPosition, men kan ha en prevPosition

    Vector2Int linkedPosition;

    public void Init(
        Grid<Building> grid,
        Vector2Int position,
        Vector2Int nextPosition,
        Vector2Int prevPosition,
        Vector2Int linkedPosition
    )
    {
        base.Init(grid, position);

        this.linkedPosition = linkedPosition;
        this.NextPosition = nextPosition;
        this.PrevPosition = prevPosition;
        //base.AutoConnect();
    }

    public void TellRemoveBuilding()
    {
        base.RemoveBuilding();
    }

    public override void RemoveBuilding()
    {
        if (linkedPosition != null)
        {
            Movable linkedBuilding = (Movable)grid.GetObj(linkedPosition);
            if (linkedBuilding != null)
            {
                linkedBuilding.RemoveBuilding();
            }
        }
    }

    public override string CreateInstructions()
    {
        return "";
    }
}
