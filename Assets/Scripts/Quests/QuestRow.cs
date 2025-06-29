using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Görev satırlarını temsil eden bir UI bileşeni.
public class QuestRow : MonoBehaviour
{
    public TextMeshProUGUI questName; // Görevin adını gösterecek UI öğesi.
    public TextMeshProUGUI questGiver; // Görevi veren NPC'nin adını gösterecek UI öğesi.

    public Button trackingButton; // Görevin takip edilip edilmeyeceğini kontrol eden buton.

    public bool isActive; // Görevin aktif olup olmadığını belirler.
    public bool isTracking; // Görevin takip edilip edilmediğini belirler.

    public Text coinAmount; // Görevin ödül olarak verdiği para miktarını gösterecek UI öğesi.

    public Image firstReward; // İlk ödülün görselini gösterecek UI öğesi.
    public Text firstRewardAmount; // İlk ödülün miktarını gösterecek UI öğesi.

    public Image secondReward; // İkinci ödülün görselini gösterecek UI öğesi.
    public Text secondRewardAmount; // İkinci ödülün miktarını gösterecek UI öğesi.

    public Quest thisQuest; // Bu satırın temsil ettiği görev nesnesi.

    private void Start()
    {
        // Takip butonuna tıklanıldığında çalışacak olay dinleyicisini ekler.
        trackingButton.onClick.AddListener(() => {
            if(isActive) // Sadece aktif görevlerde çalışır.
            {
                if(isTracking) // Eğer görev zaten takip ediliyorsa takip bırakılır.
                {
                    isTracking = false;
                    trackingButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Not Tracking";
                    QuestManager.Instance.UntrackQuest(thisQuest); // QuestManager içindeki takip fonksiyonunu çağırır.
                }
                else // Eğer görev takip edilmiyorsa takip başlatılır.
                {
                    isTracking = true;
                    trackingButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Tracking";
                    QuestManager.Instance.TrackQuest(thisQuest); // QuestManager içindeki takip fonksiyonunu çağırır.
                }
            }
        });
    }
}
