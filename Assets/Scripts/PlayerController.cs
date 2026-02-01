using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private LayerMask groundLayer;
    [Header("Visuals")]
    [SerializeField] private GameObject destinationIndicatorPrefab;
    private GameObject _indicatorInstance;
    private NavMeshAgent _agent;
    private Camera _mainCamera;
    
    private GameControls _controls;
    private bool _isMouseHeld = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _mainCamera = Camera.main;
        _controls = new GameControls();

        if (destinationIndicatorPrefab != null)
        {
            _indicatorInstance = Instantiate(destinationIndicatorPrefab);
            _indicatorInstance.SetActive(false);
        }
    }

    private void OnEnable()
    {
        _controls.Enable();
        
        _controls.Player.Interact.started += _ => _isMouseHeld = true;
        
        _controls.Player.Interact.canceled += _ => _isMouseHeld = false;
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Update()
    {
        if (_isMouseHeld)
        {
            MoveToCursor();
        }
    
        // Auto-hide indicator when the player gets close enough
        if (_indicatorInstance != null && _indicatorInstance.activeSelf)
        {
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                _indicatorInstance.SetActive(false);
            }
        }
    }

    private void MoveToCursor()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            _agent.SetDestination(hit.point);

            // Update indicator position and show it
            if (_indicatorInstance != null)
            {
                _indicatorInstance.SetActive(true);
                // Offset Y slightly so it doesn't "z-fight" with the floor
                _indicatorInstance.transform.position = hit.point + Vector3.up * 0.05f;
            }
        }
    }
}