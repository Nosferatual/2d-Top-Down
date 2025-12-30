using UnityEngine;

public class HealthBarFix : MonoBehaviour
{
    void LateUpdate()
    {
        // 1. ROTASYON KİLİDİ:
        // Karakter (Parent) nereye dönerse dönsün, bu obje hep dünyaya göre (0,0,0) açısında kalsın.
        transform.rotation = Quaternion.identity;

        // 2. TERS DÖNME (AYNA) KİLİDİ:
        // Eğer karakterin Scale.X değeri -1 olursa (yani yüzünü sola dönerse),
        // yazı ters çıkmasın diye biz de kendimizi ters çeviriyoruz ki (Eksi * Eksi = Artı) olsun.
        if (transform.parent != null)
        {
            // Babamızın (Player/Enemy) scale değerini al
            float parentX = transform.parent.localScale.x;
            
            // Kendi scale değerimizi al
            Vector3 myScale = transform.localScale;

            // Eğer baba negatifse (-1), biz de negatif olmalıyız ki sonuç pozitif görünsün.
            // Eğer baba pozitifse (1), biz de pozitif olmalıyız.
            // Mathf.Sign ile işaretleri kıyaslıyoruz.
            if (Mathf.Sign(parentX) != Mathf.Sign(myScale.x))
            {
                myScale.x *= -1;
                transform.localScale = myScale;
            }
        }
    }
}