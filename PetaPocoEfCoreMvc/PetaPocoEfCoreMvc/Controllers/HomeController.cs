using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetaPocoEfCoreMvc.Models;

namespace PetaPocoEfCoreMvc.Controllers
{
    using System.Text;

    using AutoMapper;

    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Logging;

    using PetaPocoEfCoreMvc.BogusResp;
    using PetaPocoEfCoreMvc.Profiles.DTOs;
    using PetaPocoEfCoreMvc.Service;

    public class HomeController : Controller
    {
        private IUserService _userService;

        private ISampleCustomerRepository _customerRepository;

        private ILogger<HomeController> _logger;

        private IMapper _mapper;

        private IDistributedCache _distributedCache;

        public HomeController(IUserService userService,ISampleCustomerRepository customerRepository,ILogger<HomeController> logger,IMapper mapper,IDistributedCache distributedCache)
        {
            _userService = userService;
            _customerRepository = customerRepository;
            _logger = logger;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }
        public IActionResult Index()
        {
            var cs = _customerRepository.GetUsers();

            _distributedCache.Set("mzy190505", Encoding.UTF8.GetBytes("sdriewf"));
            _distributedCache.SetString("mzy190505001", Guid.NewGuid().ToString());
            
            //var x = _userService.GetAll();
            return View();
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
