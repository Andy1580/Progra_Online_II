using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("DIEGO");
        }

    }

}
