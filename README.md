# C# 개발 연습 및 템플릿 솔루션

이 프로젝트는 C#으로 다양한 기능을 연습하고, 코드 템플릿을 활용하여 효율적인 개발을 할 수 있도록 돕기 위한 솔루션입니다. 
기본적인 코드 구조와 패턴을 연습하고 있으며, 다양한 C# 기능을 실습하는 데 사용 하고 있습니다.

## 프로젝트 소개
### kwangho.context
EF Core Context로 DB 연결 및 테이블 생성을 위한 클래스 라이브러리입니다.
IdentityUser 클래스를 상속받아 사용자 정보를 저장하는 테이블을 생성합니다.
JWT 새로고침 토큰 정보를 저장 합니다.

### kwangho.restapi
asp.net core Rest API 개발 테스트 용으로 RDB는 메모리 데이터베이스를 사용<br>
토스 결제 연습<br>
JWT Token 인증<br>

### kwangho.mvc
asp.net core mvc 개발 테스트 용<br>
kwangho.restapi 와 동일한 기능을 mvc로 구현<br>
Cookie 인증<br>

### kwangho.tosspay
Toss 결제 API 연동을 위한 클래스 라이브러리입니다.