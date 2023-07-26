using UnityEngine;
using Photon.Pun;

public class PhysicsSync : MonoBehaviour, IPunObservable
{
    Rigidbody rb;
    PhotonView photonView;

    Vector3 networkedPosition;

    float distance;

    Transform tableTransform;

    public float teleportIfDistanceGreaterThan = 1.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();

        tableTransform = GameObject.Find("Air Hockey Table").transform;

        networkedPosition = new Vector3();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkedPosition, distance * (1.0f / PhotonNetwork.SerializationRate));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // sending - local to remote
        {
            stream.SendNext(rb.position - tableTransform.position);
            stream.SendNext(rb.velocity);
        }
        else // receiving - remote to local
        {
            // Position Synchronization
            networkedPosition = (Vector3)stream.ReceiveNext() + tableTransform.position;

            // teleportation
            if (Vector3.Distance(rb.position, networkedPosition) > teleportIfDistanceGreaterThan)
            {
                rb.position = networkedPosition;
            }

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

            // Velocity Synchronization
            rb.velocity = (Vector3)stream.ReceiveNext();
            networkedPosition += rb.velocity * lag;
            distance = Vector3.Distance(rb.position, networkedPosition);
        }
    }
}
