using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menuGameObject, regresar, textoPantalla, abrirMenu, Salir;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activarMenu() 
    { 
        menuGameObject.SetActive(true);
        animator.SetBool("Menu", true);
        textoPantalla.SetActive(false);
        abrirMenu.SetActive(false);
        Salir.SetActive(false);
    }
    public void cerrarMenu()
    {
        menuGameObject.SetActive(true);
        animator.SetBool("Menu", false);
        textoPantalla.SetActive(true);
        abrirMenu.SetActive(true);
        Salir.SetActive(true);
    }
    public void abrirInfo()
    {
    }
    public void abrirAbout()
    {
    }
}
