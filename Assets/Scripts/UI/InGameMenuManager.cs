using UnityEngine;

// Oyun sırasında "M" tuşuyla açılıp kapanan oyun içi menüyü yöneten sınıf
public class InGameMenuManager : MonoBehaviour
{
    public GameObject menuPanel; // Ana menü UI root objesi
    public GameObject mainButtonsPanel; // Ana butonlar: Save, Settings, Back
    public GameObject settingsPanel; // Ayarlar paneli
    public GameObject saveGamePanel; // Kayıt paneli
    public GameObject guidePanel; // Oyun rehberi paneli

    private bool isMenuOpen = false; // Menü açık mı?

    void Update()
    {
        // "M" tuşuna basıldığında menüyü aç/kapat
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    // Menü durumunu değiştirir
    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        menuPanel.SetActive(isMenuOpen); // Menü panelini göster/gizle

        if (isMenuOpen)
        {
            // Menü açıldığında sadece ana butonlar görünür
            mainButtonsPanel.SetActive(true);
            settingsPanel.SetActive(false);
            saveGamePanel.SetActive(false);
        }

        // İmleç kontrolü
        Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isMenuOpen;
    }

    // "Save Game" butonuna basıldığında çağrılır
    public void OnSaveGameClicked()
    {
        mainButtonsPanel.SetActive(false);
        saveGamePanel.SetActive(true);
        settingsPanel.SetActive(false);

        // Save panelini başlat (slotları yükle)
        var manager = saveGamePanel.GetComponent<SaveGameMenuManager>();
        manager.InitializeSlots(SaveManager.Instance.GetPendingUserId(), SaveMenuMode.SaveInGame);
    }

    // "Settings" butonuna basıldığında çağrılır
    public void OnSettingsClicked()
    {
        mainButtonsPanel.SetActive(false);
        settingsPanel.SetActive(true);
        saveGamePanel.SetActive(false);
    }

    // "Guide" butonuna basıldığında çağrılır
    public void OnGuideClicked()
    {
        mainButtonsPanel.SetActive(false);
        guidePanel.SetActive(true);
    }

    // "Back to Game" butonuna basıldığında menüyü kapatır
    public void OnBackToGameClicked()
    {
        ToggleMenu();
    }

    // Save veya Settings panelinden geri dönüldüğünde ana butonlar panelini geri açar
    public void OnBackFromSubPanel()
    {
        settingsPanel.SetActive(false);
        saveGamePanel.SetActive(false);
        guidePanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
    }
}
