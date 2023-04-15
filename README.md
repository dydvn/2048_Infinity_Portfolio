❗ 인피니티 2048 프로젝트에서 제가 작성했고, 불필요한 부분은 제거한 스크립트를 올리는 리포지토리입니다. ❗

🎮 Android download link :
https://play.google.com/store/apps/details?id=com.TwoDHyperSND.Infinity2048

🎞 게임 소개 영상 : 
https://www.youtube.com/watch?v=CHMZl2t7sP4

------------------------------------------------------------------------

Release date : 2020.05

Platform : Mobile (Google play)


------------------------------------------------------------------------


🛠 저는 이 게임에서 이런걸 구현했습니다!
- 플레이어 조작
- 특정 조건을 만족하는 Box를 생성하는 Coroutine
- 파티클, 애니메이션, UI 구성 및 버튼의 각 기능
- Google Ads


------------------------------------------------------------------------

🛠 플레이어 조작

![Untitled](https://user-images.githubusercontent.com/62327209/232215272-eb56d524-44ae-49da-b479-902aa84d811a.png)
- 플레이어는 중력으로 인해 저절로 하강합니다.
- 화면을 터치하면 Rigidbody2D.AddForce를 이용해 캐릭터를 순간적으로 상승시킵니다.
- Code - [https://github.com/dydvn/Infinity_2048/blob/main/PlayerControl.cs](https://github.com/dydvn/Infinity_2048/blob/main/PlayerControl.cs)


------------------------------------------------------------------------

🛠 특정 조건을 만족하는 Box를 생성하는 Coroutine

![Untitled](https://user-images.githubusercontent.com/62327209/232215898-3ddd99f9-b5d2-4a3e-aa13-24d7f100e0c1.png)
- 한 번의 반복에는 4개의 Box가 등장합니다.
- 2 Box가 플레이어가 닿아야 하는 정답 Box라면, 정답 Box의 2배수인 4 Box는 필수로 나오고, 나머지 두 개의 Box는 2와 4를 제외한 2의 n승 값을 랜덤으로 가지고 나옵니다. (이때, 1 ≤ n ≤ 11)
- Code - [https://github.com/dydvn/Infinity_2048/blob/main/Game_master.cs](https://github.com/dydvn/Infinity_2048/blob/main/Game_master.cs) (box_re2() 함수)

------------------------------------------------------------------------

🛠 파티클, 애니메이션, UI 구성 및 버튼의 각 기능


https://user-images.githubusercontent.com/62327209/232220123-7c3cb81d-986a-4dc2-89a6-15de82ebc704.mp4


- 주인공 캐릭터의 애니메이션 구현
- 화면 하단의 파티클 제작
