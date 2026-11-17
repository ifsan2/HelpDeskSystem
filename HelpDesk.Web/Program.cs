using HelpDesk.DAL.Extensions;
using HelpDesk.Web.Components;
using Microsoft.AspNetCore.Identity;
using HelpDesk.BLL.Extensions;
using HelpDesk.BLL.Interfaces.Services;
using HelpDesk.BLL.Services;
using HelpDesk.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

// Register DAL services (Database + Identity)
builder.Services.AddDataAccess(builder.Configuration);

builder.Services.AddScoped<ITicketCommentService, TicketCommentService>();

// Register BLL services
builder.Services.AddBusinessLogic();

// Authentication and authorization
builder.Services.AddAuthentication(
    Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

await IdentitySeeder.SeedAdminAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();