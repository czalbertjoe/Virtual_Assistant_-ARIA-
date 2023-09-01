using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Escenes : MonoBehaviour
{
    Animator anim; 

    private IEnumerator Start()
    {
        anim = GetComponent<Animator>(); 
         
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length); 
         
        SceneManager.LoadScene(1);
    }
}
