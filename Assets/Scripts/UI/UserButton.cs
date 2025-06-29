using UnityEngine;
using UnityEngine.UI;

// Kullanıcı seçim butonunu temsil eder.
// Bu butona tıklanınca, ilgili kullanıcı ID’si MenuManager’a gönderilir.
public class UserButton : MonoBehaviour
{
    public int userId; // Bu butonun temsil ettiği kullanıcı ID’si
    public MenuManager menuManager; // Menü işlemlerini yöneten manager referansı

    void Start()
    {
        // Butona tıklanınca OnClick fonksiyonunu çağır
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (menuManager != null)
        {
            // Kullanıcı seçildiğinde MenuManager’a bildir
            menuManager.OnUserSelected(userId);
        }
        else
        {
            Debug.LogError("MenuManager atanmadı!"); // Hata kontrolü
        }
    }
}
