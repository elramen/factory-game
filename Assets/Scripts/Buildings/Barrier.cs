using UnityEngine;

/*  Invisble building used to block tiles */
public class Barrier : Building {
    public new void Init(Grid<Building> grid, Vector2Int position) {
        base.Init(grid, position);
    }
}