using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robust
{
    public class EntityTypeService
    {
        public EntityType GetEntityType(int id)
        {
            using (RobustEntities entities = new RobustEntities())
            {
                return entities.EntityTypes.FirstOrDefault(t => t.ID == id);
            }
        }
    }
}
