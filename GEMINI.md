# Gemini Project Memories

## User Preferences
- 사용자는 모든 답변을 한국어로 하기를 선호합니다.
- 사용자는 파일 수정 전에 항상 확인을 받기를 원합니다.

## Project Coding Rules

### 코딩 스타일 규칙

#### 주석 작성 규칙

##### 기본 원칙
- 주석은 간결하게 작성
- XML 주석(`/// <summary>`) 대신 한 줄 주석(`//`) 사용
- 공손한 표현("합니다", "됩니다" 등) 지양, 간결한 표현 사용
- 불필요한 설명 제거, 핵심만 명시

##### 좋은 예시
```csharp
// 아이템 수량을 반환
public int GetItemCount(string itemKey) => GetVar($"item_{itemKey}");

// 아이템 수량을 설정하고, 필요시 flag를 자동 업데이트
public void SetItemCount(string itemKey, int count, int threshold = 0, string flagKey = null)

// 아이템이 일정 수량 이상인지 확인
public bool HasEnoughItem(string itemKey, int threshold)
```

##### 나쁜 예시
```csharp
/// <summary>
/// 아이템 수량을 가져옵니다.
/// </summary>
public int GetItemCount(string itemKey) => GetVar($"item_{itemKey}");

// 아이템 수량을 가져와서 반환해줍니다.
public int GetItemCount(string itemKey) => GetVar($"item_{itemKey}");
```

##### 주석 작성 가이드
- 메서드: 동작을 간단히 설명 (예: "수량 반환", "flag 설정", "조건 확인")
- 복잡한 로직: 왜 그렇게 하는지 설명
- TODO: 명확한 작업 내용만 기록
- 자명한 코드는 주석 생략 (단, 파일 정리 요청 시에는 예외)

#### 코드 변경 시 주석 규칙
- 코드를 수정할 때는 수정된 부분에 주석으로 `// 수정됨: (수정 내용)` 형식을 추가
- 예시: `// 수정됨: 선택지에서 새 대화 전환 시 onComplete 호출 방지`

#### 파일 정리 요청 시 규칙
파일 정리를 요청받으면 다음 규칙을 따라주세요:

1. **각 함수/메서드 역할 주석 추가**
   - 모든 public/protected 메서드와 중요한 private 메서드에 역할을 설명하는 주석을 추가
   - 주석 형식: `// (역할 설명)`
   - 단, 메서드명만으로 역할이 명확한 경우는 간결하게만 작성 (예: `// flag 설정`)

2. **Region 사용**
   - 기능별로 region을 사용하여 코드를 그룹화
   - Region 이름은 한글로 작성 (예: `#region 초기화`, `#region 대화 시작/종료`)
   - 일반적인 region 구조:
     - `#region 컴포넌트` (SerializeField 컴포넌트)
     - `#region 필드` (private/public 필드)
     - `#region 초기화` (Awake, Start, Initialize 등)
     - `#region (주요 기능별)` (각 기능별로 분리)
     - `#region 이벤트 훅` (이벤트 관련 메서드)

3. **코드 구조**
   - 컴포넌트 → 필드 → 초기화 → 주요 기능 → 이벤트 순서로 정리
   - 관련된 메서드들은 같은 region에 그룹화

#### 기타 규칙
- Unity 프로젝트이므로 Unity 코딩 컨벤션을 따릅니다
- 한글 주석 사용 가능
- 코드 가독성을 최우선으로 합니다
