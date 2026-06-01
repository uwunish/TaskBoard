using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using TaskBoard.API.Hubs;
using TaskBoard.API.Middleware;
using TaskBoard.Application;
using TaskBoard.Application.Common.Interfaces;
using TaskBoard.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "TaskBoard API",
    });

    // Tell swagger about jwt
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter: Bearer {your token here}"
    });

    // Make every endpoint require the token by default
    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
    });

}
);
builder.Services.AddSignalR();
builder.Services.AddScoped<IBoardHubService, TaskBoard.API.Services.BoardHubService>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// allow angular server dev to connect
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    policy.WithOrigins(
        "http://localhost:4200",
        "http://taskboard-anishkoirala.runasp.net",
        "https://taskboard-anishkoirala.netlify.app"
        )
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials() // required for signalR
    );
});

// builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

// must be first, so it catches all exceptions
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
// app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors("Angular"); // should be before auth

//if (!app.Environment.IsDevelopment())
//{
//     app.UseHttpsRedirection();
//}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<BoardHub>("/hubs/board"); // signalR endpoint

app.Run();
