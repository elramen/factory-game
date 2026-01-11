using System.Collections;
using UnityEngine;

public class QuarterBase : Movable
{
    Base mainBase;
    public Item previousItem;

    public void Init(
        Grid<Building> grid,
        Vector2Int position,
        Vector2Int prevPosition,
        Vector2Int nextPosition,
        Base mainBase
    )
    {
        base.Init(grid, position);
        this.PrevPosition = prevPosition;
        this.NextPosition = nextPosition;
        this.mainBase = mainBase;
        this.hasPrevPos = false;
        grid.AddObj(this, position);
    }

    public override void CustomAction()
    {
        mainBase.Receive(item);
        // DestroyItem(item);

        // TODO - Destruction of item should have some sort of animation
        //      - Probably placed wrong to
        // Remove the item that got loaded into the base last update

        previousItem = item;
        item = null;
    }

    public override IEnumerator moveItem(Vector2 notUsed)
    {
        Vector3 startPosition = item.transform.position;
        Vector2 endPosition = grid.GridPositionToPosition(position);
        Vector3 change = ((Vector3)endPosition - startPosition);

        float timeElapsed = 0f;

        while (timeElapsed < movementTime)
        {
            item.transform.position =
                startPosition + change * movement.Evaluate(timeElapsed / movementTime);

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        item.transform.position = (Vector3)endPosition;

        yield return null;
    }

    public IEnumerator DestroyItem()
    {
        if (previousItem != null)
        {
            Vector3 startPosition = previousItem.transform.position;
            Vector3 endPosition = (Vector3)this.transform.position;
            Vector3 change = (endPosition - startPosition);

            float timeElapsed = 0f;

            while (timeElapsed < movementTime / 2)
            {
                if (previousItem != null)
                {
                    previousItem.transform.position =
                        startPosition + change * movement.Evaluate(timeElapsed / movementTime);

                    timeElapsed += Time.deltaTime;
                }
                yield return new WaitForEndOfFrame();
            }
            previousItem.transform.position = (Vector3)endPosition;

            Destroy(previousItem.gameObject);
        }

        yield return null;
    }

    public override string CreateInstructions()
    {
        return "";
    }
}
