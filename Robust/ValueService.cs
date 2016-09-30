using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robust
{
    public static class ValueService
    {
        private static Dictionary<int, Func<FieldValue, object>> getterCache = new Dictionary<int, Func<FieldValue, object>>();

        public static object GetValue(FieldValue fieldValue)
        {
            var field = fieldValue.Field;
            if (getterCache.ContainsKey(field.FieldTypeID))
                return getterCache[field.FieldTypeID](fieldValue);
            
            // get the "top-level" field type
            var type = field.FieldType;
            while (type.ParentFieldTypeID != null)
                type = type.ParentFieldType;

            var getter = GetTypeGetter(type.ID);
            getterCache.Add(field.FieldTypeID, getter);
            return getter(fieldValue);
        }

        private const int Boolean = 1, Date = 2, Decimal = 3, ForeignKey = 4, FreeText = 5, Integer = 6, Text = 7;

        private static Func<FieldValue, object> GetTypeGetter(int rootFieldTypeID)
        {
            switch (rootFieldTypeID)
            {
                case Boolean:
                    return GetBoolean;
                case Date:
                    return GetDate;
                case Decimal:
                    return GetDecimal;
                case ForeignKey:
                    return GetForeignKey;
                case FreeText:
                    return GetFreeText;
                case Integer:
                    return GetInteger;
                case Text:
                    return GetText;
                default:
                    throw new NotImplementedException("You need to specify how to access field values stored as the type with ID " + rootFieldTypeID);
            }
        }

        private static object GetBoolean(FieldValue fieldValue)
        {
            var actualValue = fieldValue.BitValue;
            if (actualValue == null)
                return null;
            return actualValue.Value;
        }

        private static object GetDate(FieldValue fieldValue)
        {
            var actualValue = fieldValue.DateValue;
            if (actualValue == null)
                return null;
            return actualValue.Value;
        }

        private static object GetDecimal(FieldValue fieldValue)
        {
            var actualValue = fieldValue.DecimalValue;
            if (actualValue == null)
                return null;
            return actualValue.Value;
        }

        private static object GetForeignKey(FieldValue fieldValue)
        {
            var actualValue = fieldValue.ForeignKeyValue;
            if (actualValue == null)
                return null;
            return actualValue.ValueEntity;
        }

        private static object GetFreeText(FieldValue fieldValue)
        {
            var actualValue = fieldValue.FreeTextValue;
            if (actualValue == null)
                return null;
            return actualValue.Value;
        }

        private static object GetInteger(FieldValue fieldValue)
        {
            var actualValue = fieldValue.IntValue;
            if (actualValue == null)
                return null;
            return actualValue.Value;
        }

        private static object GetText(FieldValue fieldValue)
        {
            var actualValue = fieldValue.TextValue;
            if (actualValue == null)
                return null;
            return actualValue.Value;
        }
    }
}
