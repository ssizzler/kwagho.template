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

//�޴� ���� ���
builder.Services.AddScoped<NavMenuService>();

builder.Services.Configure<TossConfig>(builder.Configuration.GetSection("TossInfo"));
//Toss ���� ���� ���
builder.Services.AddScoped<ITossPaymentService, TossPaymentService>();

//Http Client ����
builder.Services.AddHttpClient();

//���� ����
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

//Application Gateway, Load Balancer ���� Health check�� url ����
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
//X-Forward heder ���
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


#region ���� �׽�Ʈ��

// �Ʒ� �ڵ�� ���� �׽�Ʈ������ ����� ������ �ʱ�ȭ �ϱ� ���� �ڵ��̴�.
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
