using System;

namespace Demo.Aop.AspectCore.Web.Services
{
	public class TestService : ITestService
	{
		public void Call()
		{
			Console.WriteLine("TestCall");
		}
	}
}
