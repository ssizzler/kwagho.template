using kwangho.restapi.Config;
using kwangho.restapi.Context;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

var builder = WebApplication.CreateBuilder(args);

#region Application gateway x-forword-for 설정
// 클라우드 환경에서 대부분 서비스 앞단에 Application Gateway, Load Balancer 등이 존재하고,
// 이를 통해 클라이언트의 IP를 식별하기 위해 X-Forwarded-For 헤더를 사용한다.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 5;
    // Azure Application Gateway의 경우 Gateway에서 내부로 전달되는 IP 대역을 지정하여 IP를 프로그램에서 IP로 처리하지 않도록 한다. 
    options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("172.16.0.0"), 24));
});
#endregion

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseInMemoryDatabase("AppDb"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
//swagger UI에서 JWT 인증 토큰을 입력할 수 있도록 설정
builder.Services.AddSwaggerGen(c =>
{    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "다음과 같은 형식으로 JWT Authorization header에 토큰을 보내도록 합니다.<br /> \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Name = "Authorization",
                In = ParameterLocation.Header,
                BearerFormat = "Bearer token",

            },
            new List<string>()
        }
    });
});

//jwt 토큰 발급자 정보 설정하기 위하여 구성 정보를 가져온다.
var jwtIssuer = builder.Configuration.GetSection("JwtTokenIssuer").Get<JwtTokenIssuer>();
builder.Services.AddSingleton(jwtIssuer!);

builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = jwtIssuer!.Authority;
                    options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer.Issuer,
                        ValidAudience = jwtIssuer.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtIssuer.SecretKey)),
                    };
                });

//Application Gateway, Load Balancer 에서 Health check용 url 생성
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//X-Forward heder 사용
app.UseForwardedHeaders();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
