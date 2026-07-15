using UnityEngine;

/// <summary>
/// ElementSystem - lookup afinitas elemen Tower vs Enemy.
/// Berdasarkan tabel "Sistem Element Tower" di data balancing.
///
/// ⚠️ CATATAN UNTUK DIKONFIRMASI KE TIM DESAIN:
/// Tabel asli cuma menyebutkan EKSPLISIT 4 relasi "Unggul" (1.5x)
/// dan HANYA 1 relasi "Lemah" (Fire vs Water = 0.5x).
/// Kombinasi lain yang secara siklus logikanya juga "harusnya" lemah
/// (misal Earth vs Fire, Wind vs Earth, Water vs Wind) TIDAK disebut
/// lemah di tabel aslinya — jadi di sini dianggap netral (1x) sesuai
/// data mentah, BUKAN asumsi saya sendiri.
/// Kalau tim desain maunya simetris (tiap elemen punya 1 lawan yang
/// bikin dia lemah juga), tinggal ubah angka di matrix bawah ini.
/// </summary>
public static class ElementSystem
{
    // matrix[attacker, target] — urutan sesuai enum ElementType: Fire=0, Water=1, Wind=2, Earth=3
    private static readonly float[,] matrix = new float[4, 4]
    {
        //              Fire    Water   Wind    Earth   ← target
        /* Fire   */  { 1f,     0.5f,   1f,     1.5f },
        /* Water  */  { 1.5f,   1f,     1f,     1f   },
        /* Wind   */  { 1f,     1.5f,   1f,     1f   },
        /* Earth  */  { 1f,     1f,     1.5f,   1f   },
    };

    public static float GetMultiplier(ElementType attacker, ElementType target)
    {
        return matrix[(int)attacker, (int)target];
    }
}
