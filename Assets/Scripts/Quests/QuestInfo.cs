using System.Collections.Generic;
using UnityEngine;

// Unity editöründe ScriptableObject olarak tanımlanabilen görev bilgi veri yapısı.
// Görevler hakkındaki tüm metinsel içerikler ve koşullar burada saklanır.
[CreateAssetMenu(fileName ="Data", menuName = "ScriptableObjects/QuestInfo", order = 1)]
public class QuestInfo : ScriptableObject
{
    [TextArea(5,10)] 
    public List<string> initialDialog; // Görevin başlangıcında NPC'nin söylediği diyaloglar.

    [Header("Options")] 
    [TextArea(5, 10)]
    public string acceptOption; // Oyuncuya gösterilecek "[Kabul Et]" buton metni.
    [TextArea(5, 10)]
    public string acceptAnswer; // Görevi kabul ettikten sonra NPC'nin cevabı.
    [TextArea(5, 10)]
    public string declineOption; // Oyuncuya gösterilecek "[Reddet]" buton metni.
    [TextArea(5, 10)]
    public string declineAnswer; // Görev reddedildiğinde NPC'nin cevabı.
    [TextArea(5, 10)]
    public string comebackAfterDecline; // Görev reddedilip tekrar konuşulduğunda söylenen metin.
    [TextArea(5, 10)]
    public string comebackInProgress; // Görev devam ederken tekrar konuşulduğunda verilen metin.
    [TextArea(5, 10)]
    public string comebackCompleted; // Görev başarıyla tamamlandığında verilen metin.
    [TextArea(5, 10)]
    public string finalWords; // Görev tamamen bittikten sonra söylenen kapanış cümleleri.

    [Header("Rewards")] 
    public int coinReward; // Görevi tamamlayınca verilen altın miktarı.
    public string rewardItem1; // Envantere eklenecek 1. ödül.
    public string rewardItem2; // Envantere eklenecek 2. ödül.

    [Header("Requirements")]
    public string firstRequirmentItem; // Görev için toplanması gereken 1. item.
    public int firstRequirementAmount; // O item'dan kaç tane gerektiği.
    public string secondRequirmentItem; // Görev için toplanması gereken 2. item.
    public int secondRequirementAmount; // O item'dan kaç tane gerektiği.

    public bool hasCheckPoints; // Görevde belirli noktaların tamamlanması gerekiyorsa true.
    public List<Checkpoint> checkpoints; // Görev sırasında takip edilen kontrol noktaları.
}
