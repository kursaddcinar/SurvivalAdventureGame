# 🎕️ Survival Adventure Game(Özgürlüğe Doğru)

Bu proje, Unity 2019.4.16f1 ile gelistirilmis, 3D tabanli bir survival/adventure oyunudur. Oyuncular, terk edilmis bir adada hayatta kalmaya çalışırken çevreyle etkileşim, kaynak toplama, inşaat, görev tamamlama gibi sistemlerle etkileşimde bulunurlar.

---

## 🤠 Hikaye

Bir gemi kaptanı olan **Buka**, bir sabah kendini bilinmeyen bir adada bulur. Kısa sürede anlar ki burası efsanelerle bilinen **Anamas Adası**'dır ve gemisi bir gece önceki fırtınada alabora olmuştur. Bu gizemli adada, sadece efsanelerde yaşadığı söylenen **Taygun** adlı bir bilge bulunmaktadır.

Buka, Taygun'u bulur ve adadan kurtulmak için onun rehberliğini ister. Ancak adadan çıkabilmek kolay değildir; çıkış kapısının bekçisi olan lanetli bir **fil** vardır ve bu yaratığı yok etmek için gereken silah parçası yalnızca Taygun'dan elde edilebilir.

Taygun, Buka'yı hem geliştirir hem de ondan kendi adına birtakım görevleri yerine getirmesini ister. Bu görevlerin tamamlanmasıyla birlikte, Buka nihayet adadan kurtulacak ve ailesine kavşabilecektir.

---

## 📄 Bitirme Raporu

Tüm teknik detaylara ve sistem açıklamalarına ulaşmak için:

➡️ [BitirmeRaporu.pdf](Reports/BitirmeRaporu.pdf)


## 🎮 Oyun Özellikleri

### 🏃‍♂️ Temel Mekanikler

* Karakter hareketi (WASD + zıplama)
* Kamera kontrolü (mouse)
* Kalori, su ve can sistemleri (düşen değerler hız ve canı etkiler)

### 🏟️ İnşaat Sistemi

**Dosyalar**: `Constructable.cs`, `ConstructionManager.cs`, `FoundationCheck.cs`, `GhostItem.cs`, `PlacebleItem.cs`, `PlacementSystem.cs`

* Yerleştirilebilir yapılar
* Zemin uygunluk kontrolü
* Şeffaf görünümle yerleştirme önizlemesi

### 💼 Envanter Sistemi

**Dosyalar**: `InventorySystem.cs`, `InventoryItem.cs`, `InventorySlot.cs`, `ItemSlot.cs`, `StorageBox.cs`, `StorageManager.cs`, `EquipableItem.cs`, `EquipSystem.cs`, `TrashSlot.cs`, `Weapon.cs`, `Lootable.cs`, `BowController.cs`

* Sürükle bırak destekli envanter
* Silah, alet, ekipman sistemi
* Depolama kutuları ve çöp slotu

### 🛠️ Crafting Sistemi

**Dosyalar**: `CraftingSystem.cs`, `Blueprint.cs`

* Blueprint tabanlı tarif sistemi
* Envanter değişikliklerine göre otomatik güncelleme

### 🌍 Çevre Sistemi

**Dosyalar**: `ChoppableTree.cs`, `EnvironmentData.cs`, `EnvironmentManager.cs`

* Kesilebilir ağaçlar ve kaydedilen çevre durumu
* Dinamik dünya güncellemeleri

### 🐺 Yaratık AI Sistemi

**Dosyalar**: `AI_Movement.cs`, `Animal.cs`, `CreatureAttackState.cs`, `CreatureChaseState.cs`, `CreatureIdleState.cs`, `CreatureWalkState.cs`, `Arrow.cs`

* Durum bazlı AI: idle, yürü, takip, saldır
* Ok ve yakın dövüş mekanikleri

### 🤝 NPC Sistemi & Diyaloglar

**Dosyalar**: `NPC.cs`, `DialogSystem.cs`, `NPCManager.cs`, `NPCWaypoints.cs`, `NPCGizmos.cs`, `TaygunDialogueTrigger.cs`

* NPC ile etkileşim ve görev alma
* Diyalog sistemi ve waypoint ile devriye
* Replik tetikleyicileri

### 🔍 Görev Sistemi

**Dosyalar**: `Quest.cs`, `QuestManager.cs`, `QuestRow.cs`, `Checkpoint.cs`, `QuestInfo.cs`, `QuestSaveData.cs`, `NPCQuestProgressData.cs`, `Village.cs`, `QuestBuildChecker.cs`

* Görev durumu: aktif, tamamlandı, reddedildi
* Kontrol noktaları ve yapı tamamlama ile görev geçişi

### 🔹 Kaydetme/Yükleme

**Dosyalar**: `SaveManager.cs`, `AllGameData.cs`, `SaveSlot.cs`, `SaveMenuMode.cs`, `SaveSummary.cs`, `LoadSlot.cs`, `PlayerData.cs`, `EnvironmentData.cs`, `SelectionManager.cs`, `RankingManager.cs`

* 5 kullanıcı ve slotlu sistem
* Tüm oyun verilerinin JSON ile saklanması

### 🎧 Ses Sistemi

**Dosyalar**: `SoundManager.cs`

* Arka plan sesi ve efektler

### 📅 Ayarlar

**Dosyalar**: `SettingsManager.cs`, `GlobalState.cs`

* Genel oyun durumları ve ayar yönetimi

---

## 📁 Proje Yapısı (Klasörler)

```
Scripts/
├── Construction/
├── Core/
├── Crafting/
├── Creatures/
├── Environment/
├── Inventory/
├── NPC/
├── Quests/
└── UI/
```

## 🌐 UI ve Menü Sistemleri

**Dosyalar**: `MainMenu.cs`, `MainMenuUIManager.cs`, `InGameMenu.cs`, `MenuManager.cs`, `UserSelectMenu.cs`, `SaveGameMenuManager.cs`, `SaveSlotUI.cs`, `LoadGameMenuManager.cs`, `UserButton.cs`, `MapManager.cs`, `TrackerRow.cs`, `ResourceHealthBar.cs`, `HealthBar.cs`, `CaloriesBar.cs`, `HydrationBar.cs`, `AlertDialogManager.cs`, `AlertDialogManager2.cs`, `DialogManagerGenel.cs`, `FaceCamera.cs`, `StoryManager.cs`, `LoadSlotUI.cs`, `DialogManagerGenel.cs`, `DragDrop.cs`

* Ana menü, kullanıcı seçimi ve slot yönetimi
* Oyun içi menüler, sağlık/şekil çubukları, görev takip arayüzü
* Harita kontrolü ve kamera takip sistemi

---

## 🚀 Kurulum

### Gereksinimler

* Unity 2019.4.16f1
* TextMeshPro, Cinemachine, InputSystem paketleri
* Minimum 4 GB RAM, DirectX 11 destekli ekran kartı

### Adımlar

1. Projeyi indir veya klonla
2. Unity Hub ile projeyi ekle
3. Gerekli paketleri kontrol et ve yükle
4. MainMenu sahnesini çalıştır

---

## 🕹️ Kontroller

```
W / A / S / D     - Hareket
Space            - Zıplama
Mouse            - Kamera
E                - Etkileşim
I                - Envanter Aç / Kapat
C                - İşçilik Modu
ESC              - Menü / Ayarlar
M                - Harita
```

---

## 🚫 Bilinen Sorunlar

* NPC pathfinding bazen engellere takılabilir
* Büyük envanterlerde UI performansı düşebilir

---

## 👨‍💻 Geliştirici

**Kürşat Cınar**
Bilgisayar Mühendisliği Bitirme Projesi
[GitHub Profili](https://github.com/kursaddcinar)

---

## 💼 Lisans

Bu proje [MIT License](LICENSE) ile lisanslanmıştır.

---

## 🧱 Katkıda Bulunma

1. Fork yap
2. Yeni bir branch oluştur: `feature/BenimOzelligim`
3. Commit at: `git commit -m "Ozellik eklendi"`
4. Push et ve PR aç

---

*Bu README projenin güncel yapısına göre sürekli güncellenmektedir.*
