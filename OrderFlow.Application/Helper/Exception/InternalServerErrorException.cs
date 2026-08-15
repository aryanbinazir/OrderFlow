using OrderFlow.Application.Helper.Exception.Enums;

namespace OrderFlow.Application.Helper.Exception
{
    public class InternalServerErrorException(string message, _CriticalLevel? criticalLevel) : System.Exception(message)
    {
        public string ErrorMessage { get; } = message;
        public _CriticalLevel CriticalLevel { get; } = criticalLevel ?? _CriticalLevel.Five;
    }
}
