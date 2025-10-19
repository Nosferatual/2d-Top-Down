using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPivotAim : MonoBehaviour
{
    public Transform player;           // Player referansı
    public Transform weaponChild;      // Silah objesi (Weapon)
    public Vector2 handLocalOffset = new Vector2(0.6f, 0f); // El mesafesi

    private Camera cam;

    void Awake() => cam = Camera.main;

    void LateUpdate()
    {
        if (player == null || weaponChild == null || cam == null || Mouse.current == null)
            return;

        // Pivot her frame'de player merkezine taşınır
        transform.position = player.position;

        // Mouse konumu -> world
        Vector3 mp = Mouse.current.position.ReadValue();
        float depth = cam.WorldToScreenPoint(player.position).z;
        Vector3 mWorld = cam.ScreenToWorldPoint(new Vector3(mp.x, mp.y, depth));
        mWorld.z = 0f;

        // Pivot mouse yönüne döner
        Vector2 dir = (mWorld - transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);


        // El pozisyonu – local offset world scale'den etkilenmesin
        weaponChild.localPosition = handLocalOffset;

        // 🔥 Asıl kritik kısım: sola dönünce ters çevirmeyi düzelt
        // (Artık 180 değil, sadece dikey flip yapıyoruz)
        if (player.localScale.x < 0)
        {
            weaponChild.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            weaponChild.localScale = Vector3.one;
        }
    }
}
