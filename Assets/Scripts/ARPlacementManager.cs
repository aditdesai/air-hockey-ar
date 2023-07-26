using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementManager : MonoBehaviour
{
    ARRaycastManager m_ARRaycastManager;
    
    List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

    public GameObject tableGameObject;

    #region UNITY Callback Methods
    void Start()
    {
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
        
    }

    
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (m_ARRaycastManager.Raycast(ray, raycastHits, TrackableType.PlaneWithinPolygon))
        {
            Vector3 positionToBePlaced = raycastHits[0].pose.position;

            tableGameObject.transform.position = positionToBePlaced;
        }
    }
    #endregion


    

}
