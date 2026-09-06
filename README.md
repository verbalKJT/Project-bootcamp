# Project-BootCamp

Unity와 Photon PUN으로 개발한 최대 3인 3D 협동 액션 디펜스 게임입니다.  
플레이어는 Fire, Rock, Nature 중 하나를 선택해 역할별 전투 기능으로 거점을 방어합니다.

[게임 시연 영상](https://youtu.be/pKoffzxP2_M)

## 프로젝트 정보

| 항목 | 내용 |
| --- | --- |
| 개발 기간 | 2026.02.08 - 2026.04.12 |
| 개발 인원 | 2인 |
| 역할 | 팀장 |
| 엔진 | Unity 6000.5.3f1 |
| 언어 | C# |
| 네트워크 | Photon PUN |

## 담당

- 플레이어 조작, 전투·스킬 시스템
- 소환수·적·보스 AI
- Photon PUN 기반 플레이어·스킬 동기화
- 인게임 시스템 통합 및 테스트

## 주요 구현

### 역할 기반 전투 시스템

- Fire: SphereCastAll과 대상 ViewID 기록을 활용한 근접 공격 판정 및 중복 타격 방지
- Rock: Raycast 기반 방어벽 설치 프리뷰와 사거리 보정
- Nature: Idle, Move, Attack 상태를 분리한 늑대 소환수 FSM

### 멀티플레이 실행 권한 분리

- `PhotonView.IsMine`으로 로컬 입력, 스킬 실행, 소환수 행동의 실행 주체 분리
- `PhotonNetwork.IsMasterClient`로 보스 전격 패턴 결정과 Room Object 생성 담당 분리

### 원격 플레이어 동기화 개선

- PhotonTransformView 설정만으로 해소되지 않은 원격 이동 지연 문제를 확인
- `IPunObservable`과 `OnPhotonSerializeView`를 적용해 위치·회전·Rigidbody 속도를 직접 직렬화
- 원격 플레이어 이동이 연속적으로 보이도록 개선

