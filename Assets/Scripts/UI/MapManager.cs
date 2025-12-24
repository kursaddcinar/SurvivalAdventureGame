using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [Header("Harita UI Bileşenleri")]
    public GameObject mapPanel;
    public RectTransform mapBackground;
    public RectTransform playerIcon;

    [Header("Dünya Ayarları (Kalibrasyon)")]
    public Transform playerTransform;

    // BURASI ÖNEMLİ: Harita resminin SOL ALT köşesinin denk geldiği dünya koordinatı.
    // Terrain'in -84, -60'da ise buraya o değerleri gireceğiz.
    public Vector2 mapWorldOrigin; 
    
    // Haritanın kapladığı alan (1000, 1000)
    public Vector2 worldSize = new Vector2(1000, 1000); 

    private bool isMapOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) ToggleMap();
        if (isMapOpen) UpdatePlayerIconPosition();
    }

    public void ToggleMap()
    {
        isMapOpen = !isMapOpen;
        mapPanel.SetActive(isMapOpen);
        if (isMapOpen) UpdatePlayerIconPosition();
    }

    private void UpdatePlayerIconPosition()
    {
        if (playerTransform == null) return;

        Vector3 playerPos = playerTransform.position;

        // 1. ADIM: Oyuncunun pozisyonunu harita başlangıç noktasına göre sıfırlıyoruz (Offset removal)
        // Eğer Origin -84 ise ve oyuncu -84'teyse sonuç 0 olur.
        float offsetPosX = playerPos.x - mapWorldOrigin.x;
        float offsetPosZ = playerPos.z - mapWorldOrigin.y; // Z -> Y (2D)

        // 2. ADIM: Normalize et (0 ile 1 arasına çek)
        // Eğer offsetPos 500 ise ve Size 1000 ise sonuç 0.5 olur.
        float normalizedX = offsetPosX / worldSize.x;
        float normalizedY = offsetPosZ / worldSize.y;

        // UI HESAPLAMASI
        float mapWidth = mapBackground.rect.width;
        float mapHeight = mapBackground.rect.height;

        // UI Pivotun (0.5, 0.5) yani tam ortada olduğunu varsayarak:
        // 0.5 çıkartıyoruz ki koordinat sistemi 0..1'den -0.5..+0.5'e dönüşsün.
        float iconX = (normalizedX - 0.5f) * mapWidth;
        float iconY = (normalizedY - 0.5f) * mapHeight;

        playerIcon.anchoredPosition = new Vector2(iconX, iconY);
        
        // Rotasyon (Opsiyonel)
        playerIcon.localEulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
    }

    // DEBUG: Harita sınırlarını Sol Alt'tan başlayarak çiziyoruz.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Başlangıç noktası (Sol Alt)
        Vector3 origin = new Vector3(mapWorldOrigin.x, 0, mapWorldOrigin.y);
        
        // Bitiş noktası (Sağ Üst)
        Vector3 end = new Vector3(mapWorldOrigin.x + worldSize.x, 0, mapWorldOrigin.y + worldSize.y);

        // Dikdörtgenin 4 köşesini hesaplayıp çizelim (WireCube yerine Line kullanıyorum daha net anlaşılsın diye)
        Vector3 topLeft = new Vector3(origin.x, 0, origin.z + worldSize.y);
        Vector3 bottomRight = new Vector3(origin.x + worldSize.x, 0, origin.z);

        // Çizgiler
        Gizmos.DrawLine(origin, topLeft);       // Sol kenar
        Gizmos.DrawLine(topLeft, end);          // Üst kenar
        Gizmos.DrawLine(end, bottomRight);      // Sağ kenar
        Gizmos.DrawLine(bottomRight, origin);   // Alt kenar
        
        // Köşeye bir küre koyalım ki başlangıcın neresi olduğunu gör
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(origin, 5f); // Sol alt köşeyi işaretler
    }
}
/*
using UnityEngine;
using UnityEngine.UI;

// Oyuncu haritasını yöneten sınıf – haritayı açma/kapama ve oyuncu ikonunu güncelleme işlemlerini yapar.
public class MapManager : MonoBehaviour
{
    [Header("Harita UI Bileşenleri")]
    public GameObject mapPanel;               // Harita panelini temsil eden GameObject (Canvas içindeki panel)
    public RectTransform mapBackground;       // Harita arka plan görüntüsü (örn. UI Image)
    public RectTransform playerIcon;          // Oyuncuyu haritada temsil eden ikon (örn. küçük bir daire)

    [Header("Oyuncu Bilgileri")]
    public Transform playerTransform;         // Sahnedeki oyuncu GameObject'inin transform'u
    public Vector2 worldSize = new Vector2(100, 100); // Gerçek oyun dünyasının boyutları (X ve Z düzleminde)

    private bool isMapOpen = false; // Harita açık mı kapalı mı kontrolü

    void Update()
    {
        // Oyuncu 'Y' tuşuna bastığında haritayı aç/kapat
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }

        // Harita açıksa her karede oyuncunun haritadaki pozisyonu güncellenir
        if (isMapOpen)
        {
            UpdatePlayerIconPosition();
        }
    }

    // Harita panelini görünür/gizli yapar
    public void ToggleMap()
    {
        isMapOpen = !isMapOpen; // Aksi duruma geçir
        mapPanel.SetActive(isMapOpen); // Harita panelini aktif veya pasif yap

        // Eğer harita açıldıysa, oyuncunun ikon pozisyonu anında güncellenir
        if (isMapOpen)
        {
            UpdatePlayerIconPosition();
        }
    }

    // Oyuncunun dünya üzerindeki pozisyonuna göre haritadaki konumunu günceller
    private void UpdatePlayerIconPosition()
    {
        Vector3 playerPos = playerTransform.position; // Oyuncunun dünya üzerindeki pozisyonu alınır

        // Oyuncu pozisyonu dünya boyutlarına göre normalize edilir (0 ile 1 arasında)
        float normalizedX = Mathf.Clamp01(playerPos.x / worldSize.x); // X ekseni
        float normalizedY = Mathf.Clamp01(playerPos.z / worldSize.y); // Z ekseni → haritada Y olarak kullanılır

        // Harita arka planının genişlik ve yüksekliği alınır
        float mapWidth = mapBackground.rect.width;
        float mapHeight = mapBackground.rect.height;

        // Normalize edilmiş pozisyon, haritadaki koordinatlara dönüştürülür
        float iconX = (normalizedX - 0.5f) * mapWidth;   // -0.5'le merkeze göre konumlanır
        float iconY = (normalizedY - 0.5f) * mapHeight;

        // Oyuncu ikonu yeni koordinatlara yerleştirilir
        playerIcon.anchoredPosition = new Vector2(iconX, iconY);
    }
}*/
