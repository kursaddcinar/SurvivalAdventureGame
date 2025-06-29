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
}
