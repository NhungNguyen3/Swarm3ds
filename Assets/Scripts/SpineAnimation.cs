
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class SpineAnimation : MonoBehaviour
{
/*

    [SerializeField] static SkeletonAnimation skeletonAnimation;
    private JobHandle jobHandle;
    private bool isPlaying;
    private void Awake()
    {
    }
    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        isPlaying = skeletonAnimation.AnimationState.GetCurrent(0) != null;
    }

    private void Update()
    {
        if (isPlaying)
        {
            NativeArray<float> timeScaleArray = new NativeArray<float>(1, Allocator.TempJob);
            timeScaleArray[0] = 1f; // Set your desired time scale here

            SpineAnimationJob job = new SpineAnimationJob
            {
                timeScale = timeScaleArray,
                deltaTime = Time.deltaTime
            };

            jobHandle = job.Schedule();
        }
    }

    private void LateUpdate()
    {
        if (isPlaying)
        {
            jobHandle.Complete();
        }
    }

    [BurstCompile]
    struct SpineAnimationJob : IJob
    {
        public NativeArray<float> timeScale;
        public float deltaTime;

        public void Execute()
        {
            skeletonAnimation.timeScale = timeScale[0];
            skeletonAnimation.Update(deltaTime);
            skeletonAnimation.LateUpdate();
        }
    }*/

}
