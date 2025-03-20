using kwangho.context;
using kwangho.mvc.Service;
using kwangho.tosspay;
using kwangho.tosspay.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json.Serialization;
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

//Cache 사용이 필요할 경우....
//개발시  Memory cache 사용
//운영환경에서는 Redis 등을 사용한다.
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

//메뉴 서비스 등록
builder.Services.AddScoped<NavMenuService>();

builder.Services.Configure<TossConfig>(builder.Configuration.GetSection("TossInfo"));
//Toss 결제 서비스 등록
builder.Services.AddScoped<ITossPaymentService, TossPaymentService>();

//Http Client 지원
builder.Services.AddHttpClient();

//인증 설정
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/User/SignIn";
        o.LogoutPath = "/User/Signout";
        o.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        o.SlidingExpiration = true;
    });

builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//Application Gateway, Load Balancer 에서 Health check용 url 생성
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
//X-Forward heder 사용
app.UseForwardedHeaders();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHealthChecks("/health");


#region 개발 테스트용

// 아래 코드는 개발 테스트용으로 사용자 정보를 초기화 하기 위한 코드이다.
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

    context.NavMemu.Add(new() { Id = 1, ParentId = 0, SortOrder = 1, Title = "Home", ActionName = "Index", ControllerName = "Home", Disabled=false, Created=DateTime.Now });
    context.NavMemu.Add(new() { Id = 2, ParentId = 0, SortOrder = 2, Title = "Privacy", ActionName = "Privacy", ControllerName = "Home", Disabled = false, Created = DateTime.Now });
    context.NavMemu.Add(new() { Id = 3, ParentId = 1, SortOrder = 1, Title = "Home", ActionName = "Index", ControllerName = "Home", Disabled = false, Created = DateTime.Now });
    context.NavMemu.Add(new() { Id = 4, ParentId = 1, SortOrder = 2, Title = "Privacy", ActionName = "Privacy", ControllerName = "Home", Disabled = false, Created = DateTime.Now });

    context.SaveChanges();
}

#endregion

app.Run();
