using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody _rb;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _isSprinting;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        InputManager.Instance.OnMoveInput += HandleMove;
        InputManager.Instance.OnLookInput += HandleLook;
        InputManager.Instance.OnConfirmInput += HandleConfirm;
        InputManager.Instance.OnSprintInput += HandleSprint;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMoveInput -= HandleMove;
        InputManager.Instance.OnLookInput -= HandleLook;
        InputManager.Instance.OnConfirmInput -= HandleConfirm;
        InputManager.Instance.OnSprintInput -= HandleSprint;
    }

    private void Update()
    {
        float speed = _isSprinting ? moveSpeed * 2f : moveSpeed;
        transform.Translate(Vector3.forward * _moveInput.y * speed * Time.deltaTime);
        transform.Translate(Vector3.right * _moveInput.x * speed * Time.deltaTime);
        transform.Rotate(Vector3.up, _lookInput.x * rotateSpeed * Time.deltaTime);
    }

    private void HandleMove(Vector2 input)
    {
        _moveInput = input;
    }

    private void HandleLook(Vector2 input)
    {
        _lookInput = input;
    }

    private void HandleConfirm()
    {
        if (_rb != null)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleSprint(bool isSprinting)
    {
        _isSprinting = isSprinting;
    }
}