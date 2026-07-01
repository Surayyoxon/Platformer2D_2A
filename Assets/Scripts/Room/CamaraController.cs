using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("--- FOLLOW PLAYER (X-AXIS ONLY) ---")]
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance = 2f; // O'yinchi yo'nalishiga qarab oldindan ko'rish masofasi
    [SerializeField] private float cameraSpeed = 5f;    // Kameraning playerga yetib olish tezligi
    [SerializeField] private float smoothTime = 0.2f;   // SmoothDamp silliqlik vaqti

    private Vector3 velocity = Vector3.zero;
    private float lookAhead;

    // Kameraning o'yinda o'rnatilgan boshlang'ich Y va Z koordinatalarini saqlash uchun
    private float fixedY;
    private float fixedZ;

    private void Start()
    {
        // 1. Agar o'yinchi biriktirilmagan bo'lsa, avtomatik topamiz
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // 2. Kameraning Unity Inspector'da o'rnatilgan Y va Z pozitsiyalarini qulflaymiz
        fixedY = transform.position.y;
        fixedZ = transform.position.z;

        // O'yin boshlanganda kamerani player turgan X koordinatasiga to'g'rilaymiz
        if (player != null)
        {
            transform.position = new Vector3(player.position.x, fixedY, fixedZ);
        }
    }

    private void LateUpdate()
    {
        // O'yinchi yo'qligini tekshirish (NullReferenceException oldini oladi)
        if (player == null) return;

        // 3. AYLANISHNI MUTLOQ TAQIQLASH (Kamera rotatsiyasini har doim nol holatda ushlaydi)
        transform.rotation = Quaternion.identity;

        // O'yinchi o'ngga yoki chapga qaraganiga qarab oldinga siljishni silliq hisoblash
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * Mathf.Sign(player.localScale.x)), Time.deltaTime * cameraSpeed);

        // 4. MAQSADLI POZITSIYA (Faqat X o'zgaradi, Y va Z har doim qulflangan)
        float targetX = player.position.x + lookAhead;
        Vector3 targetPosition = new Vector3(targetX, fixedY, fixedZ);

        // Kamerani faqat gorizontal yo'nalishda silliq ko'chirish
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }

    // Eski skriptlardan qolgan eshik va checkpoint funksiyasi xato bermasligi uchun saqlandi
    public void MoveToNewRoom(Transform _newRoom)
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(player.position.x, fixedY, fixedZ);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}