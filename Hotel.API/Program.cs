using Serilog;
using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Configurations;
using Hotel.API.Contracts;
using Hotel.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectStr = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");

builder.Services.AddDbContext<HotelListingDbContext>(options =>
{
    options.UseSqlServer(connectStr); 
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow All", build => build.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

builder.Host.UseSerilog((context,loggerConfig) =>
{
    loggerConfig.WriteTo.Console().ReadFrom.Configuration(context.Configuration); // context.Configuration = config file of project (appsetting.json)
});

builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Allow All");

app.UseAuthorization();

app.MapControllers();

app.Run();
