using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Demo.Aop.AspectCore.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Aop.AspectCore.Web.Extensions
{
	public static class AspectCoreExtensions
	{
		public static void ConfigAspectCore(this IServiceCollection services)
		{
			services.ConfigureDynamicProxy(config =>
			{
				config.Interceptors.AddTyped<TestInterceptorAttribute>(Predicates.Implement(typeof(ITestService)));
				config.Interceptors.AddTyped<DemoInterceptor>(Predicates.Implement(typeof(DemoService)));
			});
			services.BuildAspectInjectorProvider();
		}
	}
}
