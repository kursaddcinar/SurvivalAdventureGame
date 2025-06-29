using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostItem : MonoBehaviour
{
    public BoxCollider solidCollider; // Solid collider elle inspector'dan atanmalı

    public Renderer mRenderer;
    private Material semiTransparentMat; // Debug için kullanılan yarı saydam materyal
    private Material fullTransparentnMat; // Tam saydam materyal (normalde kullanılan)
    private Material selectedMaterial; // Seçildiğinde uygulanan materyal (yeşil gibi)

    public bool isPlaced; // Yerleştirilmiş mi

    // Silme algoritması için işaret bayrağı
    public bool hasSamePosition = false;

    private void Start()
    {
        mRenderer = GetComponent<Renderer>();

        // Materyaller ConstructionManager'dan alınır, böylece her zaman geçerli referans olur
        semiTransparentMat = ConstructionManager.Instance.ghostSemiTransparentMat;
        fullTransparentnMat = ConstructionManager.Instance.ghostFullTransparentMat;
        selectedMaterial = ConstructionManager.Instance.ghostSelectedMat;

        // Başlangıçta tam saydam materyal atanır
        mRenderer.material = fullTransparentnMat;

        // Yerleştirilmemişse collider kapatılır (inşa modundayken açılabilir)
        solidCollider.enabled = false;
    }

    private void Update()
    {
        // Eğer inşa modundaysak, oyuncu ile ghost çakışmaları engellenir
        if (ConstructionManager.Instance.inConstructionMode)
        {
            Physics.IgnoreCollision(
                gameObject.GetComponent<Collider>(),
                ConstructionManager.Instance.player.GetComponent<Collider>()
            );
        }

        // Eğer hem inşa modundaysa hem de yerleştirilmişse collider açılır (raycast için)
        if (ConstructionManager.Instance.inConstructionMode && isPlaced)
        {
            solidCollider.enabled = true;
        }

        // İnşa modu kapalıysa collider kapatılır
        if (!ConstructionManager.Instance.inConstructionMode)
        {
            solidCollider.enabled = false;
        }

        // Seçilen ghost nesnesi bu ise, yeşil materyal atanır
        if (ConstructionManager.Instance.selectedGhost == gameObject)
        {
            mRenderer.material = selectedMaterial; // seçilmiş (örneğin yeşil)
        }
        else
        {
            mRenderer.material = fullTransparentnMat; // normalde tam saydam görünüm
        }
    }
}
