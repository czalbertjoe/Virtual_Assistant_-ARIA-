using UnityEngine;

public class BtnChangeColor : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject canvasObjeto;

    void Start()
    {
        // Desactivar el Canvas al inicio del juego
        canvasObjeto.SetActive(false);
    }

    public BtnChangeColor() 
    {
        // Activar o desactivar el Canvas
        canvasObjeto.SetActive(!canvasObjeto.activeSelf);
    }
}





