using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bu sınıf, bir NPC'nin yürüyebileceği noktaları (waypoint) gruplar hâlinde saklamak için kullanılır
public class NPCWaypoints : MonoBehaviour
{
    // Bu değişken, ilgili NPC'ye ait waypoint'lerin bulunduğu GameObject (boş bir nesne içinde tüm noktalar tutulur)
    public GameObject npcWaypointsCluster;
}
