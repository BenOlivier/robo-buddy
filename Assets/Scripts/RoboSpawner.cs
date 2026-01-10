using UnityEngine;
using Meta.XR;
using Meta.XR.MRUtilityKit;

public class RoboSpawner : MonoBehaviour
{
    [SerializeField] private Transform _centerEyeAnchor;
    [SerializeField] private Transform _placementPreview;
    [SerializeField] private GameObject _roboPrefab;
    [SerializeField] private EnvironmentRaycastManager _raycastManager;
    [SerializeField] private float _rayAngleDownDegrees;
    [SerializeField] private float _maxSpawnHeight;
    [SerializeField] private float _smoothTime;

    private Vector3 _targetPosition;
    private Vector3 _velocity;
    private bool _spawnPossible;
    private bool _roboSpawned;

    private void Start()
    {
        InputManager.Instance.OnConfirmInput += SpawnRobo;
        _targetPosition = _placementPreview.position;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnConfirmInput -= SpawnRobo;
    }

    private void Update()
    {
        if (_roboSpawned) return;

        Ray ray = GetPlacementRay();
        UpdatePreviewTarget(ray);
        MovePreviewToTarget();
    }

    private Ray GetPlacementRay() => new(
        _centerEyeAnchor.position,
        Quaternion.AngleAxis(_rayAngleDownDegrees, _centerEyeAnchor.right) * _centerEyeAnchor.forward
    );

    private Quaternion GetPlacementRotation(Ray ray, Vector3 normal)
    {
        Quaternion upRotation = Quaternion.FromToRotation(Vector3.up, normal);
        Quaternion facingRotation = Quaternion.LookRotation(-ray.direction, normal);
        return upRotation * Quaternion.Euler(0, facingRotation.eulerAngles.y, 0);
    }

    private bool IsValidPlacement(EnvironmentRaycastHit hit)
    {
        bool isFlat = Vector3.Dot(hit.normal.normalized, Vector3.up) > 0.95f;
        bool isBelowMaxHeight = hit.point.y < _maxSpawnHeight;
        return isFlat && isBelowMaxHeight;
    }

    private void UpdatePreviewTarget(Ray ray)
    {
        if (!_raycastManager.Raycast(ray, out EnvironmentRaycastHit hit))
        {
            _placementPreview.gameObject.SetActive(false);
            _spawnPossible = false;
            return;
        }

        if (!IsValidPlacement(hit))
        {
            _placementPreview.gameObject.SetActive(false);
            _spawnPossible = false;
            return;
        }

        _placementPreview.gameObject.SetActive(true);
        _placementPreview.position = hit.point;
        _placementPreview.rotation = GetPlacementRotation(ray, hit.normal);
        _targetPosition = hit.point;
        _spawnPossible = true;
    }

    private void MovePreviewToTarget()
    {
        _placementPreview.position = Vector3.SmoothDamp(
            _placementPreview.position, _targetPosition, ref _velocity, _smoothTime
        );
    }

    private void SpawnRobo()
    {
        if (_roboSpawned || !_spawnPossible) return;
        
        Ray ray = GetPlacementRay();
        if (!_raycastManager.Raycast(ray, out EnvironmentRaycastHit hit)) return;

        GameObject robo = Instantiate(_roboPrefab);
        robo.transform.position = hit.point;
        robo.transform.rotation = GetPlacementRotation(ray, hit.normal);

        _placementPreview.gameObject.SetActive(false);
        _roboSpawned = true;
    }
}
