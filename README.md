# MultiTank
 
# 프로젝트 영상



# 개요
- Unity Netcode / Relay / LobbySystem 학습용 프로젝트
- 기간 : 2024.06 ~ 2024.07
- 개발 엔진 및 언어 : Unity 2022 & C#
- 인원 : 1명


##  게임 플레이 
- 캐릭터 이동 방법


Key|상(위)|좌(왼쪽)|우(오른쪽)|하(아래)  
---|:---:|:---:|:---:|:---:
키보드|W|A|D|S  
키보드|⬆️|⬅️|➡️|⬇️  
설명|전진|왼쪽 방향 회전|오른쪽 방향 회전|후진


- 캐릭터 공격 방법


Key|Fire|
---|:---:
Mouse|LeftClick


### 1. Lobby System  

Host|Create 방 생성  
:---:|:---:
Clinet|Host가 생성한 로비 코드로 참여
Lobby| 생성된 로비 리스트



이름 입력|로비 UI|로비리스트(Lobby) - Refresh(새로고침)
:---:|:---:|:---:
![01](https://github.com/oh-bba-ya/MultiTank/assets/49023743/d41f232a-b033-48f0-ae69-bc5cafbbd0bb)|![02](https://github.com/oh-bba-ya/MultiTank/assets/49023743/9f091bf3-99fb-4b22-adc4-26af31b53b67)|![04](https://github.com/oh-bba-ya/MultiTank/assets/49023743/f515b305-6921-4373-90eb-3734168a23e2)




### 2. Game
#### (1) Coin & Fire
|Coin&Fire|Description
|:---:|:---:|
![06](https://github.com/oh-bba-ya/MultiTank/assets/49023743/a57f5545-41c4-45f0-aa25-8ac5af7839e1)| 코인을 획득하여 포탄을 발사 할 수 있습니다.   코인은 랜덤으로 생성되며 플레이어 사망시 소유하고 있던 코인 일부를 드랍합니다.


#### (2) HUD
|Health Bar & Player Name|Description
|:---:|:---:|
![07](https://github.com/oh-bba-ya/MultiTank/assets/49023743/806e6f4c-6c2d-4d18-9b0d-8e19c49d4637) | 플레이어는 체력바를 통해 본인의 체력을 확인할 수 있으며 처음 설정한 이름이 Health Bar위에 표시됩니다.

LeaderBoard|Description







