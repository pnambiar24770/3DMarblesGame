using UnityEngine;

public class SoftbodyToggle : MonoBehaviour
{
    public GameObject softbodyBones; // Reference to GameObject with softbody bones
    public PlayerManager playerManager;

    // Store the initial local positions and rotations of the bones
    private Transform[] boneTransforms;
    private Vector3[] initialPositions;
    private Quaternion[] initialRotations;

    public bool softBodyActive = false;

    void Start()
    {
        softbodyBones.SetActive(false);

        // Cache the bone transforms
        boneTransforms = softbodyBones.GetComponentsInChildren<Transform>();

        // Store the initial local positions and rotations
        initialPositions = new Vector3[boneTransforms.Length];
        initialRotations = new Quaternion[boneTransforms.Length];

        for (int i = 0; i < boneTransforms.Length; i++)
        {
            initialPositions[i] = boneTransforms[i].localPosition;
            initialRotations[i] = boneTransforms[i].localRotation;

            /*// Debug.Log the initial positions and rotations for debugging
            Debug.Log("Initial Position " + i + ": " + initialPositions[i]);
            Debug.Log("Initial Rotation " + i + ": " + initialRotations[i]);*/
        }
    }


    void Update()
    {
        // Toggle key for softbody
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleSoftbodyEffect();

            Debug.Log("Softbody active state:" + softBodyActive);
        }
    }

    public void ToggleSoftbodyEffect()
    {
        if (playerManager.currentSize == PlayerManager.BallSize.Medium || playerManager.currentSize == PlayerManager.BallSize.Softbody)
        {
            softBodyActive = !softBodyActive;

            if (softBodyActive == true)
            {
                playerManager.currentSize = PlayerManager.BallSize.Softbody;
                playerManager.UpdatePhysicsParameters();
                //play jump sound
                FindObjectOfType<AudioManager>().Play("Water");

            }
            else

            {
                playerManager.currentSize = PlayerManager.BallSize.Medium;
                playerManager.UpdatePhysicsParameters();
                //stop water sound
                FindObjectOfType<AudioManager>().StopPlaying("Water");

            }

            // Toggle the softbody bones on/off
            softbodyBones.SetActive(!softbodyBones.activeSelf);

            // Reset bone transforms when softbody is deactivated
            if (!softbodyBones.activeSelf)
            {
                for (int i = 0; i < boneTransforms.Length; i++)
                {
                    boneTransforms[i].localPosition = initialPositions[i];
                    boneTransforms[i].localRotation = initialRotations[i];
                }
            }
        }
    }
}
