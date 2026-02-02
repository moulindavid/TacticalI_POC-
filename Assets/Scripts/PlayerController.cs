using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private LayerMask groundLayer;
    [Header("Visuals")]
    [SerializeField] private GameObject destinationIndicatorPrefab;
    private GameObject _indicatorInstance;
    private NavMeshAgent _agent;
    private Camera _mainCamera;
    private DecalProjector _decal;
    private GameControls _controls;
    private bool _isMouseHeld = false;

    private void Awake()
    {
        
        _agent = GetComponent<NavMeshAgent>();
        _mainCamera = Camera.main;
        _controls = new GameControls();
        _decal =  GetComponent<DecalProjector>();
        if (destinationIndicatorPrefab)
        {
            _indicatorInstance = Instantiate(destinationIndicatorPrefab);
        
            _decal = _indicatorInstance.GetComponent<DecalProjector>();
        
            _indicatorInstance.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        _controls.Enable();
    
        // Initial Click (Started): Trigger the visual "Pop"
        _controls.Player.Interact.started += _ => 
        {
            _isMouseHeld = true;
            TriggerIndicatorAnimation();
        };
    
        _controls.Player.Interact.canceled += _ => _isMouseHeld = false;
    }

    private void Update()
    {
        if (_isMouseHeld)
        {
            UpdateDestination(); // Move the agent and indicator position
        }
    }

    private void TriggerIndicatorAnimation()
    {
        if (_indicatorInstance == null) return;

        _indicatorInstance.SetActive(true);
        _indicatorInstance.transform.localScale = Vector3.zero;
    
        StopAllCoroutines(); 
        StartCoroutine(AnimateIndicator());
    }

    private void UpdateDestination()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            _agent.SetDestination(hit.point);

            if (_indicatorInstance)
            {
                _indicatorInstance.transform.position = hit.point;
            }
        }
    }

    //TODO: feels a bit fast but fine for the moment
    private System.Collections.IEnumerator AnimateIndicator()
    {
        float t = 0;
        float animationSpeed = 3f;

        while (t < 1)
        {
            t += Time.deltaTime * animationSpeed;
        
            _indicatorInstance.transform.localScale = Vector3.one * t;
            _decal.fadeFactor = t; 
            yield return null;
        }
    
        _indicatorInstance.transform.localScale = Vector3.one;
        _decal.fadeFactor = 1f;
    }
}