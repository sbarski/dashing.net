using System;
using System.Collections.Generic;
using System.Dynamic;


namespace dashing.net.Infrastructure
{
    public static class JsonHelper
    {
        public static dynamic Merge(object item1, object item2)
        {
            if (item1 == null || item2 == null)
            {
                return item1 ?? item2 ?? new ExpandoObject();
            }

            dynamic expando = new ExpandoObject();

            var result = expando as IDictionary<string, object>;

            foreach (System.Reflection.PropertyInfo fi in item1.GetType().GetProperties())
            {
                result[fi.Name] = fi.GetValue(item1, null);
            }

            foreach (System.Reflection.PropertyInfo fi in item2.GetType().GetProperties())
            {
                result[fi.Name] = fi.GetValue(item2, null);
            }

            return result;
        }

        public static dynamic MergeDynamic(object item1, dynamic item2)
        {
            return item2;
        }
    }
}