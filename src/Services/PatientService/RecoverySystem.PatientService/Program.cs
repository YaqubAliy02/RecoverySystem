using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RecoverySystem.BuildingBlocks.Messaging.Extensions;
using RecoverySystem.PatientService.Data;
using RecoverySystem.PatientService.Models;
using RecoverySystem.PatientService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Recovery System API",
        Version = "v1",
        Description = "API for the Recovery System microservices"
    });

    // Add JWT Authentication support in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);
// Add DbContext
builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add RabbitMQ
builder.Services.AddRabbitMQMessaging(builder.Configuration);

// Add Services
builder.Services.AddScoped<EventPublisher>();
builder.Services.AddHostedService<EventConsumerService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PatientDbContext>();
        context.Database.Migrate();

        // Seed data if needed
        if (!context.Patients.Any())
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Seeding patient data...");

            // Add sample patients
            var patients = new List<Patient>
            {
                new Patient
                {
                    Name = "John Doe",
                    Age = 45,
                    Gender = "Male",
                    Phone = "555-123-4567",
                    Email = "john.doe@example.com",
                    Address = "123 Main St, Anytown, USA",
                    Status = "active",
                    LastVisit = DateTime.UtcNow.AddDays(-10),
                    NextAppointment = DateTime.UtcNow.AddDays(20),
                    Avatar = "/placeholder.svg?height=128&width=128",
                    MedicalHistory = "Patient has a history of hypertension and type 2 diabetes.",
                    Medications = new List<string> { "Metformin", "Lisinopril" }
                },
                new Patient
                {
                    Name = "Jane Smith",
                    Age = 38,
                    Gender = "Female",
                    Phone = "555-987-6543",
                    Email = "jane.smith@example.com",
                    Address = "456 Oak Ave, Somewhere, USA",
                    Status = "active",
                    LastVisit = DateTime.UtcNow.AddDays(-5),
                    NextAppointment = DateTime.UtcNow.AddDays(15),
                    Avatar = "/placeholder.svg?height=128&width=128",
                    MedicalHistory = "Patient has asthma and seasonal allergies.",
                    Medications = new List<string> { "Albuterol", "Cetirizine" }
                }
            };

            context.Patients.AddRange(patients);
            context.SaveChanges();

            // Add vitals for the first patient
            var vitals = new List<PatientVital>
            {
                new PatientVital
                {
                    PatientId = patients[0].Id,
                    Date = DateTime.UtcNow.AddDays(-10),
                    HeartRate = 72,
                    BloodPressure = "120/80",
                    Temperature = 98.6,
                    RespiratoryRate = 16,
                    OxygenSaturation = 98,
                    Pain = 2
                },
                new PatientVital
                {
                    PatientId = patients[0].Id,
                    Date = DateTime.UtcNow.AddDays(-5),
                    HeartRate = 68,
                    BloodPressure = "118/78",
                    Temperature = 98.4,
                    RespiratoryRate = 14,
                    OxygenSaturation = 99,
                    Pain = 1
                }
            };

            context.PatientVitals.AddRange(vitals);
            context.SaveChanges();

            // Add notes for the first patient
            var notes = new List<PatientNote>
            {
                new PatientNote
                {
                    PatientId = patients[0].Id,
                    Content = "Patient reports feeling better after starting new medication.",
                    Date = DateTime.UtcNow.AddDays(-10),
                    AuthorId = "system",
                    AuthorName = "Dr. Smith",
                    AuthorAvatar = "/placeholder.svg?height=40&width=40",
                    AuthorRole = "doctor",
                    Category = "general"
                },
                new PatientNote
                {
                    PatientId = patients[0].Id,
                    Content = "Blood pressure is improving. Continue current treatment plan.",
                    Date = DateTime.UtcNow.AddDays(-5),
                    AuthorId = "system",
                    AuthorName = "Dr. Johnson",
                    AuthorAvatar = "/placeholder.svg?height=40&width=40",
                    AuthorRole = "doctor",
                    Category = "treatment"
                }
            };

            context.PatientNotes.AddRange(notes);
            context.SaveChanges();

            logger.LogInformation("Patient data seeded successfully");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();