using DataAccessLayer;
using studentDataAccessLayer;

var builder = WebApplication.CreateBuilder(args);


DatabaseSettings.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("StudentApiCorsPolicy", policy =>
    {
        policy
         .WithOrigins(
             "https://localhost:7217",
             "http://localhost:5215"
         )
         .AllowAnyHeader()
         .AllowAnyMethod();
    });
});
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("StudentApiCorsPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();