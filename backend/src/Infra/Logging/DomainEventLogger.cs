using Domain.Common;
using System.Reflection;
using Serilog;
namespace Infra.Logging;
public static class DomainEventLogger
{
    public static void LogDomainEvent(DomainEvent domainEvent)
    {
        var eventType = domainEvent.GetType();
        var eventName = eventType.Name;
        var properties = eventType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertyValues = new List<object>();
        var propertyNames = new List<string>();
        foreach (var property in properties)
        {
            var value = property.GetValue(domainEvent);
            propertyNames.Add(property.Name);
            propertyValues.Add(value ?? "null");
        }
        var messageTemplate = $"Evento de Domínio: {eventName}";
        var parameters = new List<object> { eventName };
        for (int i = 0; i < propertyNames.Count; i++)
        {
            messageTemplate += $" - {propertyNames[i]}: {{{propertyNames[i]}}}";
            parameters.Add(propertyValues[i]);
        }
        Log.Information(messageTemplate, parameters.ToArray());
    }
}
