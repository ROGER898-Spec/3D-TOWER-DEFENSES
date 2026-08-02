using UnityEngine;

/// <summary>
/// CameraController - Kontrol kamera gaya Clash of Clans: drag buat geser,
/// scroll buat zoom, TIDAK ADA rotate (sudut pandang tetap/fixed).
///
/// CARA SETUP DI UNITY:
/// 1. Buat GameObject kosong, rename "CameraRig", posisikan di tengah map (misal 0,0,0).
/// 2. Pindahkan "Main Camera" jadi CHILD dari "CameraRig".
/// 3. Atur posisi LOKAL Main Camera supaya "melihat ke bawah" ke arah CameraRig,
///    misal Position (0, 15, -12), Rotation (50, 0, 0) — sesuaikan sampai sudut pandang enak.
/// 4. Attach script ini ke "CameraRig" (BUKAN ke Main Camera).
/// 5. Drag "Main Camera" (child tadi) ke field "Camera Transform" di Inspector.
///
/// CATATAN: geser (drag) pakai klik KIRI mouse — sama seperti BuildNode.OnMouseDown().
/// Supaya tidak ketuker sama klik buat bangun tower, ada threshold jarak drag
/// (dragThreshold) — klik singkat tanpa gerak dianggap "tap" (build), bukan pan.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("Drag child Main Camera di sini (bukan drag GameObject ini sendiri)")]
    public Transform cameraTransform;

    [Header("Pan (Drag)")]
    [Tooltip("Jarak minimal drag (pixel layar) sebelum dianggap geser, bukan tap/klik build node")]
    public float dragThreshold = 8f;
    [Tooltip("Batas area geser, sesuaikan dengan ukuran map (contoh: map 87x65 -> kira-kira -45..45 dan -35..35)")]
    public float minX = -45f, maxX = 45f;
    public float minZ = -35f, maxZ = 35f;

    [Header("Zoom")]
    public float zoomSpeed = 15f;
    public float minZoomDistance = 5f;
    public float maxZoomDistance = 30f;

    private Camera cam;
    private float currentZoomDistance;
    private Vector3 cameraLocalDirection;

    private bool isDragging = false;
    private bool dragThresholdPassed = false;
    private Vector3 dragStartWorldPos;
    private Vector2 mouseDownScreenPos;

    private void Start()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("[CameraController] Camera Transform belum di-drag di Inspector!");
            enabled = false;
            return;
        }

        cam = cameraTransform.GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        currentZoomDistance = cameraTransform.localPosition.magnitude;
        cameraLocalDirection = cameraTransform.localPosition.normalized;
    }

    private void Update()
    {
        HandleDragPan();
        HandleZoom();
    }

    // ─── Geser via drag mouse (gaya Clash of Clans) ──────────────────────────
    private void HandleDragPan()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownScreenPos = Input.mousePosition;
            dragThresholdPassed = false;
            isDragging = true;
            dragStartWorldPos = GetGroundPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            dragThresholdPassed = false;
        }

        if (!isDragging) return;

        // Cek apakah sudah melewati threshold (baru dianggap "geser", bukan klik build node)
        if (!dragThresholdPassed)
        {
            float screenDist = Vector2.Distance(mouseDownScreenPos, (Vector2)Input.mousePosition);
            if (screenDist < dragThreshold) return; // masih dianggap tap, jangan gerakkan kamera
            dragThresholdPassed = true;
            dragStartWorldPos = GetGroundPoint(Input.mousePosition); // reset titik acuan tepat saat mulai geser
        }

        Vector3 currentWorldPos = GetGroundPoint(Input.mousePosition);
        Vector3 delta = dragStartWorldPos - currentWorldPos;

        Vector3 newPos = transform.position + delta;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);
        transform.position = newPos;
    }

    /// <summary>Raycast dari titik layar ke bidang tanah (Y=0) di world space</summary>
    private Vector3 GetGroundPoint(Vector3 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return transform.position; // fallback kalau ray sejajar bidang (jarang terjadi)
    }

    // ─── Zoom pakai Scroll Wheel ──────────────────────────────────────────────
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0f) return;

        currentZoomDistance -= scroll * zoomSpeed;
        currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoomDistance, maxZoomDistance);

        cameraTransform.localPosition = cameraLocalDirection * currentZoomDistance;
    }
}