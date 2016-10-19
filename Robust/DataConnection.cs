using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Robust
{
    public class DataConnection : IDisposable
    {
        public DataConnection()
        {
            Context = new RobustEntities();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        internal RobustEntities Context { get; private set; }
        public DbSet<EntityType> EntityTypes { get { return Context.EntityTypes; } }
        public DbSet<Entity> Entities { get { return Context.Entities; } }

        private Dictionary<BaseFieldType, FieldType> fieldTypeCache = new Dictionary<BaseFieldType, FieldType>();
        public FieldType GetFieldType(BaseFieldType type)
        {
            FieldType output;
            if (!fieldTypeCache.TryGetValue(type, out output))
            {
                output = Context.FieldTypes.First(ft => ft.ID == (int)type);
                fieldTypeCache.Add(type, output);
            }
            return output;
        }
    }
}
