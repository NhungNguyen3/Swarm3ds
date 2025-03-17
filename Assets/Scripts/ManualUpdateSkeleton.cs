
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualUpdateSkeleton : MonoBehaviour
{

/*
   // public SkeletonAnimation skeletonAnimation;

    [Range(1 / 60f, 1f / 8f)] // slider from 60fps to 8fps
    public float timeInterval = 1f / 24f; // 24fps
    public Camera cam;
    public MeshRenderer enemyRenderer;
    float deltaTime;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();
    }
#endif

    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();

        skeletonAnimation.Initialize(false);
        skeletonAnimation.clearStateOnDisable = false;
        skeletonAnimation.enabled = false;
        ManualUpdate();
    }

    void Update()
    {
        if(IsVisibleFromCamera())
        {
            deltaTime += Time.deltaTime;
            if (deltaTime >= timeInterval)
                ManualUpdate();
        }

    }

    void ManualUpdate()
    {
        skeletonAnimation.Update(deltaTime);
        skeletonAnimation.LateUpdate();
        deltaTime -= timeInterval; //deltaTime = deltaTime % timeInterval; // optional time accuracy.
    }

    bool IsVisibleFromCamera()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, enemyRenderer.bounds);
    }*/

}
