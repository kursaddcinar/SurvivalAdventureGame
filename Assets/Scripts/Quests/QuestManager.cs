using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Tüm görevlerin takibini ve UI yönetimini yapan ana sınıf
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; set; } // Singleton pattern

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Başka bir instance varsa bu objeyi yok et
        }
        else
        {
            Instance = this; // Singleton ataması
        }
    }

    public List<Quest> allActiveQuests;        // Oyuncunun aldığı ama tamamlamadığı görevler
    public List<Quest> allCompletedQuests;     // Oyuncunun tamamladığı görevler

    [Header("QuestMenu")] // Görev menüsünü yöneten bileşenler
    public GameObject questMenu;               // Görev menüsü UI objesi
    public bool isQuestMenuOpen;               // Görev menüsünün açık olup olmadığını kontrol eder

    public GameObject activeQuestPrefab;       // Aktif görev prefabı
    public GameObject completedQuestPrefab;    // Tamamlanmış görev prefabı
    public GameObject questMenuContent;        // Scroll içeriği vs.

    [Header("QuestTracker")] // Oyun ekranında görevleri canlı takip etmek için
    public GameObject questTrackerContent;
    public GameObject trackerRowPrefab;

    public List<Quest> allTrackedQuests;       // Oyuncunun takip ettiği görevler

    // Görevi takip listesine ekler
    public void TrackQuest(Quest quest)
    {
        allTrackedQuests.Add(quest);
        RefreshTrackerList();
    }

    // Görevi takip listesinden çıkarır
    public void UntrackQuest(Quest quest)
    {
        allTrackedQuests.Remove(quest);
        RefreshTrackerList();
    }

    // Checkpoint'leri formatlı bir şekilde string'e dönüştürür
    private string PrintCheckpoints(Quest trackedQuest, string existingText)
    {
        var finalText = existingText;

        foreach (Checkpoint cp in trackedQuest.info.checkpoints)
        {
            if (cp.isCompleted)
            {
                finalText = finalText + "\n" + cp.name + " [completed]";
            }
            else
            {
                finalText = finalText + "\n" + cp.name;
            }
        }
        return finalText;
    }

    // Görev takip listesini ekrana yeniden çizer
    public void RefreshTrackerList()
    {
        // Eski listeyi temizle
        foreach (Transform child in questTrackerContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Yeni görevleri sırayla listele
        foreach (Quest trackedQuest in allTrackedQuests)
        {
            GameObject trackerPrefab = Instantiate(trackerRowPrefab, Vector3.zero, Quaternion.identity);
            trackerPrefab.transform.SetParent(questTrackerContent.transform, false);

            TrackerRow tRow = trackerPrefab.GetComponent<TrackerRow>();

            tRow.questName.text = trackedQuest.questName;
            tRow.description.text = trackedQuest.questDescription;

            var req1 = trackedQuest.info.firstRequirmentItem;
            var req1Amount = trackedQuest.info.firstRequirementAmount;
            var req2 = trackedQuest.info.secondRequirmentItem;
            var req2Amount = trackedQuest.info.secondRequirementAmount;

            // İki gereksinim varsa ikisini de göster
            if (trackedQuest.info.secondRequirmentItem != "")
            {
                tRow.requirements.text = $"{req1} {InventorySystem.Instance.CheckItemAmount(req1)}/{req1Amount}\n" +
                                         $"{req2} {InventorySystem.Instance.CheckItemAmount(req2)}/{req2Amount}";
            }
            else // Sadece bir gereksinim varsa
            {
                tRow.requirements.text = $"{req1} {InventorySystem.Instance.CheckItemAmount(req1)}/{req1Amount}";
            }

            // Checkpoint'ler varsa listeye ekle
            if (trackedQuest.info.hasCheckPoints)
            {
                var existingText = tRow.requirements.text;
                tRow.requirements.text = PrintCheckpoints(trackedQuest, existingText);
            }
        }
    }

    // Q tuşuyla görev menüsünü açma-kapatma işlemi
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isQuestMenuOpen && ConstructionManager.Instance.inConstructionMode == false)
        {
            questMenu.SetActive(true);
            //questMenu.GetComponentInChildren<Canvas>().sortingOrder = MenuManager.Instance.SetAsFront();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisabledSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;

            isQuestMenuOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && isQuestMenuOpen)
        {
            questMenu.SetActive(false);

            if (CraftingSystem.Instance.isOpen == false || InventorySystem.Instance.isOpen == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SelectionManager.Instance.DisabledSelection();
                SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
                SelectionManager.Instance.centerDotImage.enabled = true;
            }

            isQuestMenuOpen = false;
        }
    }

    // Bir görevi aktif görev listesine ekler, takibe alır ve listeyi yeniler
    public void AddActiveQuest(Quest quest)
    {
        allActiveQuests.Add(quest);       // Aktif görev listesine ekle
        TrackQuest(quest);                // Takip listesine de ekle
        RefreshQuestList();              // UI listesini yenile
    }

    // Görevi tamamlandı olarak işaretler ve ilgili işlemleri yapar
    public void MarkQuestCompleted(Quest quest)
    {
        allActiveQuests.Remove(quest);             // Aktif görev listesinden çıkar
        allCompletedQuests.Add(quest);             // Tamamlanan görev listesine ekle
        UntrackQuest(quest);                       // Takipten kaldır
        RefreshQuestList();                        // UI listesini yenile

        NPCManager.Instance.CheckQuestProgressAndAdvance(); // NPC zinciri yönetimi

        //PointManager.Instance.AddPoints(quest.info.coinReward); // Puan sistemi entegrasyonu
    }

    // Görev menüsündeki görev listesini UI'da yeniler
    public void RefreshQuestList()
    {
        // Önceki görev UI objelerini temizle
        foreach (Transform child in questMenuContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Aktif görevleri oluştur ve UI'ya yerleştir
        foreach (Quest activeQuest in QuestManager.Instance.allActiveQuests)
        {
            GameObject questPrefab = Instantiate(activeQuestPrefab, Vector3.zero, Quaternion.identity);
            questPrefab.transform.SetParent(questMenuContent.transform, false);

            QuestRow qRow = questPrefab.GetComponent<QuestRow>();

            qRow.thisQuest = activeQuest;
            qRow.questName.text = activeQuest.questName;
            qRow.questGiver.text = activeQuest.questGiver;

            qRow.isActive = true;
            qRow.isTracking = true;

            //qRow.coinAmount.text = $"{activeQuest.info.coinReward}";

            // Ödül 1 varsa göster, yoksa gizle
            if (activeQuest.info.rewardItem1 != "")
            {
                qRow.firstReward.sprite = GetSpriteForitem(activeQuest.info.rewardItem1);
                qRow.firstRewardAmount.text = "";
            }
            else
            {
                qRow.firstReward.gameObject.SetActive(false);
                qRow.firstRewardAmount.text = "";
            }

            // Ödül 2 varsa göster, yoksa gizle
            if (activeQuest.info.rewardItem2 != "")
            {
                qRow.secondReward.sprite = GetSpriteForitem(activeQuest.info.rewardItem2);
                qRow.secondRewardAmount.text = "";
            }
            else
            {
                qRow.secondReward.gameObject.SetActive(false);
                qRow.secondRewardAmount.text = "";
            }

            // Ödül 3 varsa göster, yoksa gizle
            if (activeQuest.info.rewardItem3 != "")
            {
                qRow.thirdReward.sprite = GetSpriteForitem(activeQuest.info.rewardItem3);
                qRow.thirdRewardAmount.text = "";
            }
            else
            {
                qRow.thirdReward.gameObject.SetActive(false);
                qRow.thirdRewardAmount.text = "";
            }
        }

        // Tamamlanan görevleri oluştur ve UI'ya yerleştir
        foreach (Quest completedQuest in QuestManager.Instance.allCompletedQuests)
        {
            GameObject questPrefab = Instantiate(completedQuestPrefab, Vector3.zero, Quaternion.identity);
            questPrefab.transform.SetParent(questMenuContent.transform, false);

            QuestRow qRow = questPrefab.GetComponent<QuestRow>();

            qRow.questName.text = completedQuest.questName;
            qRow.questGiver.text = completedQuest.questGiver;

            qRow.isActive = false;
            qRow.isTracking = false;

            //qRow.coinAmount.text = $"{completedQuest.info.coinReward}";

            // Ödül 1 varsa göster, yoksa gizle
            if (completedQuest.info.rewardItem1 != "")
            {
                qRow.firstReward.sprite = GetSpriteForitem(completedQuest.info.rewardItem1);
                qRow.firstRewardAmount.text = "";
            }
            else
            {
                qRow.firstReward.gameObject.SetActive(false);
                qRow.firstRewardAmount.text = "";
            }

            // Ödül 2 varsa göster, yoksa gizle
            if (completedQuest.info.rewardItem2 != "")
            {
                qRow.secondReward.sprite = GetSpriteForitem(completedQuest.info.rewardItem2);
                qRow.secondRewardAmount.text = "";
            }
            else
            {
                qRow.secondReward.gameObject.SetActive(false);
                qRow.secondRewardAmount.text = "";
            }

            // Ödül 3 varsa göster, yoksa gizle
            if (completedQuest.info.rewardItem3 != "")
            {
                qRow.thirdReward.sprite = GetSpriteForitem(completedQuest.info.rewardItem3);
                qRow.thirdRewardAmount.text = "";
            }
            else
            {
                qRow.thirdReward.gameObject.SetActive(false);
                qRow.thirdRewardAmount.text = "";
            }
        }
    }

    // İlgili item'ın sprite'ını Resources klasöründen bulur
    private Sprite GetSpriteForitem(string item)
    {
        var itemToGet = Resources.Load<GameObject>(item); // Prefab'ı yükle
        return itemToGet.GetComponent<UnityEngine.UI.Image>().sprite; // Image bileşeninden sprite al
    }

    public void ReconstructQuestLists()
    {
        // 1. Önce listeleri sıfırla ki üst üste binmesin
        allActiveQuests.Clear();
        allCompletedQuests.Clear();
        allTrackedQuests.Clear();

        // 2. Kaynak gerçek (NPC'ler) üzerinden veriyi çek
        if (NPCManager.Instance != null && NPCManager.Instance.npcList != null)
        {
            foreach (var npc in NPCManager.Instance.npcList)
            {
                foreach (var quest in npc.quests)
                {
                    // Eğer görev tamamlanmışsa
                    if (quest.isCompleted)
                    {
                        if (!allCompletedQuests.Contains(quest))
                        {
                            allCompletedQuests.Add(quest);
                        }
                    }
                    // Eğer görev kabul edilmiş ama henüz bitmemişse (Aktif)
                    else if (quest.accepted)
                    {
                        if (!allActiveQuests.Contains(quest))
                        {
                            allActiveQuests.Add(quest);
                            // Varsayılan olarak aktif görevleri takibe de alalım
                            allTrackedQuests.Add(quest); 
                        }
                    }
                }
            }
        }

        // 3. UI'ı Yenile
        RefreshQuestList();    // Büyük menüyü çiz
        RefreshTrackerList();  // Sol üstteki ufak listeyi çiz
    }
}

