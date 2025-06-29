// SaveGameMenuManager.cs
// Bu script, SaveGameMenu panelindeki kayıt slotlarını başlatır, günceller ve
// hangi modda çalışacağını (yeni oyun başlatma mı, oyun içi kayıt mı) belirler.

using UnityEngine;

public class SaveGameMenuManager : MonoBehaviour
{
    private SaveMenuMode currentMode;           // Panelin hangi modda çalıştığını tutar

    public GameObject slotContainer;            // Slot UI'larını içeren parent obje (örneğin VerticalLayoutGroup)
    public SaveSlotUI[] slotUIs;                // Paneldeki tüm slotları temsil eden UI script dizisi

    // SaveSlotUI'de RefreshSlots() çağırıldığında bu method kullanılır
    public void LoadSlotStates(SaveMenuMode mode)
    {
        currentMode = mode;
        int userId = SaveManager.Instance.GetPendingUserId();

        for (int i = 0; i < slotUIs.Length; i++)
        {
            bool hasData = SaveManager.Instance.HasSave(userId, i);
            slotUIs[i].Initialize(userId, i, hasData, mode);
        }
    }

    // MenuManager üzerinden çağrılır → kayıtlı verileri gösterir
    public void InitializeSlots(int userId, SaveMenuMode mode)
    {
        currentMode = mode;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            bool hasData = SaveManager.Instance.HasSave(userId, i);
            slotUIs[i].Initialize(userId, i, hasData, mode);
        }
    }

    // Harici çağrılarda mevcut mod ve kullanıcı ID'yi kullanarak slotları günceller
    public void RefreshSlots()
    {
        InitializeSlots(SaveManager.Instance.GetPendingUserId(), currentMode);
    }
}
