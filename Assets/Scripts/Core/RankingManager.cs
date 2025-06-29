using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;

// Bu sınıf, kullanıcıların oyun kayıtlarını puana göre sıralayan ve ekranda gösteren sistemi yönetir.
public class RankingManager : MonoBehaviour
{
    public GameObject rowPrefab;         // Her bir sıralama satırı için prefab (örneğin: bir Text nesnesi içeren UI prefabı)
    public Transform listContainer;      // Tüm satırların ekleneceği ScrollView içindeki liste nesnesi

    // Sıralama listesini oluşturan fonksiyon
    public void ShowRanking()
    {
        List<SaveSummary> summaries = new List<SaveSummary>(); // Her slot için özet bilgi tutar

        // 5 kullanıcı, her biri için 3 slotu kontrol et
        for (int userId = 0; userId < 5; userId++)
        {
            for (int slot = 0; slot < 3; slot++)
            {
                // Kayıt dosyasının yolu
                //string path = Application.dataPath + $"/Resources/Saves/save_user_{userId}_slot_{slot}.json";
                string path = System.IO.Path.Combine(Application.persistentDataPath, $"save_user_{userId}_slot_{slot}.json");


                // Eğer kayıt dosyası varsa...
                if (File.Exists(path))
                {
                    // JSON verisini oku ve deserialize et
                    string json = File.ReadAllText(path);
                    AllGameData data = JsonUtility.FromJson<AllGameData>(json);

                    // Kullanıcı, slot, puan ve kayıt tarihi bilgileri ile SaveSummary nesnesi oluştur
                    SaveSummary summary = new SaveSummary
                    {
                        userId = userId,
                        slotIndex = slot,
                        points = data.playerData.points,
                        date = data.playerData.saveDateTime
                    };
                    summaries.Add(summary);
                }
            }
        }

        // Puanlara göre büyükten küçüğe sırala
        var ordered = summaries.OrderByDescending(s => s.points).ToList();

        // Önceki UI satırlarını temizle
        foreach (Transform child in listContainer)
        {
            Destroy(child.gameObject);
        }

        // Sıralı listeyi UI olarak göster
        foreach (var s in ordered)
        {
            GameObject row = Instantiate(rowPrefab, listContainer); // Yeni satır oluştur
            row.GetComponentInChildren<TextMeshProUGUI>().text = 
                $"User {s.userId} - Slot {s.slotIndex} | <color=#FF0000>Puan: {s.points}</color> | Tarih: {s.date}";
        }
    }
}
