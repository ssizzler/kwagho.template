using kwangho.context;
using kwangho.tosspay.Models;
using kwangho.tosspay;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Net;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json.Serialization;

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
        o.ExpireTimeSpan = TimeSpan.FromHours(2);
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

app.Run();
