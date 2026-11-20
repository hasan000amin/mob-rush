using UnityEngine;

/// <summary>
/// Mob Rush - player cannon. Auto-fires player mobs and moves along X
/// by dragging (mouse or touch). Fire rate and mob stats come from GameManager.
/// </summary>
public class CannonController : MonoBehaviour
{
    [Header("References")]
    public GameObject playerMobPrefab;
    public Transform muzzle;

    [Header("Settings")]
    public float moveRange = 6f;

    float fireTimer;
    float lastPointerX;
    bool dragging;

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.state != GameManager.GameState.Playing) return;

        HandleDrag();

        fireTimer += Time.deltaTime;
        if (fireTimer >= GameManager.Instance.fireInterval)
        {
            fireTimer = 0f;
            Fire();
        }
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
            lastPointerX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        if (dragging && Input.GetMouseButton(0))
        {
            float dx = (Input.mousePosition.x - lastPointerX) * (12f / Screen.width);
            lastPointerX = Input.mousePosition.x;
            Vector3 p = transform.position;
            p.x = Mathf.Clamp(p.x + dx, -moveRange, moveRange);
            transform.position = p;
        }
    }

    void Fire()
    {
        if (playerMobPrefab == null) return;
        Vector3 pos = muzzle != null ? muzzle.position : transform.position + Vector3.forward;
        GameObject go = Instantiate(playerMobPrefab, pos, Quaternion.identity);
        Mob mob = go.GetComponent<Mob>();
        if (mob != null)
        {
            mob.isPlayer = true;
            mob.hp = GameManager.Instance.mobHp;
            mob.damage = GameManager.Instance.mobDamage;
            mob.speed = GameManager.Instance.mobSpeed;
        }
    }
}
