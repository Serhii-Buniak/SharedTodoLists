using Microsoft.OpenApi.Models;
using SharedTodoLists.Api.Middleware;
using SharedTodoLists.Api.Services;
using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.Extensions;
using SharedTodoLists.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(HeaderProvider.UserIdHeaderName, new OpenApiSecurityScheme
    {
        Name = HeaderProvider.UserIdHeaderName,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = HeaderProvider.UserIdHeaderName
                }
            },
            []
        }
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, HeaderProvider>();
builder.Services.AddApplication();
builder.Services.AddPersistence();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
