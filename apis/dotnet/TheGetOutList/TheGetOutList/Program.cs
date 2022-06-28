using Microsoft.AspNetCore.Authentication.JwtBearer;
using TheGetOutList.Data;
using TheGetOutList.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Auth0 Authentication Services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Auth0:Authority"];
    options.Audience = builder.Configuration["Auth0:Audience"];
});

var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"];
var mongoDatabase = builder.Configuration["MongoDB:DatabaseName"];

var mongoRepository = new MongoRepository().Connect(mongoConnectionString).SetDatabase(mongoDatabase);

builder.Services.AddSingleton<IDataRepository>(mongoRepository);

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthenticationAdministrationService, AuthenticationAdministrationService>();

//await mongoRepository.SeedDatabase();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();