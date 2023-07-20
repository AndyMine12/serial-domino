using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MAIN : MonoBehaviour
{
    

    public void VolverMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void JugarJuego()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SalirJuego()
    {
        Application.Quit(); 
    }

    private void Start() {
        Setup.Score = new int[2] {0, 0};
    }
}
