using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Ana menüdeki butonları ve arayüz geçişlerini yöneten sınıf
public class MainMenu : MonoBehaviour
{
    public Button LoadGameBTN; // "Load Game" butonuna referans
    public UserSelectMenu userSelectMenu; // Kullanıcı seçme paneline referans
    public LoadSlot[] slotButtons; // Slot butonları (örneğin 2 slot için)

    public int slotIndex; // Seçilen slot
    public int userId; // Seçilen kullanıcı

    public MainMenuUIManager uiManager; // Ana menü UI geçişlerini yöneten yardımcı sınıf

    // Yeni Oyun başlatıldığında çalışır
    public void NewGame()
    {
        // Kullanıcı seçme panelini yeni oyun için açar
        userSelectMenu.OpenForNewGame();

        // UI Manager üzerinden sadece kullanıcı seçme panelini görünür yapar
        uiManager.ShowOnly(userSelectMenu.gameObject); 
    }

    // Kayıtlı oyun yükleme menüsü açıldığında çalışır
    public void LoadGame()
    {
        // Kullanıcı seçme panelini kayıt yükleme modu için açar
        userSelectMenu.OpenForLoadGame();

        // UI Manager ile sadece kullanıcı seçme menüsünü göster
        uiManager.ShowOnly(userSelectMenu.gameObject);
    }

    // Oyunu kapatır
    public void ExitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    // Slot tıklanınca çalışır, geçici olarak kullanıcı ve slot bilgisi SaveManager’a aktarılır
    public void OnClickLoad()
    {
        SaveManager.Instance.currentUserId = userId;
        SaveManager.Instance.currentSlotIndex = slotIndex;

        // Yüklemeyi doğrudan burada yapmıyoruz. Sadece bilgiyi taşıyoruz.
        // LoadGame çağrısı sahne geçişiyle yapılmalı.
        //SaveManager.Instance.LoadGame(userId, slotIndex);
    }

    // Seçilen kullanıcıya ait slotları arayüzde gösterir
    public void LoadUserSlots(int userId)
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            slotButtons[i].Initialize(userId, i); // Her slot için kullanıcı ve index atanır
        }
    }
}
