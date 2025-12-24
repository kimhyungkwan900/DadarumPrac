using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        // TODO: 개발 완료시 변경
        //if (TurnManager.Instance == null)
            //return;

        //if (TurnManager.Instance.myTurn == false)
            //return;

        TurnManager.Instance.EndTurn();
    }
}
