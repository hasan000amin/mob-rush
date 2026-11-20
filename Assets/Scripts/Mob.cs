using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Mob Rush - a small capsule unit. Moves forward, fights enemy mobs
/// it touches, and damages the opposite base on arrival.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Mob : MonoBehaviour
{
    [Header("Stats")]
    public int hp = 10;
    public int damage = 5;
    public float speed = 4f;
    public bool isPlayer = true;

    Mob target;
    float attackTimer;
    HashSet<int> usedGates = new HashSet<int>();

    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        GetComponent<CapsuleCollider>().isTrigger = true;
    }

    void Start()
    {
        if (GameManager.Instance != null) GameManager.Instance.RegisterMob(this);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.UnregisterMob(this);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.state != GameManager.GameState.Playing) return;

        if (target != null)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= 0.5f)
            {
                attackTimer = 0f;
                if (target != null) target.TakeDamage(damage);
            }
            return;
        }

        float dir = isPlayer ? 1f : -1f;
        transform.Translate(0f, 0f, dir * speed * Time.deltaTime);
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0) Destroy(gameObject);
    }

    public bool TryUseGate(int gateId)
    {
        if (usedGates.Contains(gateId)) return false;
        usedGates.Add(gateId);
        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        Mob otherMob = other.GetComponent<Mob>();
        if (otherMob != null && otherMob.isPlayer != isPlayer && target == null)
        {
            target = otherMob;
            return;
        }

        Base otherBase = other.GetComponent<Base>();
        if (otherBase != null && otherBase.isPlayer != isPlayer)
        {
            otherBase.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (target != null) return;
        Mob otherMob = other.GetComponent<Mob>();
        if (otherMob != null && otherMob.isPlayer != isPlayer) target = otherMob;
    }
}
