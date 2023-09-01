using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonSpeech : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnButtonPressed;
    public UnityEvent OnButtonReleased;
    // Start is called before the first frame update
    void Start()
    {
        // Simular la presión del botón al inicio de la escena
        OnButtonPressed?.Invoke();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnButtonPressed?.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        OnButtonReleased?.Invoke();
    }
}
