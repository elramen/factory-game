using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movable : Building
{
    protected Vector2Int prevPosition;
    protected Vector2Int nextPosition;

    public Item item;
    public Item itemQueue;

    public bool hasPrevPos = false;
    public bool hasNextPos = false;
    public bool canMove;

    // Än så länge är bara merger inte movable, för att den ska göra funky grejer med sina förflyttningar men pekarna måste funka.
    // Borde miner vara samma, eftersom movable är typ att man inte kan movea in saker på objektet?
    public bool movable = true;

    public virtual void CustomAction()
    {
        return;
    }

    public DummyBelt dummyBelt;

    // Används av maskiner för att det ska se ut som ett bält under typ, maskiner borde kanske vara en egen klass sen
    public void updateDummyBelt()
    {
        if (dummyBelt != null) // dummy belt exists then update its values
        {
            dummyBelt.Init(this.position, NextPosition, PrevPosition);
        }
        else // Create a dummy belt
        {
            GameObject newObject = Instantiate(Data.Instance.buildings["Dummy Belt"]);
            newObject.transform.parent = transform;

            newObject.transform.localPosition = new Vector3(0, 0, 0);

            DummyBelt belt = newObject.GetComponent<DummyBelt>();

            belt.Init(this.position, NextPosition, PrevPosition);
            dummyBelt = belt;
        }
    }

    public virtual IEnumerator moveItem(Vector2 endPosition)
    {
        item.isMoving = true;
        Vector3 startPosition = item.transform.position;
        Vector3 change = ((Vector3)endPosition - startPosition);

        float timeElapsed = 0f;
        bool itemRemoved = false;

        while (timeElapsed < movementTime)
        {
            if (item.deathRow)
            {
                Destroy(this.item.gameObject);
                itemRemoved = true;
                break;
            }
            item.transform.position =
                startPosition + change * movement.Evaluate(timeElapsed / movementTime);

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (!itemRemoved)
        {
            item.transform.position = (Vector3)endPosition;
            item.isMoving = false;
        }

        yield return null;
    }

    public override void RemoveBuilding()
    {
        if (this.GetType().Equals(typeof(QuarterBase)))
        {
            return;
        }
        if (hasPrevPos)
        {
            Movable prevObj = (Movable)grid.GetObj(PrevPosition);
            prevObj.hasNextPos = false;
            prevObj.NextPosition = prevObj.position;
        }
        if (hasNextPos)
        {
            Movable nextObj = (Movable)grid.GetObj(NextPosition);
            nextObj.hasPrevPos = false;
            if (!nextObj.GetType().Equals(typeof(QuarterBase)))
            {
                nextObj.PrevPosition = nextObj.position;
            }
        }

        grid.AddObj(null, position);

        if (this.item != null)
        {
            if (this.item.isMoving)
            {
                this.item.deathRow = true;
            }
            else
            {
                Destroy(this.item.gameObject);
            }
        }

        if (transform.parent.GetComponent<Game>() != null)
        {
            // Ganska dumt men okej då.
            Game game = transform.parent.GetComponent<Game>();
            game.currentMovables.Remove(this);
        }

        Destroy(this.gameObject);
    }

    public Vector2Int PrevPosition // property
    {
        get { return this.prevPosition; } // get method
        set
        {
            Vector2Int prevPrevPosition = prevPosition;
            bool prevHasPrevPosition = hasPrevPos;

            prevPosition = value;
            hasPrevPos = (prevPosition != position);
            updateDummyBelt();

            // reset potential earlier connection
            // if (prevHasPrevPosition)
            // {
            //     Movable other = (Movable)grid.GetObj(prevPrevPosition);
            //     other.ResetNextPosition();
            // }
        }
    }

    public Vector2Int NextPosition // property
    {
        get { return this.nextPosition; }
        set
        {
            Vector2Int prevNextPosition = nextPosition;
            bool prevHasNextPosition = hasNextPos;

            this.nextPosition = value;
            this.hasNextPos = (this.nextPosition != this.position);
            this.updateDummyBelt();
            // reset potential earlier connection
            if (prevHasNextPosition)
            {
                Movable other = (Movable)grid.GetObj(prevNextPosition);
                if (other != null && !other.GetType().Equals(typeof(QuarterBase)))
                {
                    other.ResetPrevPosition();
                }
            }
        }
    }

    public Vector2Int[] GetNeighbors()
    {
        return new Vector2Int[4]
        {
            this.position + new Vector2Int(-1, 0),
            this.position + new Vector2Int(0, 1),
            this.position + new Vector2Int(1, 0),
            this.position + new Vector2Int(0, -1)
        };
    }

    // TODO: All connect-kod (det som finns i input-manager också) borde abstraheras så att allting kör
    // Connect-funktionen som tar två objekt, och checksen för vilka objekt som ska göras grejer mer sker där
    // (kanske ska ha en true/false flagga för om auto-connect eller nåt, venne)

    public void AutoConnect()
    {
        List<Movable> possibleInputNeighbors = new List<Movable>();
        List<Movable> possibleOutputNeighbors = new List<Movable>();

        foreach (Vector2Int neighborPos in GetNeighbors())
        {
            Building neighborBuilding = grid.GetObj(neighborPos);

            if (neighborBuilding != null)
            {
                if (neighborBuilding.GetType().IsSubclassOf(typeof(Movable)) == false) return;

                Movable neighbor = (Movable) neighborBuilding; 

                if (
                    this.GetType().Equals(typeof(ConveyorBelt))
                    && neighbor.GetType().Equals(typeof(ConveyorBelt))
                )
                {
                    if (neighbor.hasPrevPos && this.hasPrevPos)
                    {
                        continue;
                    }
                }
                if (
                    (
                        this.GetType().Equals(typeof(EmptyBoi))
                        && neighbor.GetType().Equals(typeof(Merger))
                    )
                    || (
                        neighbor.GetType().Equals(typeof(EmptyBoi))
                        && this.GetType().Equals(typeof(Merger))
                    )
                )
                {
                    continue;
                }
                if (
                    !neighbor.hasNextPos
                    && !neighbor.GetType().Equals(typeof(QuarterBase))
                    && !this.GetType().Equals(typeof(Merger))
                    && !neighbor.GetType().Equals(typeof(EmptyBoi))
                    && !this.hasPrevPos
                    && !this.GetType().Equals(typeof(Miner))
                )
                {
                    // Debug.Log("InputNeighbor");
                    // Debug.Log("Neighbor pos: " + neighbor.position);
                    // Debug.Log("Neighbor type: "+ neighbor.GetType());
                    if (neighbor.GetType().Equals(typeof(Miner)))
                    {
                        if (!this.hasPrevPos)
                        {
                            possibleInputNeighbors.Add(neighbor);
                            // Debug.Log("Added");
                        }
                    }
                    else if (
                        possibleInputNeighbors.Count == 1
                        && neighbor.GetType().Equals(typeof(ConveyorBelt))
                        && !this.GetType().Equals(typeof(EmptyBoi))
                    )
                    {
                        if (neighbor.hasPrevPos && !possibleInputNeighbors[0].hasPrevPos)
                        {
                            possibleOutputNeighbors.Add(possibleInputNeighbors[0]);
                            possibleInputNeighbors[0] = neighbor;
                            // Debug.Log("Replaced");
                        }
                    }
                    else
                    {
                        possibleInputNeighbors.Add(neighbor);
                        // Debug.Log("Added");
                    }
                }
                if (
                    (!neighbor.hasPrevPos || neighbor.GetType().Equals(typeof(QuarterBase)))
                    && !neighbor.GetType().Equals(typeof(Miner))
                    && !(possibleInputNeighbors.Contains(neighbor))
                    && !(neighbor.nextPosition == this.position)
                    && !neighbor.GetType().Equals(typeof(Merger))
                    && !this.GetType().Equals(typeof(EmptyBoi))
                )
                {
                    // Debug.Log("Output neighbor");
                    // Debug.Log("Neighbor pos: " + neighbor.position);
                    // Debug.Log("Neighbor type: "+ neighbor.GetType());
                    // QuarterBases can only be connected from one previous position
                    if (neighbor.GetType().Equals(typeof(QuarterBase)))
                    {
                        // Debug.Log("neighbor prevpos: " +  neighbor.prevPosition);
                        // Debug.Log("this position: " +  this.position);
                        if (this.position == neighbor.prevPosition)
                        {
                            possibleOutputNeighbors.Add(neighbor);
                            //Debug.Log("Added QuarterBase");
                        }
                    }
                    else
                    {
                        possibleOutputNeighbors.Add(neighbor);
                        //Debug.Log("Added");
                    }
                }
            }
        }
        if (possibleInputNeighbors.Count == 1 && possibleOutputNeighbors.Count == 1)
        {
            foreach (Movable inputNeighbour in possibleInputNeighbors)
            {
                inputNeighbour.NextPosition = this.position;
                this.prevPosition = inputNeighbour.position;
                this.hasPrevPos = true;
                inputNeighbour.hasNextPos = true;
                inputNeighbour.AutoRotate();
            }
            foreach (Movable outputNeighbour in possibleOutputNeighbors)
            {
                outputNeighbour.PrevPosition = this.position;
                this.NextPosition = outputNeighbour.position;
                this.hasNextPos = true;
                outputNeighbour.hasPrevPos = true;
                outputNeighbour.AutoRotate();
            }
            AutoRotate();
        }
        else
        {
            if (possibleInputNeighbors.Count == 1)
            {
                foreach (Movable inputNeighbour in possibleInputNeighbors)
                {
                    if (
                        inputNeighbour.GetType().Equals(typeof(Miner))
                        && this.GetType().Equals(typeof(Miner))
                    )
                    {
                        break;
                    }
                    inputNeighbour.NextPosition = this.position;
                    this.prevPosition = inputNeighbour.position;
                    this.hasPrevPos = true;
                    inputNeighbour.hasNextPos = true;
                    inputNeighbour.AutoRotate();
                }
                AutoRotate();
            }
            if (possibleOutputNeighbors.Count == 1)
            {
                foreach (Movable outputNeighbour in possibleOutputNeighbors)
                {
                    if (this.GetType().Equals(typeof(EmptyBoi)))
                    {
                        break;
                    }
                    outputNeighbour.PrevPosition = this.position;
                    this.NextPosition = outputNeighbour.position;
                    this.hasNextPos = true;
                    outputNeighbour.hasPrevPos = true;
                    outputNeighbour.AutoRotate();
                }
                AutoRotate();
            }
        }

        updateDummyBelt();
    }

    public void AutoRotate()
    {
        Transform baseLayer = null;
        Sprite baseSprite;
        Movable prevBuilding = (Movable)grid.GetObj(this.prevPosition);
        Movable currentBuilding = (Movable)grid.GetObj(this.position);
        Building nextBuilding = (Building)grid.GetObj(this.nextPosition);

        if (currentBuilding == null)
        {
            return;
        }

        // Debug.Log("Current: " + currentBuilding.GetType());
        // Debug.Log("Prev: " + prevBuilding.GetType());
        // Debug.Log("NextPos: "+ this.nextPosition);
        // Debug.Log("Next: " + nextBuilding.GetType());
        Movable buildingToRotate = this;

        if (!currentBuilding.GetType().Equals(typeof(ConveyorBelt)))
        {
            baseLayer = currentBuilding.transform.Find("BaseLayer");
        }
        else if (!prevBuilding.GetType().Equals(typeof(ConveyorBelt)))
        {
            baseLayer = prevBuilding.transform.Find("BaseLayer");
            buildingToRotate = prevBuilding;
        }
        else if (!nextBuilding.GetType().Equals(typeof(ConveyorBelt)))
        {
            baseLayer = nextBuilding.transform.Find("BaseLayer");
            buildingToRotate = (Movable)nextBuilding;
        }
        if (baseLayer == null)
        {
            return;
        }

        if (buildingToRotate.GetType().Equals(typeof(Miner)))
        {
            baseSprite = Data.Instance.sprites["BaseLayerMiner"];
        }
        else if (buildingToRotate.GetType().Equals(typeof(EmptyBoi)))
        {
            baseSprite = Data.Instance.sprites["BaseLayerEmptyBoi"];
        }
        else if (buildingToRotate.GetType().Equals(typeof(Merger)))
        {
            baseSprite = Data.Instance.sprites["BaseLayerMerger"];
        }
        else
        {
            baseSprite = Data.Instance.sprites["BaseLayerMachine"];
        }

        Vector2Int difference = buildingToRotate.nextPosition - buildingToRotate.prevPosition;

        Vector2Int up = new Vector2Int(0, 1);
        Vector2Int upUp = new Vector2Int(0, 2);
        Vector2Int down = new Vector2Int(0, -1);
        Vector2Int downDown = new Vector2Int(0, -2);
        Vector2Int right = new Vector2Int(1, 0);
        Vector2Int rightRight = new Vector2Int(2, 0);
        Vector2Int left = new Vector2Int(-1, 0);
        Vector2Int leftLeft = new Vector2Int(-2, 0);

        Vector2Int upRight = new Vector2Int(1, 1);
        Vector2Int upLeft = new Vector2Int(-1, 1);
        Vector2Int downLeft = new Vector2Int(-1, -1);
        Vector2Int downRight = new Vector2Int(1, -1);

        if (difference == up || difference == upUp)
        {
            baseLayer.GetComponent<SpriteRenderer>().sprite = baseSprite;
            SetRotation(baseLayer, 180);
        }
        else if (difference == down || difference == downDown)
        {
            baseLayer.GetComponent<SpriteRenderer>().sprite = baseSprite;
            SetRotation(baseLayer, 0);
        }
        else if (difference == right || difference == rightRight)
        {
            baseLayer.GetComponent<SpriteRenderer>().sprite = baseSprite;
            SetRotation(baseLayer, 90);
        }
        else if (difference == left || difference == leftLeft)
        {
            baseLayer.GetComponent<SpriteRenderer>().sprite = baseSprite;
            SetRotation(baseLayer, 270);
        }
        else if (difference == upRight)
        {
            if (buildingToRotate.prevPosition.y == buildingToRotate.position.y)
            {
                baseLayer.GetComponent<SpriteRenderer>().sprite = Data.Instance.sprites[
                    "BaseLayerCorner2"
                ];
                SetRotation(baseLayer, 90);
            }
            else
            {
                baseLayer.GetComponent<SpriteRenderer>().sprite = Data.Instance.sprites[
                    "BaseLayerCorner"
                ];
                SetRotation(baseLayer, 180);
            }
        }
        else if (difference == upLeft)
        {
            if (buildingToRotate.prevPosition.y == buildingToRotate.position.y)
            {
                baseLayer.GetComponent<SpriteRenderer>().sprite = Data.Instance.sprites[
                    "BaseLayerCorner"
                ];
                SetRotation(baseLayer, 270);
            }
            else
            {
                baseLayer.GetComponent<SpriteRenderer>().sprite = Data.Instance.sprites[
                    "BaseLayerCorner2"
                ];
                SetRotation(baseLayer, 180);
            }
        }
        else if (difference == downLeft)
        {
            if (buildingToRotate.prevPosition.y == buildingToRotate.position.y)
            {
                baseLayer.GetComponent<SpriteRenderer>().sprite = Data.Instance.sprites[
                    "BaseLayerCorner2"
                ];
                SetRotation(baseLayer, 270);
            }
            else
            {
                baseLayer.GetComponent<SpriteRenderer>().sprite = Data.Instance.sprites[
                    "BaseLayerCorner"
                ];
                SetRotation(baseLayer, 0);
            }
        }
        else if (difference == downRight)
        {
            if (buildingToRotate.prevPosition.y == buildingToRotate.position.y)
            {
                baseLayer.GetComponent<SpriteRenderer>().sprite = Data.Instance.sprites[
                    "BaseLayerCorner"
                ];
                SetRotation(baseLayer, 90);
            }
            else
            {
                baseLayer.GetComponent<SpriteRenderer>().sprite = Data.Instance.sprites[
                    "BaseLayerCorner2"
                ];
                SetRotation(baseLayer, 0);
            }
        }
    }

    private void SetRotation(Transform baseLayer, float zRotation)
    {
        baseLayer.transform.eulerAngles = new Vector3(
            baseLayer.transform.eulerAngles.x,
            baseLayer.transform.eulerAngles.y,
            zRotation
        );
    }

    public void ResetPrevPosition()
    {
        PrevPosition = position;
    }

    public void ResetNextPosition()
    {
        NextPosition = position;
    }

    public abstract string CreateInstructions();
}