using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }

    public bool onTarget;                            // Oyuncu bir objeye nişan alıyor mu?
    public GameObject selectedObject;                // Seçilen obje (etkileşim yapılacak)

    public GameObject interaction_Info_UI;           // Ekranda etkileşim bilgisini gösteren panel
    Text interaction_text;                           // Etkileşim yazısı (örnek: "Talk", "Pick Up")

    public Image centerDotImage;                     // Ekran ortasındaki nişangah
    public Image HandIcon;                           // Eşyayı al simgesi

    public bool handIsVisible;                       // El ikonu aktif mi?

    public GameObject selectedTree;                  // Kesilecek ağaç
    public GameObject chopHolder;                    // Balta tutan oyuncu objesi

    public GameObject selectedStorageBox;            // Açılacak sandık kutusu

    private void Start()
    {
        onTarget = false;
        interaction_text = interaction_Info_UI.GetComponent<Text>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            // NPC ile etkileşim
            NPC npc = selectionTransform.GetComponent<NPC>();
            if (npc && npc.playerInRange)
            {
                interaction_Info_UI.SetActive(true);
                interaction_text.text = "Konuş";

                if (Input.GetMouseButtonDown(0) && npc.isTalkingWithPlayer == false)
                {
                    npc.StartConversation();
                }

                if (DialogSystem.Instance.dialogUIActive)
                {
                    interaction_Info_UI.SetActive(false);
                    centerDotImage.gameObject.SetActive(false);
                }
            }

            // Kesilebilir ağaç kontrolü
            ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();
            if (choppableTree && choppableTree.playerInRange)
            {
                choppableTree.canBeChopped = true;
                selectedTree = choppableTree.gameObject;
                chopHolder.gameObject.SetActive(true);
            }
            else
            {
                if (selectedTree != null)
                {
                    selectedTree.GetComponent<ChoppableTree>().canBeChopped = false;
                    selectedTree = null;
                    chopHolder.gameObject.SetActive(false);
                }
            }

            // Eşya ile etkileşim
            InteractableObject ourInteractable = selectionTransform.GetComponent<InteractableObject>();
            if (ourInteractable && ourInteractable.playerInRange)
            {
                onTarget = true;
                selectedObject = ourInteractable.gameObject;
                interaction_text.text = ourInteractable.GetItemName();
                interaction_Info_UI.SetActive(true);

                centerDotImage.gameObject.SetActive(false);
                HandIcon.gameObject.SetActive(true);
                handIsVisible = true;
            }

            // Sandık ile etkileşim
            StorageBox storageBox = selectionTransform.GetComponent<StorageBox>();
            if (storageBox && storageBox.playerInRange && PlacementSystem.Instance.inPlacementMode == false)
            {
                interaction_text.text = "Aç";
                interaction_Info_UI.SetActive(true);
                selectedStorageBox = storageBox.gameObject;

                if (Input.GetMouseButtonDown(0))
                {
                    StorageManager.Instance.OpenBox(storageBox);
                }
            }
            else
            {
                if (selectedStorageBox != null)
                {
                    selectedStorageBox = null;
                }
            }

            // ayvan etkileşimi (ölü veya canlı)
            Animal animal = selectionTransform.GetComponent<Animal>();
            if (animal && animal.playerInRange)
            {
                if (animal.isDead)
                {
                    interaction_text.text = "Topla";
                    interaction_Info_UI.SetActive(true);
                    centerDotImage.gameObject.SetActive(false);
                    HandIcon.gameObject.SetActive(true);
                    handIsVisible = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        Lootable lootable = animal.GetComponent<Lootable>();
                        Loot(lootable);
                    }
                }
                else
                {
                    interaction_text.text = animal.animalName;
                    interaction_Info_UI.SetActive(true);
                    centerDotImage.gameObject.SetActive(true);
                    HandIcon.gameObject.SetActive(false);
                    handIsVisible = false;

                    if (Input.GetMouseButtonDown(0) && EquipSystem.Instance.IsHoldingWeapon() && !EquipSystem.Instance.IsThereASwingLock())
                    {
                        StartCoroutine(DealDamageTo(animal, 0.3f, (int)EquipSystem.Instance.GetWeaponDamage()));
                    }
                }
            }

            // 🔹 Herhangi bir şey hedeflenmediyse sıfırla
            if (!ourInteractable && !animal)
            {
                onTarget = false;
                handIsVisible = false;
                centerDotImage.gameObject.SetActive(true);
                HandIcon.gameObject.SetActive(false);
            }

            // 🔹 Hiçbiri seçilmemişse etkileşim yazısını kapat
            if (!npc && !ourInteractable && !animal && !choppableTree && !storageBox)
            {
                interaction_text.text = "";
                interaction_Info_UI.SetActive(false);
            }
        }
    }

    public void Loot(Lootable lootable)
    {
        if(lootable.wasLootCalculated == false)
        {
            List<LootRecieved> recievedLoot = new List<LootRecieved>();

            foreach(LootPossibility loot in lootable.possibleLoot)
            {

                //0 ->1(%50 drop rate )
                //1 ->1(%100 drop rate )
                //-1 ->1(%33 drop rate ) -1,0,1
                //-1 ->0(%0 drop rate )


                var lootAmount = UnityEngine.Random.Range(loot.amountMin,loot.amountMax+1);
                if(lootAmount != 0 )
                {
                    LootRecieved lt = new LootRecieved();
                    lt.item=loot.item;
                    lt.amount=lootAmount;

                    recievedLoot.Add(lt);
                }

            }

            lootable.finalLoot = recievedLoot;
            lootable.wasLootCalculated = true;
        }

        //spawning the loot on the ground
        Vector3 lootSpawnPosition = lootable.gameObject.transform.position;

/*
        foreach(LootRecieved lootRecieved in lootable.finalLoot)//old
        {
            for(int i=0; i<lootRecieved.amount; i++)
            {
                GameObject lootSpawn = Instantiate(Resources.Load<GameObject>(lootRecieved.item.name+"_Model"),
                new Vector3(lootSpawnPosition.x,lootSpawnPosition.y+0.2f,lootSpawnPosition.z),
                Quaternion.Euler(0,0,0));
            }
        }
*/
        foreach (LootRecieved lootRecieved in lootable.finalLoot)
        {
            for (int i = 0; i < lootRecieved.amount; i++)
            {
                // Rastgele X ve Z ekseninde sapma (dağınık yerleşim)
                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-0.5f, 0.5f), // X
                    0.2f,                                  // Y (yükseklik, sabit kalsın)
                    UnityEngine.Random.Range(-0.5f, 0.5f)  // Z
                );

                GameObject lootSpawn = Instantiate(
                    Resources.Load<GameObject>(lootRecieved.item.name + "_Model"),
                    lootSpawnPosition + randomOffset,
                    Quaternion.Euler(0, 0, 0)
                );
            }
        }

        // if we want to blood puddle to stay on yhe ground
        if(lootable.GetComponent<Animal>())
        {
            lootable.GetComponent<Animal>().bloodPuddle.transform.SetParent(lootable.transform.parent);
        }

        //destroy loodted body
        Destroy(lootable.gameObject);

        //if(chest){dont destroy}
    }
    
    IEnumerator DealDamageTo(Animal animal, float delay, int damage)
    {
        yield return new WaitForSeconds(delay);
        animal.TakeDamage(damage);
    }

    public void DisabledSelection()
    {
        HandIcon.enabled = false;
        centerDotImage.enabled = false;
        interaction_Info_UI.SetActive(false);

        selectedObject = null;
    }


    public void EnabledSelection()
    {
        HandIcon.enabled = true;
        centerDotImage.enabled = true;
        interaction_Info_UI.SetActive(true);

    }
}