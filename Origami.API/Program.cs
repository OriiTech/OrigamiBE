using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.API.Extensions;
using Origami.API.Middlewares;
using Origami.BusinessTier.Constants;
using Origami.DataTier.Models;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration.Json;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsProduction())
{
    foreach (var source in builder.Configuration.Sources.OfType<JsonConfigurationSource>())
    {
        source.ReloadOnChange = false;
    }
}

// Lấy port từ biến môi trường của Render
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}
var firebaseSection = builder.Configuration.GetSection("Firebase");

var credentialPath = firebaseSection["CredentialPath"]
    ?? throw new InvalidOperationException("Firebase:CredentialPath is not configured");

var credentialFile = Path.Combine(
    builder.Environment.ContentRootPath,
    credentialPath
);

if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(credentialFile)
    });
}


// Add services to the container.
builder.Services.AddDbContext<OrigamiDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
var authBuilder = builder.Services.AddJwtValidation(builder.Configuration);
builder.Services.AddConfigSwagger();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsConstant.PolicyName,
        policy =>
        {
            policy.WithOrigins()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    x.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
});
builder.Services.AddDatabase(builder);
builder.Services.AddUnitOfWork();
builder.Services.AddServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

authBuilder.AddCookie("External");
authBuilder.AddGoogle(options =>
{
    IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuthNSection["ClientId"] ?? throw new InvalidOperationException("Google ClientId is not configured");
    options.ClientSecret = googleAuthNSection["ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret is not configured");
    options.CallbackPath = googleAuthNSection["CallbackPath"] ?? "/signin-google";
    options.SignInScheme = "External";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
var swaggerEnabled = app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled");
if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseForwardedHeaders();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors(CorsConstant.PolicyName);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
