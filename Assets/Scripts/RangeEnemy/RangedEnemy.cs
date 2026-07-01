using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;

    [Header("Ranged Attack")]
    [SerializeField] private Transform firepoint;
    [SerializeField] private GameObject[] fireballs;

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    [SerializeField] private AudioClip fireballSound;

    // References
    private Animator anim;
    private EnemyPatrol enemyPatrol;
    private EnemyHealth enemyHealth; // Yangi qo'shildi

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        enemyHealth = GetComponent<EnemyHealth>(); // Yangi qo'shildi
    }

    private void Update()
    {
        // --- DIQQAT: Agar dushman o'lgan bo'lsa, pastdagi kodlarni ishlatma! ---
        if (enemyHealth != null && enemyHealth.currentHealth <= 0)
        {
            if (enemyPatrol != null) enemyPatrol.enabled = false;
            return;
        }
        // ---------------------------------------------------------------------

        cooldownTimer += Time.deltaTime;

        // Attack only when player in sight
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("rangedAttack");
                if (SoundManager.instance != null) SoundManager.instance.PlaySound(fireballSound);
            }
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    private void RangedAttack()
    {
        // Animatsiyadan chaqirilayotganda ham dushman o'lib qolgan bo'lsa otmasligi kerak
        if (enemyHealth != null && enemyHealth.currentHealth <= 0) return;

        int fireballIndex = FindFireball();

        // Agar o'q topilsa va dushman o'lmagan bo'lsa otadi
        if (fireballs[fireballIndex] != null)
        {
            cooldownTimer = 0;
            fireballs[fireballIndex].transform.position = firepoint.position;

            // O'q otilganda dushmanning o'ng/chapga qarab turgan yo'nalishini o'qqa ham berish (agar kerak bo'lsa)
            // fireballs[fireballIndex].transform.localScale = transform.localScale;

            fireballs[fireballIndex].GetComponent<RangeProjectile>().ActivateProjectile();
        }
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (fireballs[i] != null && !fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    private bool PlayerInSight()
    {
        if (boxCollider == null) return false;

        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
}