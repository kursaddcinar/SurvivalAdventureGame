// GameState.cs
// Oyun genelinde kullanıcı ve slot bilgilerini tutmak için kullanılan statik sınıf.
// Sahne geçişlerinde veri taşımak veya geçici bilgileri merkezi olarak yönetmek için kullanılır.

public static class GameState
{
    // Seçili kullanıcı ID'si (kayıt sistemi üzerinden alınır)
    public static int userId = -1;

    // Seçili kayıt slotu ID'si (her kullanıcı için birden fazla kayıt olabilir)
    public static int slotId = -1;
}
