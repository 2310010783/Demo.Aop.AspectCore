using AspectCore.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace Demo.Aop.AspectCore
{
	public class TestInterceptorAttribute : AbstractInterceptorAttribute
	{
		public override Task Invoke(AspectContext context, AspectDelegate next)
		{
			Console.WriteLine("TestInvoke");
			return context.Invoke(next);
		}
	}
}
