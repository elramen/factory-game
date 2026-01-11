using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections;

public class Game : MonoBehaviour
{
    // TODO: Måste fixa vilken nivå saker spawnar på när det gäller det nya del-sprite systemet, så det inte blir typ z-fighting

    // TODO: Se till att vi inte tar bort grejer under tiden det är en move som pågår, isf ska man queuea förändirngen
    // och köra den när moven är över (typ inte mitt i ett tick) - annars så pajar movement-koden och kan krasha unity

    // --------------------------------
    // ---------- CONSTANTS -----------
    // --------------------------------

    public int gridWidth;
    public int gridHeight;

    public float gameSpeed;

    // --------------------------------
    // ---------- REFERENCES ----------
    // --------------------------------

    public Creator creator;
    public SaveManager saveManager;
    public ResourceManager resourceManager;
    public InputManager controls;
    public GameObject worldBorder;
    private Coroutine gameSpeedCoroutine;
    
    // --------------------------------
    // ---------- GAME DATA -----------
    // --------------------------------

    public Grid<Building> buildingGrid;
    public Grid<Resource> resourceGrid;
    public Grid<Item> itemGrid;

    public List<Movable> currentMovables = new List<Movable>();
    public List<Movable> currentMachines = new List<Movable>();

    // --------------------------------
    // ---------- MOVEMENT ------------
    // --------------------------------

    public bool safeToRemove = true;
    public bool safeToMove = true;


    // --------------------------------
    // ---------- TUTORIAL ------------
    // --------------------------------
    
    public delegate void UpdateTutorial();
    public static event UpdateTutorial accomplishedGoal;

    // ----------------------------
    // ---------- MISC ------------
    // ----------------------------

    public Stars stars;
   
    private float elapsedTime;
    private List<GameObject> trashOverlays = new List<GameObject>();

    void Start(){
        stars = new Stars(this);
    }

    //Awake körs innan Start och gör att Data hinner få korrekta värden 
    //innan den börjar användas
    void Awake(){
        Data.Instance.Init();

        if (!Application.isEditor)   
        {
            Application.targetFrameRate = 300;
        }

        gridWidth = Data.Instance.gridWidth;
        gridHeight = Data.Instance.gridHeight;

        saveManager = new SaveManager(this);
        creator = new Creator(this);
        resourceManager = new ResourceManager(this);

        buildingGrid = new Grid<Building>(gridWidth, gridHeight, 1f);
        resourceGrid = new Grid<Resource>(gridWidth, gridHeight, 1f);

        worldBorder = Instantiate(worldBorder);
        worldBorder.GetComponent<WorldBorder>().Init(this);

        saveManager.LoadGame();

        elapsedTime = 0f;        
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;                  
        if (elapsedTime >= gameSpeed)
        {
            stars.Update(elapsedTime);
            elapsedTime = elapsedTime % gameSpeed;
            safeToRemove = false;
            while (!safeToMove) { }
            CheckMachines();
            CustomActions();
            CheckMovement(); // Iterates over currentMovables and marks which can move and which cant.
            QueueMovement(); // Queues movement.
            Move(); // Clears queue and does all movement.
            safeToRemove = true;
            saveManager.SaveGame();
        }
    }

    public void AccomplishedGoal()
    {
        accomplishedGoal();
    }

    // Movement
    void CheckMovement()
    {
        // see who can move
        foreach (Movable building in currentMovables)
        {
            // cant move if there is no next position and has resources.
            building.canMove = (
                building.hasNextPos && buildingGrid.GetObj(building.NextPosition) != null
            );
        }

        // Testa positionerna av grejer och se vad som vill/inte vill röra på sig
        // foreach (Movable building in currentMovables)
        // {
        //     Debug.Log("" + building.canMove + building + building.position);
        // }

        // disable movement for all previous blocks
        foreach (Movable building in currentMovables)
        {
            if (!building.canMove && building.item != null)
            {
                Vector2Int prevPos = building.PrevPosition;
                while (
                    ((Movable)buildingGrid.GetObj(prevPos)) != null
                    && ((Movable)buildingGrid.GetObj(prevPos)).item != null
                )
                {
                    ((Movable)buildingGrid.GetObj(prevPos)).canMove = false;
                    prevPos = ((Movable)buildingGrid.GetObj(prevPos)).PrevPosition;

                    if (!((Movable)buildingGrid.GetObj(prevPos)).hasPrevPos)
                    {
                        ((Movable)buildingGrid.GetObj(prevPos)).canMove = false;
                        break;
                    }
                }
            }
        }
    }

    void CheckMachines()
    {
        foreach (Miner miner in currentMachines)
        {
            if (miner != null)
            {
                Movable next = ((Movable)buildingGrid.GetObj(miner.NextPosition));

                if (miner.hasNextPos && miner.canMove)
                {
                    creator.CreateItem(miner, miner.color);
                }
            }
        }
    }

    void QueueMovement()
    {
        foreach (Movable building in currentMovables)
        {
            if (building.item != null && building.canMove)
            {
                Movable nextBuilding = (Movable)buildingGrid.GetObj(building.NextPosition);
                if (nextBuilding.movable)
                {
                    nextBuilding.itemQueue = building.item;
                    building.item = null;
                }
            }
        }
    }

    // Borde köras mer som en egen grej, vissa custom action har sidoeffekter (merger)
    // Just nu blir det problem med att man inte tar hänsyn till allt sånt tror jag
    // Ska köra om itemet inte är null, men finns mer restriktioner tror jag för att move sen ska gilla mergern
    void CustomActions()
    {
        foreach (Movable building in currentMovables)
        {
            if (
                building.item != null || (building.movable == false) // movable är för att merger bankar på sakerna utanför typ
            )
            {
                building.CustomAction();
            }
        }
    }

    void Move()
    {
        // move
        foreach (Movable building in currentMovables)
        {
            if (
                building.GetType().Equals(typeof(QuarterBase))
                && ((QuarterBase)building).previousItem != null
            )
            {
                StartCoroutine(((QuarterBase)building).DestroyItem());
            }
            if (building.itemQueue != null)
            {
                // Debug.Log(building);
                //Debug.Log(building.itemQueue);
                // Debug.Log(building.transform.position);
                building.item = building.itemQueue;
                building.itemQueue = null;

                if (safeToMove && building != null)
                {
                    StartCoroutine(building.moveItem(building.transform.position));
                }
                // building.CustomAction();
            }
        }
    }

    public void ShowTrashOverlay()
    {
        foreach (Movable building in currentMovables)
        {
            if (!building.GetType().Equals(typeof(QuarterBase)))
            {
                GameObject trashOverlay = Instantiate(Data.Instance.prefabs["Trash Overlay"]);
                trashOverlay.transform.SetParent(building.transform);
                trashOverlay.transform.localPosition = Vector3.zero;
                trashOverlays.Add(trashOverlay);
            }
        }
    }

    public void HideTrashOverlay()
    {
        foreach (GameObject trashOverlay in trashOverlays)
        {
            Destroy(trashOverlay);
        }
    }

    private IEnumerator startDoubleGameSpeed()
    {
        RewardedAdsButton.rewardedAdsPopUp.SetActive(false);
        gameSpeed = gameSpeed / 2;
        yield return new WaitForSeconds(120f);
        gameSpeed = gameSpeed * 2;
        StopCoroutine(gameSpeedCoroutine);
    }

    public void getDoubleGameSpeed()
    {
        gameSpeedCoroutine = StartCoroutine(startDoubleGameSpeed());
    }
}
