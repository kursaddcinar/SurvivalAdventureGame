using UnityEngine;
using UnityEngine.SceneManagement;

// Bu sınıf, fil (Elephant) öldüğünde final hikayesini başlatır ve oyunu bitirir.
public class FinalController : MonoBehaviour
{
    public StoryManager storyManager; // Hikaye kontrol sistemi
    private bool hasTriggered = false; // Finalin sadece bir kez tetiklenmesini sağlar

    void Update()
    {
        // Final bir kere tetiklendiyse tekrar kontrol etme
        if (hasTriggered) return;

        // Eğer sahnede "Elephant" ismini içeren hayvan yoksa final başlatılır
        if (storyManager.IsAnimalDead("Elephant"))
        {
            hasTriggered = true;

            // Özel final hikayesi yüklenir
            storyManager.LoadCustomDialogue(new string[] {
                "Fil’in ayak sesleri artık duyulmuyor.",
                "Anamas Adası, bir sessizliğe gömüldü.",
                "Tömür’ün gösterdiği yoldan yürüyen Buka, nihayet adanın lanetini kırdı.",
                "Gökyüzü yeniden maviydi. Rüzgar, bu kez dosttu.",
                "Ve Buka, özgürlüğe doğru ilk adımını attı.",
                "Buka artık özgürsün, ailene kavuşma vakti."
            });

            // Hikaye başlatılır, tamamlanınca OnFinalStoryComplete çalışır
            storyManager.BeginStory(false, OnFinalStoryComplete);
        }
    }

    // Final hikayesi bittikten sonra yapılacak işlemler
    void OnFinalStoryComplete()
    {
        // Oyunun tamamlandığını işaretle
        PlayerState.Instance.gameIsCompleted = true;

        // Oyunu kaydet
        SaveManager.Instance.SaveGame(
            SaveManager.Instance.GetPendingUserId(),
            SaveManager.Instance.GetPendingSlotIndex()
        );

        // Fareyi serbest bırak ve ana menüye dön
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
}
