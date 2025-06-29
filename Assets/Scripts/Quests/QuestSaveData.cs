using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Görevlerin kaydedilmesi için kullanılan veri sınıfı.
// Bu sınıf, her bir görevin oyuncu tarafından hangi aşamada olduğunu kaydeder.
[System.Serializable]
public class QuestSaveData
{
    public string questName;              // Görevin adı (eşleştirme için kullanılır)

    public bool accepted;                 // Görev kabul edilmiş mi?
    public bool declined;                 // Görev reddedilmiş mi?
    public bool isCompleted;              // Görev tamamlanmış mı?
    public bool initialDialogCompleted;   // Başlangıç diyaloğu tamamlanmış mı?
    public int currentReplyIndex;         // Diyalog sırasında oyuncunun geldiği metin satırı (kaçıncı cevapta kaldığı)
}
