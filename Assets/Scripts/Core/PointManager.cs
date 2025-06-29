using UnityEngine;
using TMPro;

// Oyuncunun puanlarını yöneten sınıf
public class PointManager : MonoBehaviour
{
    public static PointManager Instance { get; private set; }

    public int playerPoints = 0; // Başlangıç puanı (şu anda kullanılmıyor)
    public TextMeshProUGUI PointText; // UI'de puanı gösterecek TextMeshPro referansı

    private void Awake()
    {
        // Singleton yapısı — sahnede yalnızca bir PointManager olsun
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Oyun başlarken puanı arayüzde güncelle
        UpdatePointUI();
    }

    // Puan ekleme fonksiyonu (görevden ödül gibi)
    public void AddPoints(int amount)
    {
        PlayerState.Instance.currentPoints += amount; // Oyuncunun puanına ekle
        UpdatePointUI(); // UI'de güncelle
    }

    // UI'deki puan metnini günceller
    public void UpdatePointUI()
    {
        if (PointText != null)
        {
            PointText.text = PlayerState.Instance.currentPoints.ToString(); // Sayısal değeri yaz
        }
        else
        {
            Debug.LogError("Point Text (TMP) UI nesnesi atanmadı!");
        }
    }
}
