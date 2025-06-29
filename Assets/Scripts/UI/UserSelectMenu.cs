using UnityEngine;
using UnityEngine.SceneManagement;

// Kullanıcı seçimini yöneten menü sınıfı.
// Yeni oyun başlatma ve kayıt yükleme modlarında çalışır.
public class UserSelectMenu : MonoBehaviour
{
    public GameObject loadGameMenu;     // Kayıt yükleme menüsü referansı
    public GameObject userSelectMenu;   // Bu menünün kendisi
    public GameObject mainMenu;         // Ana menü referansı

    private bool isNewGame = false;     // Yeni oyun mu, yoksa yükleme mi?

    // Yeni oyun başlatmak için kullanıcı seçimi menüsünü açar
    public void OpenForNewGame()
    {
        isNewGame = true;

        // Sadece UserSelectMenu görünür olacak şekilde menüleri ayarla
        mainMenu.SetActive(false);
        loadGameMenu.SetActive(false);
        userSelectMenu.SetActive(true);
    }

    // Kayıt yüklemek için kullanıcı seçimi menüsünü açar
    public void OpenForLoadGame()
    {
        isNewGame = false;

        mainMenu.SetActive(false);
        loadGameMenu.SetActive(false);
        userSelectMenu.SetActive(true);
    }

    // Bir kullanıcı seçildiğinde çağrılır
    public void SelectUser(int userId)
    {
        SaveManager.Instance.currentUserId = userId;
        userSelectMenu.SetActive(false);

        if (isNewGame)
        {
            SaveManager.Instance.currentSlotIndex = 0;
            SaveManager.Instance.shouldLoadFromFile = false; // Yeni oyun: kayıt yüklenmeyecek
            SceneManager.LoadScene("GameScene"); // GameScene’e geçiş yapılır
        }
        else
        {
            loadGameMenu.SetActive(true);
            FindObjectOfType<MainMenu>().LoadUserSlots(userId); // Kayıtlar yüklenir
        }
    }

    // Geri tuşuna basıldığında ana menüye dönüş yapılır
    public void GoBack()
    {
        userSelectMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
