using UnityEngine;
using UnityEngine.SceneManagement;

// Oyuncu öldüğünde ölüm panelini gösteren ve oyunu durduran sistem
public class DeathManager : MonoBehaviour
{
    // Singleton erişimi için statik instance
    public static DeathManager Instance;

    // UI elemanları
    public GameObject deathPanel;       // Oyuncu öldüğünde gösterilecek panel
    public GameObject mainMenuButton;   // Ana menüye dön butonu
    public GameObject quitButton;       // Oyunu kapatma butonu

    private void Awake()
    {
        // Singleton kurulumu
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Oyunun başında ölüm panelini gizle
        deathPanel.SetActive(false);
    }

    // Oyuncu öldüğünde çağrılır
    public void ShowDeathPanel()
    {
        deathPanel.SetActive(true); // Ölüm panelini göster
        Time.timeScale = 0f;        // Oyunu durdur

        // İmleci serbest bırak ve görünür yap
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Ana menüye dönme butonuna basıldığında çağrılır
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Zamanı tekrar normale al
        SceneManager.LoadScene("MainMenu"); // Ana menü sahnesini yükle
    }

    // Oyundan çıkış butonuna basıldığında çağrılır
    public void QuitGame()
    {
        Application.Quit(); // Oyunu kapat
    }
}
