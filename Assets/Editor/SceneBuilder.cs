using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

/// <summary>
/// Mob Rush - builds the whole Main scene from scratch.
/// Open any (empty) scene, run menu Mob Rush/Build Scene, then press Play.
/// Everything is created from primitives: ground, lane, cannon, gates,
/// bases, mobs (as prefabs), light, camera, GameManager and full UI.
/// </summary>
public static class SceneBuilder
{
    const string kScenePath = "Assets/Scenes/Main.unity";

    [MenuItem("Mob Rush/Build Scene")]
    public static void BuildScene()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        foreach (GameObject go in scene.GetRootGameObjects()) Object.DestroyImmediate(go);
        Directory.CreateDirectory("Assets/Materials");
        Directory.CreateDirectory("Assets/Prefabs");
        Directory.CreateDirectory("Assets/Scenes");

        Material groundMat = Mat("Ground", new Color(0.15f, 0.35f, 0.18f));
        Material laneMat = Mat("Lane", new Color(0.25f, 0.28f, 0.32f));
        Material blueMat = Mat("Player", new Color(0.15f, 0.5f, 1f));
        Material redMat = Mat("Enemy", new Color(1f, 0.25f, 0.2f));
        Material grayMat = Mat("Gray", new Color(0.6f, 0.6f, 0.65f));
        Material darkMat = Mat("Dark", new Color(0.2f, 0.2f, 0.25f));
        Material goldMat = Mat("Gold", new Color(1f, 0.8f, 0.2f));

        Prim(PrimitiveType.Plane, "Ground", new Vector3(0f, 0f, 0f), new Vector3(2f, 1f, 4.5f), groundMat);
        Prim(PrimitiveType.Cube, "Lane", new Vector3(0f, 0.05f, 0f), new Vector3(16f, 0.1f, 40f), laneMat);

        GameObject playerPrefab = MakeMobPrefab("PlayerMob", blueMat, true);
        GameObject enemyPrefab = MakeMobPrefab("EnemyMob", redMat, false);

        GameObject cannon = new GameObject("Cannon");
        cannon.transform.position = new Vector3(0f, 0f, -14f);
        GameObject cBase = Prim(PrimitiveType.Cylinder, "CannonBase", new Vector3(0f, 0.3f, -14f), new Vector3(1.6f, 0.6f, 1.6f), grayMat);
        cBase.transform.SetParent(cannon.transform, true);
        GameObject barrel = Prim(PrimitiveType.Cylinder, "Barrel", new Vector3(0f, 1f, -13.4f), new Vector3(0.5f, 1.2f, 0.5f), darkMat);
        barrel.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        barrel.transform.SetParent(cannon.transform, true);
        GameObject muzzle = new GameObject("Muzzle");
        muzzle.transform.SetParent(cannon.transform, false);
        muzzle.transform.localPosition = new Vector3(0f, 1f, 1.2f);
        CannonController cc = cannon.AddComponent<CannonController>();
        cc.muzzle = muzzle.transform;
        cc.playerMobPrefab = playerPrefab;

        var gates = new (float x, float z, Gate.GateType t, int v, string label, bool good)[]
        {
            (-4f, -4f, Gate.GateType.Multiply, 2, "x2", true),
            ( 0f, -4f, Gate.GateType.Add, 10, "+10", true),
            ( 4f, -4f, Gate.GateType.Divide, 2, "÷2", false),
            (-4f,  4f, Gate.GateType.Multiply, 3, "x3", true),
            ( 0f,  4f, Gate.GateType.Add, 5, "+5", true),
            ( 4f,  4f, Gate.GateType.Subtract, 5, "-5", false),
        };
        foreach (var g in gates) MakeGate(g.x, g.z, g.t, g.v, g.label, g.good ? goldMat : redMat);

        Base playerBase = MakeBase("PlayerBase", new Vector3(0f, 1f, -18f), true, blueMat, darkMat);
        Base enemyBase = MakeBase("EnemyBase", new Vector3(0f, 1f, 18f), false, redMat, darkMat);
        EnemySpawner sp = enemyBase.gameObject.AddComponent<EnemySpawner>();
        sp.enemyMobPrefab = enemyPrefab;

        GameManager gm = new GameObject("GameManager").AddComponent<GameManager>();
        gm.playerBase = playerBase;
        gm.enemyBase = enemyBase;

        GameObject dirLight = new GameObject("Directional Light");
        Light l = dirLight.AddComponent<Light>();
        l.type = LightType.Directional;
        l.intensity = 1f;
        dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        GameObject cam = new GameObject("MainCamera");
        cam.tag = "MainCamera";
        Camera c = cam.AddComponent<Camera>();
        c.clearFlags = CameraClearFlags.SolidColor;
        c.backgroundColor = new Color(0.05f, 0.07f, 0.12f);
        cam.transform.position = new Vector3(0f, 26f, -30f);
        cam.transform.LookAt(new Vector3(0f, 0f, 0f));

        BuildUI();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), kScenePath);
        AssetDatabase.Refresh();
        Debug.Log("Mob Rush: scene built and saved to " + kScenePath);
    }

    static Material Mat(string name, Color color)
    {
        Shader shader = Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit");
        Material m = new Material(shader);
        m.color = color;
        AssetDatabase.CreateAsset(m, "Assets/Materials/" + name + ".mat");
        return m;
    }

    static GameObject Prim(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Material mat)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = scale;
        if (mat != null) go.GetComponent<Renderer>().material = mat;
        return go;
    }

    static GameObject MakeMobPrefab(string name, Material mat, bool isPlayer)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = name;
        go.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
        go.GetComponent<Renderer>().material = mat;
        Mob mob = go.AddComponent<Mob>();
        mob.isPlayer = isPlayer;
        Rigidbody rb = go.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        go.GetComponent<CapsuleCollider>().isTrigger = true;
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, "Assets/Prefabs/" + name + ".prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    static void MakeGate(float x, float z, Gate.GateType type, int value, string label, Material mat)
    {
        GameObject go = Prim(PrimitiveType.Cube, "Gate_" + label, new Vector3(x, 2f, z), new Vector3(3f, 4f, 0.3f), mat);
        Gate gate = go.AddComponent<Gate>();
        gate.type = type;
        gate.value = value;
        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform, false);
        labelGO.transform.localPosition = new Vector3(0f, 0f, -0.35f);
        labelGO.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        TextMeshPro tmp = labelGO.AddComponent<TextMeshPro>();
        tmp.text = label;
        tmp.fontSize = 96f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.font = TMP_Settings.defaultFontAsset ?? Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        labelGO.GetComponent<RectTransform>().sizeDelta = new Vector2(3f, 1.5f);
    }

    static Base MakeBase(string name, Vector3 pos, bool isPlayer, Material mat, Material darkMat)
    {
        GameObject go = Prim(PrimitiveType.Cube, name, pos, new Vector3(4f, 2f, 1f), mat);
        Base b = go.AddComponent<Base>();
        b.isPlayer = isPlayer;
        b.maxHp = isPlayer ? 100 : 150;
        GameObject fill = new GameObject("HpBarFill");
        fill.transform.SetParent(go.transform, false);
        fill.transform.localPosition = new Vector3(0f, 1.6f, -0.6f);
        GameObject q = GameObject.CreatePrimitive(PrimitiveType.Quad);
        q.name = "FillQuad";
        q.transform.SetParent(fill.transform, false);
        q.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        q.transform.localScale = new Vector3(3.6f, 0.5f, 1f);
        q.GetComponent<Renderer>().material = Mat("HpFill" + name, isPlayer ? Color.green : Color.red);
        GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bg.name = "HpBarBg";
        bg.transform.SetParent(go.transform, false);
        bg.transform.localPosition = new Vector3(0f, 1.6f, -0.55f);
        bg.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        bg.transform.localScale = new Vector3(3.8f, 0.7f, 1f);
        bg.GetComponent<Renderer>().material = darkMat;
        return b;
    }

    static Font DefaultFont()
    {
        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
            ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    static Text UIText(Transform parent, string name, string text, Vector2 anchor, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor align = TextAnchor.MiddleLeft)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.text = text;
        t.font = DefaultFont();
        t.fontSize = fontSize;
        t.color = color;
        t.alignment = align;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return t;
    }

    static Button UIButton(Transform parent, string name, string label, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.45f, 0.9f);
        Button b = go.AddComponent<Button>();
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        UIText(go.transform, "Label", label, new Vector2(0.5f, 0.5f), Vector2.zero, size, 28, Color.white, TextAnchor.MiddleCenter);
        return b;
    }

    static GameObject UIPanel(Transform parent, string name, Vector2 anchor, Vector2 pos, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }

    static Slider UISlider(Transform parent, string name, Vector2 anchor, Vector2 pos, Vector2 size, Color fillColor)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image bg = go.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.5f);
        Slider s = go.AddComponent<Slider>();
        s.minValue = 0f;
        s.maxValue = 100f;
        s.value = 100f;
        s.interactable = false;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        GameObject area = new GameObject("Fill Area");
        area.transform.SetParent(go.transform, false);
        RectTransform art = area.AddComponent<RectTransform>();
        art.anchorMin = Vector2.zero;
        art.anchorMax = Vector2.one;
        art.offsetMin = Vector2.zero;
        art.offsetMax = Vector2.zero;
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(area.transform, false);
        Image fi = fill.AddComponent<Image>();
        fi.color = fillColor;
        RectTransform frt = fill.GetComponent<RectTransform>();
        frt.anchorMin = Vector2.zero;
        frt.anchorMax = Vector2.one;
        frt.offsetMin = Vector2.zero;
        frt.offsetMax = Vector2.zero;
        s.fillRect = frt;
        s.targetGraphic = fi;
        s.direction = Slider.Direction.LeftToRight;
        return s;
    }

    static void BuildUI()
    {
        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();

        GameObject canvas = new GameObject("Canvas");
        Canvas cv = canvas.AddComponent<Canvas>();
        cv.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        canvas.AddComponent<GraphicRaycaster>();
        Transform root = canvas.transform;

        Text levelText = UIText(root, "LevelText", "Level 1", new Vector2(0f, 1f), new Vector2(30f, -40f), new Vector2(300f, 60f), 36, Color.white);
        Text coinsText = UIText(root, "CoinsText", "Koin: 0", new Vector2(1f, 1f), new Vector2(-30f, -40f), new Vector2(300f, 60f), 36, Color.white, TextAnchor.MiddleRight);
        UIText(root, "PlayerHpLabel", "Markas Kamu", new Vector2(0f, 1f), new Vector2(30f, -110f), new Vector2(300f, 40f), 24, Color.white);
        Slider playerHp = UISlider(root, "PlayerHpSlider", new Vector2(0f, 1f), new Vector2(30f, -160f), new Vector2(400f, 30f), Color.green);
        UIText(root, "EnemyHpLabel", "Markas Musuh", new Vector2(1f, 1f), new Vector2(-30f, -110f), new Vector2(300f, 40f), 24, Color.white, TextAnchor.MiddleRight);
        Slider enemyHp = UISlider(root, "EnemyHpSlider", new Vector2(1f, 1f), new Vector2(-30f, -160f), new Vector2(400f, 30f), Color.red);

        GameObject shop = UIPanel(root, "ShopPanel", new Vector2(0.5f, 0f), new Vector2(0f, 30f), new Vector2(1000f, 280f), new Color(0f, 0f, 0f, 0.55f));
        UIText(shop.transform, "ShopTitle", "UPGRADE", new Vector2(0.5f, 1f), new Vector2(0f, -50f), new Vector2(600f, 60f), 32, Color.white, TextAnchor.MiddleCenter);
        Button b1 = UIButton(shop.transform, "BuyFireRateButton", "Tembak Cepat", new Vector2(0.5f, 0.5f), new Vector2(-320f, -40f), new Vector2(300f, 150f));
        Button b2 = UIButton(shop.transform, "BuyMobHpButton", "HP Pasukan", new Vector2(0.5f, 0.5f), new Vector2(0f, -40f), new Vector2(300f, 150f));
        Button b3 = UIButton(shop.transform, "BuyMobDamageButton", "Damage", new Vector2(0.5f, 0.5f), new Vector2(320f, -40f), new Vector2(300f, 150f));

        GameObject win = UIPanel(root, "WinPanel", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700f, 520f), new Color(0f, 0f, 0f, 0.7f));
        UIText(win.transform, "WinTitle", "MENANG!", new Vector2(0.5f, 1f), new Vector2(0f, -80f), new Vector2(600f, 80f), 56, Color.yellow, TextAnchor.MiddleCenter);
        Text winCoins = UIText(win.transform, "WinCoinsText", "Koin didapat: +60", new Vector2(0.5f, 1f), new Vector2(0f, -190f), new Vector2(600f, 60f), 32, Color.white, TextAnchor.MiddleCenter);
        Button nextBtn = UIButton(win.transform, "NextLevelButton", "Level Berikutnya", new Vector2(0.5f, 0.5f), new Vector2(0f, -120f), new Vector2(420f, 100f));
        win.SetActive(false);

        GameObject lose = UIPanel(root, "LosePanel", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700f, 420f), new Color(0f, 0f, 0f, 0.7f));
        UIText(lose.transform, "LoseTitle", "KALAH", new Vector2(0.5f, 1f), new Vector2(0f, -80f), new Vector2(600f, 80f), 56, Color.red, TextAnchor.MiddleCenter);
        Button retryBtn = UIButton(lose.transform, "RetryButton", "Coba Lagi", new Vector2(0.5f, 0.5f), new Vector2(0f, -60f), new Vector2(420f, 100f));
        lose.SetActive(false);

        UIManager ui = new GameObject("UIManager").AddComponent<UIManager>();
        ui.levelText = levelText;
        ui.coinsText = coinsText;
        ui.playerHpSlider = playerHp;
        ui.enemyHpSlider = enemyHp;
        ui.winPanel = win;
        ui.losePanel = lose;
        ui.shopPanel = shop;
        ui.winCoinsText = winCoins;
        ui.nextLevelButton = nextBtn;
        ui.retryButton = retryBtn;
        ui.buyFireRateButton = b1;
        ui.buyMobHpButton = b2;
        ui.buyMobDamageButton = b3;
        ui.fireRateCostText = b1.transform.Find("Label").GetComponent<Text>();
        ui.mobHpCostText = b2.transform.Find("Label").GetComponent<Text>();
        ui.mobDamageCostText = b3.transform.Find("Label").GetComponent<Text>();
        nextBtn.onClick.AddListener(ui.OnNextLevel);
        retryBtn.onClick.AddListener(ui.OnRetry);
        b1.onClick.AddListener(ui.OnBuyFireRate);
        b2.onClick.AddListener(ui.OnBuyMobHp);
        b3.onClick.AddListener(ui.OnBuyMobDamage);
    }
}
