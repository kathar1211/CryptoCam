using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderWithLabel : UIControlWithHighlight { 

    public Slider slider;
    public TextMeshProUGUI label;

    public string formatString = "{0}<size=90%>{1}<size=75%>%";
    public bool isPercentBased;
    public bool displayRawNumber;

    public float value { get { return slider.value; } set { slider.value = value; } }

    // Start is called before the first frame update
    void Awake()
    {
        slider.onValueChanged.AddListener(UpdateLabel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(UpdateLabel);
    }

    public void UpdateLabel(float val)
    {
        if (displayRawNumber)
        {
            label.text = val.ToString();
            return;
        }

        string hundredsPlace;
        string tensOnesPlace;
        int percent;
        if (isPercentBased)
        {
            //if the values on this slider already represent a percent, just reformat
            percent = Mathf.RoundToInt(val * 100);

        }
        else
        {
            //otherwise, map values to percent based on min and max
            float range = slider.maxValue - slider.minValue;
            percent = Mathf.RoundToInt(( (val - slider.minValue) / range) * 100);
        }

        int hundreds = Mathf.FloorToInt(percent / 100);
        if (hundreds > 0) { hundredsPlace = hundreds.ToString("0"); } else { hundredsPlace = ""; }
        tensOnesPlace = (percent % 100).ToString("00");

        label.text = string.Format(formatString, hundredsPlace, tensOnesPlace);
    }
}
