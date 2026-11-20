using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>Mob Rush - central game state (singleton). Level, coins,
/// win/lose state, difficulty and upgrade stats.</summary>
public class GameManager : MonoBehaviour
{
    public enum GameState { Playing, Won, Lost }

    public static GameManager Instance { get; private set; }

    [Header("Progress")]
    public int level = 1;
    public int coins = 0;
    public GameState state = GameState.Playing;

    [Header("References")]
    public Base playerBase;
    public Base enemyBase;

    [Header("Upgradable stats")]
    public float fireInterval = 0.5f;
    public int mobHp = 10;
    public int mobDamage = 5;
    public float mobSpeed = 4f;

    [HideInInspector] public List<Mob> playerMobs = new List<Mob>();
    [HideInInspector] public List<Mob> enemyMobs = new List<Mob>();

    public event Action OnWin;
    public event Action OnLose;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    void Start() { StartLevel(level); }

    public void StartLevel(int lvl)
    {
        level = lvl;
        state = GameState.Playing;
        foreach (Mob m in new List<Mob>(playerMobs)) if (m != null) Destroy(m.gameObject);
        foreach (Mob m in new List<Mob>(enemyMobs)) if (m != null) Destroy(m.gameObject);
        playerMobs.Clear();
        enemyMobs.Clear();
        if (enemyBase != null) { enemyBase.maxHp = EnemyBaseHp(level); enemyBase.ResetHp(); }
        if (playerBase != null) { playerBase.maxHp = 100; playerBase.ResetHp(); }
    }

    public int EnemyBaseHp(int lvl) { return 100 + lvl * 50; }
    public float EnemySpawnInterval() { return Mathf.Max(0.6f, 2.0f - level * 0.1f); }

    public void WinLevel()
    {
        if (state != GameState.Playing) return;
        state = GameState.Won;
        AddCoins(50 + level * 10);
        OnWin?.Invoke();
    }

    public void LoseLevel()
    {
        if (state != GameState.Playing) return;
        state = GameState.Lost;
        OnLose?.Invoke();
    }

    public void AddCoins(int amount) { coins += amount; }
    public bool SpendCoins(int amount) { if (coins < amount) return false; coins -= amount; return true; }

    public bool BuyFireRateUpgrade(int cost)
    {
        if (!SpendCoins(cost)) return false;
        fireInterval = Mathf.Max(0.15f, fireInterval - 0.05f); return true;
    }

    public bool BuyMobHpUpgrade(int cost)
    {
        if (!SpendCoins(cost)) return false;
        mobHp += 5; return true;
    }

    public bool BuyMobDamageUpgrade(int cost)
    {
        if (!SpendCoins(cost)) return false;
        mobDamage += 2; return true;
    }

    public void RegisterMob(Mob mob)
    {
        if (mob == null) return;
        var list = mob.isPlayer ? playerMobs : enemyMobs;
        if (!list.Contains(mob)) list.Add(mob);
    }

    public void UnregisterMob(Mob mob)
    {
        playerMobs.Remove(mob);
        enemyMobs.Remove(mob);
    }
}
