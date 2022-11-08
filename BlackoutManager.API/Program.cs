using BlackoutManager.CORE.Extensions;
using BlackoutManager.CORE.MiddleWare;
using BlackoutManager.DATA.AutoMapper;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//add SeriLog
builder.Host.UseSerilog((context, loggerConfiruration) => loggerConfiruration.WriteTo.Console().ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplicationPostGresContext(builder.Configuration);
builder.Services.AddApplicationAuthentication(builder.Configuration);
builder.Services.AddApplicationIdentity(builder.Configuration);

builder.Services.AddApplicationServices();
builder.Services.AddApplicationRepositories();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//addition of swagger
builder.Services.AddApplicationSwagger();

//add cross origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

builder.Services.AddAutoMapper(typeof(MapperConfig));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//addition of custom exception middleware
app.UseMiddleware<ExceptionMiddleWare>();

app.UseHttpsRedirection();

//addition oc cors middleware
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
