using System.Collections.Generic;

// Bu sınıf oyunun tüm kayıt verilerini kapsar.
// [System.Serializable] etiketi, Unity'nin bu sınıfı JSON olarak serileştirebilmesini sağlar.
[System.Serializable]
public class AllGameData
{
    // Oyuncuya ait sağlık, konum, skor vb. verileri tutar.
    public PlayerData playerData;

    // Ortamdaki (çevre) ağaçlar, hayvanlar, depolama kutuları ve alınan item verileri burada tutulur.
    public EnvironmentData environmentData;

    // NPC'lere ait görev ilerlemeleri burada saklanır. Her bir NPC için görev adı, görev durumu vs. kayıt edilir.
    public List<NPCQuestProgressData> questProgressData;

    // NPCManager'da hangi NPC'nin aktif olduğunu temsil eder.
    public int currentNPCIndex;

    // Yapıcı metod: Genellikle sadece oyuncu verisini vererek başlatmak için kullanılır.
    public AllGameData(PlayerData playerData)
    {
        this.playerData = playerData;
    }
}
