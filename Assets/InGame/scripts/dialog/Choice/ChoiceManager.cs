using System;
using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    #region 컴포넌트

    [Header("Components")]
    [SerializeField] private ChoiceView view;

    #endregion

    #region 필드

    private Action<ChoiceData> onSelected;

    #endregion

    #region 선택지 표시

    public void ShowChoices(ChoiceGroupSO choiceGroup, Action<ChoiceData> onSelected){
        if (choiceGroup == null || view == null) return;

        this.onSelected = onSelected;

        view.ShowChoices(choiceGroup, OnChoiceClicked);
    }

    #endregion

    #region 선택지 처리

    private void OnChoiceClicked(ChoiceData choice){
        if (choice == null) return;

        ExecuteChoice(choice);

        view.Hide();
        
        this.onSelected?.Invoke(choice);
        this.onSelected = null;
    }

    public void ExecuteChoice(ChoiceData choice)
    {
        if (choice.effects == null) return;

        foreach ( var effect in choice.effects){
            effect?.Apply();
        }
    }

    #endregion
}
