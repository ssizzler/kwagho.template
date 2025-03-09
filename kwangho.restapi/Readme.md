# ASP.NET API 연습 및 템플릿

개발 참조: https://learn.microsoft.com/ko-kr/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio

# 사용자 및 인증
사용자 등록은 대부분 독자적인 데이터구조로 만들지만,
ASP.NET Core에서는 `Identity`를 사용하여 사용자 관리를 쉽게 할 수 있다.
인증 비밀번호 해쉬등을 자동으로 처리해준다.
참조: https://andrewlock.net/exploring-the-asp-net-core-identity-passwordhasher/

JWT Token 인증
참조: https://learn.microsoft.com/ko-kr/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-9.0

# 데이터이스
개발 테스트 용으로 메모리 내 RDB는 데이터베이스를 사용
참조: https://learn.microsoft.com/ko-kr/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-9.0
몽고 디비도 개발용으로 사용
참조: https://learn.microsoft.com/ko-kr/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-9.0&tabs=visual-studio