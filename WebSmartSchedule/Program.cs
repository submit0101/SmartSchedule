var builder = WebApplication.CreateBuilder(args);

// 1. Поддержка Razor Pages
builder.Services.AddRazorPages();

// 2. Настройка HttpClient для запросов к твоему API
// ВНИМАНИЕ: Замени порт 5062 на реальный порт твоего API, если он другой!
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5062");
});

// 3. Простая Cookie-авторизация для сайта (без Identity и БД!)
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(12); // Сколько держать логин
    });

var app = builder.Build();

// Настройка конвейера
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

// ВАЖНО: Порядок имеет значение! Сначала аутентификация, потом авторизация
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();