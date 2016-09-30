using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robust
{
    public class MappingBuilder<FixedType>
        where FixedType : new()
    {
        private Type TypeInfo;
        private EntityMapping<FixedType> Mapping;

        public MappingBuilder(EntityType entityType)
        {
            TypeInfo = typeof(FixedType);
            Mapping = new EntityMapping<FixedType>(entityType);
        }

        public MappingBuilder<FixedType> AddField(Field field, string propertyName)
        {
            return AddField(field.ID, propertyName);
        }

        public MappingBuilder<FixedType> AddField(Field field, string propertyName, Type propertyType)
        {
            return AddField(field.ID, propertyName, propertyType);
        }

        public MappingBuilder<FixedType> AddField(int fieldID, string propertyName)
        {
            Mapping.FieldMappings.Add(fieldID, TypeInfo.GetProperty(propertyName));
            return this;
        }

        public MappingBuilder<FixedType> AddField(int fieldID, string propertyName, Type propertyType)
        {
            Mapping.FieldMappings.Add(fieldID, TypeInfo.GetProperty(propertyName, propertyType));
            return this;
        }

        public EntityMapping<FixedType> GetResult()
        {
            return Mapping;
        }
    }
}
