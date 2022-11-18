var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    // var portalUrl = Environment.GetEnvironmentVariable("PORTAL_URL");

    // if (string.IsNullOrEmpty(portalUrl)) {
    //     portalUrl = "http://localhost:4200";
    // }

    options.AddPolicy(
        name: "portal",
        policy =>
        {
            // TODO - expose origin via configuration
            // policy.WithOrigins(portalUrl);
            policy.AllowAnyOrigin();
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("portal");

app.UseAuthorization();

app.MapControllers();

app.Run(Environment.GetEnvironmentVariable("IMAGES_URL"));
