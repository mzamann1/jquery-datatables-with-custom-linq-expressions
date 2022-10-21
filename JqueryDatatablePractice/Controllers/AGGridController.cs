using JqueryDatatablePractice.Models.ViewModels;
using JqueryDatatablePractice.Models;
using Microsoft.AspNetCore.Mvc;
using LINQExtensions.Interfaces;

namespace LINQExtensions.Controllers;

[Route("ag-grid")]
public class AGGridController : Controller
{
    private readonly IAGGridService _agGridService;

    public AGGridController(IAGGridService _agGridService)
    {
        this._agGridService = _agGridService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet, Route("get-users")]
    public async Task<IActionResult> FetchUsers()
    {
        var userQuery = await Task.Run(() => _agGridService.GetUsers().AsQueryable());

        return Ok(userQuery.ToList());
    }
}

