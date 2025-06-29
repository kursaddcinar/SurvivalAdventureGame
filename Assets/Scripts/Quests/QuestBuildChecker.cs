using UnityEngine;

// Bu sınıf, kulübe inşasının görev tamamlanma şartlarını kontrol eder.
// Belirli sayıda zemin ve duvar yerleştirilmişse, bağlı görev tamamlanır.
public class QuestBuildChecker : MonoBehaviour
{
    public Checkpoint kulubeCheckpoint;   // Kontrolün sonucunu etkileyecek görev noktası

    public int requiredFoundations = 1;   // Gerekli zemin sayısı
    public int requiredWalls = 4;         // Gerekli duvar sayısı

    // Bu fonksiyon çağrıldığında kulübe inşa edilip edilmediği kontrol edilir
    public void CheckIfHutBuilt()
    {
        int foundationCount = 0;
        int wallCount = 0;

        // Sahnedeki tüm objeleri al
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // Tüm objeleri gez ve tag'lere göre sayım yap
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("placedFoundation"))
                foundationCount++;

            if (obj.CompareTag("placedWall"))
                wallCount++;
        }

        // Gerekli sayılar sağlandıysa görevi tamamla
        if (foundationCount >= requiredFoundations && wallCount >= requiredWalls)
        {
            Debug.Log("Kulübe inşa edildi!");
            kulubeCheckpoint.isCompleted = true; // Görev burada tamamlanmış sayılır
        }
    }
}
