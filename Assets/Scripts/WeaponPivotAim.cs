using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPivotAim : MonoBehaviour
{
    public Transform player;         // Player (root)
    public Transform weaponChild;    // Weapon (SpriteRenderer olan obje)
    public Vector2 handLocalOffset = new Vector2(0.6f, 0f);
    [Tooltip("Silah sprite'ının varsayılanı sağa bakıyorsa true.")]
    public bool spriteFacesRight = true;

    Camera cam;

    void Awake() { cam = Camera.main; }

    void LateUpdate()
    {
        if (!player || !weaponChild || !cam || Mouse.current == null) return;

        // Pivot her kare player merkezinde dursun (child değilse emniyet)
        transform.position = player.position;

        // Mouse -> world
        Vector2 mp = Mouse.current.position.ReadValue();
        float z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        Vector3 mWorld = cam.ScreenToWorldPoint(new Vector3(mp.x, mp.y, z));
        mWorld.z = 0f;

        // Pivotu mouse'a döndür (firePoint.right bu yöne bakacak)
        Vector2 dir = (mWorld - transform.position);
        if (dir.sqrMagnitude < 1e-6f) return;
        transform.right = dir.normalized;

        // Yarı-düzleme göre sadece görseli flip'le (firePoint etkilenmez)
        bool leftSide = dir.x < 0f;
        float yFlip = leftSide ? -1f : 1f;
        if (!spriteFacesRight) yFlip *= -1f;      // sprite defaultu sola ise tersle
        weaponChild.localScale = new Vector3(1f, yFlip, 1f);

        // Elde duruş ofseti (flipte X ters döner)
        weaponChild.localPosition = new Vector3(leftSide ? -handLocalOffset.x : handLocalOffset.x,
                                                handLocalOffset.y, 0f);
    }
}
