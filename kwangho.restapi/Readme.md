# ASP.NET API 연습 및 템플릿

개발 참조: https://learn.microsoft.com/ko-kr/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio

# 사용자 및 인증
사용자 등록은 대부분 독자적인 데이터구조로 만들지만,<br />
ASP.NET Core에서는 `Identity`를 사용하여 사용자 관리를 쉽게 할 수 있다.<br />
인증 비밀번호 해쉬등을 자동으로 처리해준다.
참조: https://andrewlock.net/exploring-the-asp-net-core-identity-passwordhasher/<br />

JWT Token 인증<br />
참조: https://learn.microsoft.com/ko-kr/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-9.0<br />

# 데이터이스
개발 테스트 용으로 RDB는 메모리 데이터베이스를 사용<br />

# 토스 결제 API 호출 
C#에서 토스 결제 API 호출<br />
sdk문서: https://docs.tosspayments.com/sdk/v2/js#%EA%B2%B0%EC%A0%9C%EC%B0%BD <br />
sample: https://github.com/tosspayments/tosspayments-sample/blob/main/express-javascript/public/payment/checkout.html