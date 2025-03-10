using kwangho.context;
using kwangho.restapi.Config;
using kwangho.tosspay.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

var builder = WebApplication.CreateBuilder(args);

#region Application gateway x-forword-for ����
// Ŭ���� ȯ�濡�� ��κ� ���� �մܿ� Application Gateway, Load Balancer ���� �����ϰ�,
// �̸� ���� Ŭ���̾�Ʈ�� IP�� �ĺ��ϱ� ���� X-Forwarded-For ����� ����Ѵ�.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 5;
    // Azure Application Gateway�� ��� Gateway���� ���η� ���޵Ǵ� IP �뿪�� �����Ͽ� IP�� ���α׷����� IP�� ó������ �ʵ��� �Ѵ�. 
    options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("172.16.0.0"), 24));
});
#endregion

//Cache ����� �ʿ��� ���....
//���߽�  Memory cache ���
//�ȯ�濡���� Redis ���� ����Ѵ�.
if (builder.Environment.IsProduction())
{
    //builder.Services.AddStackExchangeRedisCache(options =>
    //{
    //    options.Configuration = builder.Configuration.GetConnectionString("cacheConnString");
    //    options.InstanceName = "mycache";
    //});
}
else
{
    builder.Services.AddDistributedMemoryCache();
}


builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseInMemoryDatabase("AppDb"));

builder.Services.Configure<TossConfig>(builder.Configuration.GetSection("TossInfo"));

//Http Client ����
builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//swagger UI���� JWT ���� ��ū�� �Է��� �� �ֵ��� ����
builder.Services.AddSwaggerGen(c =>
{    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "������ ���� �������� JWT Authorization header�� ��ū�� �������� �մϴ�.<br /> \"Authorization: Bearer {token}\"",
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

//jwt ��ū �߱��� ���� �����ϱ� ���Ͽ� ���� ������ �����´�.
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

//Application Gateway, Load Balancer ���� Health check�� url ����
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//X-Forward heder ���
app.UseForwardedHeaders();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

#region ���� �׽�Ʈ��

// �Ʒ� �ڵ�� �ȯ�濡���� ������� ����
// �ʱ� ������ ����
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var passwordHasher = new PasswordHasher<ApiUser>();
    var adminuser = new ApiUser
    {
        UserName = "admin",
        Name = "Administrator",
        RegisterDate = DateTime.Now,
        Active = true
    };
    adminuser.PasswordHash = passwordHasher.HashPassword(adminuser, "test!!");
    var user1 = new ApiUser
    {
        UserName = "user1",
        Name = "User One",
        RegisterDate = DateTime.Now,
        Active = true
    };
    user1.PasswordHash = passwordHasher.HashPassword(user1, "test!!");

    context.Users.AddRange(adminuser, user1);
    context.SaveChanges();
}
#endregion


app.Run();
