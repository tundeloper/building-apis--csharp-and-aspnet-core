using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiApp.Data;
using MyApiApp.Employees;

var builder = WebApplication.CreateBuilder(args);
var employees = new List<Employee>
{
    new Employee {Id = 1, FirstName = "Babatunde", LastName = "Isiaka", SocialSequreNumber = "12345-443-5"},
    new Employee {Id = 2, FirstName = "Aleem", LastName = "Isiaka", SocialSequreNumber = "12345-443-55"}
};

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // <- from Swashbuckle

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
var EmployeesRout = app.MapGroup("employees");

app.Use(async (HttpContext context, RequestDelegate next) =>
{
    await next(context);
    return;

    // context.Response.Headers.Append("Content-Type", "Text/html");
    // await context.Response.WriteAsJsonAsync("<h1>Welcome Babatunde</h1>");
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();          // serve OpenAPI JSON at /swagger/v1/swagger.json
    app.UseSwaggerUI();        // serve interactive UI at /swagger
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )).ToArray();

    return forecast;
})
.WithName("GetWeatherForecast");

EmployeesRout.MapGet(String.Empty, () =>
{
    Results.Ok(employees.Select(employee => new GetEmployeeResponse
    {
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email,
    }));
    return employees;
});

EmployeesRout.MapGet("{id:int}", (int id) =>
{
    var employee = employees.SingleOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new GetEmployeeResponse
    {
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.State,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email,
    });
});

EmployeesRout.MapPost(string.Empty, ([FromBody] CreateEmployeeReuest employee) =>
{
    var newEmployee = new Employee
    {
        Id = employees.Max(e => e.Id) + 1,
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        SocialSequreNumber = employee.SocialSequreNumber,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email
    };
    // employee.Id = employees.Max(e => e.Id) + 1; // We're not using a database, so we need to manually assign an ID
    // employees.Add(employee);
    return Results.Created($"/employees/{newEmployee.Id}", employee);
});

EmployeesRout.MapPut("{id:int}", ([FromBody] UpdateEmployeeRequest employee, [FromRoute] int id) =>
{
    var existingEmployee = employees.SingleOrDefault(e => e.Id == id);
    if (existingEmployee == null)
    {
        return Results.NotFound();
    }
    // existingEmployee.FirstName = employee.FirstName;
    // existingEmployee.LastName = employee.LastName;
    existingEmployee.Address1 = employee.Address1;
    existingEmployee.Address2 = employee.Address2;
    existingEmployee.City = employee.City;
    existingEmployee.State = employee.State;
    existingEmployee.ZipCode = employee.ZipCode;
    existingEmployee.PhoneNumber = employee.PhoneNumber;
    existingEmployee.Email = employee.Email;
    return Results.Ok(existingEmployee);
});

app.MapGet("/api/hello", () => Results.Ok(new { message = "Hello World!" }))
   .WithName("GetHelloWorld");


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
