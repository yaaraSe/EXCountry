using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Repository;
using Repository.Models;
using serverPortal.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy
    (
       "CorsPolicy",
    policy =>
    {
        policy.SetIsOriginAllowed(_ => true);
        policy.AllowAnyHeader().WithMethods("OPTIONS", "HEAD", "CONNECT", "GET", "POST", "PUT", "DELETE", "PATCH")
           .AllowCredentials();
    }

    );
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MongoDatabaseSettings>(
    builder.Configuration.GetSection(nameof(MongoDatabaseSettings)));
builder.Services.AddSingleton<IDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDatabaseSettings>>().Value);
builder.Services.AddSingleton<MongoRepository>();
builder.Services.AddSingleton<ImportDataCountry>(); // Register DataCountryService

var app = builder.Build();
var importDataCountry = app.Services.GetRequiredService<ImportDataCountry>();// Retrieve an instance of DataCountryService from application services
await importDataCountry.FetchCountriesAsync();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
