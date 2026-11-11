using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFollower : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D body;
    private ShipController target;
    private GameManager gameManager;
    private bool defeated;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Initialize(ShipController ship, GameManager manager)
    {
        target = ship;
        gameManager = manager;
    }

    private void FixedUpdate()
    {
        if (defeated || target == null ||
            gameManager == null || gameManager.HasEnded)
        {
            return;
        }

        Vector2 nextPosition = Vector2.MoveTowards(
            body.position,
            target.transform.position,
            moveSpeed * Time.fixedDeltaTime
        );

        body.MovePosition(nextPosition);
    }

    public void Defeat()
    {
        if (defeated || gameManager == null || gameManager.HasEnded)
        {
            return;
        }

        defeated = true;
        gameManager.AddPoint();
        Destroy(gameObject);
    }
}