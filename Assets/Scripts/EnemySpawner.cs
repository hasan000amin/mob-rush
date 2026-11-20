using UnityEngine;

/// <summary>
/// Mob Rush - spawns enemy mobs from the enemy base at the difficulty
/// interval set by GameManager, at a random X in the lane.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyMobPrefab;

    [Header("Settings")]
    public float spawnRange = 6f;
    public float spawnZOffset = -1.5f;

    float timer;

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.state != GameManager.GameState.Playing) return;

        timer += Time.deltaTime;
        if (timer >= GameManager.Instance.EnemySpawnInterval())
        {
            timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        if (enemyMobPrefab == null) return;
        Vector3 pos = transform.position
            + new Vector3(Random.Range(-spawnRange, spawnRange), 0f, spawnZOffset);
        GameObject go = Instantiate(enemyMobPrefab, pos, Quaternion.identity);
        Mob mob = go.GetComponent<Mob>();
        if (mob != null)
        {
            mob.isPlayer = false;
            mob.hp = GameManager.Instance.mobHp;
            mob.damage = GameManager.Instance.mobDamage;
            mob.speed = GameManager.Instance.mobSpeed;
        }
    }
}
