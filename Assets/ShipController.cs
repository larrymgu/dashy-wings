using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipController : MonoBehaviour
{
    [SerializeField] private Camera followCamera;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private float speedPerUnit = 6f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float minSpeed = 7f;
    
    [SerializeField] private float shipLength = 1.4f;
    [SerializeField] private float dashSpeed = 25f;

    private SpriteRenderer shipSprite;
    private Color normalColor;
    private bool dashRequested;
    private float dashDistanceLeft;
    private Vector2 dashDirection;
    private float dashCooldownEndsAt;

    private Rigidbody2D body;
    private bool gameOver;
    [SerializeField] private GameManager gameManager;

    private bool isDashing;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        shipSprite = GetComponent<SpriteRenderer>();
        normalColor = shipSprite.color;
    }
    
    private void Update()
    {
        bool onCooldown = Time.time < dashCooldownEndsAt;

        if (!onCooldown)
        {
            shipSprite.color = normalColor;
        }

        if (!gameOver && !onCooldown && dashDistanceLeft <= 0f &&
            Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            dashRequested = true;
        }
    }
    
    private void FixedUpdate()
    {
        if (gameOver || Mouse.current == null)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }
        
        if (dashRequested)
        {
            dashRequested = false;
            dashDirection = transform.up;
            dashDistanceLeft = shipLength * 3f;
            dashCooldownEndsAt = Time.time + 1.5f;
            shipSprite.color = new Color(0.15f, 0.97f, 0.99f);
        }

        isDashing = dashDistanceLeft > 0f;

        if (isDashing)
        {
            float step = Mathf.Min(
                dashSpeed * Time.fixedDeltaTime, dashDistanceLeft);

            body.linearVelocity = dashDirection * (step / Time.fixedDeltaTime);
            dashDistanceLeft -= step;
            return;
        }
        
        Vector2 mouse = Mouse.current.position.ReadValue();
        Rect view = followCamera.pixelRect;
        float unitsPerPixel = 2f * followCamera.orthographicSize / view.height;
        Vector2 offset = (mouse - view.center) * unitsPerPixel;

        float distance = offset.magnitude;

        if (distance < 0.01f)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        float speed = Mathf.Clamp(distance * speedPerUnit, minSpeed, maxSpeed);
        body.linearVelocity = offset.normalized * speed;
        
        if (offset.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            body.MoveRotation(angle - 90f);
        }
    }

    private void LateUpdate()
    {
        Vector3 position = transform.position;
        Vector3 cameraPosition = followCamera.transform.position;
        followCamera.transform.position =
            new Vector3(position.x, position.y, cameraPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameOver)
        {
            return;
        }

        EnemyFollower enemy = other.GetComponentInParent<EnemyFollower>();

        if (enemy != null && isDashing)
        {
            enemy.Defeat();
            return;
        }

        gameManager.Lose();
    }

    public void StopShip()
    {
        gameOver = true;
        isDashing = false;
        dashRequested = false;
        dashDistanceLeft = 0f;

        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;
    }    
}
