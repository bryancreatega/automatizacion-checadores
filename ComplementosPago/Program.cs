using ComplementosPago;
using Microsoft.EntityFrameworkCore;
using ModelContext.Models;
using System;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(); // 👈 necesario para Worker como Windows Service


builder.Services.AddDbContext<ComplementoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<FingerPrintsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conLectores")));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
