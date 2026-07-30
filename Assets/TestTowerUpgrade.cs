using UnityEngine;

/// <summary>
/// SEMENTARA UNTUK TESTING — hapus GameObject ini setelah tombol Upgrade asli dibuat.
/// Tekan U untuk upgrade level tower PERTAMA yang ditemukan di scene (buat verifikasi data Levels[]).
/// </summary>
public class TestTowerUpgrade : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Tower tower = FindAnyObjectByType<Tower>();

            if (tower == null)
            {
                Debug.LogWarning("[TestUpgrade] Belum ada tower di scene. Bangun 1 tower dulu.");
                return;
            }

            int levelSebelum = tower.GetCurrentLevel();
            bool berhasil = tower.UpgradeLevel();

            if (berhasil)
            {
                Debug.Log($"[TestUpgrade] {tower.name} naik dari Level {levelSebelum} -> {tower.GetCurrentLevel()}. " +
                          $"Damage sekarang: {tower.damage}, Range sekarang: {tower.range}");
            }
            else
            {
                Debug.Log($"[TestUpgrade] {tower.name} gagal upgrade — kemungkinan sudah Level Max (10) atau data Levels kosong.");
            }
        }
    }
}
