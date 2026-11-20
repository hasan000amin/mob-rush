using UnityEngine;

/// <summary>
/// Mob Rush - multiplier gate. When a player mob passes through,
/// multiply/add copies, or divide/subtract mobs. Each mob uses a gate once.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class Gate : MonoBehaviour
{
    public enum GateType { Multiply, Add, Divide, Subtract }

    [Header("Settings")]
    public GateType type = GateType.Multiply;
    public int value = 2;

    void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Mob mob = other.GetComponent<Mob>();
        if (mob == null || !mob.isPlayer) return;
        if (!mob.TryUseGate(GetInstanceID())) return;
        ApplyEffect(mob);
    }

    void ApplyEffect(Mob mob)
    {
        switch (type)
        {
            case GateType.Multiply:
                SpawnCopies(mob, Mathf.Max(0, value - 1));
                break;
            case GateType.Add:
                SpawnCopies(mob, Mathf.Max(0, value));
                break;
            case GateType.Divide:
                // Halving, one mob at a time: 50% chance this mob is removed.
                if (Random.value < 0.5f) Destroy(mob.gameObject);
                break;
            case GateType.Subtract:
                Destroy(mob.gameObject);
                DestroyExtraPlayerMobs(Mathf.Max(0, value - 1), mob);
                break;
        }
    }

    void SpawnCopies(Mob mob, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = transform.position
                + new Vector3(Random.Range(-0.6f, 0.6f), 0f, Random.Range(-0.3f, 0.3f));
            Mob copy = Instantiate(mob, pos, Quaternion.identity);
            copy.TryUseGate(GetInstanceID());
        }
    }

    void DestroyExtraPlayerMobs(int count, Mob except)
    {
        if (GameManager.Instance == null) return;
        int removed = 0;
        var list = GameManager.Instance.playerMobs;
        for (int i = list.Count - 1; i >= 0 && removed < count; i--)
        {
            Mob m = list[i];
            if (m == null || m == except) continue;
            Destroy(m.gameObject);
            removed++;
        }
    }
}
