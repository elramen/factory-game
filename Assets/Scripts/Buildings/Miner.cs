using UnityEngine;
using System.Text;

public class Miner : Movable
{
    // TODO: Minern sparkar bara ur sig grejer om det finns 2 rutor framåt som är lediga,
    // bugg men typ inte jätteviktigt för det är rätt edge-casy

    float speed; // resurser/sekund
    float capacity; // int?
    bool active = true; // is the miner turned on or powered?
    float progress = 0; // 0 plocka upp, kopplat till speed
    public CoolColor color = CoolColor.Null;

    public void Init(
        Grid<Building> grid,
        Vector2Int position,
        Vector2Int nextPosition,
        Vector2Int prevPosition,
        float speed,
        float capacity,
        bool active,
        CoolColor color
    )
    {
        base.Init(grid, position);

        this.speed = speed;
        this.capacity = capacity;
        this.active = active;
        this.NextPosition = nextPosition;
        this.PrevPosition = prevPosition;
        this.color = color;
    }

    public override string CreateInstructions()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("Miner");
        sb.Append(";");
        sb.Append(position);
        sb.Append(";");
        sb.Append(nextPosition);
        sb.Append(";");
        sb.Append(prevPosition);
        sb.Append(";");
        sb.Append((int)color);
        sb.Append(";");

        return sb.ToString();
    }
}
