using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Data;
using Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AutoRepairWorkshopConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Чтение строки подключения из appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DBConnection");

            // Регистрация сервисов
            builder.Services.AddDbContext<AutoRepairWorkshopContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<CachedDataService>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            app.UseSession();

            // Главная страница
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>Главная страница</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY>";
                    strResponse += "<BR><A href='/table'>Таблицы</A>";
                    strResponse += "<BR><A href='/info'>Информация</A>";
                    strResponse += "<BR><A href='/searchform1'>SearchForm1</A>";
                    strResponse += "<BR><A href='/searchform2'>SearchForm2</A>";
                    strResponse += "</BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                    return;
                }
                await next.Invoke();
            });

            // Маршрут для страницы информации
            app.Map("/info", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Информация:</H1>";
                    strResponse += "<BR> Сервер: " + context.Request.Host;
                    strResponse += "<BR> Путь: " + context.Request.Path;
                    strResponse += "<BR> Протокол: " + context.Request.Protocol;
                    strResponse += "<BR><A href='/'>На главную</A></BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                });
            });

            // Конфигурация маршрутов для таблиц
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // Главная страница таблиц
                endpoints.MapGet("/table", async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>Таблицы</TITLE></HEAD>" +
                     "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                     "<BODY>";
                    strResponse += "<BR><A href='/table/Cars'>Cars</A>";
                    strResponse += "<BR><A href='/table/CarServices'>Car Services</A>";
                    strResponse += "<BR><A href='/table/CarStatuses'>Car Statuses</A>";
                    strResponse += "<BR><A href='/table/Mechanics'>Mechanics</A>";
                    strResponse += "<BR><A href='/table/Owners'>Owners</A>";
                    strResponse += "<BR><A href='/table/Payments'>Payments</A>";
                    strResponse += "<BR><A href='/table/RepairOrders'>Repair Orders</A>";
                    strResponse += "<BR><A href='/table/Services'>Services</A>";
                    strResponse += "</BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                });

                // Маршруты для динамической обработки таблиц
                endpoints.MapGet("/table/{tableName}", async context =>
                {
                    var tableName = (string)context.Request.RouteValues["tableName"];
                    var cachedService = context.RequestServices.GetService<CachedDataService>();

                    if (cachedService == null)
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Ошибка: CachedDataService не зарегистрирован.");
                        return;
                    }

                    try
                    {
                        if (tableName == "Cars")
                        {
                            var list = cachedService.GetCars();
                            await RenderTable(context, list);
                        }
                        else if (tableName == "CarServices")
                        {
                            var list = cachedService.GetCarServices();
                            await RenderTable(context, list);
                        }
                        else if (tableName == "CarStatuses")
                        {
                            var list = cachedService.GetCarStatuses();
                            await RenderTable(context, list);
                        }
                        else if (tableName == "Mechanics")
                        {
                            var list = cachedService.GetMechanics();
                            await RenderTable(context, list);
                        }
                        else if (tableName == "Owners")
                        {
                            var list = cachedService.GetOwners();
                            await RenderTable(context, list);
                        }
                        else if (tableName == "Payments")
                        {
                            var list = cachedService.GetPayments();
                            await RenderTable(context, list);
                        }
                        else if (tableName == "RepairOrders")
                        {
                            var list = cachedService.GetRepairOrders();
                            await RenderTable(context, list);
                        }
                        else if (tableName == "Services")
                        {
                            var list = cachedService.GetServices();
                            await RenderTable(context, list);
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            await context.Response.WriteAsync("Таблица не найдена");
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync($"Ошибка при получении данных: {ex.Message}");
                    }
                });
            });

            // Обработка searchform1 с использованием куки
            app.Map("/searchform1", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var dbContext = context.RequestServices.GetService<AutoRepairWorkshopContext>();

                    if (context.Request.Method == "GET")
                    {
                        var registrationNumber = context.Request.Cookies["RegistrationNumber"] ?? "";
                        var selectedStatusId = context.Request.Cookies["StatusId"] ?? "";
                        var selectedBrands = context.Request.Cookies["Brands"] ?? "";

                        var statuses = await dbContext.CarStatuses.ToListAsync();
                        var brands = await dbContext.Cars.Select(c => c.Brand).Distinct().ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Form 1</title></head><body>";
                        html += "<h1>Поиск машин (Куки)</h1>";
                        html += "<form method='post'>";

                        html += "<label for='RegistrationNumber'>Регистрационный номер:</label><br/>";
                        html += $"<input type='text' id='RegistrationNumber' name='RegistrationNumber' value='{registrationNumber}' /><br/><br/>";

                        html += "<label for='StatusId'>Статус:</label><br/>";
                        html += "<select id='StatusId' name='StatusId'>";
                        foreach (var status in statuses)
                        {
                            var selected = status.StatusId.ToString() == selectedStatusId ? "selected" : "";
                            html += $"<option value='{status.StatusId}' {selected}>{status.StatusName}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<label for='Brands'>Бренд машины:</label><br/>";
                        html += "<select id='Brands' name='Brands' multiple size='3'>";
                        var selectedBrandsArray = selectedBrands.Split(',');
                        foreach (var brand in brands)
                        {
                            var selected = selectedBrandsArray.Contains(brand) ? "selected" : "";
                            html += $"<option value='{brand}' {selected}>{brand}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<button type='submit'>Поиск</button>";
                        html += "</form>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                    else if (context.Request.Method == "POST")
                    {
                        var formData = await context.Request.ReadFormAsync();

                        var registrationNumber = formData["RegistrationNumber"];
                        var statusId = formData["StatusId"];
                        var brands = formData["Brands"];

                        context.Response.Cookies.Append("RegistrationNumber", registrationNumber);
                        context.Response.Cookies.Append("StatusId", statusId);
                        context.Response.Cookies.Append("Brands", string.Join(",", brands));

                        await context.Response.WriteAsync("<h1>Данные сохранены в куки.</h1>");
                    }
                });
            });

            // Обработка searchform2 с использованием сессии
            app.Map("/searchform2", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var dbContext = context.RequestServices.GetService<AutoRepairWorkshopContext>();

                    if (context.Request.Method == "GET")
                    {
                        var registrationNumber = context.Session.GetString("RegistrationNumber") ?? "";
                        var selectedStatusId = context.Session.GetString("StatusId") ?? "";
                        var selectedBrands = context.Session.GetString("Brands") ?? "";

                        var statuses = await dbContext.CarStatuses.ToListAsync();
                        var brands = await dbContext.Cars.Select(c => c.Brand).Distinct().ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Form 2</title></head><body>";
                        html += "<h1>Поиск машины (сессия)</h1>";
                        html += "<form method='post'>";

                        html += "<label for='RegistrationNumber'>Регистрационный номер:</label><br/>";
                        html += $"<input type='text' id='RegistrationNumber' name='RegistrationNumber' value='{registrationNumber}' /><br/><br/>";

                        html += "<label for='StatusId'>Статус:</label><br/>";
                        html += "<select id='StatusId' name='StatusId'>";
                        foreach (var status in statuses)
                        {
                            var selected = status.StatusId.ToString() == selectedStatusId ? "selected" : "";
                            html += $"<option value='{status.StatusId}' {selected}>{status.StatusName}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<label for='Brands'>Бренд машины:</label><br/>";
                        html += "<select id='Brands' name='Brands' multiple size='3'>";
                        var selectedBrandsArray = selectedBrands.Split(',');
                        foreach (var brand in brands)
                        {
                            var selected = selectedBrandsArray.Contains(brand) ? "selected" : "";
                            html += $"<option value='{brand}' {selected}>{brand}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<button type='submit'>Поиск</button>";
                        html += "</form>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                    else if (context.Request.Method == "POST")
                    {
                        var formData = await context.Request.ReadFormAsync();

                        var registrationNumber = formData["RegistrationNumber"];
                        var statusId = formData["StatusId"];
                        var brands = formData["Brands"];

                        context.Session.SetString("RegistrationNumber", registrationNumber);
                        context.Session.SetString("StatusId", statusId);
                        context.Session.SetString("Brands", string.Join(",", brands));

                        await context.Response.WriteAsync("<h1>Данные сохранены в сессию.</h1>");
                    }
                });
            });

            app.Run();
        }

        // Метод для рендеринга таблиц
        // Метод для рендеринга таблиц
        public static async Task RenderTable<T>(HttpContext context, IEnumerable<T> data)
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            var html = "<table border='1' style='border-collapse:collapse'>";

            var type = typeof(T);

            // Генерация заголовков таблицы на основе свойств типа
            html += "<tr>";
            foreach (var prop in type.GetProperties())
            {
                if (!IsSimpleType(prop.PropertyType)) continue;
                html += $"<th>{prop.Name}</th>";
            }
            html += "</tr>";

            // Генерация строк данных
            foreach (var item in data)
            {
                html += "<tr>";
                foreach (var prop in type.GetProperties())
                {
                    if (!IsSimpleType(prop.PropertyType)) continue;
                    var value = prop.GetValue(item);
                    if (value is DateTime dateValue)
                    {
                        html += $"<td>{dateValue.ToString("dd.MM.yyyy")}</td>";
                    }
                    else
                    {
                        html += $"<td>{value}</td>";
                    }
                }
                html += "</tr>";
            }

            html += "</table>";

            // Добавление ссылки "На главную" в конце таблицы
            html += "<br/><a href='/'>На главную</a>";

            await context.Response.WriteAsync(html);
        }


        // Проверка простых типов
        public static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive || type.IsValueType || type == typeof(string) || type == typeof(DateTime) || type == typeof(decimal);
        }
    }
}
