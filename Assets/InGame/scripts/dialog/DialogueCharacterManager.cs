using System.Collections.Generic;
using UnityEngine;

// 씬에 등장하는 캐릭터의 생성, 배치, 연출을 관리합니다.
public class DialogueCharacterManager : MonoBehaviour
{
    #region 컴포넌트

    [Header("캐릭터 위치(좌 / 중 / 우)")]
    [SerializeField] private RectTransform leftSlot;
    [SerializeField] private RectTransform centerSlot;
    [SerializeField] private RectTransform rightSlot;

    #endregion

    #region 필드

    private readonly Dictionary<CharacterProfileSO, DialogueCharacterUI> activeCharacters = new();

    #endregion

    #region 대사 처리

    public void HandleLine(DialogueLine line)
    {
        if (line.character == null) return;

        if(!activeCharacters.TryGetValue(line.character, out var ui) || ui == null){
            ui = CreateNewCharacter(line.character);
        }
        if (ui == null) return;

        HandlePresence(line, ui);

        if (line.presence == CharacterPresence.Disappear)
            return;

        HandleVisuals(line, ui);
    }

    #endregion

    #region 캐릭터 생성

    private DialogueCharacterUI CreateNewCharacter(CharacterProfileSO profile)
    {
        GameObject character = Instantiate(profile.prefab, transform);
        var ui = character.GetComponentInChildren<DialogueCharacterUI>(true);

        if (ui == null)
        {
            Debug.LogError($"{profile.name} prefab에 DialogueCharacterUI가 없습니다.");
            Destroy(character);
            return null;
        }

        ui.Initialize(profile);
        character.SetActive(false);

        activeCharacters[profile] = ui;
        return ui;
    }

    #endregion

    #region 캐릭터 상태 처리

    private void HandlePresence(DialogueLine line, DialogueCharacterUI ui)
    {
        switch (line.presence){
            case CharacterPresence.Appear:
            ui.Show();
            SetPosition(ui, line.position);
            break;

            case CharacterPresence.Disappear:
            ui.Hide();
            break;

            case CharacterPresence.Keep:
            if (ui.Position != line.position){
                SetPosition(ui, line.position);
            }
            break;
        }
    }

    private void HandleVisuals(DialogueLine line, DialogueCharacterUI ui)
    {
        ui.SetExpression(line.expressionKey, line.overrideSprite);
        ui.SetFocus(line.focus);
    }

    #endregion

    #region 위치 관리

    private void SetPosition(DialogueCharacterUI ui, CharacterPosition pos)
    {
        // 위치가 변경되지 않으면 이동하지 않음
        if (ui.Position == pos && ui.transform.parent != null)
        {
            RectTransform currentParent = (RectTransform)ui.transform.parent;
            RectTransform expectedParent = GetSlotTransform(pos);
            if (currentParent == expectedParent)
                return;
        }

        RectTransform parent = GetSlotTransform(pos);

        RectTransform rt = (RectTransform)ui.transform;
        rt.SetParent(parent, false);
        rt.anchoredPosition = Vector2.zero;
        
        // UI의 위치 정보 업데이트
        ui.SetPosition(pos);
    }

    private RectTransform GetSlotTransform(CharacterPosition pos)
    {
        return pos switch
        {
            CharacterPosition.Left => leftSlot,
            CharacterPosition.Center => centerSlot,
            CharacterPosition.Right => rightSlot,
            _ => centerSlot
        };
    }

    #endregion

    #region 초기화/정리

    // Dialogue 시작 시 기본 캐릭터 스폰
    public void SpawnInitial(CharacterProfileSO character, CharacterPosition position, ExpressionKey expressionKey)
    {
        // 이미 생성된 캐릭터가 있으면 재사용
        if (!activeCharacters.TryGetValue(character, out var ui) || ui == null)
        {
            ui = CreateNewCharacter(character);
        }
        if (ui == null) return;

        ui.gameObject.SetActive(true);
        SetPosition(ui, position);
        ui.SetExpression(expressionKey, null);
        ui.SetFocus(false);
    }

    public void ClearCharacters()
    {
        foreach (var kv in activeCharacters)
        {
            if (kv.Value != null)
                Destroy(kv.Value.gameObject);
        }
        activeCharacters.Clear();
    }

    #endregion
}
