/// <summary>
/// Daftar tampilan awal yang dapat diminta
/// ketika Battle Scene dibuka.
/// </summary>
public enum BattleEntryPanel
{
    HUD,
    Upgrade,
    Settings,
    Inventory
}


/// Menyimpan informasi panel tujuan selama perpindahan scene.

public static class BattleSceneRequest
{
    // Nilai default adalah HUD agar Battle Scene tetap normal
    // apabila scene dijalankan langsung melalui Unity Editor.
    private static BattleEntryPanel requestedPanel =
        BattleEntryPanel.HUD;

    /// <summary>
    /// Menyimpan panel yang harus dibuka di Battle Scene.
    /// Dipanggil sebelum Main Menu memuat Battle Scene.
    /// </summary>
    public static void SetRequestedPanel(BattleEntryPanel panel)
    {
        requestedPanel = panel;
    }

    /// <summary>
    /// Mengambil permintaan panel, lalu mengembalikan nilainya ke HUD.
    /// Reset dilakukan agar permintaan lama tidak terbawa
    /// ketika Battle Scene dibuka kembali.
    /// </summary>
    public static BattleEntryPanel ConsumeRequestedPanel()
    {
        BattleEntryPanel result = requestedPanel;

        // Kembalikan ke kondisi default setelah permintaan dibaca.
        requestedPanel = BattleEntryPanel.HUD;

        return result;
    }
}