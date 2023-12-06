using UnityEngine;

public class VideoButton : MonoBehaviour
{
    public VideoPlayerController videoPlayerController;
    public bool isForwardButton; // true is forwards, false is backwards

    private float cooldownTime = 1.0f;
    private float lastActivatedTime = -1.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= lastActivatedTime + cooldownTime)
        {
            lastActivatedTime = Time.time;
            ChangeVideo();
        }
    }

    private void ChangeVideo()
    {
        if (isForwardButton)
        {
            videoPlayerController.PlayNextVideo();
        }
        else
        {
            videoPlayerController.PlayPreviousVideo();
        }
    }
}
