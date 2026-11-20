# Mob Rush — Proyek Unity 6

Game tower defense ala Mob Control, dibuat dari nol dengan Unity 6.
Kamu mengarahkan meriam, menembakkan pasukan kecil, melewatkan mereka
lewat gerbang pengali (x2, x3, +10) sambil menghindari gerbang jebakan
(÷2, -5), lalu menghancurkan markas musuh sebelum markasmu hancur.

Semua visual dibuat dari primitive Unity (capsule, cube, cylinder),
tanpa aset eksternal. Nama, tampilan, dan kode semuanya asli.

## Cara membuka proyek

1. Buka **Unity Hub 4.x**.
2. Klik **Add**, lalu pilih folder proyek ini (`mob-control-unity`).
3. Pilih **Unity 6** (6000.0.x) sebagai versi editor. Tunggu proses
   import selesai (pertama kali agak lama, ini normal).

## Cara membangun scene

1. Di Unity Editor, buka scene kosong (atau buka
   `Assets/Scenes/Main.unity` kalau sudah ada).
2. Jalankan menu **Mob Rush > Build Scene**.
3. Script akan membangun SELURUH scene dari nol: tanah, jalur, meriam,
   6 gerbang, markas pemain dan musuh, lampu, kamera, GameManager,
   dan UI lengkap (HUD, panel menang/kalah, toko upgrade).
4. Scene tersimpan otomatis di `Assets/Scenes/Main.unity`.

## Cara mengetes

1. Tekan tombol **Play** di Unity Editor.
2. Seret kiri/kanan (mouse atau sentuhan) untuk mengarahkan meriam.
   Meriam menembak otomatis.
3. Pasukanmu berlari ke markas musuh. Lewatkan mereka lewat gerbang
   emas (x2, x3, +10, +5) dan hindari gerbang merah (÷2, -5).
4. Hancurkan markas musuh untuk menang. Kalau markasmu hancur, kalah.
5. Koin dari kemenangan bisa dipakai untuk upgrade di panel bawah:
   Tembak Cepat, HP Pasukan, dan Damage.

## Cara build AAB untuk Google Play

1. **Install modul Android dulu.** Di Unity Hub, buka **Installs**,
   klik ikon titik tiga di Unity 6, pilih **Add Modules**, centang
   **Android Build Support** (plus Android SDK & NDK Tools dan
   OpenJDK kalau belum tercentang), lalu install.
2. Di Unity Editor, buka **File > Build Settings**, pilih **Android**,
   klik **Switch Platform**.
3. Buka **Player Settings** (tombol di Build Settings):
   - **Company Name:** Codelab Game Studio
   - **Product Name:** Mob Rush
   - **Application Identifier:** `com.codelab.mobrush`
   - **Minimum API Level:** 24 (Android 7.0) atau lebih tinggi.
4. Kembali ke Build Settings, centang **Build App Bundle**.
5. Klik **Build**, pilih folder output. Unity akan meminta membuat
   **keystore** (file kunci penanda tangan) saat build pertama:
   buat keystore baru, isi password, alias, dan simpan baik-baik.
   Keystore yang sama HARUS dipakai untuk semua update game di
   Play Console, jangan sampai hilang.
6. File `.aab` hasil build di-upload ke **Google Play Console**
   (https://play.google.com/console) sebagai rilis baru.

## Catatan penting

- Proyek ini **belum pernah dikompilasi di Unity Editor asli**.
  Saat pertama dibuka, Unity mungkin melaporkan error kompilasi.
  Perbaiki error yang muncul di jendela Console (biasanya soal nama
  API atau referensi), lalu jalankan lagi menu Build Scene.
- Kalau label gerbang (x2, +10, dan lain-lain) tidak muncul, periksa
  font TextMeshPro: SceneBuilder memakai font default TMP
  (`TMP_Settings.defaultFontAsset`). Kalau kosong, buka
  `Window > TextMeshPro > Font Asset Creator` atau import ulang
  paket TextMeshPro.
- Kalau proyek tidak bisa dibuka karena versi Unity berbeda, buat
  proyek 3D baru di Unity Hub dengan Unity 6, lalu salin folder
  `Assets` dari proyek ini ke proyek baru tersebut.
- Level makin sulit tiap menang: HP markas musuh naik 50 per level
  dan musuh muncul makin cepat (lihat `Assets/Scripts/LevelData.cs`).

## Struktur file

- `Assets/Scripts/GameManager.cs` — status game, level, koin, upgrade.
- `Assets/Scripts/Mob.cs` — unit pasukan (gerak, serang, kena gerbang).
- `Assets/Scripts/CannonController.cs` — meriam pemain (arah + tembak).
- `Assets/Scripts/Gate.cs` — gerbang pengali dan jebakan.
- `Assets/Scripts/Base.cs` — markas dengan HP dan bar HP.
- `Assets/Scripts/EnemySpawner.cs` — musuh muncul dari markas musuh.
- `Assets/Scripts/UIManager.cs` — HUD, panel menang/kalah, toko upgrade.
- `Assets/Scripts/LevelData.cs` — rumus kesulitan dan hadiah per level.
- `Assets/Editor/SceneBuilder.cs` — menu Mob Rush/Build Scene.
