using BlackoutManager.CORE.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//add SeriLog
builder.Host.UseSerilog((context, loggerConfiruration) => loggerConfiruration.WriteTo.Console().ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplicationPostGresContext(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add cross origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//addition oc cors middleware
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
