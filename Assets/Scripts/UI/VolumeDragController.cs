using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VolumeDragController : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    enum SoundType { Volume, Music }

    [SerializeField] private SoundType soundType;
    private Text textComponent;
    private float currentValue = 100f;

    private void Awake()
    {
        textComponent = GetComponent<Text>();
    }

    private void Start()
    {
        // Avval saqlangan sozlamani yuklash
        if (soundType == SoundType.Volume)
            currentValue = PlayerPrefs.GetFloat("VolumeValue", 100f);
        else
            currentValue = PlayerPrefs.GetFloat("MusicValue", 100f);

        UpdateVisualText();
        ApplyAudioVolume(); // O'yin boshlanganda ovozni moslashtirish
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ModifyValue(eventData.delta.x);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ModifyValue(eventData.delta.x);
    }

    private void ModifyValue(float dragDelta)
    {
        currentValue += dragDelta * 0.4f; // Sudrash sezgirligi
        currentValue = Mathf.Clamp(currentValue, 0f, 100f);

        UpdateVisualText();
        SaveSettings();
        ApplyAudioVolume(); // Real vaqtda ovoz balandligini o'zgartirish
    }

    private void UpdateVisualText()
    {
        if (textComponent != null)
        {
            string prefix = soundType == SoundType.Volume ? "Volume: " : "Music: ";
            textComponent.text = prefix + Mathf.RoundToInt(currentValue) + "%";
        }
    }

    private void SaveSettings()
    {
        if (soundType == SoundType.Volume)
            PlayerPrefs.SetFloat("VolumeValue", currentValue);
        else
            PlayerPrefs.SetFloat("MusicValue", currentValue);

        PlayerPrefs.Save();
    }

    // 3) OVOZ VA MUSIQALARNI REAL VAQTDA O'ZGARTIRISH FUNKSIYASI
    private void ApplyAudioVolume()
    {
        float normalizedVolume = currentValue / 100f; // 0.0 dan 1.0 gacha oraliqqa o'tkazish

        // Sahnadagi barcha AudioSource elementlarini topamiz
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource audio in allAudioSources)
        {
            // Agar loyihangizda SoundManager bo'lsa, uning AudioSource'larini ajratib olish mumkin.
            // Quyidagi mantiq oddiy o'yinlar uchun barcha AudioSource'larni to'g'ridan-to'g'ri boshqaradi:
            if (soundType == SoundType.Music)
            {
                // Agar AudioSource "loop" (musiqa) bo'lsa Background Music deb hisoblaymiz
                if (audio.loop) audio.volume = normalizedVolume;
            }
            else
            {
                // Agar effekt tovushlari (sakrash, tanga yig'ish) bo'lsa loop bo'lmaydi
                if (!audio.loop) audio.volume = normalizedVolume;
            }
        }
    }
}