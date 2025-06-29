using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

// Envanterdeki bir öğenin sürüklenip bırakılmasını kontrol eder
public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Sürükleme başladığında çalışır
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .6f; // saydamlaştır
        canvasGroup.blocksRaycasts = false; // kendisini engel olarak saymasın
        startPosition = transform.position;
        startParent = transform.parent;
        transform.SetParent(transform.root); // üst parent'tan kopar
        itemBeingDragged = gameObject;
    }

    // Sürüklenme sırasında çalışır
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    // Sürükleme bittiğinde çalışır
    public void OnEndDrag(PointerEventData eventData)
    {
        var tempItemReference = itemBeingDragged;
        itemBeingDragged = null;

        // Eğer item bir slot yerine dışarı bırakılmışsa (envanter dışına atılmışsa)
        if (tempItemReference.transform.parent == tempItemReference.transform.root)
        {
            tempItemReference.SetActive(false); // geçici olarak gizle

            AlertDialogManager2 dialogManager = FindObjectOfType<AlertDialogManager2>();

            dialogManager.ShowDialog("Bu öğeyi bırakmak istediğinizden emin misiniz?", (response) =>
            {
                if (response) // oyuncu onayladıysa
                {
                    DropItemIntoWorld(tempItemReference);
                }
                else // iptal ettiyse
                {
                    CancelDragging(tempItemReference);
                }
            });
        }

        // Aynı slota bırakıldıysa
        if (tempItemReference.transform.parent == startParent)
        {
            CancelDragging(tempItemReference);
        }

        // Başka bir slot'a sürüklendiyse
        if (tempItemReference.transform.parent != tempItemReference.transform.root &&
            tempItemReference.transform.parent != startParent)
        {
            if (tempItemReference.transform.parent.childCount > 2)
            {
                // slot zaten doluysa işlemi iptal et
                CancelDragging(tempItemReference);
            }
            else
            {
                // Başarıyla yeni slota geçti
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    DivideStack(tempItemReference);
                }
            }
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        Debug.Log("OnEndDrag");
    }

    // Stack bölme işlemi
    private void DivideStack(GameObject tempItemReference)
    {
        InventoryItem item = tempItemReference.GetComponent<InventoryItem>();
        if (item.amountInInventory > 1)
        {
            item.amountInInventory -= 1;
            InventorySystem.Instance.AddToInventory(item.thisName, false); // stacklemeden ekle
        }
    }

    // Drag işlemini iptal et (eski yerine döndür)
    void CancelDragging(GameObject tempItemReference)
    {
        transform.position = startPosition;
        transform.SetParent(startParent);
        tempItemReference.SetActive(true);
    }

    // Envanter dışına atılan item’ı dünyaya düşür
    private void DropItemIntoWorld(GameObject tempItemReference)
    {
        string cleanName = tempItemReference.name.Split(new string[] { "(Clone)" }, StringSplitOptions.None)[0];
        GameObject item = Instantiate(Resources.Load<GameObject>(cleanName + "_Model"));

        item.transform.position = Vector3.zero;
        var dropSpawnPosition = PlayerState.Instance.playerBody.transform.Find("DropSpawn").transform.position;
        item.transform.localPosition = new Vector3(dropSpawnPosition.x, dropSpawnPosition.y, dropSpawnPosition.z);

        var itemsObject = FindObjectOfType<EnvironmentManager>().gameObject.transform.Find("[Items]");
        item.transform.SetParent(itemsObject.transform);

        DestroyImmediate(tempItemReference);
        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeedItems();
    }
}
