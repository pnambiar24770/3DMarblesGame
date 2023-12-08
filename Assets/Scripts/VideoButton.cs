using UnityEngine;

public class VideoButton : MonoBehaviour
{
    public VideoPlayerController videoPlayerController;
    public Light light1, light2;
    public GameObject screen;

    public bool isForwardButton; // true is forwards, false is backwards
    public bool isPowerButton;

    private float cooldownTime = 1.0f;
    private float lastActivatedTime = -1.0f;

    void Start()
    {
        light1.enabled = false; // Turn off the lights at the start
        light2.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= lastActivatedTime + cooldownTime)
        {
            lastActivatedTime = Time.time;
            if (isPowerButton)
            {
                ToggleTVAndLights();
            }
            else
            {
                ChangeVideo();
            }
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

    private void ToggleTVAndLights()
    {
        // Toggle the active state of the TV and lights
        screen.SetActive(!screen.activeSelf);
        light1.enabled = !light1.enabled;
        light2.enabled = !light2.enabled;
    }
}
