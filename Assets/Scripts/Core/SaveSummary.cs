// Kayıt sisteminde her bir kayıt dosyasının özet bilgilerini tutmak için kullanılan yardımcı sınıf.
class SaveSummary
{
    public int userId;        // Kayıt hangi kullanıcıya ait (0-4 arası)
    public int slotIndex;     // Kullanıcının hangi slotu (0-2 arası)
    public int points;        // Bu kayıtta oyuncunun sahip olduğu toplam puan
    public string date;       // Kayıt oluşturulma veya son güncellenme tarihi (örn: "12.06.2025 22:43")
}
