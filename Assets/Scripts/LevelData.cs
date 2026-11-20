using UnityEngine;

/// <summary>
/// Mob Rush - per-level configuration. Formulas match GameManager
/// exactly so the UI, difficulty and rewards never drift apart.
/// </summary>
public static class LevelData
{
    /// <summary>Enemy base HP: 100 + level * 50 (level 1 = 150).</summary>
    public static int GetEnemyBaseHp(int level)
    {
        return 100 + level * 50;
    }

    /// <summary>Enemy spawn interval, seconds. Faster each level, min 0.6s.</summary>
    public static float GetSpawnInterval(int level)
    {
        return Mathf.Max(0.6f, 2.0f - level * 0.1f);
    }

    /// <summary>Coins earned for winning a level: 50 + level * 10.</summary>
    public static int CoinsReward(int level)
    {
        return 50 + level * 10;
    }
}
