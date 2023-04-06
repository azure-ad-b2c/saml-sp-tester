using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SAMLTEST.Models;

var builder = WebApplication.CreateBuilder(args);
// Configure JSON logging to the console.
builder.Logging.AddJsonConsole();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.Configure<AzureAdB2C>(builder.Configuration.GetSection(AzureAdB2C.ConfigurationName));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Use Developer page error handling for development.
    app.UseDeveloperExceptionPage();
    //app.UseBrowserLink();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    var options = new RewriteOptions().AddRedirectToHttpsPermanent();
    app.UseRewriter(options);
}

// Use Status Code error handling to our custom page.
app.UseStatusCodePagesWithRedirects("/Error?StatusCode={0}");
app.UseHttpsRedirection();
// For the wwwroot folder
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapRazorPages();
app.MapControllers();

app.Run();
