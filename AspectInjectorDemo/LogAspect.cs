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
