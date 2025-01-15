using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena2 : MonoBehaviour
{
    void Update()
    {
        // Detecta si se presiona la tecla 'O'
        if (Input.GetKeyDown(KeyCode.O))
        {
            CambiarEscena2();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CambiarEscena3();
        }
    }

    // Método para cambiar a la escena especificada
    private void CambiarEscena2()
    {
       
            SceneManager.LoadScene(1); // Cambia a la escena especificada
    }private void CambiarEscena3()
    {
       
            SceneManager.LoadScene(2); // Cambia a la escena especificada
    }
        
     
    
}
