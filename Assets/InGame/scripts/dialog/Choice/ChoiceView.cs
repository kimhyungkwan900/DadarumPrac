using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceView : MonoBehaviour
{
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button choiceButtonPrefab;
    [SerializeField] private Transform choiceButtonContainer;

    private readonly List<Button> choiceButtons = new List<Button>();

    private Action<ChoiceData> onSelected;

    private void Awake(){
        Hide();
    }

    public void ShowChoices(ChoiceGroupSO choiceGroup, Action<ChoiceData> onChoiceSelected){

        Clear();
        onSelected = onChoiceSelected;

        if (choiceGroup.choices == null || choiceGroup.choices.Count == 0)
        {
            return;
        }

        choicePanel.SetActive(true);

        int validChoiceCount = 0;
        foreach (var choice in choiceGroup.choices){
            if (!AreConditionMet(choice))
            {
                continue;
            }
            
            var button = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            var textComponent = button.GetComponentInChildren<TMP_Text>();
            
            if (textComponent == null)
            {
                Destroy(button.gameObject);
                continue;
            }

            textComponent.text = choice.text;
            button.onClick.AddListener(() => onSelected?.Invoke(choice));

            choiceButtons.Add(button);
            validChoiceCount++;
        }

        if (validChoiceCount == 0)
        {
            choicePanel.SetActive(false);
        }
    }

    public void Hide(){
        Clear();
        choicePanel.SetActive(false);

    }

    #region 초기화
    private void Clear(){
        foreach (var button in choiceButtons){
            Destroy(button.gameObject);
        }
        choiceButtons.Clear();
    }

    private bool AreConditionMet(ChoiceData choice){
        if (choice.conditions == null || choice.conditions.Count == 0) return true;

        foreach (var condition in choice.conditions){
            if (condition == null) continue;

            if (!condition.IsMet()) return false;
        }

        return true;
    }
    #endregion
}
