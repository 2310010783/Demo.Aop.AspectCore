namespace Demo.Aop.AspectCore.Web.Services
{
	public interface ITestService
	{
		[TestInterceptor]
		void Call();
	}
}
