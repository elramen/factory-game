using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
using System.ComponentModel;
using System.Linq;
using UnityEngine.UI;

// CHANGES HAVE BEEN MADE

public class InputManager : MonoBehaviour
{
    // References
    public Game game;
    public CameraManager cameraManager;
    public VisualGrid backgroundGrid;

    public int previousMode = 0;
    public int mode;
    public int building;

    // Variables
    public UserInput userInput;
    private InputAction touchIsPressed;
    private InputAction touchPosition;
    private InputAction secondTouchIsPressed;
    private InputAction secondTouchPosition;
    private bool zoomIsRunning;
    private Coroutine zoomCoroutine;
    private Coroutine dragAndDropRoutine;
    private Coroutine basePopupRoutine;
    public bool basePopupIsActive;
    public float zoomSpeed;
    private bool firstZoom;
    bool trashOverLaysShowing;
    bool isBuildingBelts;
    private int modeBackUp;
    public Button belt;
    public GameObject customPopUp;

    // TODO: Ersätt "game.buildingGrid.WithinBounds(gridPosition)" med något vackrare


    void Awake()
    {
        userInput = new UserInput();

        touchIsPressed = userInput.Player.TouchIsPressed;
        touchPosition = userInput.Player.TouchPosition;
        secondTouchIsPressed = userInput.Player.SecondTouchIsPressed;
        secondTouchPosition = userInput.Player.SecondTouchPosition;
        secondTouchIsPressed.started += _ => ZoomStart();
        touchIsPressed.started += _ => TouchStart(); // byt till interact eller liknande
        zoomIsRunning = false;
        zoomSpeed = 0.005f;
        firstZoom = true;
        trashOverLaysShowing = false;
        isBuildingBelts = false;
        basePopupIsActive = false;
    }

    public void SetPreviousMode()
    {
        if (trashOverLaysShowing)
        {
            game.HideTrashOverlay();
            trashOverLaysShowing = false;
        }
        // if (modeBackUp == previousMode)
        // {
        //     this.mode = 0;
        //     HideBuildOverlay();
        // }
        // else
        // {
        //     this.mode = previousMode;
        //     ShowBuildOverlay();
        // }
    }

    public void SetMode(int mode)
    {
        this.mode = mode;

        if (mode == 3)
        {
            trashOverLaysShowing = true;
            game.ShowTrashOverlay();
        }
    }

    private void ShowBuildOverlay()
    {
        // foreach (Movable mov in game.currentMovables)
        // {
        //     Vector2Int[] neighors = mov.GetNeighbors();
        //     //Output
        //     if(!mov.GetType().Equals(typeof(EmptyBoi))){
        //         if (!mov.hasNextPos)
        //         {
        //             UnityEngine.Transform[] outputIndicators = new UnityEngine.Transform[4];
        //             outputIndicators[0] = mov.transform.Find("left");
        //             outputIndicators[1] = mov.transform.Find("up");
        //             outputIndicators[2] = mov.transform.Find("right");
        //             outputIndicators[3] = mov.transform.Find("down");

        //             if (outputIndicators[0] != null)
        //             {
        //                 for (int i = 0; i < 4; i++)
        //                 {
        //                     Movable neighbor = (Movable)game.buildingGrid.GetObj(neighors[i]);
        //                     if (neighbor == null || (!neighbor.hasPrevPos && !neighbor.GetType().Equals(typeof(Miner)) && !neighbor.GetType().Equals(typeof(Merger))))
        //                     {
        //                         outputIndicators[i].transform.position = new Vector3(neighors[i].x, neighors[i].y, 0);
        //                         outputIndicators[i].gameObject.SetActive(true);
        //                     }
        //                 }
        //             }
        //         }
        //     }

        //     //Input
        //     if (!mov.GetType().Equals(typeof(Miner)) && !mov.GetType().Equals(typeof(Merger))){
        //         if (!mov.hasPrevPos)
        //         {
        //             UnityEngine.Transform arrow = mov.transform.Find("arrowInput");
        //             if (arrow != null)
        //             {
        //                 arrow.gameObject.SetActive(true);
        //             }
        //         }
        //     }
        // }
    }

    private void HideBuildOverlay()
    {
        foreach (Movable mov in game.currentMovables)
        {
            UnityEngine.Transform left = mov.transform.Find("left");
            UnityEngine.Transform right = mov.transform.Find("right");
            UnityEngine.Transform down = mov.transform.Find("down");
            UnityEngine.Transform up = mov.transform.Find("up");
            UnityEngine.Transform arrowIn = mov.transform.Find("arrowInput");

            if (arrowIn != null)
            {
                if (mov.hasPrevPos && arrowIn.gameObject.activeSelf)
                {
                    arrowIn.gameObject.SetActive(false);
                }
                else if (!mov.hasPrevPos)
                {
                    arrowIn.gameObject.SetActive(false);
                }
            }
            if (up != null)
            {
                if (
                    mov.hasNextPos
                    && (
                        left.gameObject.activeSelf
                        || right.gameObject.activeSelf
                        || down.gameObject.activeSelf
                        || up.gameObject.activeSelf
                    )
                )
                {
                    left.gameObject.SetActive(false);
                    right.gameObject.SetActive(false);
                    down.gameObject.SetActive(false);
                    up.gameObject.SetActive(false);
                }
                else if (!mov.hasNextPos)
                {
                    left.gameObject.SetActive(false);
                    right.gameObject.SetActive(false);
                    down.gameObject.SetActive(false);
                    up.gameObject.SetActive(false);
                }
            }
        }
    }

    void OnEnable()
    {
        userInput.Enable();
    }

    void OnDisable()
    {
        userInput.Disable();
    }

    private void ZoomStart()
    {
        zoomCoroutine = StartCoroutine(ZoomUpdate());
    }

    private IEnumerator ZoomUpdate()
    {
        zoomIsRunning = true;
        if (firstZoom)
        {
            firstZoom = false;
            yield return null;
        }
        float distance = GetTouchDistance();
        float previousDistance = distance;
        float deltaDistance = 0f;
        while (
            secondTouchIsPressed.ReadValue<float>() != 0 && touchIsPressed.ReadValue<float>() != 0
        )
        {
            distance = GetTouchDistance();
            deltaDistance = Mathf.Abs(distance - previousDistance);
            if (distance > previousDistance)
            {
                cameraManager.screenUnitWidth = Mathf.Max(
                    cameraManager.screenUnitWidth - zoomSpeed * deltaDistance,
                    5f
                );
                cameraManager.SetCameraSize();
            }
            else if (distance < previousDistance)
            {
                cameraManager.screenUnitWidth = Mathf.Min(
                    cameraManager.screenUnitWidth + zoomSpeed * deltaDistance,
                    10f
                );
                cameraManager.SetCameraSize();
                Vector2 cameraMin = GetCameraMin();
                Vector2 cameraMax = GetCameraMax();
                cameraManager.transform.position = new Vector3(
                    Mathf.Clamp(Camera.main.transform.position.x, cameraMin.x, cameraMax.x),
                    Mathf.Clamp(Camera.main.transform.position.y, cameraMin.y, cameraMax.y),
                    -1
                );
            }
            previousDistance = distance;
            yield return null;
        }
        zoomIsRunning = false;
        if (secondTouchIsPressed.ReadValue<float>() != 0 || touchIsPressed.ReadValue<float>() != 0)
        {
            StartCoroutine(TouchMove());
        }
        StopCoroutine(zoomCoroutine);
        yield return null;
    }

    private float GetTouchDistance()
    {
        return Vector2.Distance(
            touchPosition.ReadValue<Vector2>(),
            secondTouchPosition.ReadValue<Vector2>()
        );
    }

    // Runs once when finger is down
    private void TouchStart()
    {
        if (!zoomIsRunning)
        {
            StartCoroutine(TouchUpdate());
        }
    }

    // Loops while finger is down
    private IEnumerator TouchUpdate()
    {
        yield return new WaitForEndOfFrame();
        if (EventSystem.current.IsPointerOverGameObject())
        {
            yield return null;
        }
        else
        {
            switch (mode)
            {
                case 0:
                    yield return TouchMove();
                    break;
                case 1:
                    yield return TouchBelt();
                    break;
                case 2:
                    yield return TouchMove();
                    break;
                case 3:
                    yield return TouchDestroy();
                    break;
                default:
                    break;
            }
        }
        modeBackUp = mode;
    }

    private Vector2 GetCameraMin()
    {
        // TODO - Förstå varför offset behövs
        Vector2 offset =
            0.5f * new Vector2(game.buildingGrid.getCellSize(), game.buildingGrid.getCellSize());

        return new Vector2(
                (-game.buildingGrid.size().x + cameraManager.screenUnitWidth) / 2,
                (-game.buildingGrid.size().y + cameraManager.screenUnitHeight) / 2
            )
            - game.worldBorder.GetComponent<WorldBorder>().GetExtension()
            - offset;
    }

    private Vector2 GetCameraMax()
    {
        Vector2 offset =
            0.5f * new Vector2(game.buildingGrid.getCellSize(), game.buildingGrid.getCellSize());

        return new Vector2(
                (game.buildingGrid.size().x - cameraManager.screenUnitWidth) / 2,
                (game.buildingGrid.size().y - cameraManager.screenUnitHeight) / 2
            )
            + game.worldBorder.GetComponent<WorldBorder>().GetExtension()
            - offset;
    }

    private IEnumerator TouchMove()
    {
        Vector2 startPosition;
        if (touchIsPressed.ReadValue<float>() != 0)
        {
            startPosition = touchPosition.ReadValue<Vector2>();
        }
        else
        {
            startPosition = secondTouchPosition.ReadValue<Vector2>();
        }
        Vector3 cameraStartPosition = Camera.main.transform.position;

        Vector2 cameraMin = GetCameraMin();
        Vector2 cameraMax = GetCameraMax();

        while (true)
        {
            Vector2 currentPosition;
            if (
                touchIsPressed.ReadValue<float>() != 0
                && secondTouchIsPressed.ReadValue<float>() != 0
            )
            {
                zoomCoroutine = StartCoroutine(ZoomUpdate());
                break;
            }
            else if (touchIsPressed.ReadValue<float>() != 0)
            {
                currentPosition = touchPosition.ReadValue<Vector2>();
            }
            else if (secondTouchIsPressed.ReadValue<float>() != 0)
            {
                currentPosition = secondTouchPosition.ReadValue<Vector2>();
            }
            else
            {
                break;
            }

            baseIsTouched(currentPosition);

            Vector3 scaledChange = cameraManager.PixelVectorToUnits(
                currentPosition - startPosition
            );

            Vector2 cameraPosition = cameraStartPosition - scaledChange;
            float cameraPositionX = cameraPosition.x;
            float cameraPositionY = cameraPosition.y;

            // make sure the camera doesn't go beyond the grid
            cameraPositionX = Math.Max(cameraPositionX, cameraMin.x);
            cameraPositionX = Math.Min(cameraPositionX, cameraMax.x);
            cameraPositionY = Math.Max(cameraPositionY, cameraMin.y);
            cameraPositionY = Math.Min(cameraPositionY, cameraMax.y);

            cameraManager.transform.position = new Vector3(cameraPositionX, cameraPositionY, -1);

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator TouchDestroy()
    {
        while (touchIsPressed.ReadValue<float>() != 0 && !zoomIsRunning)
        {
            Vector2 position = Camera.main.ScreenToWorldPoint(touchPosition.ReadValue<Vector2>());
            Vector2Int gridPosition = game.buildingGrid.PositionToGridPosition(position);

            // TODO: fixa fancy destroy funktioner
            if (game.buildingGrid.GetObj(gridPosition) != null)
            {
                Building movable = (Building)game.buildingGrid.GetObj(gridPosition);
                while (!game.safeToRemove)
                {
                    yield return null;
                }
                game.safeToMove = false;
                movable.RemoveBuilding();
                game.safeToMove = true;
            }
            yield return new WaitForEndOfFrame();
        }
        // game.HideTrashOverlay();
        // trashOverLaysShowing = false;
        // this.mode = 0;
        yield return null;
    }

    private IEnumerator TouchBelt()
    {
        GameObject menubar = GameObject.Find("Menubar");
        Canvas menubar_canvas = menubar.GetComponent<Canvas>();
        menubar_canvas.enabled = false;
        GameObject fingerCircle = new GameObject("Circle");
        SpriteRenderer fingerCircleSR = fingerCircle.AddComponent<SpriteRenderer>();
        fingerCircleSR.sprite = Data.Instance.sprites["BeltLonley"];
        fingerCircleSR.sortingOrder = 9;

        GameObject fingerLine = new GameObject("Line");
        SpriteRenderer fingerLineSR = fingerLine.AddComponent<SpriteRenderer>();
        fingerLineSR.sprite = Data.Instance.sprites["BeltStraight"];
        fingerLineSR.sortingOrder = 10;

        Vector2 lastPosition = Vector2.zero;
        bool firstIteration = true;
        Vector2Int? lastGridPosition = null;
        //autoconnect

        bool lastPosIsBuilding = true;
        Vector2Int startGridPosition = Vector2Int.zero;
        while (touchIsPressed.ReadValue<float>() != 0 && !zoomIsRunning)
        {
            Vector2 position = Camera.main.ScreenToWorldPoint(touchPosition.ReadValue<Vector2>());
            Vector2Int gridPosition = game.buildingGrid.PositionToGridPosition(position);

            if (firstIteration)
            {
                firstIteration = false;
                lastPosition = position;
                startGridPosition = gridPosition;
            }
            // TODO: Kolla att det inte blir loopar, typ ha en for-loop som går tillbaka och ser att man inte kommer in på
            // position available
            // no previous position or current position separate from previous
            fingerCircle.transform.position = position;
            fingerLine.transform.position = (position + lastPosition) / 2;
            fingerLine.transform.localScale = new Vector3(
                (position - lastPosition).magnitude,
                1,
                1
            );
            fingerLine.transform.eulerAngles = new Vector3(
                0,
                0,
                -180 - Vector2.SignedAngle(position - lastPosition, Vector2.right)
            );
            // Man petar på nån ny ruta
            if (
                (!lastGridPosition.HasValue || (gridPosition != lastGridPosition))
                && game.buildingGrid.WithinBounds(gridPosition)
            )
            {
                // Project the direction down to up, down, left or right if it is too far away (to help fast moving fingers!) --
                if (
                    lastGridPosition.HasValue
                    && (gridPosition != lastGridPosition)
                    && (gridPosition - (Vector2Int)lastGridPosition).magnitude != 1
                )
                {
                    bool xIsLarger = (
                        Math.Abs(((Vector2Int)lastGridPosition).x - gridPosition.x)
                        > Math.Abs(((Vector2Int)lastGridPosition).y - gridPosition.y)
                    );
                    if (xIsLarger)
                    {
                        gridPosition =
                            (Vector2Int)lastGridPosition
                            + new Vector2Int(
                                Math.Sign(gridPosition.x - ((Vector2Int)lastGridPosition).x),
                                0
                            );
                    }
                    else
                    {
                        gridPosition =
                            (Vector2Int)lastGridPosition
                            + new Vector2Int(
                                0,
                                Math.Sign(gridPosition.y - ((Vector2Int)lastGridPosition).y)
                            );
                    }
                }

                // Det man petar på är inte en byggnad
                if (game.buildingGrid.GetObj(gridPosition) == null)
                {
                    lastPosIsBuilding = false;
                    Vector2Int prevPosition = gridPosition;

                    if (lastGridPosition.HasValue)
                    {
                        Building previousBuilding = game.buildingGrid.GetObj(
                            (Vector2Int)lastGridPosition
                        );
                        if (
                            previousBuilding.GetType().IsSubclassOf(typeof(Movable))
                            && !previousBuilding.GetType().Equals(typeof(EmptyBoi))
                            && !previousBuilding.GetType().Equals(typeof(QuarterBase))
                        )
                        {
                            prevPosition = (Vector2Int)lastGridPosition;
                        }
                    }

                    ConveyorBelt currentBelt = game.creator.CreateConveyorBelt(
                        gridPosition,
                        gridPosition,
                        prevPosition
                    );
                    if (
                        lastGridPosition.HasValue
                        && game.buildingGrid
                            .GetObj((Vector2Int)lastGridPosition)
                            .GetType()
                            .IsSubclassOf(typeof(Movable))
                    )
                    {
                        Movable previousBuilding = (Movable)game.buildingGrid.GetObj(
                            (Vector2Int)lastGridPosition
                        );
                        if (
                            previousBuilding != null
                            && !previousBuilding.GetType().Equals(typeof(EmptyBoi))
                            && !previousBuilding.GetType().Equals(typeof(QuarterBase))
                            && previousBuilding.GetType().IsSubclassOf(typeof(Movable))
                        )
                        {
                            if (
                                ((Movable)game.buildingGrid.GetObj(previousBuilding.NextPosition))
                                    .GetType()
                                    .Equals(typeof(QuarterBase))
                            )
                            {
                                (
                                    (Movable)game.buildingGrid.GetObj(previousBuilding.NextPosition)
                                ).hasPrevPos = false;
                            }
                            previousBuilding.NextPosition = gridPosition;
                            previousBuilding.hasNextPos = true;
                            currentBelt.PrevPosition = (Vector2Int)lastGridPosition;
                            previousBuilding.AutoRotate();

                            // Update the underlying sprites
                            previousBuilding.updateDummyBelt();
                        }
                    }
                    else
                    {
                        currentBelt.AutoConnect();
                    }
                    // Update the underlying sprites
                    currentBelt.updateDummyBelt();
                }
                else // du rullar in på en byggnad
                {
                    lastPosIsBuilding = true;

                    // Handle non-movables (barrier)
                    if (
                        (
                            !game.buildingGrid
                                .GetObj(gridPosition)
                                .GetType()
                                .IsSubclassOf(typeof(Movable))
                        ) || false
                    )
                    {
                        lastPosition = game.buildingGrid.GridPositionToPosition(gridPosition);
                        lastGridPosition = gridPosition;
                        continue;
                    }

                    Movable currentBuilding = (Movable)game.buildingGrid.GetObj(gridPosition);
                    if (
                        lastGridPosition.HasValue
                        && game.buildingGrid.GetObj((Vector2Int)lastGridPosition) != null
                        && game.buildingGrid
                            .GetObj((Vector2Int)lastGridPosition)
                            .GetType()
                            .IsSubclassOf(typeof(Movable))
                    )
                    {
                        Movable previousBuilding = (Movable)game.buildingGrid.GetObj(
                            (Vector2Int)lastGridPosition
                        );
                        // Do not replace existing values!
                        // TODO: tycker vi ska göra om det här så att det är skalvänligt, om man skulle vilja flytta basen.
                        Vector2Int[] quarterBasePrevPositions =
                        {
                            new Vector2Int(0, 2),
                            new Vector2Int(2, 0),
                            new Vector2Int(0, -2),
                            new Vector2Int(-2, 0),
                        };

                        if (
                            //!currentBuilding.hasPrevPos &&
                            !(
                                currentBuilding.GetType().Equals(typeof(QuarterBase))
                                && !(quarterBasePrevPositions.Contains(previousBuilding.position))
                            )
                            && !(
                                // TODO: Fixa en lista för det här, och en till för hasNextPosition
                                // (Den andra behövs mest om man ska fixa merger på ett bra sätt)
                                // Ctrl-F:a #Fix1

                                // Byggnader man inte ska fucka med deras previous position
                                currentBuilding.GetType().Equals(typeof(Miner))
                                || currentBuilding.GetType().Equals(typeof(Merger))
                                || (
                                    previousBuilding.GetType().Equals(typeof(Merger))
                                    && currentBuilding.GetType().Equals(typeof(EmptyBoi))
                                    && (
                                        currentBuilding.position
                                            == (previousBuilding.position - new Vector2Int(1, 0))
                                        || currentBuilding.position
                                            == (previousBuilding.position + new Vector2Int(1, 0))
                                    )
                                )
                                || previousBuilding.GetType().Equals(typeof(EmptyBoi))
                                || previousBuilding.GetType().Equals(typeof(QuarterBase))
                            )
                            // TODO: bättre kommentar, kan inte dra ett converyor belt som pekar på belt det just kom ifrån.
                            && previousBuilding.PrevPosition != currentBuilding.position
                        )
                        {
                            Debug.Log("tja");
                            Debug.Log("Current: " + currentBuilding.GetType());
                            Debug.Log("Prev: " + previousBuilding.GetType());

                            // Bug fix for base
                            if (currentBuilding.GetType().Equals(typeof(QuarterBase)))
                            {
                                if (previousBuilding.hasNextPos)
                                {
                                    Movable other = (Movable)game.buildingGrid.GetObj(
                                        previousBuilding.NextPosition
                                    );
                                    if (other.hasPrevPos)
                                    {
                                        other.PrevPosition = other.position;
                                    }
                                }
                            }

                            if (
                                ((Movable)game.buildingGrid.GetObj(previousBuilding.NextPosition))
                                    .GetType()
                                    .Equals(typeof(QuarterBase))
                            )
                            {
                                (
                                    (Movable)game.buildingGrid.GetObj(previousBuilding.NextPosition)
                                ).hasPrevPos = false;
                            }
                            Movable currentInputBuilding = (Movable)game.buildingGrid.GetObj(
                                currentBuilding.PrevPosition
                            );
                            if (
                                currentInputBuilding != null
                                && currentInputBuilding != currentBuilding
                            )
                            {
                                Debug.Log("CIBP: " + currentInputBuilding.position);
                                currentInputBuilding.hasNextPos = false;
                                currentInputBuilding.NextPosition = currentInputBuilding.position;
                            }
                            previousBuilding.NextPosition = gridPosition;
                            currentBuilding.PrevPosition = (Vector2Int)lastGridPosition;
                            currentBuilding.hasPrevPos = true;
                            // Update the underlying sprites
                            previousBuilding.updateDummyBelt();
                            currentBuilding.updateDummyBelt();
                        }
                    }
                    currentBuilding.AutoRotate();

                    // TODO: Om det är en miner så vill vi inte sätta previousPosition
                }
                // Sätt alltid last position om det är en ny ruta man rullar in på
                lastPosition = game.buildingGrid.GridPositionToPosition(gridPosition);
                lastGridPosition = gridPosition;
                HideBuildOverlay();
                ShowBuildOverlay();
            }
            yield return new WaitForEndOfFrame();
        }
        if (zoomIsRunning && startGridPosition == lastGridPosition && !lastPosIsBuilding)
        {
            (
                (Movable)game.buildingGrid.GetObj(
                    game.buildingGrid.PositionToGridPosition(lastPosition)
                )
            ).RemoveBuilding();
        }
        else if (!lastPosIsBuilding)
        {
            (
                (Movable)game.buildingGrid.GetObj(
                    game.buildingGrid.PositionToGridPosition(lastPosition)
                )
            ).AutoConnect();
            HideBuildOverlay();
            ShowBuildOverlay();
        }
        Destroy(fingerCircle);
        Destroy(fingerLine);
        menubar_canvas.enabled = true;
        yield return null;
    }

    public void StartDrag()
    {
        dragAndDropRoutine = StartCoroutine(DragAndDropMachine());
    }

    private IEnumerator DragAndDropMachine()
    {
        // GameObject ui = GameObject.Find("UI");
        // ui.SetActive(false);
        GameObject menubar = GameObject.Find("Menubar");
        Canvas menubar_canvas = menubar.GetComponent<Canvas>();
        menubar_canvas.enabled = false;

        Vector2 position = Camera.main.ScreenToWorldPoint(touchPosition.ReadValue<Vector2>());
        Vector2Int gridPosition;
        GameObject dragObject = new GameObject("dragObject");
        SpriteRenderer dragObjectSR = dragObject.AddComponent<SpriteRenderer>();
        dragObjectSR.sortingOrder = 100;
        dragObjectSR.color = new Color(1f, 1f, 1f, .7f);
        dragObject.transform.position = position;

        while (touchIsPressed.ReadValue<float>() != 0)
        {
            switch (building)
            {
                case 0:
                    dragObjectSR.sprite = Data.Instance.sprites["Store Miner"];
                    break;
                case 1:
                    dragObjectSR.sprite = Data.Instance.sprites["Store Splitter"];
                    break;
                case 2:
                    dragObjectSR.sprite = Data.Instance.sprites["Store Rotator"];
                    break;
                case 3:
                    dragObjectSR.sprite = Data.Instance.sprites["Store Merger"];
                    break;
                case 4:
                    dragObjectSR.sprite = Data.Instance.sprites["Store Heart Cutter"];
                    break;
            }
            position = Camera.main.ScreenToWorldPoint(touchPosition.ReadValue<Vector2>());
            dragObject.transform.position = position;
            yield return new WaitForEndOfFrame();
        }
        gridPosition = game.buildingGrid.PositionToGridPosition(position);
        if (!zoomIsRunning)
        {
            Building currentBuilding = (Building)game.buildingGrid.GetObj(gridPosition);

            if (
                (currentBuilding == null || currentBuilding.GetType().Equals(typeof(ConveyorBelt)))
                && game.buildingGrid.WithinBounds(gridPosition)
            )
            {
                // TODO:
                // Switch case är dumma med variabelnamn, så "thing" blir rätt dumt
                // Man vill kanske ändra till if-else även om det är fult

                if (currentBuilding != null)
                {
                    if (building == 0 && game.resourceGrid.GetObj(gridPosition) != null)
                    {
                        currentBuilding.RemoveBuilding();
                    }
                    else if (building != 0)
                    {
                        currentBuilding.RemoveBuilding();
                    }
                }

                switch (building)
                {
                    case 0:
                        if (game.resourceGrid.GetObj(gridPosition) != null)
                        {
                            Movable thing = (Movable)game.creator.CreateMiner(
                                gridPosition,
                                gridPosition,
                                gridPosition,
                                game.resourceGrid.GetObj(gridPosition).color
                            );
                            thing.updateDummyBelt();
                        }
                        else
                        {
                            displaycustomPopUp("A miner must be placed on a resource");
                        }
                        break;
                    case 1:
                        Movable thing0 = (Movable)game.creator.CreateSplitter(
                            gridPosition,
                            gridPosition,
                            gridPosition
                        );
                        thing0.updateDummyBelt();
                        break;
                    case 2:
                        Movable thing1 = (Movable)game.creator.CreateRotator(
                            gridPosition,
                            gridPosition,
                            gridPosition
                        );
                        thing1.updateDummyBelt();
                        break;
                    case 3:
                        // TODO: Spawna byggnader på de här positionerna så man kan ha någon aning om hur prylen funkar
                        // Började försöka med det med EmptyBoi

                        Vector2Int stampPosition = new Vector2Int(
                            gridPosition.x + 1,
                            gridPosition.y
                        );
                        Vector2Int paperPosition = new Vector2Int(
                            gridPosition.x - 1,
                            gridPosition.y
                        );

                        if (
                            game.buildingGrid.GetObj(stampPosition) == null
                            && game.buildingGrid.GetObj(paperPosition) == null
                            && game.buildingGrid.WithinBounds(stampPosition)
                            && game.buildingGrid.WithinBounds(paperPosition)
                        )
                        {
                            Movable thing4 = (Movable)game.creator.CreateMerger(
                                gridPosition,
                                gridPosition,
                                gridPosition,
                                stampPosition,
                                stampPosition,
                                paperPosition,
                                paperPosition
                            );
                        }
                        else
                        {
                            displaycustomPopUp("A merger needs 3 empty tiles");
                        }

                        break;
                    case 4:
                        Movable thing2 = (Movable)game.creator.CreateHeartCutter(
                            gridPosition,
                            gridPosition,
                            gridPosition
                        );
                        thing2.updateDummyBelt();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // displaycustomPopUp("Try to build on an empty tile");
            }
        }

        menubar_canvas.enabled = true;

        Destroy(dragObject);
        StopCoroutine(dragAndDropRoutine);
        // HideBuildOverlay();
        // this.mode = 0;
        // ui.SetActive(true);
        yield return null;
    }

    private void displaycustomPopUp(String message)
    {
        String buildingName;
        GameObject img = customPopUp.transform.Find("Image").gameObject;
        img.SetActive(false);
        customPopUp.SetActive(true);
        TMPro.TextMeshProUGUI text = customPopUp.transform
            .GetChild(0)
            .GetComponent<TMPro.TextMeshProUGUI>();

        switch (building)
        {
            case 0:
                buildingName = "miner. ";
                break;
            case 1:
                buildingName = "splitter. ";
                break;
            case 2:
                buildingName = "rotator. ";
                break;
            case 3:
                buildingName = "merger. ";
                break;
            case 4:
                buildingName = "heart cutter. ";
                break;
            default:
                buildingName = "buildning. ";
                break;
        }
        text.SetText("Unable to build " + buildingName + message);
    }

    private void baseIsTouched(Vector2 currentPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(currentPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            GameObject bas = GameObject.Find("BaseContainer").transform.Find("Base").gameObject;
            Base baseScript = bas.GetComponent<Base>();
            GameObject img = customPopUp.transform.Find("Image").gameObject;
            img.SetActive(true);
            TMPro.TextMeshProUGUI text = customPopUp.transform
                .GetChild(0)
                .GetComponent<TMPro.TextMeshProUGUI>();
            customPopUp.SetActive(true);
            basePopupIsActive = true;
            basePopupRoutine = StartCoroutine(updateBasePopup(bas, baseScript, img, text));
        }
    }

    // TODO: Kolla på.
    private IEnumerator updateBasePopup(
        GameObject bas,
        Base baseScript,
        GameObject img,
        TMPro.TextMeshProUGUI text
    )
    {
        while (basePopupIsActive)
        {
            SpriteRenderer goalImage = baseScript.goals[
                Math.Min(Data.Instance.currentGoal, 14)
            ].item.gameObject.GetComponent<SpriteRenderer>();
            img.GetComponent<Image>().sprite = goalImage.sprite;
            int goal = baseScript.goals[Math.Min(Data.Instance.currentGoal, 14)].amount;
            text.SetText(baseScript.collectedAmount + "/" + goal);
            //Debug.Log(baseScript.collectedAmount + "/" + goal);
            yield return new WaitForSeconds(0.1f);
        }
        StopCoroutine(basePopupRoutine);
    }

    public void stopBasePopupCorouutine()
    {
        basePopupIsActive = false;
    }
}
