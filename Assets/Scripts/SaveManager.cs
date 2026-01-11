using UnityEngine;
using System.Text;

// Manages loading and saving the game state
public class SaveManager
{
    Game game;

    public SaveManager(Game game)
    {
        this.game = game;
    }

    // Loads/generates the resource grid. If there is a save, it loads from there.
    public void LoadGame()
    {
        game.creator.CreateBase(new Vector2Int(0, 0));

        if(PlayerPrefs.HasKey("levelNr")) {
            Data.Instance.levelNr = PlayerPrefs.GetInt("levelNr");
        }
        else{
            Data.Instance.levelNr = 1;
        }

        if(PlayerPrefs.HasKey("starsSubGoal")) {
            Data.Instance.starsSubGoal = PlayerPrefs.GetInt("starsSubGoal");
        }
        else{
            Data.Instance.starsSubGoal = 0;
        }

        if(PlayerPrefs.HasKey("currentTime")) {
            Data.Instance.currentTime = PlayerPrefs.GetInt("currentTime");
        }
        else{
            Data.Instance.currentTime = 0;
        }

        if(PlayerPrefs.HasKey("goalSeed")) {
            Data.Instance.goalSeed = PlayerPrefs.GetInt("goalSeed");
        }
        if(PlayerPrefs.HasKey("goalAmount")) {
            Data.Instance.goalAmount = PlayerPrefs.GetInt("goalAmount");
        }


        if (PlayerPrefs.HasKey("currentGoal"))
        {
            Data.Instance.currentGoal = PlayerPrefs.GetInt("currentGoal");
        }
        else
        {
            Data.Instance.currentGoal = 0;
        }

        if (PlayerPrefs.HasKey("currentGoal"))
        {
            Data.Instance.totalStars = PlayerPrefs.GetInt("totalStars");
        }
        else
        {
            Data.Instance.totalStars = 0;
        }

        if (!PlayerPrefs.HasKey("resourceGrid"))
        {
            game.resourceGrid = new Grid<Resource>(game.gridWidth, game.gridHeight, 1f);

            game.resourceManager.Populate(game.resourceGrid, CoolColor.Red, 80);
            game.resourceManager.Populate(game.resourceGrid, CoolColor.Green, 80);
            game.resourceManager.Populate(game.resourceGrid, CoolColor.Blue, 80);
            game.resourceManager.Populate(game.resourceGrid, CoolColor.Yellow, 80);

            SaveGame();
        }
        else
        {
            game.resourceGrid = new Grid<Resource>(game.gridWidth, game.gridHeight, 1f);
            StringToGrid(PlayerPrefs.GetString("resourceGrid"), ref game.resourceGrid);

            string instructions = PlayerPrefs.GetString("buildings");

            foreach (string line in instructions.Split('\n'))
            {
                game.creator.Create(line);
            }

            foreach (Movable movable in game.currentMovables)
            {
                if (movable.GetType().Equals(typeof(EmptyBoi)))
                {
                    movable.updateDummyBelt();
                }
                movable.AutoRotate();
            }
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetString("resourceGrid", GridToString(game.resourceGrid));

        // save buildings
        StringBuilder sb = new StringBuilder();
        foreach (Movable movable in game.currentMovables)
        {
            string instruction = movable.CreateInstructions();

            if (instruction.Length > 0)
            {
                sb.Append(movable.CreateInstructions());
                sb.Append("\n");
            }
        }

        PlayerPrefs.SetString("buildings", sb.ToString());
    }

    string GridToString(Grid<Resource> grid)
    {
        string str = "";

        for (int y = grid.min_y; y < grid.max_y; y++)
        {
            for (int x = grid.min_x; x < grid.max_x; x++)
            {
                Resource res = grid.GetObj(new Vector2Int(x, y));
                int val = res != null ? (int)res.color : 0;
                str += (char)(val + 65);
            }
        }

        return str;
    }

    void StringToGrid(string str, ref Grid<Resource> grid)
    {
        int i = 0;
        for (int y = grid.min_y; y < grid.max_y; y++)
        {
            string row = "";
            for (int x = grid.min_x; x < grid.max_x; x++)
            {
                row += str[i];
                int converted = ((int)str[i] - 65);

                if (converted != (char)0)
                    game.creator.CreateResourceInGround(
                        (CoolColor)((int)converted),
                        new Vector2Int(x, y)
                    );
                i++;
            }
        }
    }
}
