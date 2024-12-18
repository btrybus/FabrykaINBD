using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FabrykaModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add JWT configuration
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.IncludeErrorDetails = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "FabrykaIssuer",
        ValidAudience = "FabrykaAudience",
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Schemes:Bearer:Key"]))
    };
});

builder.Services.AddAuthorization();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");



app.MapGet("/maszyna", async (ApplicationDbContext db) =>
    await db.Maszyny.ToListAsync());

app.MapGet("/maszyna/{id}", async (int id, ApplicationDbContext db) =>
    await db.Maszyny.FindAsync(id)
        is Maszyna maszyna
        ? Results.Ok(maszyna)
        : Results.NotFound());

app.MapPost("/maszyna", async (Maszyna maszyna, ApplicationDbContext db) =>
{
    db.Maszyny.Add(maszyna);
    await db.SaveChangesAsync();

    return Results.Created($"/maszyna/{maszyna.Id}", maszyna);
});

app.MapPut("/maszyna/{id}", async (int id, Maszyna maszyna, ApplicationDbContext db) =>
{
    var maszynaWBazie = await db.Maszyny.FindAsync(id);

    if (maszynaWBazie is null)
        return Results.NotFound();

    maszynaWBazie.Nazwa = maszyna.Nazwa;
    maszynaWBazie.DataUruchomienia = maszyna.DataUruchomienia;
    maszynaWBazie.HalaId = maszyna.HalaId;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/maszyna/{id}", async (int id, ApplicationDbContext db) =>
{
    if (await db.Maszyny.FindAsync(id) is Maszyna maszyna)
    {
        db.Maszyny.Remove(maszyna);
        await db.SaveChangesAsync();
        return Results.Ok(maszyna);
    }

    return Results.NotFound();
});


app.MapPost("/security/getToken", (string username, string password) =>
{
    if (username == "admin@fabryka.com" && password == "P@ssword")
    {
        var token = GenerateJwtToken(username, builder.Configuration["Authentication:Schemes:Bearer:Key"]);
        return Results.Ok(new { Token = token });
    }

    return Results.Unauthorized();
});


app.MapGet("/maszynaoperator/{id}", async (int id, ApplicationDbContext db) =>
    {
        var ops = from o in db.Operatorzy
            where o.Maszyny.Any(m => m.Id == id)
            select o;
        return await ops.ToListAsync();
    })
    .RequireAuthorization();


string GenerateJwtToken(string username, string apikey)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apikey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, username),
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var token = new JwtSecurityToken(
        "FabrykaIssuer",
        "FabrykaAudience",
        claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
