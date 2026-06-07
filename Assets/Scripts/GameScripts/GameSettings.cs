using UnityEngine;

public class GameSettings : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}