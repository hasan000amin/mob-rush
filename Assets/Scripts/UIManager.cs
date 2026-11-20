using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mob Rush - HUD and menus. Level/coins text, HP bars for both bases,
/// win/lose panels and a 3-item upgrade shop. All references are wired
/// by SceneBuilder; buttons call the public methods below.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public Text levelText;
    public Text coinsText;
    public Slider playerHpSlider;
    public Slider enemyHpSlider;

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject shopPanel;
    public Text winCoinsText;

    [Header("Buttons")]
    public Button nextLevelButton;
    public Button retryButton;
    public Button buyFireRateButton;
    public Button buyMobHpButton;
    public Button buyMobDamageButton;
    public Text fireRateCostText;
    public Text mobHpCostText;
    public Text mobDamageCostText;

    [Header("Upgrade costs")]
    public int fireRateCost = 100;
    public int mobHpCost = 100;
    public int mobDamageCost = 100;

    void Start()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null) { gm.OnWin += ShowWin; gm.OnLose += ShowLose; }
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(true);
        UpdateCosts();
    }

    void OnDestroy()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null) { gm.OnWin -= ShowWin; gm.OnLose -= ShowLose; }
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;
        if (levelText != null) levelText.text = "Level " + gm.level;
        if (coinsText != null) coinsText.text = "Koin: " + gm.coins;
        if (playerHpSlider != null && gm.playerBase != null)
        {
            playerHpSlider.maxValue = gm.playerBase.maxHp;
            playerHpSlider.value = gm.playerBase.hp;
        }
        if (enemyHpSlider != null && gm.enemyBase != null)
        {
            enemyHpSlider.maxValue = gm.enemyBase.maxHp;
            enemyHpSlider.value = gm.enemyBase.hp;
        }
    }

    void ShowWin()
    {
        if (winPanel != null) winPanel.SetActive(true);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (winCoinsText != null && GameManager.Instance != null)
            winCoinsText.text = "Koin didapat: +" + LevelData.CoinsReward(GameManager.Instance.level);
    }

    void ShowLose()
    {
        if (losePanel != null) losePanel.SetActive(true);
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    public void OnNextLevel()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;
        if (winPanel != null) winPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(true);
        gm.StartLevel(gm.level + 1);
    }

    public void OnRetry()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;
        if (losePanel != null) losePanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(true);
        gm.StartLevel(gm.level);
    }

    public void OnBuyFireRate()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;
        if (gm.BuyFireRateUpgrade(fireRateCost)) { fireRateCost += 50; UpdateCosts(); }
    }

    public void OnBuyMobHp()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;
        if (gm.BuyMobHpUpgrade(mobHpCost)) { mobHpCost += 50; UpdateCosts(); }
    }

    public void OnBuyMobDamage()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;
        if (gm.BuyMobDamageUpgrade(mobDamageCost)) { mobDamageCost += 50; UpdateCosts(); }
    }

    void UpdateCosts()
    {
        if (fireRateCostText != null) fireRateCostText.text = "Tembak Cepat\n" + fireRateCost + " koin";
        if (mobHpCostText != null) mobHpCostText.text = "HP Pasukan\n" + mobHpCost + " koin";
        if (mobDamageCostText != null) mobDamageCostText.text = "Damage\n" + mobDamageCost + " koin";
    }
}
