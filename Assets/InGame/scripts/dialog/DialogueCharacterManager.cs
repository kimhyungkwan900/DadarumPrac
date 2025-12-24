using System.Collections.Generic;
using UnityEngine;

// 씬에 등장하는 캐릭터의 생성, 배치, 연출을 관리합니다.
public class DialogueCharacterManager : MonoBehaviour
{
    [Header("캐릭터 위치(좌 / 중 / 우)")]
    [SerializeField] private RectTransform leftSlot;
    [SerializeField] private RectTransform centerSlot;
    [SerializeField] private RectTransform rightSlot;

    // 현재 씬에 활성화
    private readonly Dictionary<CharacterProfileSO, DialogueCharacterUI> activeCharacters = new();

    public void HandleLine(DialogueLine line)
    {
        // 나레이션 라인
        if (line.character == null) return;

        // 없으면 생성
        bool createdNow = false;
        if (!activeCharacters.TryGetValue(line.character, out var animator) || animator == null)
        {
            animator = CreateNewCharacter(line.character);
            createdNow = animator != null;
        }

        if (animator == null) return;

        // presence 처리 (존재/등장/퇴장)
        HandlePresence(line, animator, createdNow);

        // presence=Disappear이면 이후 연출은 의미가 없을 수 있으니 return (정책)
        if (line.presence == CharacterPresence.Disappear)
            return;

        // visuals 처리 (표정/포커스/쉐이크)
        HandleVisuals(line, animator);

        // 라인 이벤트(선택)
        line.onLineStart?.Invoke();
        // onLineEnd는 보통 NextLine 직전/후나 EndDialogue에 연결하는 게 자연스럽지만,
        // 현재 구조에서는 호출 위치를 프로젝트 정책에 맞게 결정하면 됩니다.
    }

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

    private void HandlePresence(DialogueLine line, DialogueCharacterUI ui, bool createdNow)
    {
        switch (line.presence)
        {
            case CharacterPresence.Appear:
                ui.gameObject.SetActive(true);
                SetPosition(ui, line.position);
                ui.PlayAppear();
                break;

            case CharacterPresence.Disappear:
                ui.PlayDisappear(() =>
                {
                    Destroy(ui.gameObject);
                    activeCharacters.Remove(line.character);
                });
                break;

            case CharacterPresence.Keep:
                if (createdNow)
                {
                    ui.gameObject.SetActive(true);
                    SetPosition(ui, line.position);
                }
                break;
        }
    }

    private void HandleVisuals(DialogueLine line, DialogueCharacterUI ui)
    {
        ui.SetExpression(line.expressionKey, line.overrideSprite);
        ui.SetFocus(line.focus);

        if (line.shake)
            ui.PlayShake();
    }

    private void SetPosition(DialogueCharacterUI ui, CharacterPosition pos)
    {
        RectTransform parent = pos switch
        {
            CharacterPosition.Left => leftSlot,
            CharacterPosition.Center => centerSlot,
            CharacterPosition.Right => rightSlot,
            _ => centerSlot
        };

        RectTransform rt = (RectTransform)ui.transform;
        rt.SetParent(parent, false);
        rt.anchoredPosition = Vector2.zero;
    }

    // Dialogue 시작 시 기본 캐릭터 스폰(요구사항 3번)
    public void SpawnInitial(CharacterProfileSO character, CharacterPosition position, ExpressionKey expressionKey)
    {
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
}
