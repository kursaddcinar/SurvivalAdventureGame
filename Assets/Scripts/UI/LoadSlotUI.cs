using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// LoadSlot arayüzünü yöneten sınıf (Kayıt yükleme arayüzündeki her bir slot için)
public class LoadSlotUI : MonoBehaviour
{
    public Button button;                        // Slot'a tıklama butonu
    public TextMeshProUGUI slotText;             // Slot durumu (boş/dolu) yazısı
    public TextMeshProUGUI dateText;             // Kayıt tarihi gösterimi
    public TextMeshProUGUI scoreText;            // Puan gösterimi

    private int userId;                          // Bu slotun ait olduğu kullanıcı ID’si
    private int slotIndex;                       // Bu slotun index’i
    private bool hasData;                        // Bu slota ait veri var mı?

    // Slotu başlatmak için kullanılan fonksiyon
    public void Initialize(int userId, int slotIndex, bool hasData)
    {
        this.userId = userId;
        this.slotIndex = slotIndex;
        this.hasData = hasData;

        // Slot metnini doluluk durumuna göre ayarla
        slotText.text = hasData ? $"Slot {slotIndex} (Yüklenebilir)" : $"Slot {slotIndex} (Boş)";

        // Sadece veri varsa buton aktif olsun
        button.interactable = hasData;

        // Eğer kayıtlı veri varsa, tarih ve puan bilgilerini dosyadan çek
        if (hasData)
        {
            //string path = Application.dataPath + $"/Resources/Saves/save_user_{userId}_slot_{slotIndex}.json";
            string path = System.IO.Path.Combine(Application.persistentDataPath, $"save_user_{userId}_slot_{slotIndex}.json");

            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                AllGameData data = JsonUtility.FromJson<AllGameData>(json);

                if (dateText != null)
                    dateText.text = "Tarih: " + data.playerData.saveDateTime;

                if (scoreText != null)
                    scoreText.text = "Puan: " + data.playerData.points.ToString();
            }
        }
        else
        {
            // Boş slot ise tarih ve skor metinlerini temizle
            if (dateText != null)
                dateText.text = "";

            if (scoreText != null)
                scoreText.text = "";
        }

        // Tıklama event'ini temizle ve yeniden ayarla
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnSlotClicked);
    }

    // Slot'a tıklanınca çalışacak fonksiyon
    private void OnSlotClicked()
    {
        PlayerData data = SaveManager.Instance.PeekSaveData(userId, slotIndex);
        if (data != null && data.gameIsCompleted)
        {
            
            AlertDialogManager.Instance.ShowInfo(
                "Mevcut kayıtta oyun tamamlanmıştır.\nYeni oyun açınız veya başka kaydı yükleyiniz.");
            return;
        }

        // Kayıtlı kullanıcı ve slot bilgisi SaveManager'a gönderilir
        SaveManager.Instance.TriggerLoadAfterScene(userId, slotIndex);
        

        // Oyun sahnesi yüklenir
        SceneManager.LoadScene("GameScene");
    }
}
