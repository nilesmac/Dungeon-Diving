using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicBar : MonoBehaviour
{
    public Slider slider;


    public void SetMaxMagic(int magic)
    {
        slider.maxValue = magic;
        slider.value = magic;

        //fill.color = gradient.Evaluate(1f);
    }

    public void SetMagic(int magic)
    {
        slider.value = magic;

        //fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
