using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Assign in the inspector
    public VideoClip[] videoClips; // Assign video clips in the inspector

    private int currentVideoIndex = 0;

    public void PlayNextVideo()
    {
        currentVideoIndex++;
        if (currentVideoIndex >= videoClips.Length)
        {
            currentVideoIndex = 0;
        }
        videoPlayer.clip = videoClips[currentVideoIndex];
        videoPlayer.Play();
    }

    public void PlayPreviousVideo()
    {
        currentVideoIndex--;
        if (currentVideoIndex < 0)
        {
            currentVideoIndex = videoClips.Length - 1;
        }
        videoPlayer.clip = videoClips[currentVideoIndex];
        videoPlayer.Play();
    }
}
