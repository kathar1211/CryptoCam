using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionLabel : MonoBehaviour
{
    public TextMeshProUGUI Label;

    // Start is called before the first frame update
    void Start()
    {
        Label.text = "version " + Application.version;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
