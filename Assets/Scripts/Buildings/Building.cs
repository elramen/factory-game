using UnityEngine;

public class Building : MonoBehaviour
{
    public Grid<Building> grid;
    public SpriteRenderer spriteRenderer;

    public Vector2Int position;

    public float movementTime = 0.5f;
    public AnimationCurve movement;

    public void Init(Grid<Building> grid, Vector2Int position)
    {
        this.grid = grid;
        this.position = position;
        transform.position = grid.GridPositionToPosition(position);
        spriteRenderer = GetComponent<SpriteRenderer>();

        movement = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        movement.preWrapMode = WrapMode.PingPong;
        movement.postWrapMode = WrapMode.PingPong;
    }


    public virtual void ChooseSprite() { }
    public virtual void RemoveBuilding() { }
}