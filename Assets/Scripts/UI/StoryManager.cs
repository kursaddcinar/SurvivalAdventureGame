using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Bu sınıf, oyun içindeki hikaye anlatımını ve sinematik diyalog akışını yönetir.
// Oyun başında ya da özel NPC etkileşimlerinde hikaye panelini gösterir.
public class StoryManager : MonoBehaviour
{
    // Singleton yapı: Her yerden kolayca erişim sağlar
    public static StoryManager Instance { get; private set; }

    private GameObject npcToDestroyAfterStory; // Hikaye bittikten sonra sahneden silinecek NPC

    public GameObject storyPanel;             // Hikaye penceresi (UI paneli)
    public TextMeshProUGUI storyText;         // Hikaye metnini gösterecek yazı alanı
    public Button continueButton;             // "Devam" butonu

    private Queue<string> storyQueue = new Queue<string>(); // Hikaye satırları
    private bool storyFinished = false;       // Hikaye tamamlandı mı?

    private System.Action onStoryCompleteCallback = null; // Hikaye bitince çalışacak geri çağırma

    void Awake()
    {
        // Singleton kurulumu
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Butona tıklama olayını bağla
        continueButton.onClick.AddListener(OnContinueClicked);
    }

    // Hikaye başlatılır: ister varsayılan hikaye ister özel hikaye olabilir
    public void BeginStory(bool useDefaultStory = true, System.Action onComplete = null)
    {
        storyFinished = false; // Her başlangıçta sıfırlanmalı

        if (useDefaultStory)
        {
            LoadStory(); // Varsayılan hikayeyi yükle
        }

        // Paneli aç, zamanı durdur
        storyPanel.SetActive(true);
        Time.timeScale = 0f;

        // Farenin görünmesini ve serbest kalmasını sağla
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowNextLine(); // İlk satırı göster
        onStoryCompleteCallback = onComplete; // Hikaye bittiğinde ne yapılacağını sakla
    }

    // Varsayılan hikaye metni
    void LoadStory()
    {
        storyQueue.Clear();
        storyQueue.Enqueue("Dün gece gökyüzü denizle birleşti. Gök gürültüsü, yıldırımlar ve dev dalgalar...");
        storyQueue.Enqueue("Kaptan Buka, fırtına sırasında gemisiyle birlikte açık denizde alabora oldu.");
        storyQueue.Enqueue("Gözlerini açtığında, yüzü ıslak kumlara gömülüydü... Bir kıyıya vurmuştu.");
        storyQueue.Enqueue("Ama burası sıradan bir yer değildi. Burası Anamas Adası'ydı.");
        storyQueue.Enqueue("Ve bu adada onu bekleyen biri vardı: Taygun, adanın kadim bilgini.");
        storyQueue.Enqueue("Hayatta kalmak, görevleri tamamlamak ve özgürlüğe ulaşmak için onun rehberliği şart.");
        storyQueue.Enqueue("Efsanelerdeki Kadim bilgin Taygunü bul.");
    }

    // Dışarıdan özel hikaye/metin yüklenmek istenirse kullanılır
    public void LoadCustomDialogue(string[] lines, GameObject npcToDestroy = null)
    {
        storyQueue.Clear();
        storyFinished = false;

        npcToDestroyAfterStory = npcToDestroy; // Hikaye sonunda silinecek NPC

        foreach (var line in lines)
        {
            storyQueue.Enqueue(line);
        }
    }

    // Sıradaki satırı gösterir
    void ShowNextLine()
    {
        if (storyQueue.Count == 0)
        {
            // Hikaye bitti
            storyFinished = true;
            storyPanel.SetActive(false);

            // Zamanı devam ettir, fareyi kilitle
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;

            // Hikaye sonunda belirlenen NPC'yi sil
            if (npcToDestroyAfterStory != null)
                Destroy(npcToDestroyAfterStory);

            // Bitince yapılacak işlemi çalıştır
            onStoryCompleteCallback?.Invoke();
            return;
        }

        // Kuyruktan bir satır al ve yazıya aktar
        storyText.text = storyQueue.Dequeue();
    }

    // Devam butonuna tıklandığında sıradaki satırı göster
    void OnContinueClicked()
    {
        if (!storyFinished)
        {
            ShowNextLine();
        }
    }

    // Belirli isimdeki hayvan ölmüş mü diye kontrol eder
    public bool IsAnimalDead(string animalName)
    {
        foreach (var creature in FindObjectsOfType<Animal>())
        {
            if (creature.name.ToLower().Contains(animalName.ToLower()))
            {
                return false; // Hayvan sahnedeyse → ölmemiştir
            }
        }

        return true; // Sahnedeki hiçbir hayvan bu ismi taşımıyorsa → ölmüştür
    }
}
