using AutoMapper;
using MFQ2022;
using Microsoft.EntityFrameworkCore;
using Quartz;
using TestApp.Data;
using TestApp.Service.KoreaPrice;
using TestApp.Service.KoreaWarehouseInfo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<MFQService>();
builder.Services.AddSingleton<InsertJob>();
builder.Services.AddSingleton<ExcuteJob>();
builder.Services.AddScoped<WarehouseInfoAPIService>();
builder.Services.AddScoped<BusanUmgungAPIService>();
builder.Services.AddScoped<CollectBusanUmgongInfoJob>();

var ProcessDbConnectionString = builder.Configuration.GetConnectionString("ProcessDbConnection");
builder.Services.AddDbContext<ProcessDbContext>(options =>
    options.UseMySQL(ProcessDbConnectionString));
var WarehouseInfoDbConnectionString = builder.Configuration.GetConnectionString("WarehouseInfoDbConnection");
builder.Services.AddDbContext<WarehouseInfoDbContext>(options =>
    options.UseMySQL(WarehouseInfoDbConnectionString));

var BusanUmgungDbConnectionString = builder.Configuration.GetConnectionString("BusanUmgungDbConnection");
builder.Services.AddDbContext<BusanUmgungDbContext>(options =>
    options.UseMySQL(BusanUmgungDbConnectionString));
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new BusanUmgungProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
    //var InsertJobKey = new JobKey("InsertJob");
    //q.AddJob<InsertJob>(opts => opts.WithIdentity(InsertJobKey));
    //q.AddTrigger(opts => opts
    //    .ForJob(InsertJobKey)
    //    .WithIdentity("InsertJob-trigger")
    //    //This Cron interval can be described as "run every minute" (when second is zero)
    //    .WithCronSchedule("0 * * ? * *")
    //    .WithPriority(1)
    //    .StartNow().WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever()));

    //var ExcuteJobKey = new JobKey("ExcuteJobKey");
    //q.AddJob<ExcuteJob>(opts => opts.WithIdentity(ExcuteJobKey));
    //q.AddTrigger(opts => opts
    //    .ForJob(ExcuteJobKey)
    //    .WithIdentity("ExcuteJob-trigger")
    //    //This Cron interval can be described as "run every minute" (when second is zero)
    //    .WithCronSchedule("0 * * ? * *")
    //    .WithPriority(2)
    //    .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(3)).WithRepeatCount(20)));
     var CollectBusanUmgungInfoJobKey = new JobKey("CollectBusanUmgungInfoJobKey");
    q.AddJob<CollectBusanUmgongInfoJob>(opts => opts.WithIdentity(CollectBusanUmgungInfoJobKey));
    q.AddTrigger(opts => opts
        .ForJob(CollectBusanUmgungInfoJobKey)
        .WithIdentity("CollectBusanUmgungInfoJob-trigger")
        //This Cron interval can be described as "run every minute" (when second is zero)
        .WithCronSchedule("0 * * ? * *")
        .WithPriority(2)
        .StartNow()
        .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(3)).WithRepeatCount(1)));

    //var CollectWarehouseInfoJobKey = new JobKey("CollectWarehouseInfoJobKey");
    //q.AddJob<CollectWarehouseInfoJob>(opts => opts.WithIdentity(CollectWarehouseInfoJobKey));
    //q.AddTrigger(opts => opts
    //    .ForJob(CollectWarehouseInfoJobKey)
    //    .WithIdentity("CollectWarehouseInfoJob-trigger")
    //    //This Cron interval can be described as "run every minute" (when second is zero)
    //    .WithCronSchedule("0 * * ? * *")
    //    .WithPriority(2)
    //    .StartNow()
    //    .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(3)).WithRepeatCount(1)));
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
