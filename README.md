# Maze_Escape (미로탈출게임)

<img src="https://github.com/user-attachments/assets/f87a4b32-4a87-4568-9144-51970564a69e" width="250"></img>
<img src="https://github.com/user-attachments/assets/95b06cb6-e019-4617-b0fc-235e9a5b280e" width="250"></img>
<img src="https://github.com/user-attachments/assets/58cfd61a-6aec-4ba7-8ca5-76a67d38e412" width="250"></img>
</br>

<img src="https://github.com/user-attachments/assets/4b55216d-4486-45ab-be83-bfd38875e329" width="500"></img>
</br>

## 1. 게임 메커니즘
  - 랜덤한 위치에 플레이어 & 적(AI) 생성
  - 출구까지의 길을 빠르게 찾아 도달하는 게임
## 2. 주요 목표
  - 적(AI)보다 먼저 미로를 탈출해서 점수를 획득하는 게임
## 3. 개발 환경
  - Unity, C#
</br>

## 개발 일지
[2024-11-25]
- feat: 초기 캔버스 세팅
- feat: 샘플 플레이어 오브젝트 & UI 제작
  - 플레이어 이동 제작
  - 유니티 물리엔진 시스템 사용
- feat: 미로프리펩(미로의 각 셀) 제작
- feat: 미로 자동 생성 시스템 제작
  - DFS 알고리즘 사용 / 이유 : 간단한 구현, 랜덤성과 완전한 경로 보장, 미로 구조의 복잡성 조절이 쉽다고 생각
</br>

[2024-11-26]
- fix: 미로프리펩
  - UI - Image에서 빈 오브젝트로 변경 / 이유 : 물리엔진과 상호작용 하지 않기 때문에 플레이어와 벽 충돌 처리가 어려움
- refactor: Canvas 삭제
  - 물리 기반 충돌 처리를 위해 기존 Canvas를 삭제 / 이유 : RectTransform을 사용하면 Collider, Rigidbody를 사용 할 수 없고, 충돌을 직접 계산해야 하기 때문
  - 그에 맞춰 코드를 물리 기반으로 작동하게 변경함
- feat: 출구 제작
  - 출구 프리펩 생성
  - 출구는 미로 기준 가운데, 제일 위에 배치
- feat: 플레이어 프리펩화 & 위치 조정
  - 플레이어 프리펩으로 변경
  - 플레이어는 출구와 일정 거리 이상 떨어지게 세팅
- refact: 출구기준으로 랜덤 미로 생성 변경
  - 랜덤 미로 생성 로직을 출구에서 시작하게 변경 / 이유 : 출구는 무조건 길이 있어야 하기 때문(기존 코드로는 출구위치에 벽이 생길 가능성이 있음)
</br>

[2024-11-27]
- feat: Tag 추가
  - Add Exit, Player Tag
- feat: GameManager
  - 카운트다운 기능 & UI 제작
  - 게임 종료 기능 제작 (Tag로 판별)
- feat: Stage
  - 게임 Stage 제작 (Stage 1~10)
  - 매 Stage마다 새로운 미로 생성
</br>

[2024-11-28]
- feat: Tag 추가
  - Add Enemy Tag
</br>

[2024-11-29]
- feat: Add Enemy System
  - Add Enemy Prefab
  - A* 알고리즘을 사용해서 적AI가 출구까지의 최적의 경로를 계산해서 자동으로 이동하는 시스템 제작
  - Player와 Enemy의 충돌 제거
  - Stage가 증가할때마다 Enemy 속도 증가
- feat: Add GameOver UI & System
  - Enemy가 먼저 출구에 도달하면 GameOver
  - GameOver UI : Restart, Quit 버튼 추가
</br>

[2024-11-30]
- refactor: 미로 & 오브젝트 사이즈 조정
</br>

[2024-12-01]
- feat: Top Panel & Option Panel
  - 상단 UI창 제작
  - Option Panel 제작
    - 게임 퍼즈, 재시작, 타이틀 씬이동
- feat: Title Scene
  - 타이틀씬 제작
</br>

[2024-12-02]
- feat: Enemy Line Renderer
  - 플레이어가 출구 도달했을 때 적의 이동 경로 라인이 보이고 점점 줄어드는 애니메이션 제작
</br>

[2024-12-03]
- feat: 점수 시스템
  - 다음 스테이지로 넘어가기전에 +되는 점수 효과 넣어주기
  - 스테이지 종료 후 라인 그려지고 1초 뒤에 다음 스테이지로 넘어가게 조정
</br>

[2024-12-05]
- feat: GameClear Panel 추가
- feat: 미로 양옆으로 길이 생기도록 변경
  - 출구근처에 길이 기존에 1개였는데 2개로 변경
- feat: Restart & New Game 세팅 변경
</br>

[2024-12-06]
- design: UI 조정
- refactor: 스테이지별 점수 추가 로직 조정
- fix: Canvas 우선순위 조정
- fix: GameOver Panel Restart 버튼을 New Game 버튼으로 변경
- fix: Quit버튼들을 Title버튼으로 변경
- build 테스트 완료
- 제작 완료
</br>
