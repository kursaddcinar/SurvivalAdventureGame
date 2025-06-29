using UnityEngine;

// Kayıt sisteminde oyuncuya ait tüm verileri saklamak için kullanılan sınıf
[System.Serializable]
public class PlayerData
{
    // Oyuncunun pozisyon bilgileri (x, y, z)
    public float positionX;
    public float positionY;
    public float positionZ;

    // Oyuncunun sağlık, kalori ve su seviyeleri
    public float health;
    public float calories;
    public float hydration;

    // Envanterdeki item isimleri
    public string[] inventoryContent;

    // Hızlı erişim slotlarındaki item isimleri
    public string[] quickSlotContent;

    // Oyuncunun toplam puanı
    public int points;

    // Kaydın alındığı tarih ve saat
    public string saveDateTime; // 🔹 Eklendi: Kayıt tarihi

    //npc ile story sistemindeki kontrol değişkeni
    public int hasTriggeredTomurIntro;
    //oyunun bitip bitmediğine dair kayıt sistemindeki kontrol değişkeni
    public bool gameIsCompleted = false;



    // Yeni bir kayıt oluşturmak için kullanılan yapıcı (constructor)
    public PlayerData(Vector3 pos, float health, float calories, float hydration, int points, string[] inventory, string[] quickSlots, int hasTriggeredTomurIntro, bool gameIsCompleted)
    {
        // Pozisyonu kaydet
        positionX = pos.x;
        positionY = pos.y;
        positionZ = pos.z;

        // Temel durum verileri
        this.health = health;
        this.calories = calories;
        this.hydration = hydration;

        this.points = points;

        // Envanter ve hızlı slot içerikleri
        this.inventoryContent = inventory;
        this.quickSlotContent = quickSlots;

        this.hasTriggeredTomurIntro = hasTriggeredTomurIntro;

        this.gameIsCompleted = gameIsCompleted;


        // Kayıt zaman damgası
        this.saveDateTime = System.DateTime.Now.ToString("dd.MM.yyyy HH:mm");


    }

    // Kaydedilen pozisyonu Vector3 olarak geri döndürür
    public Vector3 GetPosition()
    {
        return new Vector3(positionX, positionY, positionZ);
    }
}
