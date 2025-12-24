using UnityEngine;

// Bu sınıf, oyuncu belirli bir yapıya yaklaştığında Taygun (Taygun) ile ilgili diyalogu başlatır.
// Bir defaya mahsus çalışır, hikaye sonunda NPC silinebilir.
public class TaygunDialogueTrigger : MonoBehaviour
{
    public StoryManager storyManager;  // Hikaye yönetimini sağlayan sistem
    private bool hasTriggered = false; // Diyalog daha önce tetiklenmiş mi?

    private void OnTriggerEnter(Collider other)
    {
        // Tetikleyici bir yapıya (örneğin kulübe) yerleştirildiyse, üst objeyi NPC olarak al
        GameObject npcObj = transform.parent != null ? transform.parent.gameObject : gameObject;

        // Daha önce tetiklendiyse tekrar çalışmasın
        if (hasTriggered) return;

        // Oyuncu ile değil, yerleştirilen yapı ile temas kontrolü yapılır
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            // Taygun'e ait özel diyalog metinleri sıraya alınır
            storyManager.LoadCustomDialogue(new string[] {
                "Hoş geldin yabancı... Rüzgar seni buraya sürüklemiş olmalı.",
                "Benim adım Taygun. Bu ada kolay terk edilmez, daha hiç terkedilmedi...",
                "Ama belki sen, bu döngüyü kırabilecek kişisin.",
                "İleride seni bekleyen zorluklar var. Hazırlıklı ol ve dikkatli ilerle.",
                "Adadan kurtulmak istersen benimle konuşabilirsin, yaklaş bana!"
            }, npcObj); // Diyalog bittiğinde bu NPC sahneden silinebilir

            // Hikayeyi başlat
            storyManager.BeginStory(false);

            // Oyuncunun bu sahneyi tetiklediği kaydedilir
            PlayerState.Instance.hasTriggeredTomurIntro += 1;
        }
    }
}
