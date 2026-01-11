using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class ResourceManager
{
    public Game game;

    public ResourceManager(Game game) {
        this.game = game;
    }

    /*
        Grid<Resource> grid - The grid to add resources to
        GameObject resource - The resource to add to the grid
        int amount          - The number of resources to add to the grid        
    */
    public void Populate(Grid<Resource> grid, CoolColor color, int amount)
    {
        // Offset is used to introduce randomness between different games
        // Also ensures that the symmetry line is always moved at least 1000 grid units
        int seed = UnityEngine.Random.Range(0, 1000);
        int symmetry = 1000;
        int offset = seed + symmetry;

        List<float> perlinValues = new List<float>();

        for (int x = -grid.width / 2; x < grid.width / 2; x++)
        {
            for (int y = -grid.height / 2; y < grid.height / 2; y++)
            {
                // Ensures that previously placed resources are not overwritten
                if (
                    grid.GetObj(new Vector2Int(x, y)) != null
                    || game.buildingGrid.GetObj(new Vector2Int(x, y)) != null
                )
                    continue;

                perlinValues.Add(
                    Mathf.PerlinNoise(
                        (float)x / (grid.width / 2) + offset,
                        (float)y / (grid.height / 2) + offset
                    )
                );
            }
        }

        // Find the threshold required to get the desired amount of resources
        perlinValues.Sort();
        int thresholdIndex = perlinValues.Count - amount;
        thresholdIndex = Math.Clamp(thresholdIndex, 0, perlinValues.Count - 1);
        float threshold = perlinValues[thresholdIndex];

        for (int x = -grid.width / 2; x < grid.width / 2; x++)
        {
            for (int y = -grid.height / 2; y < grid.height / 2; y++)
            {
                // Ensures that previously placed resources are not overwritten
                if (
                    grid.GetObj(new Vector2Int(x, y)) != null
                    || game.buildingGrid.GetObj(new Vector2Int(x, y)) != null
                )
                    continue;

                float perlin = Mathf.PerlinNoise(
                    (float)x / (grid.width / 2) + offset,
                    (float)y / (grid.height / 2) + offset
                );

                // Catch values that are out of the desired range
                // (Not necessary at the moment)
                if (perlin > 1.0f)
                    perlin = 1.0f;
                if (perlin < 0.0f)
                    perlin = 0.0f;

                if (perlin >= threshold)
                {
                    // Debug.Log("Added resource in ground at: x=" + x + ", y=" + y);
                    game.creator.CreateResourceInGround(color, new Vector2Int(x, y));
                }
            }
        }
    }
}
