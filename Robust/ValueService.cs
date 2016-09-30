using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robust
{
    public static class ValueService
    {
        public static FieldValue GetOrCreateValue(Entity entity, Field field, int valueNumber = 1)
        {
            var fieldValue = entity.FieldValues.FirstOrDefault(fv => fv.FieldID == field.ID && fv.ValueNumber == valueNumber);
            if (fieldValue == null)
            {
                fieldValue = new FieldValue();
                fieldValue.Entity = entity;
                fieldValue.EntityID = entity.ID;
                fieldValue.Field = field;
                fieldValue.FieldID = field.ID;
                fieldValue.ValueNumber = valueNumber;
                entity.FieldValues.Add(fieldValue);
            }
            return fieldValue;
        }

        private static Dictionary<int, Func<FieldValue, object>> getterCache = new Dictionary<int, Func<FieldValue, object>>();
        private static Dictionary<int, Action<FieldValue, object>> setterCache = new Dictionary<int, Action<FieldValue, object>>();

        public static object GetValue(FieldValue fieldValue)
        {
            var field = fieldValue.Field;
            Func<FieldValue, object> getter;

            if (!getterCache.TryGetValue(field.FieldTypeID, out getter))
            {
                // get the "top-level" field type
                var type = field.FieldType;
                while (type.ParentFieldTypeID != null)
                    type = type.ParentFieldType;

                getter = GetTypeGetter(type.ID);
                getterCache.Add(field.FieldTypeID, getter);
            }

            return getter(fieldValue);
        }

        public static void SetValue(FieldValue fieldValue, object value)
        {
            var field = fieldValue.Field;
            Action<FieldValue, object> setter;

            if (!setterCache.TryGetValue(field.FieldTypeID, out setter))
            {
                // get the "top-level" field type
                var type = field.FieldType;
                while (type.ParentFieldTypeID != null)
                    type = type.ParentFieldType;

                setter = GetTypeSetter(type.ID);
                setterCache.Add(field.FieldTypeID, setter);
            }
            
            setter(fieldValue, value);
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

        private static Action<FieldValue, object> GetTypeSetter(int rootFieldTypeID)
        {
            switch (rootFieldTypeID)
            {
                case Boolean:
                    return SetBoolean;
                case Date:
                    return SetDate;
                case Decimal:
                    return SetDecimal;
                case ForeignKey:
                    return SetForeignKey;
                case FreeText:
                    return SetFreeText;
                case Integer:
                    return SetInteger;
                case Text:
                    return SetText;
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

        private static void SetBoolean(FieldValue fieldValue, object value)
        {
            var actualValue = fieldValue.BitValue;
            if (actualValue == null)
            {
                actualValue = new FieldValue_Bit();
                actualValue.FieldValue = fieldValue;
                fieldValue.BitValue = actualValue;
            }
            actualValue.Value = (bool)value;
        }

        private static void SetDate(FieldValue fieldValue, object value)
        {
            var actualValue = fieldValue.DateValue;
            if (actualValue == null)
            {
                actualValue = new FieldValue_Date();
                actualValue.FieldValue = fieldValue;
                fieldValue.DateValue = actualValue;
            }
            actualValue.Value = (DateTime)value;
        }

        private static void SetDecimal(FieldValue fieldValue, object value)
        {
            var actualValue = fieldValue.DecimalValue;
            if (actualValue == null)
            {
                actualValue = new FieldValue_Decimal();
                actualValue.FieldValue = fieldValue;
                fieldValue.DecimalValue = actualValue;
            }
            actualValue.Value = (decimal)value;
        }

        private static void SetForeignKey(FieldValue fieldValue, object value)
        {
            var actualValue = fieldValue.ForeignKeyValue;
            if (actualValue == null)
            {
                actualValue = new FieldValue_ForeignKey();
                actualValue.FieldValue = fieldValue;
                fieldValue.ForeignKeyValue = actualValue;
            }
            actualValue.Value = (value as Entity).ID;
        }

        private static void SetFreeText(FieldValue fieldValue, object value)
        {
            var actualValue = fieldValue.FreeTextValue;
            if (actualValue == null)
            {
                actualValue = new FieldValue_FreeText();
                actualValue.FieldValue = fieldValue;
                fieldValue.FreeTextValue = actualValue;
            }
            actualValue.Value = (string)value;
        }

        private static void SetInteger(FieldValue fieldValue, object value)
        {
            var actualValue = fieldValue.IntValue;
            if (actualValue == null)
            {
                actualValue = new FieldValue_Int();
                actualValue.FieldValue = fieldValue;
                fieldValue.IntValue = actualValue;
            }
            actualValue.Value = (int)value;
        }

        private static void SetText(FieldValue fieldValue, object value)
        {
            var actualValue = fieldValue.TextValue;
            if (actualValue == null)
            {
                actualValue = new FieldValue_Text();
                actualValue.FieldValue = fieldValue;
                fieldValue.TextValue = actualValue;
            }
            actualValue.Value = (string)value;
        }
    }
}
