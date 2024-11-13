using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAnimQueue : MonoBehaviour
{
    public SkeletonGraphic skeletonGraphic;
    public List<AnimTrack> animTracks;
    private int trackIndex;
    public bool reInit = true;

	private void Awake()
	{
        skeletonGraphic = GetComponent<SkeletonGraphic>();
    }

	private void OnEnable()
	{
        if (skeletonGraphic == null) return;

        if (reInit)
        {
            trackIndex = 0;
            skeletonGraphic.AnimationState.ClearTracks();
            skeletonGraphic.Initialize(true);
        }
        PlayQueue();
	}


    public void PlayQueue()
    {
        if (animTracks == null || trackIndex >= animTracks.Count) return;

        if (animTracks[trackIndex].delay > 0)
        {
            skeletonGraphic.enabled = false;
        }

        DOVirtual.DelayedCall(animTracks[trackIndex].delay, () =>
        {
            if (animTracks[trackIndex].delay > 0)
            {
                skeletonGraphic.enabled = true;
            }

            skeletonGraphic.AnimationState.ClearTracks();
            skeletonGraphic.Initialize(true);
            if (animTracks[trackIndex].loop)
            {
                skeletonGraphic.AnimationState.SetAnimation(0, animTracks[trackIndex].animationName, animTracks[trackIndex].loop);

            }
            else
            {
                skeletonGraphic.AnimationState.SetAnimation(0, animTracks[trackIndex].animationName, animTracks[trackIndex].loop).Complete += (track) =>
                {
                    trackIndex++;
                    PlayQueue();
                };
            }

        });

    }

}

[System.Serializable]
public class AnimTrack
{
    public string animationName;
    public float delay;
    public bool loop;
}
