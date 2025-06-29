using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureIdleState : StateMachineBehaviour
{
    float timer;                         // Boşta kalınan süreyi takip eder
    public float idleTime = 4f;          // Hayvanın hareketsiz duracağı süre
    Transform player;                    // Oyuncunun Transform'u
    public float detectionAreaRadius = 18f; // Oyuncuyu fark etme yarıçapı

    // Duruma ilk girildiğinde çalışır (boşta bekleme başlar)
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("activeConstructable").transform;
    }

    // Bu durumdayken her karede çalışır
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Yeterince uzun süre boşta kaldıysa yürümeye geç
        timer += Time.deltaTime;
        if (timer > idleTime)
        {
            animator.SetBool("isWalking", true);
        }

        // Eğer oyuncu algılama alanına girdiyse kovalamaya başla
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer < detectionAreaRadius)
        {
            animator.SetBool("isChasing", true);
        }
    }
    /*
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }*/
}
