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

// Kiểm tra xem có Firebase credentials từ biến môi trường không
var firebaseCredentialsJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");
GoogleCredential? credential = null;

if (!string.IsNullOrEmpty(firebaseCredentialsJson))
{
    // Nếu có biến môi trường, đọc trực tiếp từ JSON string
    credential = GoogleCredential.FromJson(firebaseCredentialsJson);
}
else
{
    // Nếu không có biến môi trường, sử dụng file từ config
    var credentialPath = firebaseSection["CredentialPath"]
        ?? throw new InvalidOperationException("Firebase:CredentialPath is not configured and FIREBASE_CREDENTIALS_JSON environment variable is not set");

    var credentialFile = Path.Combine(
        builder.Environment.ContentRootPath,
        credentialPath
    );

    if (!File.Exists(credentialFile))
    {
        throw new FileNotFoundException($"Firebase credentials file not found at: {credentialFile}. Please set FIREBASE_CREDENTIALS_JSON environment variable or ensure the file exists.");
    }

    credential = GoogleCredential.FromFile(credentialFile);
}

if (FirebaseApp.DefaultInstance == null && credential != null)
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = credential
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
