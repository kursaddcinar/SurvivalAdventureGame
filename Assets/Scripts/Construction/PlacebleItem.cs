using System.Collections.Generic;
using UnityEngine;

public class PlacebleItem : MonoBehaviour
{
    // Yerleştirme doğrulama durumları
    [SerializeField] bool isGrounded;           // Zeminle temas var mı
    [SerializeField] bool isOverlappingItems;   // Başka objeyle çakışma var mı
    public bool isValidToBeBuilt;               // Yerleştirme geçerli mi

    [SerializeField] BoxCollider solidCollider; // Elle atanmalı
    private Outline outline;                    // Outline efekt referansı

    private void Start()
    {
        outline = GetComponent<Outline>(); // Outline bileşeni alınır
    }

    void Update()
    {
        // Eğer zeminle temas varsa ve çakışma yoksa yerleştirme geçerlidir
        if (isGrounded && isOverlappingItems == false)
        {
            isValidToBeBuilt = true;
        }
        else
        {
            isValidToBeBuilt = false;
        }

        // Kutunun merkezinden aşağı doğru ışın göndererek zemin kontrolü yapılır
        var boxHeight = transform.lossyScale.y;
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, boxHeight * 0.5f, LayerMask.GetMask("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    #region || --- On Triggers --- |
    private void OnTriggerEnter(Collider other)
    {
        // Eğer zeminle çarpıştıysa ve yerleştirme modundaysak
        if (other.CompareTag("Ground") && PlacementSystem.Instance.inPlacementMode)
        {
            // Objeyi zemine paralel hale getir
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                // Yüzeye göre rotasyon ayarlanır
                Quaternion newRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                transform.rotation = newRotation;

                isGrounded = true;
            }
        }

        // Ağaç veya toplanabilir objeyle çakışma varsa
        if (other.CompareTag("Tree") || other.CompareTag("pickable"))
        {
            isOverlappingItems = true;
        }
    }
    #endregion

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground") && PlacementSystem.Instance.inPlacementMode)
        {
            isGrounded = false;
        }

        if ((other.CompareTag("Tree") || other.CompareTag("pickable")) && PlacementSystem.Instance.inPlacementMode)
        {
            isOverlappingItems = false;
        }
    }

    #region || --- Set Outline Colors --- |
    public void SetInvalidColor()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.red; // Geçersizse kırmızı kenarlık
        }
    }

    public void SetValidColor()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.green; // Geçerliyse yeşil kenarlık
        }
    }

    public void SetDefaultColor()
    {
        if (outline != null)
        {
            outline.enabled = false; // Outline kapatılır
        }
    }
    #endregion
}
