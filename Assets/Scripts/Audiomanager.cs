using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Müzik")]
    public AudioSource musicSource;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("SFX")]
    public AudioSource sfxSource;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Müzik Klipleri")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("SFX Klipleri")]
    public AudioClip[] fireballHitClips;
    public AudioClip playerHurtClip;
    public AudioClip gameOverClip;
    public AudioClip enemyAttackClip;
    public AudioClip buttonClickClip;
    public AudioClip levelUpClip;
    public AudioClip castClip;              // Büyücü ateş ederken

    [Header("Karakter Seçim Sesleri")]
    public AudioClip mageSelectClip;
    public AudioClip archerSelectClip;
    public AudioClip lockedCharacterClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        PlaySFX(clips[Random.Range(0, clips.Length)]);
    }

    public void PlayFireballHit()       => PlayRandom(fireballHitClips);
    public void PlayPlayerHurt()        => PlaySFX(playerHurtClip);
    public void PlayGameOver()          => PlaySFX(gameOverClip);
    public void PlayEnemyAttack()       => PlaySFX(enemyAttackClip);
    public void PlayButtonClick()       => PlaySFX(buttonClickClip);
    public void PlayLevelUp()           => PlaySFX(levelUpClip);
    public void PlayCast()              => PlaySFX(castClip);
    public void PlayMageSelect()        => PlaySFX(mageSelectClip);
    public void PlayArcherSelect()      => PlaySFX(archerSelectClip);
    public void PlayLockedCharacter()   => PlaySFX(lockedCharacterClip);

    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayGameMusic() => PlayMusic(gameMusic);

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();
}