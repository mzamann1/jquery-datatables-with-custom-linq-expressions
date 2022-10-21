using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using LINQExtensions.Models.ViewModels;
using LINQExtensions.Models;
using LINQExtensions.Interfaces;
using LINQExtensions.Models.ViewModels.JQueryDatatables;

namespace LINQExtensions.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetUsers([FromForm] DtRequestModel dt)
        {
            var userQuery = await Task.Run(() => _userService.GetUsers().AsQueryable());

            if (userQuery == null)
            {

            }

            int recordsTotal = userQuery.Count();

            userQuery = _userService.GetFilteredData(userQuery, dt);


            int filteredRecords = userQuery.Count();

            userQuery = _userService.GetOrderedData(userQuery, dt);


            userQuery = _userService.GetPaginatedData(userQuery, dt);


            var userList = userQuery.ToList();

            var result = new DtResponseModel<User>()
            {
                Draw = dt.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = filteredRecords,
                Data = userList,
                Error = ""
            };
            return Ok(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}