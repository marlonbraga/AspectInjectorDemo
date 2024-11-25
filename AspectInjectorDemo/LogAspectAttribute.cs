using AspectInjector.Broker;

namespace AspectInjectorDemo
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    [Injection(typeof(LogAspect))]
    public class AutoLog : Attribute
    {
    }
}
