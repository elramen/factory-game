using UnityEngine;
using System.Text;

public class Merger : Movable
{
    // TODO: Spawna egna spoof-objekt när en merger skapas (typ i Game) så man slipper hijacka saker som finns i andra objekt typ
    // Det som behöver fixas då är destroy (On destroy kan inhertias),
    // och se till att de objekten inte kan ha en next-position (behöver ändra i input-manager tror jag, så att man inte sätter den där heller)
    // behöver ju också sprites och grejor

    Vector2Int stampPosition;
    Vector2Int paperPosition;

    /// <summary> Tries to merge two items. Pauses the flow unless there are two items ready.
    /// </summary>
    public override void CustomAction()
    {
        if (paperPosition != null && stampPosition != null && canIMove())
        {
            Movable stampBuilding = (Movable)grid.GetObj(stampPosition);
            Movable paperBuilding = (Movable)grid.GetObj(paperPosition);

            if (
                (stampBuilding != null && stampBuilding.item != null)
                && (paperBuilding != null && paperBuilding.item != null)
            )
            {
                // Får inte ha en next position för annars så stjäl man det på ett rullande rullband, skit i allt isf
                // kanske blir problematiskt med auto-connect
                if (!stampBuilding.hasNextPos && !paperBuilding.hasNextPos)
                {
                    Item stampItem = stampBuilding.item;
                    Item paperItem = paperBuilding.item;

                    paperItem.TransformMerge(stampItem);

                    paperBuilding.item.UpdateSprite();
                    paperBuilding.item.transform.position = grid.GridPositionToPosition(position); // Grafiskt flytta itemen hit redan nu
                    itemQueue = paperBuilding.item; //Tvingar in det i sin egen queue - Skitdum idé? - Verkar funka så yolo
                    paperBuilding.item = null;

                    stampBuilding.item.transform.position = new Vector2(0, 0); // behövs nog inte alls, mest debug
                    Destroy(stampBuilding.item.gameObject);
                    stampBuilding.item = null;
                }
            }
        }
    }

    public override void RemoveBuilding()
    {
        //Kanske funkar, borde paja byggnaderna som är runt omkring, kopplade till den här!
        if (paperPosition != null && stampPosition != null)
        {
            Movable stampBuilding = (Movable)grid.GetObj(stampPosition);
            Movable paperBuilding = (Movable)grid.GetObj(paperPosition);
            if (paperBuilding != null)
            {
                if (paperBuilding.GetType().Equals(typeof(EmptyBoi)))
                {
                    EmptyBoi building = (EmptyBoi)paperBuilding;
                    building.TellRemoveBuilding();
                }
            }
            if (stampBuilding != null)
            {
                if (stampBuilding.GetType().Equals(typeof(EmptyBoi)))
                {
                    EmptyBoi building = (EmptyBoi)stampBuilding;
                    building.TellRemoveBuilding();
                }
            }
        }
        base.RemoveBuilding();
    }

    public void Init(
        Grid<Building> grid,
        Vector2Int position,
        Vector2Int nextPosition,
        Vector2Int prevPosition,
        Vector2Int stampPosition,
        Vector2Int paperPosition
    )
    {
        base.Init(grid, position);
        this.NextPosition = nextPosition;
        this.PrevPosition = prevPosition;
        this.stampPosition = stampPosition;
        this.paperPosition = paperPosition;
        this.movable = false;
    }

    // Egen check-movement

    // Lmao funkar inte om man har två mergers efter varandra i en kedja,
    // eftersom den här kommer säga att saken inte kan flytta på sig, men det kommer att gå efter den här!!! ARGH!

    // /\ Verkar som att det ovan gör att sakerna pausas från att flytta en gång med den här implementationen,
    // men att det ändå typ funkar, även om användarna nog blir fett förvirrade

    bool canIMove()
    {
        Movable building = this;

        if ((hasNextPos && (Movable)grid.GetObj(building.NextPosition) != null))
        {
            Vector2Int nextPos = building.position;
            while (((Movable)grid.GetObj(nextPos)).item != null)
            {
                if (!((Movable)grid.GetObj(nextPos)).hasNextPos)
                {
                    return false;
                }
                nextPos = ((Movable)grid.GetObj(nextPos)).NextPosition;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public override string CreateInstructions()
    {
        StringBuilder sb = new StringBuilder();

        Movable stampBuilding = (Movable)grid.GetObj(stampPosition);
        Movable paperBuilding = (Movable)grid.GetObj(paperPosition);

        sb.Append("Merger");
        sb.Append(";");
        sb.Append(position);
        sb.Append(";");
        sb.Append(nextPosition);
        sb.Append(";");
        sb.Append(prevPosition);
        sb.Append(";");
        sb.Append(stampPosition);
        sb.Append(";");
        sb.Append(stampBuilding.PrevPosition);
        sb.Append(";");
        sb.Append(paperPosition);
        sb.Append(";");
        sb.Append(paperBuilding.PrevPosition);
        sb.Append(";");

        return sb.ToString();
    }
}
