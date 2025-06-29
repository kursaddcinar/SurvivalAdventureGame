using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadSlot : MonoBehaviour
{
    [SerializeField] private Button button;                    // Slot butonunu temsil eder
    [SerializeField] private TextMeshProUGUI buttonText;       // Butonun üzerindeki yazı
    public int slotIndex;                                      // Slotun kaçıncı olduğu
    public int userId;                                         // Hangi kullanıcıya ait olduğu

    private void Awake()
    {
        // Referanslar null ise otomatik olarak bul
        if (button == null)
            button = GetComponent<Button>();

        if (buttonText == null)
            buttonText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        RefreshUI();  // UI'yı başta güncelle

        // Önceki listener'ları temizle ve tıklama eventi ata
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnSlotClicked);
    }

    // Harici çağrıldığında bu slotu yapılandırmak için kullanılır
    public void Initialize(int userId, int slotIndex)
    {
        this.userId = userId;
        this.slotIndex = slotIndex;

        RefreshUI();  // Yeniden UI'yı güncelle

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnSlotClicked);
    }

    // Slot'a tıklanınca yapılacak işlem
    public void OnSlotClicked()
    {
        string path = Application.dataPath + "/Resources/Saves/" + $"SavedGame_user_{userId}_slot_{slotIndex}.json";

        if (File.Exists(path))
        {
            SaveManager.Instance.TriggerLoadAfterScene(userId, slotIndex); // Global değişkenleri ayarla
            SceneManager.LoadScene("GameScene");                          // GameScene'e geç
            Debug.Log("Kayıt dosyası var.");
        }
        else
        {
            Debug.Log("Kayıt dosyası yok.");
        }
    }

    // UI metnini kayıt durumuna göre günceller
    public void RefreshUI()
    {
        string path = Application.dataPath + "/Resources/Saves/" + $"SavedGame_user_{userId}_slot_{slotIndex}.json";

        if (File.Exists(path))
        {
            buttonText.text = $"Slot {slotIndex + 1} (Kayıt Var)";
            //button.interactable = true; // Gerekirse aktif edilebilir
        }
        else
        {
            buttonText.text = $"Slot {slotIndex + 1} (Boş)";
            //button.interactable = false;
        }
    }
}
