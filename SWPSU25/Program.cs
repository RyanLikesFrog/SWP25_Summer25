using Microsoft.EntityFrameworkCore;
using DataLayer.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ServiceLayer.Implements;
using ServiceLayer.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using RepoLayer.Implements;
using RepoLayer.Interfaces;
using System.Security.Claims;
using ServiceLayer.Implements.Reminder;
using SWPSU25.SignalRHubs;
using ServiceLayer.DTOs.Payment;
using ServiceLayer.PaymentGateways;

var builder = WebApplication.CreateBuilder(args);

// Tên policy tùy ý đặt
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// 1. Đăng ký CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("*") // hoặc "*" để cho tất cả (cẩn thận)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SWPSU25Context>(options =>
    options.UseSqlServer(connectionString));

// Cấu hình MomoSettings
builder.Services.Configure<MomoSettings>(builder.Configuration.GetSection("MomoSettings"));
// Đăng ký HttpClient và MomoClient
builder.Services.AddHttpClient<IMomoClient, MomoClient>();

// Đăng ký IHttpContextAccessor (cần để lấy IP client trong service)
builder.Services.AddHttpContextAccessor();

// Register Repositories
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IARVProtocolRepository, ARVProtocolRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
builder.Services.AddScoped<ILabResultRepository, LabResultRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientTreatmentProtocolRepository, PatientTreatmentProtocolRepository>();
builder.Services.AddScoped<ITreatmentStageRepository, TreatmentStageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRepository, BaseRepository>();
builder.Services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();

// Register Service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<ITreatmentStageService, TreatmentStageService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ILabResultService, LabResultService>();
builder.Services.AddScoped<IARVProtocolService, ARVProtocolService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IPatientTreatmentProtocolService, PatientTreatmentProtocolService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMomoClient, MomoClient>();

// add signalR
builder.Services.AddScoped<ReminderService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<ReminderBackgroundService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ClockSkew = TimeSpan.Zero, // Đảm bảo không có độ lệch thời gian cho token hết hạn
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization(); // Thêm dịch vụ ủy quyền

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    // Cấu hình Swagger để hỗ trợ JWT Bearer Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // ← Quan trọng: viết thường!
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token JWT vào đây. Không cần thêm chữ 'Bearer ' phía trước."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    });
}

app.UseHttpsRedirection();

// IMPORTANT: The order of middleware matters!
// UseRouting should be before UseCors.
app.UseRouting();

// Enable the CORS policy using the name defined above
app.UseCors(MyAllowSpecificOrigins);

// UseAuthorization should be after UseCors (and UseRouting)
app.UseAuthorization();

app.MapControllers();   
app.MapHub<ReminderHub>("/reminderHub");

app.Run();