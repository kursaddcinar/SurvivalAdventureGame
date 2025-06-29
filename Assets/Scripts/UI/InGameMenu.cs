using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Oyun içerisindeki "Ayarlar" veya "Pause" menüsünden ana menüye dönmeyi yöneten sınıf
public class InGameMenu : MonoBehaviour
{
    // Bu metod, ana menü sahnesine geçiş yapar
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // "MainMenu" adlı sahne yüklenir
    }

    /*
    // Bu metod, oyundan tamamen çıkmak için kullanılabilir
    public void ExitGame()
    {
        Debug.Log("Quitting Game");         // Editor'de çıkışı test etmek için log
        Application.Quit();                 // Derlenmiş oyunda uygulamayı kapatır
    }
    */
}
