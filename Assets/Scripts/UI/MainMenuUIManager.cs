using UnityEngine;

// Ana menüdeki farklı UI panelleri arasında geçişleri yöneten sınıf
public class MainMenuUIManager : MonoBehaviour
{
    public GameObject mainMenu;         // Ana menü paneli
    public GameObject userSelectMenu;   // Kullanıcı seçimi paneli (Yeni Oyun / Yükleme için)
    public GameObject loadGameMenu;     // Kayıt yükleme paneli
    public GameObject settingsMenu;     // Ayarlar paneli

    // Sadece belirtilen paneli açar, diğer tüm panelleri kapatır
    public void ShowOnly(GameObject menuToOpen)
    {
        // Önce tüm panelleri kapat
        mainMenu.SetActive(false);
        userSelectMenu.SetActive(false);
        loadGameMenu.SetActive(false);
        settingsMenu.SetActive(false);

        // Açılmak istenen menüyü aktif hale getir
        if (menuToOpen != null)
            menuToOpen.SetActive(true);
    }
}
