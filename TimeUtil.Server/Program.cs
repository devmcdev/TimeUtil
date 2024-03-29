var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();

if (!app.Environment.IsDevelopment()) // response compression currently conflicts with dotnet watch browser reload
{
    app.UseResponseCompression();
}

app.UseStaticFiles();

app.UseRouting();


app.MapFallbackToFile("index.html");

app.Run();
