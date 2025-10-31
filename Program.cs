using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiApp;
using MyApiApp.Abstractions;
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

builder.Services.AddSingleton<EmployeeRepository>();
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


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


EmployeesRout.MapGet(string.Empty, ([FromServices] EmployeeRepository repository) =>
{
    return Results.Ok(repository.GetAll().Select(employee => new GetEmployeeResponse
    {
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email
    }));
});

EmployeesRout.MapGet("{id:int}", ([FromRoute] int id, [FromServices] EmployeeRepository repository) =>
{
    var employee = repository.GetById(id);
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

EmployeesRout.MapPost(string.Empty, async ([FromBody] CreateEmployeeRequest employeeRequest, [FromServices] EmployeeRepository repository, IValidator<CreateEmployeeRequest> validator) =>
{
    // var validationProblems = new List<ValidationResult>();
    // var isValid = Validator.TryValidateObject(employeeRequest, new ValidationContext(employeeRequest), validationProblems, true);
    // if (!isValid)
    // {
    //     return Results.BadRequest(validationProblems.ToValidationProblemDetails());
    // }
    var validationResults = await validator.ValidateAsync(employeeRequest);
    if (!validationResults.IsValid)
    {
        return Results.ValidationProblem(validationResults.ToDictionary());
    }

    var employee = new Employee
    {
        FirstName = employeeRequest.FirstName!,
        LastName = employeeRequest.LastName!,
        Address1 = employeeRequest.Address1,
        Address2 = employeeRequest.Address2,
        City = employeeRequest.City,
        State = employeeRequest.State,
        ZipCode = employeeRequest.ZipCode,
        PhoneNumber = employeeRequest.PhoneNumber,
        Email = employeeRequest.Email
    };

    repository.Create(employee);
    return Results.Ok(employee);
});

EmployeesRout.MapPut("{id:int}", ([FromBody] UpdateEmployeeRequest employee, [FromRoute] int id, [FromServices] EmployeeRepository repository) =>
{
    var existingEmployee = repository.GetById(id);
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
