using UnityEngine;
using Photon.Pun;

public class MalletController : MonoBehaviour
{
    public float speedOfMallet = 0.15f;
    public Transform minX, maxX, minZ, maxZ;

    Rigidbody rb;
    Vector3 moveTo;
    Ray ray;
    PhotonView photonView;

    #region UNITY Callback Methods
    private void Awake()
    {
        Application.targetFrameRate = 300;    
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveTo = transform.position;
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                {
                    ray = Camera.main.ScreenPointToRay(touch.position);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Surface"))
                {
                    moveTo = hit.point;
                    moveTo.x = Mathf.Clamp(moveTo.x, minX.position.x, maxX.position.x);
                    moveTo.y = transform.position.y;
                    moveTo.z = Mathf.Clamp(moveTo.z, minZ.position.z, maxZ.position.z);
                    rb.MovePosition(moveTo);
                }

            }
        }
        
    }

   

    #endregion

}
