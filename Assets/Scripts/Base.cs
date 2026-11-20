using UnityEngine;

/// <summary>
/// Mob Rush - a base with HP. Enemy mobs damage it. When HP hits zero,
/// the enemy base means a win, the player base means a loss.
/// </summary>
public class Base : MonoBehaviour
{
    [Header("Settings")]
    public bool isPlayer = false;
    public int maxHp = 100;

    [HideInInspector] public int hp;

    Transform hpBarFill;

    void Awake()
    {
        hp = maxHp;
        hpBarFill = transform.Find("HpBarFill");
    }

    public void ResetHp()
    {
        hp = maxHp;
        UpdateHpBar();
    }

    public void TakeDamage(int amount)
    {
        if (GameManager.Instance != null && GameManager.Instance.state != GameManager.GameState.Playing) return;
        hp -= amount;
        UpdateHpBar();
        if (hp <= 0)
        {
            hp = 0;
            if (GameManager.Instance == null) return;
            if (isPlayer) GameManager.Instance.LoseLevel();
            else GameManager.Instance.WinLevel();
        }
    }

    void UpdateHpBar()
    {
        if (hpBarFill == null) return;
        float f = Mathf.Clamp01((float)hp / maxHp);
        hpBarFill.localScale = new Vector3(f, 1f, 1f);
    }
}
