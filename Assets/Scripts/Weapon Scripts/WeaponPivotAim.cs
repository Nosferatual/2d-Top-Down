using UnityEngine;

public class WeaponPivotAim : MonoBehaviour
{
    public Transform player;         // Player (root)
    public Transform weaponChild;    // Weapon (SpriteRenderer olan obje)
    public Vector2 handLocalOffset = new Vector2(0.6f, 0f);
    [Tooltip("Silah sprite'ının varsayılanı sağa bakıyorsa true.")]
    public bool spriteFacesRight = true;

    [Header("Auto-Aim Ayarları (Mobil)")]
    public float targetRange = 8f;   // Düşman arama menzili (Büyü asasının menzili)
    public LayerMask enemyLayer;     // Düşmanların olduğu Katman (Örn: "Enemy" layer'ı)
    
    // Karakterin ne tarafa döneceğini PlayerAim scriptine bildirmek için public yaptık
    public Vector2 CurrentAimDirection { get; private set; } = Vector2.right;

    void LateUpdate()
    {
        if (!player || !weaponChild) return;

        // Pivot her kare player merkezinde dursun
        transform.position = player.position;

        // Menzildeki en yakın düşmanı bul
        Transform closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            // Düşman varsa vektörü ona doğru çevir
            CurrentAimDirection = (closestEnemy.position - transform.position).normalized;
        }
        // Eğer menzilde düşman yoksa, silah en son baktığı yönde sabit kalır

        // Pivotu hedefe döndür
        transform.right = CurrentAimDirection;

        // Yarı-düzleme göre sadece görseli flip'le (Silahın ters dönmemesi için)
        bool leftSide = CurrentAimDirection.x < 0f;
        float yFlip = leftSide ? -1f : 1f;
        if (!spriteFacesRight) yFlip *= -1f;      
        
        weaponChild.localScale = new Vector3(1f, yFlip, 1f);

        // Elde duruş ofseti (flipte X ters döner)
        weaponChild.localPosition = new Vector3(leftSide ? -handLocalOffset.x : handLocalOffset.x,
                                                handLocalOffset.y, 0f);
    }

    // Etraftaki düşmanları tarayıp en yakın olanı döndüren matematiksel fonksiyon
    Transform FindClosestEnemy()
    {
        // Karakterin etrafında targetRange yarıçapında görünmez bir çember oluştur ve enemyLayer'daki objeleri bul
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, targetRange, enemyLayer);
        
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            // Bulunan her düşmanın karaktere olan mesafesini ölç
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy.transform;
            }
        }

        return closest;
    }

    // Teze eklenecek güzel bir detay: Unity Editor'de menzili kırmızı bir çember ile gösterir
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRange);
    }
}