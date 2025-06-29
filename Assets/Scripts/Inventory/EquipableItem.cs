using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))] // Bu scriptin bağlı olduğu objede Animator bileşeni olmasını zorunlu kılar
public class EquipableItem : MonoBehaviour
{
    public Animator animator;      // Ekipman animatörü
    public bool swingWait = false; // Yeni bir salınım için bekleme süresi

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Sol mouse tıklamasıyla saldırı/salınım tetiklenir
        if (
            Input.GetMouseButtonDown(0) &&
            InventorySystem.Instance.isOpen == false &&
            CraftingSystem.Instance.isOpen == false &&
            SelectionManager.Instance.handIsVisible == false &&
            swingWait == false &&
            ConstructionManager.Instance.inConstructionMode == false
           )
        {
            // Yeni bir salınıma izin verilmeden önce beklenmesini sağlar
            swingWait = true;

            // Salınım sesi gecikmeli oynatılır
            StartCoroutine(SwingSoundDelay());

            // "hit" animasyon tetiklenir
            animator.SetTrigger("hit");

            // Yeniden salınım yapılabilmesi için gecikme başlatılır
            StartCoroutine(NewSwingDelay());
        }
    }

    // Bu fonksiyon, animasyonun uygun anında (event ile) çağrılarak ağaca hasar verir
    public void getHit()
    {
        GameObject selectedTree = SelectionManager.Instance.selectedTree;

        if (selectedTree != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.chopSound);
            selectedTree.GetComponent<ChoppableTree>().GetHit();
        }
    }

    // Salınım sesi için küçük bir bekleme süresi
    IEnumerator SwingSoundDelay()
    {
        yield return new WaitForSeconds(0.002f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.toolSwingSound);
    }

    // Salınım tamamlanana kadar yeni salınıma izin verilmez
    IEnumerator NewSwingDelay()
    {
        yield return new WaitForSeconds(1f);
        swingWait = false;
    }
}
