using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ShipController ship;
    [SerializeField] private EnemyFollower enemyPrefab;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultTitle;
    [SerializeField] private TMP_Text finalScore;

    [SerializeField] private float spawnInterval = 3f;

    public bool HasEnded { get; private set; }

    private int score;
    private GameSounds sounds;
    private float nextSpawnTime;

    private void Start()
    {
        sounds = ship.GetComponent<GameSounds>();
        scoreText.text = "Score: 0";
        resultPanel.SetActive(false);
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (HasEnded || Time.time < nextSpawnTime)
        {
            return;
        }

        nextSpawnTime = Time.time + spawnInterval;

        if (!TryGetSpawnPosition(out Vector2 position))
        {
            return;
        }

        EnemyFollower enemy = Instantiate(
            enemyPrefab, position, Quaternion.identity);

        enemy.Initialize(ship, this);
    }

    private bool TryGetSpawnPosition(out Vector2 position)
    {
        Vector2 shipPosition = ship.transform.position;

        // Limit attempts so spawning can never stall the game.
        for (int attempt = 0; attempt < 100; attempt++)
        {
            position = new Vector2(
                Random.Range(-15f, 15f),
                Random.Range(-15f, 15f));

            if ((position - shipPosition).sqrMagnitude >= 3f * 3f)
            {
                return true;
            }
        }

        position = default;
        return false;
    }

    public void AddPoint()
    {
        if (HasEnded)
        {
            return;
        }

        score++;
        if (sounds != null) sounds.PlayEnemyDefeated();
        scoreText.text = $"Score: {score}";

        if (score >= 5)
        {
            EndGame(true);
        }
    }

    public void Lose()
    {
        EndGame(false);
    }

    private void EndGame(bool won)
    {
        if (HasEnded)
        {
            return;
        }

        HasEnded = true;
        ship.StopShip();

        resultTitle.text = won ? "You Win!" : "Game Over";
        finalScore.text = $"Score: {score}";
        resultPanel.SetActive(true);
    }
}
