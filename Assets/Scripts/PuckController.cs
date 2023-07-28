using UnityEngine;
using Photon.Pun;

public class PuckController : MonoBehaviour
{
    public float maxSpeed = 20f;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
    }

    void FixedUpdate()
    {
        Vector3 currVelocity = rb.velocity;
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Surface"))
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            // rb.constraints = ;
        }

        if (collision.transform.CompareTag("Player"))
        {
            GetComponent<PhotonView>().TransferOwnership(collision.gameObject.GetComponent<PhotonView>().Owner);
        }
    }
}
