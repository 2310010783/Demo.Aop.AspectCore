using System;

namespace Demo.Aop.AspectCore.Web.Services
{
	public class DemoService
	{
		public virtual void Call()
		{
			Console.WriteLine("DemoCall");
		}
	}
}
