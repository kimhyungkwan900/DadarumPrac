using Unity.Cinemachine;
using UnityEngine;

public class CameraWarpService : MonoBehaviour
{
    public static CameraWarpService Instance { get; private set; }

    [Header("Camera References")]
    [SerializeField]
    [Tooltip("CinemachineBrain이 있는 카메라")]
    private Camera cameraWithBrain;

    private CinemachineBrain brain;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // CinemachineBrain 찾기
        if (cameraWithBrain != null)
        {
            brain = cameraWithBrain.GetComponent<CinemachineBrain>();
        }
        else
        {
            // 할당되지 않았으면 Camera.main에서 찾기
            var camera = Camera.main;
            if (camera != null)
                brain = camera.GetComponent<CinemachineBrain>();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void WarpTarget(Transform target, Vector3 delta)
    {
        if (brain == null)
        {
            Debug.LogWarning("[CameraWarpService] CinemachineBrain을 찾을 수 없습니다.");
            return;
        }

        // CinemachineBrain을 통해 활성화된 VirtualCamera 찾기
        var activeCamera = brain.ActiveVirtualCamera;
        if (activeCamera != null)
        {
            // CinemachineVirtualCameraBase 타입 체크
            if (activeCamera is CinemachineVirtualCameraBase vcamBase)
            {
                vcamBase.OnTargetObjectWarped(target, delta);
            }
        }
    }
}
