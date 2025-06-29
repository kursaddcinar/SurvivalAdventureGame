using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;  // Fare hassasiyeti ayarı

    float xRotation = 0f;  // Yukarı-aşağı bakış için dönme değeri
    float YRotation = 0f;  // Sağa-sola bakış için dönme değeri

    void Start()
    {
        // Fare imlecini ekranın ortasına kilitler ve görünmez yapar
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Eğer envanter, crafting, görev, dialog veya depolama UI'ları açık değilse fareyle bakış kontrolü yapılır
        if (InventorySystem.Instance.isOpen == false &&
            CraftingSystem.Instance.isOpen == false &&
            // MenuManager.Instance.isMenuOpen == false && // ❗ Eğer yeni menü sistemine geçildiyse aktif et
            DialogSystem.Instance.dialogUIActive == false &&
            QuestManager.Instance.isQuestMenuOpen == false &&
            StorageManager.Instance.storageUIOpen == false)
        {
            // Fare hareketini oku
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Yukarı-aşağı bakışı ayarla (x ekseni)
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Aşırı dönmeyi engelle

            // Sağa-sola bakışı ayarla (y ekseni)
            YRotation += mouseX;

            // Dönüşü uygula
            transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);
        }
    }
}
