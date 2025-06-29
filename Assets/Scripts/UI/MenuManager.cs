// MenuManager.cs
// Bu script, MainMenu sahnesinde UI panelleri arasında geçişleri ve kullanıcı seçimi sonrası kayıt/başlatma akışını yönetir.

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;     // Ana menü paneli (New Game, Load Game, Settings vs.)
    public GameObject settingsPanel;     // Ayarlar menüsü
    public GameObject loadGamePanel;     // Kayıt yükleme slotlarını içeren panel
    public GameObject userSelectPanel;   // Kullanıcı seçim ekranı
    public GameObject saveGamePanel;     // Kayıt etme veya yeni oyun slot seçimi paneli
    public GameObject rankingPanel;     // skor list paneli

    // Menüden hangi amaçla kullanıcı seçildiğini belirleyen enum
    public enum MenuContext { None, NewGame, LoadGame }
    private MenuContext currentContext;

    // "New Game" butonuna basıldığında tetiklenir
    public void OnNewGameClicked()
    {
        currentContext = MenuContext.NewGame;
        ShowUserSelectMenu();
    }

    // "Load Game" butonuna basıldığında tetiklenir
    public void OnLoadGameClicked()
    {
        currentContext = MenuContext.LoadGame;
        ShowUserSelectMenu();
    }

    // Kullanıcı butonlarından biri seçildiğinde tetiklenir
    public void OnUserSelected(int userId)
    {
        SaveManager.Instance.SetPendingUser(userId);

        if (currentContext == MenuContext.NewGame)
        {
            ShowNewGameSlotMenu(); // Yeni oyun için boş/dolu slotları göster
        }
        else if (currentContext == MenuContext.LoadGame)
        {
            if (SaveManager.Instance.HasAnySave(userId))
            {
                ShowLoadSlotMenu(); // Kayıtlı slotlar varsa yükleme ekranını aç
            }
            else
            {
                Debug.Log("Kayıt yok.");
            }
        }
    }

    // Yeni oyun başlatmak için slot menüsünü açar
    private void ShowNewGameSlotMenu()
    {
        userSelectPanel.SetActive(false);
        saveGamePanel.SetActive(true);

        var manager = saveGamePanel.GetComponent<SaveGameMenuManager>();
        manager.InitializeSlots(SaveManager.Instance.GetPendingUserId(), SaveMenuMode.NewGameSelect);
    }

    // Kayıtlı oyunları yüklemek için slot menüsünü açar
    private void ShowLoadSlotMenu()
    {
        userSelectPanel.SetActive(false);
        loadGamePanel.SetActive(true);

        LoadGameMenuManager loadManager = loadGamePanel.GetComponent<LoadGameMenuManager>();
        loadManager.LoadSlots();
    }

    // Ayarlar paneline geçer
    public void OnSettingsClicked()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // skor list paneline geçer
    public void ShowRanking()
    {
        mainMenuPanel.SetActive(false);
        rankingPanel.SetActive(true);
    }
    // Kullanıcı seçim ekranını açar
    private void ShowUserSelectMenu()
    {
        mainMenuPanel.SetActive(false);
        userSelectPanel.SetActive(true);
    }

    // Ayarlardan geri döner
    public void OnBackFromSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // skordan geri döner
    public void OnBackFromRanking()
    {
        rankingPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // LoadGame panelinden geri döner
    public void OnBackFromLoadSlotMenu()
    {
        loadGamePanel.SetActive(false);
        userSelectPanel.SetActive(true);
    }

    // Kullanıcı seçiminden geri döner
    public void OnBackFromUserSelect()
    {
        userSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // SaveGame/NewGame slot seçiminden geri döner
    public void OnBackFromSaveGameMenu()
    {
        saveGamePanel.SetActive(false);
        userSelectPanel.SetActive(true);
    }

    // Oyundan çıkış yapar
    public void OnExitClicked()
    {
        Application.Quit();
    }
}
