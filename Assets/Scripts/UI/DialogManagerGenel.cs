using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

// Bu sınıf, sahnede kısa süreli uyarı mesajlarını göstermek için kullanılır.
// Uyarılar belirli bir süre görünüp ardından otomatik olarak kaybolur.
public class DialogManagerGenel : MonoBehaviour
{
    // Singleton örneği (diğer sınıflar bu nesneye doğrudan erişebilir)
    public static DialogManagerGenel Instance;

    public GameObject alertUI;            // Uyarı paneli (genellikle bir Canvas objesi içinde)
    public TextMeshProUGUI alertText;     // Uyarı mesajı metin alanı
    public float displayDuration = 2f;    // Uyarı mesajının ekranda kalma süresi (saniye)

    private void Awake()
    {
        // Singleton kurulumu
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Sahnedeki ikinci bir kopya varsa sil

        // Başlangıçta uyarı paneli kapalı olmalı
        if (alertUI != null)
            alertUI.SetActive(false);
    }

    // Uyarı mesajını ekranda gösteren fonksiyon
    public void ShowAlert(string message)
    {
        if (alertUI == null || alertText == null)
        {
            Debug.LogWarning("DialogManager: alertUI veya alertText atanmadı.");
            return;
        }

        // Mesajı yaz ve paneli göster
        alertText.text = message;
        alertUI.SetActive(true);

        // Aynı anda birden fazla coroutine çalışmasın
        StopAllCoroutines();

        // Belirli süre sonra paneli kapat
        StartCoroutine(HideAfterDelay());
    }

    // Uyarıyı belirli süre sonra gizleyen coroutine
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        alertUI.SetActive(false);
    }
}
