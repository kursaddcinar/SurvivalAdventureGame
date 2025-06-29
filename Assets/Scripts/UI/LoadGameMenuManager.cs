using UnityEngine;

// Bu sınıf, yükleme ekranındaki slotları doldurur.
// Kullanıcının seçtiği profile göre slotlarda kayıt olup olmadığını kontrol eder
public class LoadGameMenuManager : MonoBehaviour
{
    public LoadSlotUI[] loadSlotUIs; // Yükleme menüsünde gösterilecek slot UI'ları (3 slotluk dizi)

    // Seçilen kullanıcıya ait tüm slotları kontrol eder ve UI’ları günceller
    public void LoadSlots()
    {
        int userId = SaveManager.Instance.GetPendingUserId(); // Hangi kullanıcıyı yükleyeceğimizi alıyoruz

        for (int i = 0; i < loadSlotUIs.Length; i++)
        {
            bool hasData = SaveManager.Instance.HasSave(userId, i); // Belirtilen kullanıcı için slotta kayıt var mı?
            loadSlotUIs[i].Initialize(userId, i, hasData); // Slot UI'sini başlat (kayıt varsa interaktif hale getir)
        }
    }
}
