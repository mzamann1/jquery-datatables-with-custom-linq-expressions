using JqueryDatatablePractice.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using JqueryDatatablePractice.Constants;
using JqueryDatatablePractice.Interfaces;
using JqueryDatatablePractice.Models.ViewModels;

namespace JqueryDatatablePractice.Controllers
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
        public async Task<IActionResult> GetUsers([FromForm] DtRequest dt)
        {
            var userQuery = await Task.Run(() => _userService.GetUsers().AsQueryable());

            if (userQuery == null)
            {

            }

            int recordsTotal = userQuery.Count();

            if (!string.IsNullOrWhiteSpace(dt.Search.Value))
            {
                userQuery = _userService.GetFilteredData(userQuery, dt, dt.Search.Value);
            }

            int filteredRecords = userQuery.Count();


            if (dt.Order.Length > 0)
            {
                userQuery = _userService.GetOrderedData(userQuery, dt.Columns[dt.Order[0].Column].Name,
                    dt.Order[0].Dir == "asc" ? OrderByType.Ascending : OrderByType.Descending);
            }

            userQuery = _userService.GetPaginatedData(userQuery, dt.Start, dt.Length);


            var userList = userQuery.ToList();

            var result = new DtResponse<User>()
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