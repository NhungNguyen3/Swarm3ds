using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    public Terrain terrain;
    private Vector3 minBounds;
    private Vector3 maxBounds;

    [Range(0.01f,1.0f)]
    public float smoothSpeed = 0.125f;

    private void Start()
    {
        offset = transform.position - player.transform.position;
        
        // L?y kích th??c c?a terrain
        TerrainData terrainData = terrain.terrainData;
        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;

        // Tính toán gi?i h?n t?i thi?u và t?i ?a cho camera
        minBounds = terrain.transform.position + new Vector3(terrainWidth, 0, terrainLength);
        maxBounds = terrain.transform.position + new Vector3(terrainWidth, 0, terrainLength);
    }
    void LateUpdate()
    {
        Vector3 newPos = player.position + offset;
        transform.position = Vector3.Slerp(transform.position, newPos,smoothSpeed); 

        // L?y v? trí hi?n t?i c?a camera
        Vector3 clampedPosition = transform.position;

        // Gi?i h?n v? trí c?a camera trong terrain
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minBounds.z, maxBounds.z);


        // C?p nh?t v? trí c?a camera
    //    transform.position = clampedPosition;
    }
}
