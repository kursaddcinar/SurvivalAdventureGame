using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrashSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject trashAlertUI; // Silmeden önce onay penceresi
    private Text textToModify;      // Uyarı yazısı (örn: "X itemini atmak istiyor musun?")

    public Sprite trash_closed;     // Çöp kutusunun kapalı görseli
    public Sprite trash_opened;     // Çöp kutusunun açık görseli

    private Image imageComponent;   // Arka plan görüntüsü (çöp kutusu imajı)

    Button YesBTN, NoBTN;          // Onaylama ve iptal butonları

    GameObject draggedItem
    {
        get { return DragDrop.itemBeingDragged; }
    }

    GameObject itemToBeDeleted;     // Silinmek istenen eşya

    // Sürüklenen eşyanın ismini düzgün hale getir
    public string itemName
    {
        get
        {
            string name = itemToBeDeleted.name;
            string toRemove = "(Clone)";
            return name.Replace(toRemove, "");
        }
    }

    void Start()
    {
        // Gerekli UI bileşenlerini bul
        imageComponent = transform.Find("background").GetComponent<Image>();
        textToModify = trashAlertUI.transform.Find("Text").GetComponent<Text>();

        YesBTN = trashAlertUI.transform.Find("yes").GetComponent<Button>();
        YesBTN.onClick.AddListener(delegate { DeleteItem(); });

        NoBTN = trashAlertUI.transform.Find("no").GetComponent<Button>();
        NoBTN.onClick.AddListener(delegate { CancelDeletion(); });
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Eğer sürüklenen eşya silinebilir özelliğe sahipse (isTrashable)
        if (draggedItem.GetComponent<InventoryItem>().isTrashable)
        {
            itemToBeDeleted = draggedItem.gameObject;
            StartCoroutine(notifyBeforeDeletion());
        }
    }

    // Uyarı ekranını göster
    IEnumerator notifyBeforeDeletion()
    {
        trashAlertUI.SetActive(true);
        textToModify.text = "Throw away this " + itemName + "?";
        yield return new WaitForSeconds(1f);
    }

    // "Hayır" butonuna basıldığında çöp iptal edilir
    private void CancelDeletion()
    {
        imageComponent.sprite = trash_closed;
        trashAlertUI.SetActive(false);
    }

    // "Evet" butonuna basıldığında eşya yok edilir
    private void DeleteItem()
    {
        imageComponent.sprite = trash_closed;

        DestroyImmediate(itemToBeDeleted.gameObject);

        InventorySystem.Instance.ReCalculateList();       // Envanter güncellenir
        CraftingSystem.Instance.RefreshNeedItems();       // Craft sistemindeki ihtiyaçlar güncellenir

        trashAlertUI.SetActive(false);
    }

    // Fareyle çöp kutusunun üzerine gelindiğinde açılır görsel gösterilir
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (draggedItem != null && draggedItem.GetComponent<InventoryItem>().isTrashable)
        {
            imageComponent.sprite = trash_opened;
        }
    }

    // Fare kutudan ayrıldığında kapalı görsele dönülür
    public void OnPointerExit(PointerEventData eventData)
    {
        if (draggedItem != null && draggedItem.GetComponent<InventoryItem>().isTrashable)
        {
            imageComponent.sprite = trash_closed;
        }
    }
}
