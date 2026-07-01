using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    private float lifetime;
    private Animator anim;
    private BoxCollider2D coll;

    private bool hit;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    public void ActivateProjectile()
    {
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        coll.enabled = true;
    }
    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Agar allaqachon biror narsaga tekkan bo'lsa, qayta ishlamasin
        if (hit) return;

        hit = true;
        base.OnTriggerEnter2D(collision); // Ota skriptdagi zarba logikasi (masalan, playerga zarar yetkazish)
        coll.enabled = false;

        // Hech qanday animatsiyasiz o'qni srazu o'chirish
        gameObject.SetActive(false);
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
