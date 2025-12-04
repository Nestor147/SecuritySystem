using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Core.Interfaces.Core.SQLServer.ADO
{
    public class RowEntity : DynamicObject
    {
        private readonly Dictionary<string, object?> _values;

        public RowEntity(IReadOnlyDictionary<string, object?> values)
        {
            _values = new Dictionary<string, object?>(values, StringComparer.OrdinalIgnoreCase);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
            => _values.TryGetValue(binder.Name, out result);

        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            _values[binder.Name] = value;
            return true;
        }

        public object? this[string name]
            => _values.TryGetValue(name, out var v) ? v : null;

        public T Get<T>(string name)
        {
            if (!_values.TryGetValue(name, out var v) || v is null || v is DBNull) return default!;
            var target = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            return (T)Convert.ChangeType(v, target);
        }

        public bool Try<T>(string name, out T value)
        {
            value = default!;
            if (!_values.TryGetValue(name, out var v) || v is null || v is DBNull) return false;
            var target = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            value = (T)Convert.ChangeType(v, target);
            return true;
        }
        public T To<T>() where T : new()
        {
            var obj = new T();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (_values.TryGetValue(p.Name, out var v) && v is not null && v is not DBNull)
                {
                    var target = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    p.SetValue(obj, Convert.ChangeType(v, target));
                }
            }
            return obj;
        }

        public IReadOnlyDictionary<string, object?> AsDictionary() => _values;
    }
}
