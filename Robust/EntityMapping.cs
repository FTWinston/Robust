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
        internal EntityMapping(EntityType entityType)
        {
            EntityType = entityType;
            FieldMappings = new Dictionary<int, PropertyInfo>();
        }

        internal EntityType EntityType { get; private set; }
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
            FixedType data = new FixedType();
            Load(entity, data);
            return data;
        }

        public void Load(Entity entity, FixedType data)
        {
            foreach (var fieldValue in entity.FieldValues./*OnlyCurrent().*/Where(v => FieldMappings.Keys.Contains(v.FieldID)))
            {
                PropertyInfo property = FieldMappings[fieldValue.FieldID];
                object value = ValueService.GetValue(fieldValue);
                property.SetValue(data, value);
            }
        }

        public Entity SaveNew(FixedType data)
        {
            Entity entity = new Entity()
            {
                EntityTypeID = EntityType.ID,
                EntityType = EntityType,
                CreatedOn = DateTime.Now,
            };

            SaveExisting(data, entity);
            return entity;
        }

        public void SaveExisting(FixedType data, Entity entity)
        {
            foreach (var kvp in FieldMappings)
            {
                int fieldID = kvp.Key;
                Field field = entity.EntityType.Fields.First(f => f.ID == fieldID);
                PropertyInfo property = kvp.Value;

                FieldValue fieldValue = ValueService.CreateValue(entity, field);
                
                object value = property.GetValue(data);
                ValueService.SetValue(fieldValue, value);
            }
        }
    }
}
