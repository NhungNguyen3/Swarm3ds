using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    [Range(0.01f,1.0f)]
    public float smoothSpeed = 0.125f;

    private void Start()
    {
        offset = transform.position - player.transform.position;
    }
    void LateUpdate()
    {
        Vector3 newPos = player.position + offset;
        transform.position = Vector3.Slerp(transform.position, newPos,smoothSpeed);
    }
}
