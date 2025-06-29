using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructable : MonoBehaviour
{
    // Yapının geçerli olup olmadığını kontrol eden durum değişkenleri
    public bool isGrounded; // Zemine temas ediyor mu
    public bool isOverlappingItems; // Başka nesnelerle çakışıyor mu
    public bool isValidToBeBuilt; // Yapı inşa edilmeye uygun mu
    public bool detectedGhostMemeber; // Hayalet (önizleme) nesnesine temas etti mi

    // Malzeme referansları (renkler)
    private Renderer mRenderer; // Nesnenin render bileşeni
    public Material redMaterial;   // Geçersiz pozisyon için kırmızı
    public Material greenMaterial; // Geçerli pozisyon için yeşil
    public Material defaultMaterial; // Varsayılan renk

    // Bu yapıdaki hayalet (ghost) nesnelerin listesi
    public List<GameObject> ghostList = new List<GameObject>();

    // Bu collider elle inspector üzerinden atanmalı
    public BoxCollider solidCollider;

    private void Start()
    {
        // Renderer bileşeni alınır ve varsayılan malzeme atanır
        mRenderer = GetComponent<Renderer>();
        mRenderer.material = defaultMaterial;

        // Alt nesneler ghost olarak listeye eklenir
        foreach (Transform child in transform)
        {
            ghostList.Add(child.gameObject);
        }
    }

    void Update()
    {
        // Eğer yapı zeminde ve başka nesneyle çakışmıyorsa geçerli sayılır
        if (isGrounded && isOverlappingItems == false)
        {
            isValidToBeBuilt = true;
        }
        else
        {
            isValidToBeBuilt = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Zeminle temas varsa işaretle
        if (other.CompareTag("Ground") && gameObject.CompareTag("activeConstructable"))
        {
            isGrounded = true;
        }

        // Ağaç ya da toplanabilir nesneyle çakışma varsa işaretle
        if ((other.CompareTag("Tree") || other.CompareTag("pickable")) && gameObject.CompareTag("activeConstructable"))
        {
            isOverlappingItems = true;
        }

        // Başka bir ghost nesneyle temas varsa işaretle
        if (other.gameObject.CompareTag("ghost") && gameObject.CompareTag("activeConstructable"))
        {
            detectedGhostMemeber = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Zeminle temas kesildiğinde işaret kaldırılır
        if (other.CompareTag("Ground") && gameObject.CompareTag("activeConstructable"))
        {
            isGrounded = false;
        }

        // Ağaç ya da nesneyle çakışma sona ererse işaret kaldırılır
        if ((other.CompareTag("Tree") || other.CompareTag("pickable")) && gameObject.CompareTag("activeConstructable"))
        {
            isOverlappingItems = false;
        }

        // Ghost nesnesiyle temas sona ererse işaret kaldırılır
        if (other.gameObject.CompareTag("ghost") && gameObject.CompareTag("activeConstructable"))
        {
            detectedGhostMemeber = false;
        }
    }

    // Yapı geçersiz konumdaysa kırmızı renge geç
    public void SetInvalidColor()
    {
        if (mRenderer != null)
        {
            mRenderer.material = redMaterial;
        }
    }

    // Yapı geçerli konumdaysa yeşil renge geç
    public void SetValidColor()
    {
        mRenderer.material = greenMaterial;
    }

    // Varsayılan renge dön
    public void SetDefaultColor()
    {
        mRenderer.material = defaultMaterial;
    }

    // Ghost nesneleri ana yapıdan ayır ve yerleştirilmiş olarak işaretle
    public void ExtractGhostMembers()
    {
        foreach (GameObject item in ghostList)
        {
            // Parent ilişkisini değiştir
            item.transform.SetParent(transform.parent, true);

            // Ghost item yerleştirildi olarak işaretlenir
            item.gameObject.GetComponent<GhostItem>().isPlaced = true;
        }
    }
}
