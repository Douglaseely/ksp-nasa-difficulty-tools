using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace KspIntegration.Reflection;

public sealed class ReflectionObjectReader
{
    private readonly object _instance;

    public ReflectionObjectReader(object instance)
    {
        _instance = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    public string GetRequiredString(params string[] memberNames)
    {
        var value = GetRequiredValue(memberNames);
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    public double GetRequiredDouble(params string[] memberNames)
    {
        var value = GetRequiredValue(memberNames);
        return Convert.ToDouble(value, CultureInfo.InvariantCulture);
    }

    public int GetRequiredInt32(params string[] memberNames)
    {
        var value = GetRequiredValue(memberNames);
        return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    public bool GetRequiredBoolean(params string[] memberNames)
    {
        var value = GetRequiredValue(memberNames);
        return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
    }

    public string GetOptionalString(string defaultValue, params string[] memberNames)
    {
        return TryGetValue(out var value, memberNames)
            ? Convert.ToString(value, CultureInfo.InvariantCulture) ?? defaultValue
            : defaultValue;
    }

    public double GetOptionalDouble(double defaultValue, params string[] memberNames)
    {
        return TryGetValue(out var value, memberNames)
            ? Convert.ToDouble(value, CultureInfo.InvariantCulture)
            : defaultValue;
    }

    public int GetOptionalInt32(int defaultValue, params string[] memberNames)
    {
        return TryGetValue(out var value, memberNames)
            ? Convert.ToInt32(value, CultureInfo.InvariantCulture)
            : defaultValue;
    }

    public bool GetOptionalBoolean(bool defaultValue, params string[] memberNames)
    {
        return TryGetValue(out var value, memberNames)
            ? Convert.ToBoolean(value, CultureInfo.InvariantCulture)
            : defaultValue;
    }

    public object? GetOptionalObject(params string[] memberNames)
    {
        return TryGetValue(out var value, memberNames) ? value : null;
    }

    public IReadOnlyList<object> GetOptionalObjectList(params string[] memberNames)
    {
        if (!TryGetValue(out var value, memberNames) || value is null)
        {
            return Array.Empty<object>();
        }

        if (value is string)
        {
            return Array.Empty<object>();
        }

        if (value is IEnumerable enumerable)
        {
            var items = new List<object>();
            foreach (var item in enumerable)
            {
                if (item is not null)
                {
                    items.Add(item);
                }
            }

            return items;
        }

        return Array.Empty<object>();
    }

    private object GetRequiredValue(string[] memberNames)
    {
        if (TryGetValue(out var value, memberNames) && value is not null)
        {
            return value;
        }

        throw new MissingMemberException(_instance.GetType().FullName, string.Join("/", memberNames));
    }

    private bool TryGetValue(out object? value, string[] memberNames)
    {
        foreach (var memberName in memberNames)
        {
            if (TryGetValue(memberName, out value))
            {
                return true;
            }
        }

        value = null;
        return false;
    }

    private bool TryGetValue(string memberName, out object? value)
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var instanceType = _instance.GetType();

        var property = instanceType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(candidate => string.Equals(candidate.Name, memberName, comparison));

        if (property is not null)
        {
            value = property.GetValue(_instance, null);
            return true;
        }

        var field = instanceType
            .GetFields(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(candidate => string.Equals(candidate.Name, memberName, comparison));

        if (field is not null)
        {
            value = field.GetValue(_instance);
            return true;
        }

        value = null;
        return false;
    }
}