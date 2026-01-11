using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

// CHANGES HAVE BEEN MADE

// Manages creation of resources.

public class Creator : MonoBehaviour
{
    public Game game;
    private int numBelts = 0;
    System.Random rnd;

    public Creator(Game game)
    {
        this.game = game;
        rnd = new System.Random();
    }

    public void Create(string instruction)
    {
        string[] args = instruction.Split(';');

        switch (args[0])
        {
            case "ConveyorBelt":
                CreateConveyorBelt(
                    StrToVec2Int(args[1]),
                    StrToVec2Int(args[2]),
                    StrToVec2Int(args[3])
                );
                break;
            case "Miner":
                CreateMiner(
                    StrToVec2Int(args[1]),
                    StrToVec2Int(args[2]),
                    StrToVec2Int(args[3]),
                    (CoolColor)Int32.Parse(args[4])
                );
                break;
            case "VerticalSplitter":
                CreateSplitter(
                    StrToVec2Int(args[1]),
                    StrToVec2Int(args[2]),
                    StrToVec2Int(args[3])
                );
                break;
            case "Rotator":
                CreateRotator(
                    StrToVec2Int(args[1]),
                    StrToVec2Int(args[2]),
                    StrToVec2Int(args[3])
                );
                break;
            case "HeartCutter":
                CreateHeartCutter(
                    StrToVec2Int(args[1]),
                    StrToVec2Int(args[2]),
                    StrToVec2Int(args[3])
                );
                break;
            case "Merger":
                CreateMerger(
                    StrToVec2Int(args[1]),
                    StrToVec2Int(args[2]),
                    StrToVec2Int(args[3]),
                    StrToVec2Int(args[4]),
                    StrToVec2Int(args[5]),
                    StrToVec2Int(args[6]),
                    StrToVec2Int(args[7])
                );
                break;
        }
    }

    private Vector2Int StrToVec2Int(string str)
    {
        string[] vals = str.Substring(1, str.Length - 2).Split(',');
        return new Vector2Int(Int32.Parse(vals[0]), Int32.Parse(vals[1]));
    }

    // Creates a base
    // A base consists of an empty GameObject "BaseContainer
    // The container have two children; Base and ProgressBar
    // The GameObject Base contains the graphics for the base,
    // scripts for quarterbases and a script for the mainBase
    // Quarterbases are Movables that collect resources while
    // the mainBase combines theses resources to keep track
    // of progress, goals and update the progress bar.
    public Base CreateBase(Vector2Int position)
    {
        // Empty GameObject to be parent of Base and Progress Bar
        GameObject baseContainer = new GameObject();
        baseContainer.name = "BaseContainer";
        baseContainer.transform.parent = game.transform;

        // GameObject that contains graphics and scripts
        GameObject newBase = Instantiate(Data.Instance.buildings["Base"]);
        newBase.name = "Base";
        newBase.transform.SetParent(baseContainer.transform);
        // Place base graphics in front of progress bar
        newBase.GetComponent<SpriteRenderer>().sortingOrder = 8;

        // Center of the whole base
        Vector2 center =
            game.buildingGrid.GridPositionToPosition(position);
        newBase.transform.position = center;

        // Base script
        // Keeps track of goals, progress and combines the resources collected by the quarterbases
        Base mainBase = newBase.GetComponent<Base>();
        mainBase.Init();
        // Quarter bases are Movables that interact with the grid
        QuarterBase[] quarterBases = new QuarterBase[4];

        Vector2Int[] quarterBasePrevPosition =
        {
                position + new Vector2Int(0, 2),
                position + new Vector2Int(2, 0),
                position + new Vector2Int(0, -2),
                position + new Vector2Int(-2, 0),
        };

        Vector2Int[] quarterBasePosition =
        {
                position + new Vector2Int(0, 1),
                position + new Vector2Int(1, 0),
                position + new Vector2Int(0, -1),
                position + new Vector2Int(-1, 0),
        };


        // Create QuarterBases and link them to an empty gameobject
        for (int i = 0; i < quarterBases.Length; i++)
        {

            // Empty GameObject to be parent of DummyBelt for quarterBases
            GameObject qb = new GameObject();
            qb.name = "QuarterBase";
            qb.transform.SetParent(newBase.transform);

            quarterBases[i] = qb.AddComponent<QuarterBase>();

            quarterBases[i].Init(
                game.buildingGrid,
                quarterBasePosition[i],         // QB position
                quarterBasePrevPosition[i],     // QB previous, could recieve input from, position
                position,                       // base position
                mainBase
            );

            game.currentMovables.Add(quarterBases[i]);
            game.buildingGrid.AddObj(quarterBases[i], quarterBases[i].position);

        }


        // Block positions that should be unbuildable
        Vector2Int[] barrierBasePosition =
        {
            position,
            position + new Vector2Int(1, 1),
            position + new Vector2Int(1, -1),
            position + new Vector2Int(-1, -1),
            position + new Vector2Int(-1, 1)
        };

        for (int i = 0; i < barrierBasePosition.Length; i++) {
            GameObject b = new GameObject();
            b.name = "Barrier";
            b.transform.SetParent(newBase.transform);

            Building barrier = b.AddComponent<Barrier>();
            
            barrier.Init(game.buildingGrid, barrierBasePosition[i]);
            game.buildingGrid.AddObj(barrier, barrier.position);
        }

        // Add progress bar
        GameObject progressBar = Instantiate(Data.Instance.buildings["Progress Bar"]);
        progressBar.name = "ProgressBar";
        progressBar.transform.SetParent(baseContainer.transform);
        progressBar.transform.position = center;
        progressBar.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        progressBar.GetComponent<Canvas>().sortingOrder = 7;

        GameObject background = Instantiate(Data.Instance.buildings["Progress Bar"]);
        background.name = "Background";
        background.transform.SetParent(baseContainer.transform);
        background.transform.position = center;
        background.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        background.GetComponent<Canvas>().sortingOrder = 6;
        Image backgroundImage = background.GetComponent<Image>();
        backgroundImage.fillAmount = 1f;
        backgroundImage.color = new Color(0.33f, 0.37f, 0.46f);

        return mainBase;
    }

    public ConveyorBelt CreateConveyorBelt(
        Vector2Int gridPosition,
        Vector2Int nextPosition,
        Vector2Int prevPosition
    )
    {
        ConveyorBelt belt = (ConveyorBelt)game.buildingGrid.GetObj(gridPosition);

        if (belt != null)
        {
            belt.NextPosition = nextPosition;
            belt.PrevPosition = prevPosition;
        }
        else
        {
            GameObject newObject = Instantiate(Data.Instance.buildings["Belt"]);
            newObject.transform.parent = game.transform;

            belt = newObject.GetComponent<ConveyorBelt>();

            belt.Init(game.buildingGrid, gridPosition, nextPosition, prevPosition);

            game.buildingGrid.AddObj(belt, gridPosition);

            game.currentMovables.Add(belt);
        }
        if(numBelts < 4){
            numBelts++;
            if(numBelts == 4 && Data.Instance.currentGoal == 2){
                Data.Instance.currentGoal++;
                game.AccomplishedGoal();
                PlayerPrefs.SetInt("currentGoal", Data.Instance.currentGoal);
            }
        }

        // belt.TryToConnect();

        return belt;
    }

    public Building CreateMiner(
        Vector2Int gridPosition,
        Vector2Int nextPosition,
        Vector2Int prevPosition,
        CoolColor color
    )
    {
        GameObject newObject = Instantiate(Data.Instance.buildings["Miner"]);
        newObject.transform.parent = game.transform;
        Miner miner = newObject.AddComponent<Miner>();

        miner.Init(
            game.buildingGrid,
            gridPosition,
            nextPosition,
            prevPosition,
            1,
            1000,
            true,
            color
        );
        // Transform t = newObject.transform.Find("BaseLayer");
        // t.transform.eulerAngles = new Vector3(
        // t.transform.eulerAngles.x,
        // t.transform.eulerAngles.y,
        // 90
        // );

        game.buildingGrid.AddObj(miner, gridPosition);
        game.currentMovables.Add(miner);
        game.currentMachines.Add(miner);

        miner.AutoConnect();

        if (color == CoolColor.Yellow && Data.Instance.currentGoal == 1)
        {
            Data.Instance.currentGoal++;
            game.AccomplishedGoal();
            PlayerPrefs.SetInt("currentGoal", Data.Instance.currentGoal);
        }
        return miner;
    }

    public Building CreateSplitter(
        Vector2Int gridPosition,
        Vector2Int nextPosition,
        Vector2Int prevPosition
    )
    {
        GameObject newObject = Instantiate(Data.Instance.buildings["Vertical Splitter"]);
        newObject.transform.parent = game.transform;
        VerticalSplitter splitter = newObject.AddComponent<VerticalSplitter>();

        splitter.Init(game.buildingGrid, gridPosition, nextPosition, prevPosition);
        game.buildingGrid.AddObj(splitter, gridPosition);
        game.currentMovables.Add(splitter);

        splitter.AutoConnect();
        return splitter;
    }

    public Building CreateHeartCutter(
        Vector2Int gridPosition,
        Vector2Int nextPosition,
        Vector2Int prevPosition
    )
    {
        GameObject newObject = Instantiate(Data.Instance.buildings["Heart Cutter"]);
        newObject.transform.parent = game.transform;
        HeartCutter cutter = newObject.AddComponent<HeartCutter>();

        cutter.Init(game.buildingGrid, gridPosition, nextPosition, prevPosition);
        game.buildingGrid.AddObj(cutter, gridPosition);
        game.currentMovables.Add(cutter);

        cutter.AutoConnect();
        return cutter;
    }


    public Building CreateRotator(
        Vector2Int gridPosition,
        Vector2Int nextPosition,
        Vector2Int prevPosition
    )
    {
        /**
        AddComponent om man inte har en rolig prefab förgjord för just den här
        -- Alla kräver Animation curve annars funkar inte movementen i movable på rad 29 (kan inte interpolera och failar typ).
        -- Animation curve skapar inte automatiskt i den här processen utan man behöver ha den från en prefab

        **/

        GameObject newObject = Instantiate(Data.Instance.buildings["Rotator"]);
        newObject.transform.parent = game.transform;
        Rotator rotator = newObject.AddComponent<Rotator>();

        rotator.Init(game.buildingGrid, gridPosition, nextPosition, prevPosition);
        game.buildingGrid.AddObj(rotator, gridPosition);
        game.currentMovables.Add(rotator);

        rotator.AutoConnect();
        return rotator;
    }

    public Building CreateMerger(
        Vector2Int gridPosition,
        Vector2Int nextPosition,
        Vector2Int prevPosition,
        Vector2Int stampPosition,
        Vector2Int stampPrevPosition,
        Vector2Int paperPosition,
        Vector2Int paperPrevPosition
    )
    {
        EmptyBoi emptyBoi1 = CreateEmptyBoi(
            stampPosition,
            stampPosition,
            stampPrevPosition,
            gridPosition
        );
        EmptyBoi emptyBoi2 = CreateEmptyBoi(
            paperPosition,
            paperPosition,
            paperPrevPosition,
            gridPosition
        );

        GameObject newObject = Instantiate(Data.Instance.buildings["Merger"]);
        newObject.transform.parent = game.transform;
        Merger merger = newObject.GetComponent<Merger>();

        merger.Init(
            game.buildingGrid,
            gridPosition,
            nextPosition,
            prevPosition,
            stampPosition,
            paperPosition
        );
        game.buildingGrid.AddObj(merger, gridPosition);
        game.currentMovables.Add(merger);

        merger.AutoConnect();
        
        emptyBoi1.updateDummyBelt();
        emptyBoi2.updateDummyBelt();

        return merger;
    }

    public EmptyBoi CreateEmptyBoi(
        Vector2Int gridPosition,
        Vector2Int nextPosition,
        Vector2Int prevPosition,
        Vector2Int linkedPosition
    )
    {
        GameObject newObject = Instantiate(Data.Instance.buildings["Empty Boi"]);
        newObject.transform.parent = game.transform;
        EmptyBoi emptyBoi = newObject.AddComponent<EmptyBoi>();

        emptyBoi.Init(game.buildingGrid, gridPosition, nextPosition, prevPosition, linkedPosition);
        game.buildingGrid.AddObj(emptyBoi, gridPosition);
        game.currentMovables.Add(emptyBoi);
        emptyBoi.AutoConnect();
        return emptyBoi;
    }

    public void CreateItem(Movable building, CoolColor color)
    {
        GameObject original = Data.Instance.resources["Resource0"];
        GameObject newObject = Instantiate(original);
        newObject.transform.parent = game.transform;
        newObject.transform.position = building.transform.position;
        Item buildingItem = newObject.AddComponent<Item>();
        buildingItem.Init(color);
        building.item = buildingItem;
    }

    public void CreateResourceInGround(CoolColor color, Vector2Int position)
    {
        GameObject original = Data.Instance.resourcesInGround["ResourceInGround0"];
        GameObject newObject = Instantiate(
            original,
            game.resourceGrid.GridPositionToPosition(position),
            Quaternion.identity
        );

        SpriteRenderer sr = newObject.GetComponent<SpriteRenderer>();
        int i = rnd.Next(1,4);

        sr.sprite = Data.Instance.sprites["Resource " + i];
        

        newObject.transform.parent = game.transform;

        Resource resource = newObject.AddComponent<Resource>();
        resource.Init(color);
        resource.transform.position = game.resourceGrid.GridPositionToPosition(position);
        game.resourceGrid.AddObj(resource, position);
    }
}