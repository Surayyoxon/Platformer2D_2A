using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Checkpoint")]
    [SerializeField] private AudioClip checkpointSound;

    private Transform currentCheckpoint;
    private Health playerHealth;
    private Vector3 startPosition;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
    }

    private void Start()
    {
        startPosition = transform.position;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetHasCheckpoint(false);
        }
    }

    // --- MANA BU FUNKSIYA ENDI TO'G'RI JOYDASHDI VA QAVSLARI JOYIDA ---
    public void RespawnCheck()
    {
        if (playerHealth != null)
            playerHealth.Respawn();

        if (currentCheckpoint != null)
        {
            Vector3 spawnPosition = currentCheckpoint.position;
            spawnPosition.y += 0.5f;
            transform.position = spawnPosition;

            CameraController cameraController = Camera.main != null ? Camera.main.GetComponent<CameraController>() : null;
            if (cameraController != null && currentCheckpoint.parent != null)
            {
                cameraController.MoveToNewRoom(currentCheckpoint.parent);
            }
        }
        else
        {
            transform.position = startPosition;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Agar Unity versiyangiz eskiroq bo'lsa va bu yerda xato bersa, 
            // rb.linearVelocity o'rniga rb.velocity = Vector2.zero; deb yozing
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Checkpoint"))
            return;

        currentCheckpoint = collision.transform;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetCheckpoint(currentCheckpoint.position);
        }

        if (SoundManager.instance != null && checkpointSound != null)
        {
            SoundManager.instance.PlaySound(checkpointSound);
        }

        Animator animator = collision.GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("activate");

        Collider2D col = collision.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        Debug.Log("Checkpoint saqlandi: " + currentCheckpoint.position);
    }

    public void ResetCheckpoint()
    {
        currentCheckpoint = null;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetHasCheckpoint(false);
        }
    }
}