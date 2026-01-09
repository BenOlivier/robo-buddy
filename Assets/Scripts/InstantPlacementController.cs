using UnityEngine;
using Meta.XR;
using Meta.XR.MRUtilityKit;

public class InstantPlacementController : MonoBehaviour
{
    public Transform rightControllerAnchor;
    public GameObject prefabToPlace;
    public EnvironmentRaycastManager raycastManager;

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            Ray ray = new (
                rightControllerAnchor.position,
                rightControllerAnchor.forward
            );

            TryPlace(ray);
        }
    }

    private void TryPlace(Ray ray)
    {
        if (raycastManager.Raycast(ray, out var hit))
        {
            GameObject objectToPlace = Instantiate(prefabToPlace);
            objectToPlace.transform.position = hit.point;
            
            Quaternion upRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Quaternion facingRotation = Quaternion.LookRotation(-ray.direction, hit.normal);
            objectToPlace.transform.rotation = upRotation * Quaternion.Euler(0, facingRotation.eulerAngles.y, 0);

            // If no MRUK component is present in the scene, we add an OVRSpatialAnchor component
            // to the instantiated prefab to anchor it in the physical space and prevent drift.
            if (MRUK.Instance?.IsWorldLockActive != true)
            {
                objectToPlace.AddComponent<OVRSpatialAnchor>();
            }
        }
    }
}
