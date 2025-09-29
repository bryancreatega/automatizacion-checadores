using ComplementosPago;
using ComplementosPago.Controllers;
using ComplementosPago.Services;
using Encrypt;
using Functions;
using Microsoft.EntityFrameworkCore;
using ModelContext.Models;
using System;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(); // 👈 necesario para Worker como Windows Service

builder.Services.AddDbContext<ComplementoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<FingerPrintsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conLectores")));

builder.Services.AddDbContext<LaboraContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conLabora")));

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<LectoresController>();
builder.Services.AddSingleton<RespaldoLectores>();
builder.Services.AddSingleton<ExtraccionChecadas>();
builder.Services.AddSingleton<EnvioLabora>();
builder.Services.AddSingleton<funFprGra>();
builder.Services.AddScoped<IEmailService, EmailService>();


var host = builder.Build();
host.Run();
