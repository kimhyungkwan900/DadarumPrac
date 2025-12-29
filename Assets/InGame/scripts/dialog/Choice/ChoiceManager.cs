using System;
using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ChoiceView choiceView;

    private Action<ChoiceData> onChoiceSelected;

    public void ShowChoices(ChoiceGroupSO choiceGroup, Action<ChoiceData> onSelected){
        if (choiceGroup == null || choiceView == null) return;

        onChoiceSelected = onSelected;

        choiceView.ShowChoices(choiceGroup, OnChoiceClicked);
    }

    private void OnChoiceClicked(ChoiceData choice){
        if (choice == null) return;

        ExecuteChoice(choice);

        choiceView.Hide();
        
        onChoiceSelected?.Invoke(choice);
        onChoiceSelected = null;
    }

    public void ExecuteChoice(ChoiceData choice)
    {
        if (choice.effects == null) return;

        foreach ( var effect in choice.effects){
            effect?.Apply();
        }
    }
}
