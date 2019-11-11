# ASP.NET Core 3.0 使用AspectCore-Framework实现AOP
AspectCore是适用于Asp.Net Core 平台的轻量级Aop(Aspect-oriented programming)解决方案，它更好的遵循Asp.Net Core的模块化开发理念，使用AspectCore可以更容易构建低耦合、易扩展的Web应用程序。
 
在使用过程中，由于相关文档、博客还未更新到.Net Core 3.0,本文操作参考了使用.Net Core 3.0的EasyCaching，并对其中公用的方法进行封装简化。
### 安装Aspectcore
此处配合微软自家的DI实现，安装Nuget包AspectCore.Extensions.DependencyInjection，其中包含AspectCore.Core和Microsoft.Extensions.DependencyInjection两个依赖。
``` shell
Install-Package AspectCore.Extensions.DependencyInjection -Version 1.3.0
```

### 拦截器
-  特性拦截器
新建一个特性拦截器TestInterceptorAttribute，继承AbstractInterceptorAttribute，并重写Invoke方法，在方法中实现拦截相关业务。
```C#
public class TestInterceptorAttribute : AbstractInterceptorAttribute
{
    public override Task Invoke(AspectContext context, AspectDelegate next)
    {
        return context.Invoke(next);
    }
}
```

- 全局拦截器
新建一个全局拦截器TestInterceptor，继承AbstractInterceptor，并重写Invoke方法，在方法中实现拦截相关业务。
```C#
public class TestInterceptor : AbstractInterceptor
{
    public override Task Invoke(AspectContext context, AspectDelegate next)
    {
        return context.Invoke(next);
    }
}
```

### 注册服务
以下注册方式仅适用于asp.net core 3.0(目前只到3.0)，已知在2.2版本中，需要在ConfigureServices方法中返回IServiceProvider，并且program.cs中也不再需要替换ServiceProviderFactory。
1. 创建AspectCoreEctensions.cs扩展IServiceCollection
```C#
public static class AspectCoreExtensions
{
	public static void ConfigAspectCore(this IServiceCollection services)
	{
        services.ConfigureDynamicProxy(config =>
		{
            //TestInterceptor拦截器类
            //拦截代理所有Service结尾的类
			config.Interceptors.AddTyped<TestInterceptor>(Predicates.ForService("*Service"));
		});
		services.BuildAspectInjectorProvider();
	}
}
```
2. 在Startup.cs中注册服务
```C#
public void ConfigureServices(IServiceCollection services)
{   
    services.ConfigAspectCore();
}
```
3. 在Program.cs中替换ServiceProviderFactory
```C#
public static IHostBuilder CreateHostBuilder(string[] args) =>
	Host.CreateDefaultBuilder(args)
	.ConfigureWebHostDefaults(webBuilder =>
	{
		webBuilder.UseStartup<Startup>();
	}).UseServiceProviderFactory(new AspectCoreServiceProviderFactory());
```

### 被拦截方法编写
- 代理接口：在接口上标注Attribute
```C#
public interface ITestService
{
	[TestInterceptor]
	void Test();
}
```

- 代理类(方法)：在方法上标注Attribute，并且标注virtual

```C#
public class TestService
{
    [TestInterceptor]
    public virtual void Test()
    {
        //业务代码
    }
}
```

### 拦截器业务编写
-  执行被拦截方法
```C#
private async Task<object> RunAndGetReturn()
{
	await Context.Invoke(Next);
	return Context.IsAsync()
		? await Context.UnwrapAsyncReturnValue()
		: Context.ReturnValue;
}
```

-  拦截器中的依赖注入
```C#
[FromContainer]
private RedisClient RedisClient { get; set; }
```

-  获取被拦截方法的Attribute
```C#
private static readonly ConcurrentDictionary<MethodInfo, object[]>
					MethodAttributes = new ConcurrentDictionary<MethodInfo, object[]>();

public static T GetAttribute<T>(this AspectContext context) where T : Attribute
{
	MethodInfo method = context.ServiceMethod;
	var attributes = MethodAttributes.GetOrAdd(method, method.GetCustomAttributes(true));
	var attribute = attributes.FirstOrDefault(x => typeof(T).IsAssignableFrom(x.GetType()));
	if (attribute is T)
	{
		return (T)attribute;
	}
	return null;
}
```

-  获取被拦截方法返回值类型
```C#
public static Type GetReturnType(this AspectContext context)
{
	return context.IsAsync()
		? context.ServiceMethod.ReturnType.GetGenericArguments()First()
		: context.ServiceMethod.ReturnType;
}
```

-  处理拦截器返回结果
```C#
private static readonly ConcurrentDictionary<Type, MethodInfo>
				   TypeofTaskResultMethod = new ConcurrentDictionary<Type, MethodInfo>();
public object ResultFactory(this AspectContext context,object result)
{
	var returnType = context.GetReturnType();

    //异步方法返回Task<T>类型结果
	if (context.IsAsync())
	{
		return TypeofTaskResultMethod
				.GetOrAdd(returnType, t => typeof(Task)
				.GetMethods()
				.First(p => p.Name == "FromResult" && p.ContainsGenericParameters)
				.MakeGenericMethod(returnType))
				.Invoke(null, new object[] { result });
	}
	else
	{
		return result;
	}
}
```


#### 相关链接
- [GitHub:本文代码](https://github.com/2310010783/Demo.Aop.AspectCore)
- [GitHub:AspectCore-Framework](https://github.com/dotnetcore/AspectCore-Framework)