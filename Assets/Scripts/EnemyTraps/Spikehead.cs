using UnityEngine;

public class Spikehead : EnemyDamage
{
    [Header("SpikeHead Attributes")]
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask layerMask; // YANGI: Bu yerga ham Player, ham Wall layerlarini beramiz
    [SerializeField] private AudioClip impactSound;

    private Vector3[] directions = new Vector3[4];
    private Vector3 destination;
    private float checkTimer;
    private bool attacking;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void OnEnable()
    {
        Stop();
    }

    private void Update()
    {
        if (attacking)
        {
            // Izoh: Agar Rigidbody2D ishlatayotgan bo'lsangiz, transform.Translate o'rniga 
            // rb.velocity = destination * speed; ishlatish tavsiya etiladi.
            transform.Translate(destination * Time.deltaTime * speed);
            CheckForWalls(); // YANGI: Harakatlanayotganda oldida devor borligini tekshiradi
        }
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > checkDelay)
                CheckForPlayer();
        }
    }

    private void CheckForPlayer()
    {
        CalculateDirections();

        for (int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i], Color.red);

            // O'ZGARTIRILDI: Endi nafaqat playerLayer, balki layerMask (Player + Wall) tekshiriladi
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i].normalized, range, layerMask);

            if (hit.collider != null && !attacking)
            {
                // Agar birinchi ko'rgan obyekti Player bo'lsa, keyin hujum qiladi (Devor bo'lsa hujum qilmaydi)
                if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
                {
                    attacking = true;
                    SoundManager.instance.PlaySound(impactSound);
                    destination = directions[i].normalized; // To'g'ri yo'nalish olinishi uchun .normalized qo'shildi
                    checkTimer = 0;
                }
            }
        }
    }

    // YANGI FUNKSIYA: Hujum paytida devorga urilishidan oldin to'xtatish
    private void CheckForWalls()
    {
        // Spikehead markazidan harakat yo'nalishi bo'yicha kichik lazer otamiz
        RaycastHit2D hit = Physics2D.Raycast(transform.position, destination, 0.6f, layerMask);

        // Agar biror narsaga urilsa va u Player bo'lmasa (ya'ni devor bo'lsa) to'xtaydi
        if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & playerLayer) == 0)
        {
            Stop();
        }
    }

    private void CalculateDirections()
    {
        directions[0] = transform.right * range;
        directions[1] = -transform.right * range;
        directions[2] = transform.up * range;
        directions[3] = -transform.up * range;
    }

    private void Stop()
    {
        destination = Vector3.zero; // Harakat yo'nalishini nolga tushiramiz
        attacking = false;
    }

    public void ResetToDefaultPosition()
    {
        Stop();
        transform.position = initialPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Agar o'yindagi devorlaringiz Trigger bo'lsa yoki Wall tegi bo'lsa to'xtatish
        if (collision.CompareTag("Wall"))
        {
            Stop();
            return;
        }

        if (!collision.CompareTag("Door"))
        {
            base.OnTriggerEnter2D(collision);
            Stop();
        }
    }
}