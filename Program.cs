using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TestSignalR;
using TestSignalR.Actors;
using TestSignalR.Hubs;
using TestSignalR.Middleware;
using TestSignalR.Models;
using TestSignalR.Models.Enums;
using TestSignalR.Models.Helper;
using TestSignalR.Models.Settings;
using TestSignalR.Services;
using TestSignalR.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

JwtSetting jwtSettings = builder.Configuration.GetSection("Auth:Jwt").Get<JwtSetting>()!;

// Configuration settings

builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddConnections();
builder.Services.AddSignalR();

// services DI
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// actors DI
builder.Services.AddScoped<IHasher, Hasher>();
builder.Services.AddScoped<IAvatar, Avatar>();
builder.Services.AddScoped<ICookie, Cookie>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteDb"));
});

var authService = builder.Services.BuildServiceProvider().GetRequiredService<IAuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["authToken"];
            return Task.CompletedTask;
        },
        OnChallenge = async context =>
        {

            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                string? token = context.Request.Cookies["refreshToken"];

                context.HandleResponse();

                if (token == null)
                {
                    context.Response.Redirect("/login");
                    return;
                }

                User? user = await authService.ValidateRefreshTokenAsync(token);

                if (user != null)
                {
                    var newAccessToken = authService.GenerateAccessToken(user.Login, user.Id.ToString());
                    CookieOptions accessCookieOptions = authService.GetCookieOptions(TokenType.Access);
                    context.HttpContext.Response.Cookies.Append("authToken", newAccessToken, accessCookieOptions);
                    context.Response.Redirect(context.Request.Path);

                    return;
                }

                context.Response.Redirect("/login");
            }

            return;
        }
    };
});
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "VALIDATION-TOKEN";
    options.HeaderName = "X-VALIDATION-TOKEN";
    options.FormFieldName = "validationTiken";
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7091")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    User? systemUser = dbContext.Users.Where((u) => u.Login == "system").FirstOrDefault();

    if(systemUser == null)
    {
        dbContext.Users.Add(new User
        {
            Username = "�������",
            Login = "system",
            PasswordHash = "system",
            RegistredAt = DateTimeHelper.GetMoscowTimestampNow(),
            Avatar = "system/avatar_system.webp"
        });
        await dbContext.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
} else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chat");

app.Use(async (context, next) =>
{
    var securityMiddleware = new Security(builder.Configuration);
    await securityMiddleware.UseCSRF(context, next);
});

app.Run();
