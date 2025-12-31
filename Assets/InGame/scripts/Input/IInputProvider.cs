using UnityEngine;

// 입력 제공자의 표준 인터페이스
public interface IInputProvider
{
    // 입력 시스템의 활성화 여부
    bool Enabled { get; set; }

    // 정규화된 이동 입력 값 (Vector2)
    Vector2 MoveInput { get; }
}
