using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller; // Unity'nin yerleşik fiziksel karakter kontrolörü

    public float baseSpeed  = 12f;   // Kaloriye göre değişmeden önceki sabit hız
    public float speed      = 12f;   // Gerçek zamanlı değişen hız
    public float gravity    = -9.81f * 2; // Yerçekimi
    public float jumpHeight = 2.3f;  // Zıplama yüksekliği

    public Transform groundCheck;     // Yere temas kontrolü için referans nokta
    public float groundDistance = 0.4f; // Yere ne kadar yakınsak "yerde" sayılacağız
    public LayerMask groundMask;       // Ne tür objeler "yer" kabul edilecek

    Vector3 velocity;      // Zıplama ve düşüş hareketi için hız vektörü
    bool isGrounded;       // Oyuncu yerde mi?

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    public bool isMoving;  // Oyuncu hareket ediyor mu?

    void Update()
    {
        float multiplier = 1f;

        // Kaloriye bağlı hız azaltma mekanizması
        float calories      = PlayerState.Instance.currentCalories;
        float maxCalories   = PlayerState.Instance.maxCalories;

        if (calories < maxCalories * 0.25f)     
            multiplier = 0.5f;  // %25'in altı → yavaşlama
        else if (calories < maxCalories * 0.5f) 
            multiplier = 0.75f;

        speed = baseSpeed * multiplier;

        // UI panelleri açık değilken hareket aktif olur
        if (DialogSystem.Instance.dialogUIActive == false &&
            StorageManager.Instance.storageUIOpen == false)
        {
            Movement();
        }
    }

    public void Movement()
    {
        // Yere temas kontrolü (küresel alanla)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // küçük negatif değerle sabitle
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Local eksenlerde yön bulma
        Vector3 move = transform.right * x + transform.forward * z;

        // Yürütme
        controller.Move(move * speed * Time.deltaTime);

        // Zıplama
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Yerçekimini uygula
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Yürüyüş sesi kontrolü
        if (lastPosition != transform.position && isGrounded)
        {
            isMoving = true;
            SoundManager.Instance.PlaySound(SoundManager.Instance.grassWalkSound);
        }
        else
        {
            isMoving = false;
            SoundManager.Instance.grassWalkSound.Stop();
        }

        lastPosition = transform.position;
    }
}
