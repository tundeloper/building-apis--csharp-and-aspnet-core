using System;
using Microsoft.AspNetCore.Mvc;

namespace MyApiApp.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class BaseController : Controller
{
}
