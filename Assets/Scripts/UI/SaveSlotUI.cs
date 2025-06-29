using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveSlotUI : MonoBehaviour
{
    // Slotun kendisine ait UI bileşenleri
    public Button button;                     // Kayıt slotuna ait buton
    public TextMeshProUGUI slotText;          // Slotun başlığı ("Slot 0 (Dolu)" gibi)
    public TextMeshProUGUI dateText;          // Kayıt tarihi
    public TextMeshProUGUI scoreText;         // Kayıttaki skor bilgisi

    // Dahili veriler
    private int userId;                       // Hangi kullanıcıya ait
    private int slotIndex;                    // Slot numarası (0,1,2...)
    private bool hasData;                     // Slotta veri var mı?
    private SaveMenuMode mode;                // Slotun hangi menüden çağrıldığı: Yeni Oyun mu, Oyun İçi Kayıt mı?

    // Slotu başlatmak için çağrılır
    public void Initialize(int userId, int slotIndex, bool hasData, SaveMenuMode mode)
    {
        this.userId = userId;
        this.slotIndex = slotIndex;
        this.hasData = hasData;
        this.mode = mode;

        // Slot başlığını güncelle
        slotText.text = hasData ? $"Slot {slotIndex} (Dolu)" : $"Slot {slotIndex} (Boş)";

        // Eğer slotta veri varsa tarihi ve skoru göster
        if (hasData)
        {
            //string path = Application.dataPath + $"/Resources/Saves/save_user_{userId}_slot_{slotIndex}.json";
            string path = System.IO.Path.Combine(Application.persistentDataPath, $"save_user_{userId}_slot_{slotIndex}.json");

            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                AllGameData data = JsonUtility.FromJson<AllGameData>(json);

                if (dateText != null)
                    dateText.text = data.playerData.saveDateTime;

                if (scoreText != null)
                    scoreText.text = "Puan: " + data.playerData.points.ToString();
            }
        }
        else
        {
            if (dateText != null) dateText.text = "";
            if (scoreText != null) scoreText.text = "";
        }

        // Tıklama ayarı
        button.interactable = true;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnSlotClicked);
    }

    // Butona tıklanınca
    public void OnSlotClicked()
    {   
        int userId = SaveManager.Instance.GetPendingUserId(); // Geçici olarak seçili kullanıcı

        // Yeni oyun başlatma akışı
        if (mode == SaveMenuMode.NewGameSelect)
        {
            if (hasData)
            {
                // Dolu slota tıklanınca uyarı ver
                AlertDialogManager.Instance.Show(
                    "Bu slot dolu. Üzerine yeni oyun başlatmak istiyor musun?",
                    () =>
                    {
                        SaveManager.Instance.PrepareNewGame(userId, slotIndex);
                        SceneManager.LoadScene("GameScene");
                    });
            }
            else
            {
                // Boş slota tıklanınca direkt başlat
                SaveManager.Instance.PrepareNewGame(userId, slotIndex);
                SceneManager.LoadScene("GameScene");
            }
        }

        // Oyun içinden kayıt yapma akışı
        else if (mode == SaveMenuMode.SaveInGame)
        {
            if (hasData)
            {
                AlertDialogManager.Instance.Show(
                    "Bu slota mevcut ilerlemenizle kaydetmek istiyor musunuz?",
                    () =>
                    {
                        SaveManager.Instance.SaveGame(userId, slotIndex);
                        RefreshSlots(); // Slotları güncelle
                    });
            }
            else
            {
                SaveManager.Instance.SaveGame(userId, slotIndex);
                RefreshSlots(); // Slotları güncelle
            }
        }
    }

    // Slotlar yeniden yüklensin (örneğin kayıttan sonra güncelleme)
    private void RefreshSlots()
    {
        SaveGameMenuManager manager = FindObjectOfType<SaveGameMenuManager>();
        if (manager != null)
            manager.LoadSlotStates(mode);
    }
}
