using Hangfire;
using Serilog;
using SurveyBasket.Api;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHangfireDashboard("/jops", new DashboardOptions
{
    //Authorization = [
    //    new HangfireCustomBasicAuthenticationFilter
    //    {
    //        User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
    //        Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
    //    }
    //],
    DashboardTitle = "Survey Basket"
});

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

RecurringJob.AddOrUpdate("SendNewPollsNotification", () => notificationService.SendNewPollsNotification(null), Cron.Daily);

app.UseHangfireDashboard("/jops");

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();