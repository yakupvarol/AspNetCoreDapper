using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNetCoreDapper.Models;
using AspNetCoreDapper.DTO;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using AspNetCoreDapper.Business;
using System.Linq;

namespace AspNetCoreDapper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUserService _userService;

        public HomeController(IUserService userService, ILogger<HomeController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var Count = await _userService.CountAsync();
            var QueryGetAll= await _userService.GetAllAsync();
            var QueryGetID = await _userService.GetIDAsync(1);
            var QueryFind =  await _userService.FindAsync(new UserListRequestDTO { Groupid =1012 });
            var QueryOneToOne = await _userService.QueryOneToOneAsync(2);
            var QueryOneToMay = await _userService.QueryOneToManyAsync(1012);
            var QueryManyToOne = await _userService.QueryManyToOneAsync(1012);
            var QuerySlapperOneToOne = await _userService.QuerySlapperOneToOneAsync(2);
            var QuerySlapperOneToMany = await _userService.QuerySlapperOneToManyAsync(1012);
            var QuerySlapperManyToOne = await _userService.QuerySlapperManyToOneAsync();
            var QuerySlapperManyToMany = await _userService.QuerySlapperManyToManyAsync();

            return View(new HomeIndexViewModel {ListModel = QueryFind.ToList() });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> InsertAsync()
        {
            var rs= await _userService.InsertAsync(new UserDTO { FiRST_NAME="Eyüp", LAST_NAME="Ensar", EMAiL="eyupensar@gmail.com", PASSWORD="123", PHONE="05550000000", GROUPiD=1, USERTYPEiD=1});
            return View();
        }

        public async Task<IActionResult> InsertMultipleAsync()
        {
            var dt = new List<UserDTO>
            {
                new UserDTO { FiRST_NAME="Eyüp", LAST_NAME="Ensar", EMAiL="eyupensar@gmail.com", PASSWORD="123", PHONE="05550000000", GROUPiD=1, USERTYPEiD=1},
                new UserDTO { FiRST_NAME="Eyüp", LAST_NAME="Ensar", EMAiL="eyupensar@gmail.com", PASSWORD="123", PHONE="05550000000", GROUPiD=1, USERTYPEiD=1}
            };

            var rs = await _userService.InsertMultipleAsync(dt);
            return View();
        }

        public async Task<IActionResult> InsertOneToOneAsync()
        {
            var dt = new GroupOneToOneDTO
            {
                NAME = "One To One 1",
                USERS = new UserOneDTO { FiRST_NAME = "Eyüp One To One 1", LAST_NAME = "Ensar", EMAiL = "eyupensar@gmail.com", PASSWORD = "123", PHONE = "05550000000", GROUPiD = 1, USERTYPEiD = 1 },
            };
            var rs = await _userService.InsertOneToOneAsync(dt);
            return View();
        }

        public async Task<IActionResult> InsertOneToManyAsync()
        {
            var dt = new UserGroupDTO
            {
                NAME = "One To Many 2",
                USERS = new List<UserDTO>
                {
                    new UserDTO { FiRST_NAME="Eyüp One To Many 1", LAST_NAME="Ensar", EMAiL="eyupensar@gmail.com", PASSWORD="123", PHONE="05550000000", GROUPiD=1, USERTYPEiD=1},
                    new UserDTO { FiRST_NAME="Eyüp One To Many 2", LAST_NAME="Ensar", EMAiL="eyupensar@gmail.com", PASSWORD="123", PHONE="05550000000", GROUPiD=1, USERTYPEiD=1}
                }
            };
            var rs = await _userService.InsertOneToManyAsync(dt);
            return View();
        }
        
        public async Task<IActionResult> InsertManyToManyAsync()
        {
            var dt = new List<UserGroupDTO>
            {
                new UserGroupDTO { 
                    NAME="Many To Many 3", 
                    USERS= new List<UserDTO> 
                    {
                        new UserDTO { FiRST_NAME="Eyüp Many To Many 1", LAST_NAME="Ensar", EMAiL="eyupensar@gmail.com", PASSWORD="123", PHONE="05550000000", GROUPiD=1, USERTYPEiD=1},
                        new UserDTO { FiRST_NAME="Eyüp Many To Many 2", LAST_NAME="Ensar", EMAiL="eyupensar@gmail.com", PASSWORD="123", PHONE="05550000000", GROUPiD=1, USERTYPEiD=1}
                    } 
                },

                new UserGroupDTO {
                    NAME="Many To Many 4",
                    USERS= new List<UserDTO>
                    {
                        new UserDTO { FiRST_NAME="Eyüp Many To Many 3", LAST_NAME="Ensar", EMAiL="eyupensar@gmail.com", PASSWORD="123", PHONE="05550000000", USERTYPEiD=1},
                        new UserDTO { FiRST_NAME="Eyüp Many To Many 4", LAST_NAME="Ensar", EMAiL="eyupensar@gmail.com", PASSWORD="123", PHONE="05550000000", USERTYPEiD=1}
                    }
                }
            };

            var rs = await _userService.InsertManyToManyAsync(dt);
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UpdateAsync()
        {
            var rs = await _userService.UpdateAsync(new UserDTO { UseriD = 2019, FiRST_NAME="Test" });
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DeleteAsync()
        {
            var rs = await _userService.DeleteAsync(new UserDTO { UseriD= 2018 });
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
