using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Aop.AspectCore.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Aop.AspectCore.Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HomeController : ControllerBase
	{
		private readonly ITestService _testService;
		private readonly DemoService _demoService;

		public HomeController(ITestService testService, DemoService demoService)
		{
			_testService = testService;
			_demoService = demoService;
		}

		public string Get()
		{
			_testService.Call();
			_demoService.Call();
			return DateTime.Now.ToString();
		}
	}
}