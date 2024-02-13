using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressAnything : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}
