using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public bool playerInRange;               // Oyuncu bu NPC'nin yakınında mı?
    public bool isTalkingWithPlayer;         // Oyuncu şu anda bu NPC ile konuşuyor mu?

    TextMeshProUGUI npcDialogText;           // NPC'nin konuşma metni UI öğesi

    Button optionButton1;                    // Seçenek 1 butonu
    TextMeshProUGUI optionButton1Text;       // Seçenek 1 metni

    Button optionButton2;                    // Seçenek 2 butonu
    TextMeshProUGUI optionButton2Text;       // Seçenek 2 metni

    public List<Quest> quests;               // Bu NPC'nin sahip olduğu görev listesi
    public Quest currentActiveQuest = null;  // Şu anda aktif olan görev
    public int activeQuestIndex = 0;         // Görev sırası
    public bool firstTimeInteraction = true; // Oyuncu bu NPC ile ilk kez mi konuşuyor?
    public int currentReply;                 // Yanıt sırası (diyalogda)


    //klube görevi verileri
    public QuestBuildChecker questBuildChecker; // Kulübe kontrolünü yapan sınıf
    public Checkpoint kulubeCheckpoint; // Bu görevle eşleşen checkpoint (ScriptableObject)
    public bool isHutQuestNPC = false; // Bu NPC kulübe görevinden sorumlu mu?

    private void Start()
    {
        // DialogSystem'den UI öğelerini alır
        npcDialogText = DialogSystem.Instance.dialogText;

        optionButton1 = DialogSystem.Instance.option1BTN;
        optionButton1Text = DialogSystem.Instance.option1BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        optionButton2 = DialogSystem.Instance.option2BTN;
        optionButton2Text = DialogSystem.Instance.option2BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (isHutQuestNPC && kulubeCheckpoint != null && questBuildChecker != null)
        {
            // Görev daha önce tamamlanmamışsa ve kontrol başarılıysa görevi tamamla
            if (!kulubeCheckpoint.isCompleted)
            {
                questBuildChecker.CheckIfHutBuilt(); // kontrolü yap
                /*
                if (kulubeCheckpoint.isCompleted)
                {
                    Debug.Log("NPC kulübe görevini tamamlandı olarak işaretledi.");
                    QuestManager.Instance.MarkQuestCompleted(kulubeCheckpoint); // varsa senin mevcut sistemin
                                                                                // İstersen burada ödül veya ilerleme ver
                }*/
            }
        }
    }


    // Oyuncu bu NPC ile konuşmaya başladığında çağrılır
    public void StartConversation()
    {
        isTalkingWithPlayer = true;
        LookAtPlayer(); // NPC yüzünü oyuncuya döner

        // İlk etkileşim
        if (firstTimeInteraction)
        {
            firstTimeInteraction = false;
            currentActiveQuest = quests[activeQuestIndex]; // İlk görevi ata
            StartQuestInitialDialog(); // Giriş diyaloğu başlat
            currentReply = 0;
        }
        else
        {
            // Görev daha önce reddedilmişse
            if (currentActiveQuest.declined)
            {
                DialogSystem.Instance.OpenDialogUI();
                npcDialogText.text = currentActiveQuest.info.comebackAfterDecline;
                SetAcceptAndDeclineOptions(); // Kabul / Red butonlarını göster
            }

            // Görev kabul edilmiş ama henüz tamamlanmamışsa
            if (currentActiveQuest.accepted && currentActiveQuest.isCompleted == false)
            {
                if (AreQuestRequirmentsCompleted()) // Gerekli eşyalar varsa
                {
                    SubmitRequiredItems(); // Eşyaları teslim et
                    DialogSystem.Instance.OpenDialogUI();
                    npcDialogText.text = currentActiveQuest.info.comebackCompleted;

                    // Ödül al butonu
                    optionButton1Text.text = "[Ödülleri Al]";
                    optionButton1.onClick.RemoveAllListeners();
                    optionButton1.onClick.AddListener(() =>
                    {
                        ReceiveRewardAndCompleteQuest();
                    });
                }
                else
                {
                    DialogSystem.Instance.OpenDialogUI();
                    npcDialogText.text = currentActiveQuest.info.comebackInProgress;

                    // Kapat butonu
                    optionButton1Text.text = "[Kapat]";
                    optionButton1.onClick.RemoveAllListeners();
                    optionButton1.onClick.AddListener(() =>
                    {
                        DialogSystem.Instance.CloseDialogUI();
                        isTalkingWithPlayer = false;
                    });
                }
            }

            // Görev tamamlanmışsa
            if (currentActiveQuest.isCompleted == true)
            {
                DialogSystem.Instance.OpenDialogUI();
                npcDialogText.text = currentActiveQuest.info.finalWords;

                // Kapat butonu
                optionButton1Text.text = "[Kapat]";
                optionButton1.onClick.RemoveAllListeners();
                optionButton1.onClick.AddListener(() =>
                {
                    DialogSystem.Instance.CloseDialogUI();
                    isTalkingWithPlayer = false;
                });
            }

            // NPC'de yeni bir görev varsa giriş diyaloğunu başlat
            if (currentActiveQuest.initialDialogCompleted == false)
            {
                StartQuestInitialDialog();
            }
        }
    }

    private void SetAcceptAndDeclineOptions()
    {
        // Görevi kabul etme butonu
        optionButton1Text.text = currentActiveQuest.info.acceptOption;
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() =>
        {
            AcceptedQuest();
        });

        // Görevi reddetme butonu
        optionButton2.gameObject.SetActive(true);
        optionButton2Text.text = currentActiveQuest.info.declineOption;
        optionButton2.onClick.RemoveAllListeners();
        optionButton2.onClick.AddListener(() =>
        {
            DeclinedQuest();
        });
    }

    private void SubmitRequiredItems()
    {
        // İlk gereken öğeyi envanterden çıkar
        string firstRequiredItem = currentActiveQuest.info.firstRequirmentItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirementAmount;

        if (firstRequiredItem != "")
        {
            InventorySystem.Instance.RemoveItem(firstRequiredItem, firstRequiredAmount);
        }

        // İkinci gereken öğeyi envanterden çıkar
        string secondtRequiredItem = currentActiveQuest.info.secondRequirmentItem;
        int secondRequiredAmount = currentActiveQuest.info.secondRequirementAmount;

        if (firstRequiredItem != "")
        {
            InventorySystem.Instance.RemoveItem(secondtRequiredItem, secondRequiredAmount);
        }
    }

    private bool AreQuestRequirmentsCompleted()
    {
        // İlk gereksinim kontrolü
        string firstRequiredItem = currentActiveQuest.info.firstRequirmentItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirementAmount;

        var firstItemCounter = 0;
        foreach (string item in InventorySystem.Instance.itemList)
        {
            if (item == firstRequiredItem)
            {
                firstItemCounter++;
            }
        }

        // İkinci gereksinim kontrolü
        string secondRequiredItem = currentActiveQuest.info.secondRequirmentItem;
        int secondRequiredAmount = currentActiveQuest.info.secondRequirementAmount;

        var secondItemCounter = 0;
        foreach (string item in InventorySystem.Instance.itemList)
        {
            if (item == secondRequiredItem)
            {
                secondItemCounter++;
            }
        }

        // Görev için checkpoint varsa kontrol et
        SetQuestHasCheckPoints(currentActiveQuest);
        bool allCheckPointsCompleted = false;

        if (currentActiveQuest.info.hasCheckPoints)
        {
            foreach (Checkpoint cp in currentActiveQuest.info.checkpoints)
            {
                if (cp.isCompleted == false)
                {
                    allCheckPointsCompleted = false;
                    break;
                }
                allCheckPointsCompleted = true;
            }
        }

        // Tüm koşullar sağlandıysa true döndür
        if (firstItemCounter >= firstRequiredAmount && secondItemCounter >= secondRequiredAmount)
        {
            if (currentActiveQuest.info.hasCheckPoints)
            {
                return allCheckPointsCompleted;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    private void SetQuestHasCheckPoints(Quest activeQuest)
    {
        // Eğer görevde checkpoint varsa bayrağı aktif eder
        if (activeQuest.info.checkpoints.Count > 0)
        {
            activeQuest.info.hasCheckPoints = true;
        }
        else
        {
            activeQuest.info.hasCheckPoints = false;
        }
    }

    private void StartQuestInitialDialog()
    {
        // Diyalog ekranını açar ve ilk diyalog satırını gösterir
        DialogSystem.Instance.OpenDialogUI();

        npcDialogText.text = currentActiveQuest.info.initialDialog[currentReply];
        optionButton1Text.text = "Devam";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() =>
        {
            currentReply++;
            CheckIfDialogDone();
        });

        optionButton2.gameObject.SetActive(false); // İkinci buton kapatılır
    }

    private void CheckIfDialogDone()
    {
        // Eğer son diyalog satırına geldiysek
        if (currentReply == currentActiveQuest.info.initialDialog.Count - 1)
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[currentReply];

            currentActiveQuest.initialDialogCompleted = true;

            SetAcceptAndDeclineOptions(); // Görev kabul/ret butonlarını ayarla
        }
        else  // Hâlâ devam eden satırlar varsa
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[currentReply];

            optionButton1Text.text = "Devam";
            optionButton1.onClick.RemoveAllListeners();
            optionButton1.onClick.AddListener(() =>
            {
                currentReply++;
                CheckIfDialogDone();
            });
        }
    }

    private void AcceptedQuest()
    {
        // Görev kabul edildiğinde tetiklenir

        QuestManager.Instance.AddActiveQuest(currentActiveQuest);

        currentActiveQuest.accepted = true;
        currentActiveQuest.declined = false;

        if (currentActiveQuest.hasNoRequirements)
        {
            // Eğer görevde istenen bir nesne yoksa, ödül alımı tetiklenir
            npcDialogText.text = currentActiveQuest.info.comebackCompleted;
            optionButton1Text.text = "[Ödülleri Al]";
            optionButton1.onClick.RemoveAllListeners();
            optionButton1.onClick.AddListener(() =>
            {
                ReceiveRewardAndCompleteQuest();
            });
            optionButton2.gameObject.SetActive(false);
        }
        else
        {
            // Görev kabul edildi, açıklama gösterilir
            npcDialogText.text = currentActiveQuest.info.acceptAnswer;
            CloseDialogUI();
        }
    }

    private void CloseDialogUI()
    {
        // Diyalog ekranını kapatmak için kullanılan fonksiyon

        optionButton1Text.text = "[Kapat]";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() =>
        {
            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer = false;
        });
        optionButton2.gameObject.SetActive(false);
    }

    private void ReceiveRewardAndCompleteQuest()
    {
        // Görevi tamamlandı olarak işaretle
        currentActiveQuest.isCompleted = true;

        // Görevi QuestManager üzerinden tamamlanmış görevler listesine ekle
        QuestManager.Instance.MarkQuestCompleted(currentActiveQuest);

        // Ödül olarak verilecek altın miktarını al
        var coinsRecieved = currentActiveQuest.info.coinReward;

        // Puan sistemine altın eklenmesi planlanmış ama kodda yorum satırında
        // PointManager.Instance.AddCoins(coinsRecieved);

        // Eğer ilk ödül nesnesi varsa envantere ekle
        if (!string.IsNullOrEmpty(currentActiveQuest.info.rewardItem1))
        {
            InventorySystem.Instance.AddToInventory(currentActiveQuest.info.rewardItem1, true);
        }

        // Eğer ikinci ödül nesnesi varsa envantere ekle
        if (!string.IsNullOrEmpty(currentActiveQuest.info.rewardItem2))
        {
            InventorySystem.Instance.AddToInventory(currentActiveQuest.info.rewardItem2, true);
        }

        // NPC içindeki görev listesinden bir sonrakine geç
        activeQuestIndex++;

        if (activeQuestIndex < quests.Count)
        {
            // Hâlâ başka görev varsa yeni göreve geçiş yapılır
            currentActiveQuest = quests[activeQuestIndex];
            currentReply = 0;

            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer = false;
        }
        else
        {
            // Tüm görevler tamamlandıysa
            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer = false;
            Debug.Log(gameObject.name + " görevleri tamamlandı!");

            // NPC sahnede devre dışı bırakılır
            gameObject.SetActive(false);
        }

        // NPC zinciri kontrol edilir, bir sonraki NPC aktifleştirilir
        NPCManager npcManager = FindObjectOfType<NPCManager>();
        if (npcManager != null)
        {
            npcManager.CheckQuestProgressAndAdvance(); // Yeni NPC devreye girer
        }
        else
        {
            Debug.LogWarning("NPCManager sahnede bulunamadı!");
        }
    }

    private void DeclinedQuest()
    {
        // Görev reddedildi olarak işaretlenir
        currentActiveQuest.declined = true;

        // Reddetme cevabı ekrana yazılır ve UI kapanır
        npcDialogText.text = currentActiveQuest.info.declineAnswer;
        CloseDialogUI();
    }

    public void LookAtPlayer()
    {
        // NPC’nin yönünü oyuncuya çevirir
        var player = PlayerState.Instance.playerBody.transform;
        Vector3 direction = player.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);

        // Yalnızca Y ekseniyle döndür
        var yRotation = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Oyuncu menzile girdiğinde
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Oyuncu menzilden çıktığında
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}