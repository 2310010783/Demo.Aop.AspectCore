using AspectCore.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace Demo.Aop.AspectCore
{
	public class DemoInterceptor : AbstractInterceptor
	{
		public override Task Invoke(AspectContext context, AspectDelegate next)
		{
			Console.WriteLine("DemoInvoke");
			return context.Invoke(next);
		}
	}
}
