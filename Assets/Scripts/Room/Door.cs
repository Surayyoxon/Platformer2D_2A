using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("--- ROOM TRANSITION ---")]
    [SerializeField] private Transform previousRoom;
    [SerializeField] private Transform nextRoom;
    [SerializeField] private CameraController cam;

    [Header("--- LEVEL EXIT (YUTUQ) ---")]
    [Tooltip("Agar bu darajaning eng oxiridagi marra eshigi bo'lsa, buni yoqing!")]
    [SerializeField] private bool isLevelExit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // --- 1. AGAR BU MARRA ESHIGI BO'LSA ---
            if (isLevelExit)
            {
                if (UIManager.Instance != null)
                {
                    PlayerCollectible collectible = collision.GetComponent<PlayerCollectible>();
                    if (collectible != null)
                    {
                        // Sahna o'zgargani uchun transform shartmas, null yuboramiz
                        UIManager.Instance.OnPlayerWin(
                            collectible.AppleCount,
                            collectible.BananaCount,
                            collectible.StrawberryCount,
                            null
                        );
                    }
                    else
                    {
                        UIManager.Instance.OnPlayerWin(0, 0, 0, null);
                    }
                }
                else
                {
                    Debug.LogError("Sahnada UIManager topilmadi!");
                }

                return; // Marra eshigi bo'lsa, pastdagi xonalararo o'tish kodlari ishlamaydi
            }

            // --- 2. ODDIY XONALARARO O'TISH LOGIKASI ---
            if (cam != null)
            {
                if (collision.transform.position.x < transform.position.x)
                    cam.MoveToNewRoom(nextRoom);
                else
                    cam.MoveToNewRoom(previousRoom);
            }

            // --- Spikehead'larni qaytarish qismi ---
            Spikehead[] allSpikeheads = Object.FindObjectsByType<Spikehead>(FindObjectsSortMode.None);
            foreach (Spikehead spike in allSpikeheads)
            {
                spike.ResetToDefaultPosition();
            }
        }
    }
}