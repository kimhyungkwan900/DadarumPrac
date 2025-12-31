using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 지도 UI 관리
public class MapUI : MonoBehaviour
{
    #region 컴포넌트
    [Header("UI References")]
    [SerializeField]
    private GameObject mapPanel;

    [SerializeField]
    private Transform buttonParent;

    [SerializeField]
    private Button destinationButtonPrefab;

    [SerializeField]
    private TextMeshProUGUI energyText;

    [SerializeField]
    private Button closeButton;
    #endregion

    #region 필드
    private MapNode currentMapNode;
    private List<Button> destinationButtons = new List<Button>();
    #endregion

    #region 초기화
    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMap);

        if (mapPanel != null)
            mapPanel.SetActive(false);
    }

    private void Start()
    {
        // EnergyManager가 초기화된 후 이벤트 구독
        if (EnergyManager.Instance != null)
            EnergyManager.Instance.OnEnergyChanged += OnEnergyChanged;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (EnergyManager.Instance != null)
            EnergyManager.Instance.OnEnergyChanged -= OnEnergyChanged;
    }
    #endregion

    #region UI 표시
    // 지도 UI 열기
    public void OpenMap(MapNode mapNode)
    {
        currentMapNode = mapNode;

        if (mapPanel == null)
        {
            Debug.LogWarning("[MapUI] mapPanel이 설정되지 않았습니다.");
            return;
        }

        mapPanel.SetActive(true);
        UpdateDestinationButtons();
        UpdateEnergyDisplay();

        // 입력 컨텍스트를 UI로 변경
        if (InputManager.Instance != null)
            InputManager.Instance.PushContext(InputContext.UI);
    }

    // 지도 UI 닫기
    public void CloseMap()
    {
        if (mapPanel != null)
            mapPanel.SetActive(false);

        // 입력 컨텍스트 복원
        if (InputManager.Instance != null)
            InputManager.Instance.PopContext();
    }
    #endregion

    #region 목적지 버튼 관리
    // 목적지 버튼 업데이트
    private void UpdateDestinationButtons()
    {
        // 기존 버튼 제거
        foreach (var button in destinationButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        destinationButtons.Clear();

        if (currentMapNode == null || currentMapNode.connectedMaps == null)
            return;

        if (MoveService.Instance == null || MoveService.Instance.DestinationDatabase == null)
        {
            Debug.LogWarning("[MapUI] MoveService 또는 destinationDatabase가 없습니다.");
            return;
        }

        // 연결된 맵에 대한 버튼 생성
        foreach (var connectedNode in currentMapNode.connectedMaps)
        {
            if (connectedNode == null) continue;

            if (!MoveService.Instance.DestinationDatabase.TryGet(connectedNode, out var destination))
                continue;

            // 외부 이동만 버튼으로 표시 (내부 이동은 Interaction으로 처리)
            if (destination.moveRange != MapMoveRange.External)
                continue;

            CreateDestinationButton(destination);
        }
    }

    // 목적지 버튼 생성
    private void CreateDestinationButton(MapDestination destination)
    {
        if (destinationButtonPrefab == null || buttonParent == null)
        {
            Debug.LogWarning("[MapUI] destinationButtonPrefab 또는 buttonParent가 설정되지 않았습니다.");
            return;
        }

        var button = Instantiate(destinationButtonPrefab, buttonParent);
        destinationButtons.Add(button);

        // 버튼 텍스트 설정
        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            var nodeName = destination.node != null ? destination.node.displayName : "알 수 없음";
            var cost = destination.actionPointCost > 0 ? $" (에너지 {destination.actionPointCost})" : "";
            text.text = $"{nodeName}{cost}";
        }

        // 버튼 클릭 이벤트
        button.onClick.AddListener(() => OnDestinationButtonClicked(destination));

        // 에너지 부족 시 버튼 비활성화
        if (EnergyManager.Instance != null && destination.actionPointCost > 0)
        {
            button.interactable = EnergyManager.Instance.CanConsume(destination.actionPointCost);
        }
    }

    // 목적지 버튼 클릭 처리
    private void OnDestinationButtonClicked(MapDestination destination)
    {
        if (MoveService.Instance == null)
        {
            Debug.LogWarning("[MapUI] MoveService가 없습니다.");
            return;
        }

        if (MoveService.Instance.TryMoveTo(destination))
        {
            CloseMap();
        }
    }
    #endregion

    #region 에너지 표시
    // 에너지 변경 이벤트 핸들러
    private void OnEnergyChanged(int current, int max)
    {
        UpdateEnergyDisplay(current, max);
    }

    // 에너지 표시 업데이트
    private void UpdateEnergyDisplay(int current, int max)
    {
        if (energyText == null)
            return;

        energyText.text = $"에너지: {current}/{max}";
    }

    // 에너지 표시 업데이트 (파라미터 없이 호출 시)
    private void UpdateEnergyDisplay()
    {
        if (EnergyManager.Instance == null)
            return;

        var current = EnergyManager.Instance.GetCurrentEnergy();
        var max = EnergyManager.Instance.GetMaxEnergy();
        UpdateEnergyDisplay(current, max);
    }
    #endregion
}

