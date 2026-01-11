using UnityEngine;

public class DummyBelt : MonoBehaviour
{

    // TODO: Funkar inte perfekt när man tar bort grejor, uppdaterar inte korrekt för de bredvid
    public SpriteRenderer spriteRenderer;
    Vector2Int position;
    Vector2Int NextPosition;
    Vector2Int PrevPosition;

    // Används också för att uppdatera grejen
    public void Init(
        Vector2Int position,
        Vector2Int nextPosition,
        Vector2Int prevPosition
    )
    {
        this.position = position;
        this.NextPosition = nextPosition;
        this.PrevPosition = prevPosition;
        ChooseSprite();
    }

    public void ChooseSprite()
    {
        this.spriteRenderer.color = new Color32(255, 255, 255, 255);
        // No connections
        if (this.NextPosition == this.position && this.PrevPosition == this.position)
        {
            this.spriteRenderer.sprite = Data.Instance.sprites["BeltLonley"];
        }
        // One connection (edge)
        else if (this.NextPosition == this.position || this.PrevPosition == this.position)
        {
            this.spriteRenderer.sprite = Data.Instance.sprites["BeltEdge"];

            // determine rotation
            Vector2Int other =
                this.NextPosition == this.position ? this.PrevPosition : this.NextPosition;
            Vector2Int difference = other - this.position;

            if (difference == new Vector2Int(0, -1))
                SetRotation(0);
            else if (difference == new Vector2Int(1, 0))
                SetRotation(90);
            else if (difference == new Vector2Int(0, 1))
                SetRotation(180);
            else if (difference == new Vector2Int(-1, 0))
                SetRotation(270);
        }
        // Two connections in line
        else if (
            this.NextPosition.x == this.PrevPosition.x || this.NextPosition.y == this.PrevPosition.y
        )
        {
            this.spriteRenderer.sprite = Data.Instance.sprites["BeltStraight"];

            if (this.NextPosition.x == this.PrevPosition.x)
                SetRotation(90);
            else
                SetRotation(0);
        }
        // Two connections in curve (corner)
        else
        {
            this.spriteRenderer.sprite = Data.Instance.sprites["BeltCorner"];

            Vector2Int point1 =
                this.NextPosition.x > this.PrevPosition.x ? this.NextPosition : this.PrevPosition;
            Vector2Int point2 =
                this.NextPosition.x > this.PrevPosition.x ? this.PrevPosition : this.NextPosition;

            // set rotation
            if (point1.x > position.x && point2.y > position.y)
                SetRotation(0);
            else if (point2.x < position.x && point1.y > position.y)
                SetRotation(90);
            else if (point2.x < position.x && point1.y < position.y)
                SetRotation(180);
            else
                SetRotation(270);
        }
    }

    private void SetRotation(float zRotation)
    {
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            zRotation
        );
    }
}
