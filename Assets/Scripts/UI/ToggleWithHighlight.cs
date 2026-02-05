using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleWithHighlight : UIControlWithHighlight
{
    public Toggle Toggle;

    public bool isOn { set { Toggle.isOn = value; } get { return Toggle.isOn; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
