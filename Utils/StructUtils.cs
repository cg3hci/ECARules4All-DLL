using System;
using System.Collections;

public static class StructUtils
{
    public static bool AreStructsEqual(object obj1, object obj2)
    {
        // Handle nulls
        if (obj1 == null && obj2 == null) return true;
        if (obj1 == null || obj2 == null) return false;

        // Check for same type
        if (obj1.GetType() != obj2.GetType()) return false;

        // Handle primitive types or strings
        if (obj1 is IComparable comparableObj1 && obj2 is IComparable comparableObj2)
        {
            return comparableObj1.CompareTo(comparableObj2) == 0;
        }

        // Handle collections
        if (obj1 is IEnumerable enumerableObj1 && obj2 is IEnumerable enumerableObj2)
        {
            var enumerator1 = enumerableObj1.GetEnumerator();
            var enumerator2 = enumerableObj2.GetEnumerator();

            while (enumerator1.MoveNext())
            {
                if (!enumerator2.MoveNext() || !AreStructsEqual(enumerator1.Current, enumerator2.Current))
                    return false;
            }

            return !enumerator2.MoveNext();
        }

        // Handle structs or complex types by comparing each field or property
        var fields = obj1.GetType().GetFields();
        foreach (var field in fields)
        {
            var value1 = field.GetValue(obj1);
            var value2 = field.GetValue(obj2);
            if (!AreStructsEqual(value1, value2)) return false;
        }

        var properties = obj1.GetType().GetProperties();
        foreach (var property in properties)
        {
            // Skip indexed properties
            if (property.GetIndexParameters().Length > 0) continue;

            var value1 = property.GetValue(obj1);
            var value2 = property.GetValue(obj2);
            if (!AreStructsEqual(value1, value2)) return false;
        }

        return true;
    }
}