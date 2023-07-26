using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ARScaleController : MonoBehaviour
{
    ARSessionOrigin m_ARSessionOrigin;
    public Slider slider;
    
    void Awake()
    {
        m_ARSessionOrigin = GetComponent<ARSessionOrigin>();
    }

    private void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        if (slider != null)
        {
            m_ARSessionOrigin.transform.localScale = Vector3.one / value;
        }
    }
}
