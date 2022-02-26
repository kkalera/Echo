using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshPro))]
public class fpsCounter : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float fps = 1 / Time.deltaTime;
        TMPro.TextMeshProUGUI tmpro = GetComponent<TMPro.TextMeshProUGUI>();        
        tmpro.text = Mathf.FloorToInt(fps).ToString();
    }
}
