using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robust
{
    public static class Extensions
    {
        public static IEnumerable<Entity> OnlyCurrent(this IEnumerable<Entity> entities)
        {
            // equivalent to filtering only records in the CurrentEntities view
            return entities.Where(e => !e.DeletedOn.HasValue);
        }

        public static IEnumerable<FieldValue> OnlyCurrent(this IEnumerable<FieldValue> fieldValues, DataConnection connection)
        {
            return fieldValues.Where(fv => connection.Context.CurrentFieldValues.Any(cfv => cfv.ID == fv.ID));
        }
    }
}
