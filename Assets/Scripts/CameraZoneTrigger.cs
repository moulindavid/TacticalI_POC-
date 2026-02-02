using UnityEngine;
using Unity.Cinemachine; // Use Unity.Cinemachine for Unity 6

public class CameraZoneTrigger : MonoBehaviour
{
    [SerializeField] private float outdoorLensSize = 8f;
    [SerializeField] private float indoorLensSize = 5f;
    private CinemachineCamera _vcam;

    private void Awake() => _vcam = FindFirstObjectByType<CinemachineCamera>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _vcam.Lens.OrthographicSize = outdoorLensSize;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) _vcam.Lens.OrthographicSize = indoorLensSize;
    }
}   