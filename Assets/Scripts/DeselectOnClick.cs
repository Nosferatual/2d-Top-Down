using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class DeselectOnClick : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => 
            EventSystem.current.SetSelectedGameObject(null));
    }
}