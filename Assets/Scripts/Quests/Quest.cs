using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Görevleri temsil eden veri sınıfı.
// Bu sınıf, bir görevin temel bilgilerini ve durumunu tutar.
[System.Serializable] // Unity Inspector'da düzenlenebilir olması için.
public class Quest
{
    // Görevin başlığı
    public string questName;

    // Görevi veren karakterin adı
    public string questGiver;

    // Görev açıklaması (bilgilendirici metin)
    public string questDescription;

    [Header("Bools")] // Inspector'da bu değişkenleri ayırmak için başlık
    public bool accepted;               // Görev oyuncu tarafından kabul edildi mi?
    public bool declined;              // Görev reddedildi mi?
    public bool initialDialogCompleted;// Başlangıç diyaloğu tamamlandı mı?
    public bool isCompleted;           // Görev bitirildi mi?

    public bool hasNoRequirements;     // Görev herhangi bir gereklilik olmadan tamamlanabilir mi?

    [Header("Quest Info")] // Detaylı görev bilgilerini tutacak alan
    public QuestInfo info; // Görevle ilgili daha detaylı bilgileri içeren ScriptableObject referansı
}
