using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceView : MonoBehaviour
{
    #region 컴포넌트

    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button choiceButtonPrefab;
    [SerializeField] private Transform choiceButtonContainer;

    #endregion

    #region 필드

    private readonly List<Button> choiceButtons = new List<Button>();
    private Action<ChoiceData> onSelected;

    #endregion

    #region 초기화

    private void Awake(){
        Hide();
    }

    #endregion

    #region 선택지 표시

    public void ShowChoices(ChoiceGroupSO choiceGroup, Action<ChoiceData> onSelected){

        Clear();
        this.onSelected = onSelected;

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

    #endregion

    #region 초기화/정리

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
