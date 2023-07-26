using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlaneController : MonoBehaviour
{
    ARPlaneManager m_ARPlaneManager;
    ARPlacementManager m_ARPlacementManager;

    public GameObject placeButtonGameObject;
    public GameObject scaleSliderGameObject;
    public GameObject joinButtonGameObject;

    #region UNITY Callback Methods
    private void Start()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        m_ARPlacementManager = GetComponent<ARPlacementManager>();
    }
    #endregion



    #region UI Callback Methods
    public void OnPlaceButtonClicked()
    {
        placeButtonGameObject.SetActive(false);
        scaleSliderGameObject.SetActive(false);
        joinButtonGameObject.SetActive(true);

        m_ARPlaneManager.enabled = false;
        m_ARPlacementManager.enabled = false;

        foreach (var plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }
    #endregion
}
