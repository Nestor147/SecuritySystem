using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Text.Json;

namespace SecuritySystem.Core.Interfaces.Core.SQLServer
{
    public enum ListHandling
    {
        AsJson,
        AsCsv,
        AsTvp
    }

    public static class SqlParamsADO
    {

        public static SqlParameter In(string name, object value, SqlDbType? type = null, int? size = null)
        {
            var p = new SqlParameter(Normalize(name), value ?? DBNull.Value)
            {
                Direction = ParameterDirection.Input
            };
            if (type.HasValue) p.SqlDbType = type.Value;
            if (size.HasValue) p.Size = size.Value;
            return p;
        }

        public static SqlParameter Out(string name, SqlDbType type, int? size = null)
        {
            var p = new SqlParameter(Normalize(name), type)
            {
                Direction = ParameterDirection.Output
            };
            if (size.HasValue) p.Size = size.Value;
            return p;
        }

        public static SqlParameter Return(string name = "@RETURN_VALUE")
            => new SqlParameter(Normalize(name), SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };

        public static SqlParameter Tvp(string name, string tvpTypeName, DataTable table)
            => new SqlParameter(Normalize(name), SqlDbType.Structured)
            {
                TypeName = tvpTypeName,
                Value = table ?? (object)DBNull.Value
            };

        public static T GetOut<T>(SqlParameter p)
        {
            if (p?.Value == null || p.Value == DBNull.Value) return default!;
            return (T)Convert.ChangeType(p.Value, typeof(T));
        }

        public static SqlParameter InFlexible(
            string name,
            object? value,
            SqlDbType? type = null,
            int? size = null,
            ListHandling listHandling = ListHandling.AsJson,
            string? tvpTypeName = null
        )
        {
            var paramName = Normalize(name);

            if (value is null)
                return BuildSimple(paramName, DBNull.Value, type, size);

            if (value is IEnumerable enumerable && value is not string && value is not byte[])
            {
                switch (listHandling)
                {
                    case ListHandling.AsJson:
                        {

                            string json = JsonSerializer.Serialize(enumerable);
                            var p = new SqlParameter(paramName, json);
                            if (type.HasValue) p.SqlDbType = type.Value;
                            if (size.HasValue) p.Size = size.Value;
                            return p;
                        }
                    case ListHandling.AsCsv:
                        {
                            var csv = string.Join(",", enumerable.Cast<object?>()
                                                .Select(v => v?.ToString()?.Trim())
                                                .Where(s => !string.IsNullOrWhiteSpace(s)));
                            var p = new SqlParameter(paramName, (object)(csv.Length == 0 ? DBNull.Value : csv));
                            if (type.HasValue) p.SqlDbType = type.Value;
                            if (size.HasValue) p.Size = size.Value;
                            return p;
                        }
                    case ListHandling.AsTvp:
                        {
                            if (string.IsNullOrWhiteSpace(tvpTypeName))
                                throw new ArgumentException("Para ListHandling.AsTvp debes indicar tvpTypeName (ej: dbo.NVarcharList).");

                            var table = BuildSingleColumnTable(enumerable);
                            return new SqlParameter(paramName, SqlDbType.Structured)
                            {
                                TypeName = tvpTypeName,
                                Value = table
                            };
                        }
                    default:
                        throw new NotSupportedException($"ListHandling '{listHandling}' no soportado.");
                }
            }

            return BuildSimple(paramName, value, type, size);
        }


        public static SqlParameter TvpFromList(
            string name,
            string tvpTypeName,
            IEnumerable values,
            string columnName = "Valor"
        )
        {
            var dt = new DataTable();
            dt.Columns.Add(columnName, typeof(string));
            foreach (var v in values)
            {
                if (v is null) continue;
                dt.Rows.Add(v.ToString());
            }

            return new SqlParameter(Normalize(name), SqlDbType.Structured)
            {
                TypeName = tvpTypeName,
                Value = dt
            };
        }

        public static SqlParameter[] ToSqlParameters(
            object? parameters,
            ListHandling defaultListHandling = ListHandling.AsJson,
            string? tvpTypeNameForLists = null
        )
        {
            if (parameters is null) return Array.Empty<SqlParameter>();

            if (parameters is IEnumerable<SqlParameter> ready)
                return ready is SqlParameter[] arr ? arr : ready.ToArray();

            if (parameters is IDictionary<string, object?> dict)
                return dict.Select(kv => InFlexible(kv.Key, kv.Value, listHandling: defaultListHandling, tvpTypeName: tvpTypeNameForLists)).ToArray();

            var props = parameters.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            return props.Select(p =>
            {
                var val = p.GetValue(parameters, null);
                return InFlexible(p.Name, val, listHandling: defaultListHandling, tvpTypeName: tvpTypeNameForLists);
            }).ToArray();
        }


        private static SqlParameter BuildSimple(string name, object value, SqlDbType? type, int? size)
        {
            var p = new SqlParameter(name, value);
            if (type.HasValue) p.SqlDbType = type.Value;
            if (size.HasValue) p.Size = size.Value;
            p.Direction = ParameterDirection.Input;
            return p;
        }

        private static string Normalize(string name) => name.StartsWith("@") ? name : "@" + name;

        private static DataTable BuildSingleColumnTable(IEnumerable values, string columnName = "Valor")
        {
            var dt = new DataTable();
            dt.Columns.Add(columnName, typeof(string));
            foreach (var v in values)
            {
                if (v is null) continue;
                dt.Rows.Add(v.ToString());
            }
            return dt;
        }
    }
    }
