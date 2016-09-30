using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Robust
{
    public class EntityMapping<FixedType>
        where FixedType : new()
    {
        internal EntityMapping()
        {
             FieldMappings = new Dictionary<int, PropertyInfo>();
        }

        internal Dictionary<int, PropertyInfo> FieldMappings { get; private set; }

        public FixedType Load(int entityID)
        {
            Entity entity;

            using (RobustEntities entities = new RobustEntities())
            {
                entity = entities.Entities.FirstOrDefault(t => t.ID == entityID);
            }

            return Load(entity);
        }

        public FixedType Load(Entity entity)
        {
            FixedType destination = new FixedType();

            foreach (var fieldValue in entity.FieldValues.Where(v => FieldMappings.Keys.Contains(v.FieldID)))
            {
                PropertyInfo property = FieldMappings[fieldValue.FieldID];
                object value = ValueService.GetValue(fieldValue);
                property.SetValue(destination, value);
            }

            return destination;
        }
    }
}
