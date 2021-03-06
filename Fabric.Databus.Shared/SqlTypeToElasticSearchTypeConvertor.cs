﻿namespace Fabric.Databus.Shared
{
    using System;

    public static class SqlTypeToElasticSearchTypeConvertor
    {
        public static string GetElasticSearchType(Type type)
        {
            if (type == typeof(string)) return "keyword"; //TODO; use text or keyword

            if (type == typeof(int)) return "integer";

            if (type == typeof(DateTime)) return "date";

            if (type == typeof(Decimal)) return "double";

            if (type == typeof(double)) return "double";

            if (type == typeof(Int64)) return "integer";

            if (type == typeof(Single)) return "integer";

            if (type == typeof(Boolean)) return "boolean";
            if (type == typeof(Int16)) return "short";
            if (type == typeof(Guid)) return "keyword";

            throw new NotImplementedException("No Elastic Search type found for type=" + type);
        }
    }
}
