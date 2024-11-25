# AspectInjectorDemo
This is a simple demo project to show how to use AspectInjector in a .NET Core project.
## Steps to reproduce
### Step 1: Set Up the WeatherForecast API Project
Let's start by creating a new .NET 8 Web API.
Create a new Web API project:
```bash
dotnet new webapi -n AspectInjectorDemo
cd AspectInjectorDemo
```
Run the project to verify it works correctly:
```bash
dotnet run
```
Ensure the endpoint `https://localhost:5001/WeatherForecast` is working.
### Step 2: Add AspectInjector
Install the AspectInjector package version 2.8.2.
[Nuget Package](https://www.nuget.org/packages/AspectInjector "AspectInjector package version 2.8.2")
Install AspectInjector:
```bash
dotnet add package AspectInjector --version 2.8.2
```
Confirm the package has been added by checking your `.csproj` file.
### Step 3: Create a Logging Aspect to Apply Globally
To ensure that every method within the project logs its entry and exit, you'll create an aspect that works without requiring attributes on each method or controller.
Create a folder named `Aspects`.
Add a new file named `LogAspect.cs` inside the `Aspects` folder with the following code:
```csharp
using AspectInjector.Broker;
namespace AspectInjectorDemo
{
    [Aspect(Scope.Global)]
    public class LogAspect
    {
        [Advice(Kind.Before, Targets = Target.Method)]
        public void LogEnter([Argument(Source.Name)] string methodName, [Argument(Source.Type)] Type classType)
        {
            Console.WriteLine($"[DEBUG] Enter: {classType.Name}.{methodName}");
        }
        [Advice(Kind.After, Targets = Target.Method)]
        public void LogExit([Argument(Source.Name)] string methodName, [Argument(Source.Type)] Type classType)
        {
            Console.WriteLine($"[DEBUG] Exit: {classType.Name}.{methodName}");
        }
        [Advice(Kind.Around, Targets = Target.Method)]
        public object LogExceptions([Argument(Source.Name)] string methodName, [Argument(Source.Type)] Type classType, [Argument(Source.Target)] Func<object[], object> methodDelegate, [Argument(Source.Arguments)] object[] arguments)
        {
            try
            {
                var result = methodDelegate(arguments);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception in method: {classType.Name}.{methodName} at {DateTime.Now}");
                Console.WriteLine($"[ERROR] Exception message: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
```
Explanation:
`[Aspect(Scope.Global)]`: By setting the scope to `Global`, this aspect will be applied throughout the entire application. This is a key difference because it tells AspectInjector to target every method globally, without the need for individual attributes.
`Advice(Kind.Before, Targets = Target.Method)`: Injects behavior before any method execution.
`Advice(Kind.After, Targets = Target.Method)`: Injects behavior after any method execution.
### Step 4: Create an atribute
```csharp
using AspectInjector.Broker;
namespace AspectInjectorDemo
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    [Injection(typeof(LogAspect))]
    public class AutoLog : Attribute
    {
    }
}
```
### Step 5: Using Attribute to methods, class or assembly
If you want to log only a specific class, you can use the attribute in the class or in a method. Like this:
```csharp
[ApiController]
[Route("[controller]")]
[AutoLog] // Add this line for log all methods in this class
public class WeatherForecastController : ControllerBase
{
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        ...
    }
}
```
```csharp
public class WeatherForecastService
{
    private string[] Summaries = new[]
    {
        ...
    };
    
    [AutoLog] // Add this line ofr log just this method
    public IEnumerable<WeatherForecast> Get()
    {
        ...
    }
}
```
If you want to log all methods in the project, you can use the attribute in the AssemblyInfo file.
Create a AssemblyInfo file in `Properties > AssemblyInfo.cs`
```csharp
using AspectInjectorDemo;
[assembly: AutoLog]
```
With this, all methods in the project will be logged.
### Step 6: Verify the Behavior
Since this aspect is global, every method call in your entire project will now have logging automatically injected.
Test the Application:
Run the Application:
```bash
dotnet run
```
Access the WeatherForecast Endpoint:
Go to `https://localhost:5001/WeatherForecast` in your browser.
Observe the Console Output:
You should see something like:
```
[DEBUG] Enter: WeatherForecastController.Get
[DEBUG] Enter: WeatherForecastService.Get
[DEBUG] Exit: WeatherForecastService.Get
[DEBUG] Exit: WeatherForecastController.Get
```
This behavior will automatically apply to every method across all classes in your application.
### Step 7: Additional Considerations
Target Specific Classes or Methods (Optional)
You may find that logging every method entry and exit is too verbose. In that case, you can adjust the targeting by changing the `[Aspect(Scope.Global)]` to other scopes such as `Class` or specifying it for particular namespaces or assemblies.
To make this more efficient or filter based on specific methods/classes, you could create a logic filter inside the `LogAspect` using conditions like method name patterns, namespaces, etc.
### Summary of Changes
Install AspectInjector Version 2.8.2.
Create LogAspect with Global Scope:
The global scope allows you to apply cross-cutting concerns without any additional attributes.
Automatic Logging for Every Method:
With the above aspect setup, all method entries and exits are logged without requiring explicit application to each method.
### Conclusion
By configuring AspectInjector with a Global scope, you've effectively added logging functionality to every method throughout your entire project without having to modify individual methods or controllers with attributes. This is particularly useful for applications where you need to ensure consistent logging across all parts of the codebase, without the overhead of explicitly marking each method.
If you need to refine the approach to target only specific assemblies, namespaces, or class types, AspectInjector's flexible advice and targeting mechanisms can help further refine this behavior.
