using MFQ2022;
using Microsoft.EntityFrameworkCore;
using Quartz;
using TestApp.Data;
using TestApp.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<MFQService>();
builder.Services.AddSingleton<InsertJob>();
builder.Services.AddSingleton<ExcuteJob>();
builder.Services.AddSingleton<TimeCheckJob>();
var ProcessDbConnectionString = builder.Configuration.GetConnectionString("ProcessDbConnection");
builder.Services.AddDbContext<ProcessDbContext>(options =>
    options.UseMySQL(ProcessDbConnectionString));

// �����ٷ��� ����մϴ�.
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
    // Just use the name of your job that you created in the Jobs folder.
    //var TimeJobKey = new JobKey("TimeJobKey");
    //q.AddJob<TimeCheckJob>(opts => opts.WithIdentity(TimeJobKey));
    //q.AddTrigger(opts => opts
    //    .ForJob(TimeJobKey)
    //    .WithIdentity("TimeJobKey-trigger")
    //    //This Cron interval can be described as "run every minute" (when second is zero)
    //    .WithCronSchedule("0 * * ? * *")
    //    .WithPriority(1)
    //    .StartNow().WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever()));
    var InsertJobKey = new JobKey("InsertJob");
    q.AddJob<InsertJob>(opts => opts.WithIdentity(InsertJobKey));
    q.AddTrigger(opts => opts
        .ForJob(InsertJobKey)
        .WithIdentity("InsertJob-trigger")
        //This Cron interval can be described as "run every minute" (when second is zero)
        .WithCronSchedule("0 * * ? * *")
        .WithPriority(2)
        .StartNow().WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever()));

    var ExcuteJobKey = new JobKey("ExcuteJobKey");
    q.AddJob<ExcuteJob>(opts => opts.WithIdentity(ExcuteJobKey));
    q.AddTrigger(opts => opts
        .ForJob(ExcuteJobKey)
        .WithIdentity("ExcuteJob-trigger")
        //This Cron interval can be described as "run every minute" (when second is zero)
        .WithCronSchedule("0 * * ? * *")
        .WithPriority(3)
        .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(3)).WithRepeatCount(20)));
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
