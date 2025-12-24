// GameInitializer.cs
// Bu script, GameScene sahnesi yüklendiğinde çalışır. Eğer yeni oyun başlatılıyorsa başlangıç pozisyonunu ayarlar
// ve kaydeder. Eğer var olan bir kayıt yükleniyorsa, kaydedilmiş pozisyona oyuncuyu yerleştirir.

using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public StoryManager storyManager;
    void Start()
    {
        // Eğer kullanıcı yeni bir oyun başlatmışsa
        if (SaveManager.Instance.isStartingNewGame)
        {
            // Oyuncunun sahneye ilk spawn olacağı konum
            //Vector3 startPos = new Vector3(60, 5, 30);

            // Sahnedeki oyuncu objesini bul (Tag'e dikkat!)
            GameObject player = GameObject.FindWithTag("Player");
            //player.transform.position = startPos;

            // Oyuncuyu başlangıç pozisyonunda ilk kez kaydet
            SaveManager.Instance.SaveGame(
                SaveManager.Instance.GetPendingUserId(),        // Geçici kullanıcı ID'si (önceden seçilmiş)
                SaveManager.Instance.GetPendingSlotIndex()      // Geçici slot index'i (önceden seçilmiş)
            );

            // Hikaye panelini göster
            if (storyManager != null)
                storyManager.BeginStory(true);
                

            // Yeni oyun bayrağını sıfırla, böylece sahne tekrar yüklendiğinde yeni oyun işlemi tekrarlanmaz
            SaveManager.Instance.isStartingNewGame = false;
        }
        else
        {
            // Eğer var olan bir kayıt yükleniyorsa, bu kayıt verisini uygula
            SaveManager.Instance.LoadGameDataAfterSceneLoad();

            // Kayıt verisinin sadece bir kez yüklenmesini sağlamak için bu bayrak sıfırlanır
            SaveManager.Instance.shouldLoadFromFile = false;
        }
    }
}