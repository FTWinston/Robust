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

        public MappingBuilder()
        {
            TypeInfo = typeof(FixedType);
            Mapping = new EntityMapping<FixedType>();
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
