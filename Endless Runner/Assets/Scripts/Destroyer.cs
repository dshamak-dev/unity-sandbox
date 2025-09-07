using UnityEngine;
using System.Collections.Generic;

public class Destroyer : MonoBehaviour
{
    public Transform follow;
    public Vector3 offset;

    public List<string> tags;

    void OnCollisionEnter(Collision collision)
    {
        bool inList = tags == null || tags.Contains(collision.gameObject.tag);

        if (inList)
        {
            Destroy(collision.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!follow)
        {
            return;
        }

        Vector3 nextPosition = Vector3.zero;

        nextPosition.x = offset.x == 0 ? follow.position.x : offset.x;
        nextPosition.y = offset.y == 0 ? follow.position.y : offset.y;
        nextPosition.z = offset.z == 0 ? follow.position.z : offset.z;

        transform.position = nextPosition;
    }
}
