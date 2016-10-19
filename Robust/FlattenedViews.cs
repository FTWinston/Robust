using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Robust
{
    public class FlattenedViews
    {
        public static void DropViews()
        {
            var query = "select name from sys.views where schema_id = (select schema_id from sys.schemas where name = @p0)";
            string schema = ConfigurationManager.AppSettings["ViewSchema"];
            using (var entities = new RobustEntities())
            {
                foreach (string viewName in RunQuery<string>(entities, query, schema))
                    RunCommand(entities, "drop view " + schema + "." + viewName);
            }
        }

        public static void CreateViews()
        {
            using (var entities = new RobustEntities())
            {
                var allFields = entities.Fields.OrderBy(f => f.EntityTypeID).ThenBy(f => f.SortOrder).ToList();
                var entityTypes = entities.EntityTypes.Where(et => et.Active).OrderBy(et => et.Name).ToList();

                foreach (var entityType in entityTypes)
                    CreateView(entities, entityType, allFields.Where(f => f.EntityTypeID == entityType.ID));
            }
        }

        private static void CreateView(RobustEntities entities, EntityType entityType, IEnumerable<Field> fields)
        {
            string viewSchema = ConfigurationManager.AppSettings["ViewSchema"];
            string dataSchema = ConfigurationManager.AppSettings["DataSchema"];

            var sb = new StringBuilder();
            sb.Append("create view ");
            sb.Append(viewSchema);
            sb.Append(".[");
            sb.Append(entityType.PluralName());
            sb.AppendLine("] as");
            sb.AppendLine("select ID");

            foreach (var field in fields)
            {
                WriteField(sb, field, dataSchema);
            }

            sb.Append("from ");
            sb.Append(dataSchema);
            sb.Append(".CurrentEntities e where EntityTypeID = ");
            sb.Append(entityType.ID);

            RunCommand(entities, sb.ToString());
        }

        private static void WriteField(StringBuilder sb, Field field, string dataSchema)
        {
            var fieldType = field.FieldType;
            while (fieldType.ParentFieldType != null)
                fieldType = fieldType.ParentFieldType;

            int maxNum;
            if (field.MaxNumber.HasValue)
                maxNum = field.MaxNumber.Value;
            else
            {
                // find the largest value stored anywhere for this field, and output that number of "value" columns
                maxNum = field.FieldValues.Where(fv => !fv.Deleted).Max(fv => fv.ValueNumber);
            }

            for (var num = 1; num <= maxNum; num++)
            {
                sb.Append(",(select top 1 Value from ");
                sb.Append(dataSchema);
                sb.Append(".");
                sb.Append(fieldType.ValueTable);
                sb.Append(" where FieldValueID = (select top 1 ID from ");
                sb.Append(dataSchema);
                sb.Append(".CurrentFieldValues where EntityID = e.ID and FieldID = ");
                sb.Append(field.ID);
                sb.Append(" and ValueNumber = ");
                sb.Append(num);
                sb.Append(")) as [");
                sb.Append(field.Name);
                if (maxNum > 1)
                    sb.Append(" " + num);
                sb.AppendLine("]");
            }
        }

        private static IList<T> RunQuery<T>(RobustEntities entities, string query, params object[] parameters)
        {
            return entities.Database.SqlQuery<T>(query, parameters).ToList();
        }

        private static void RunCommand(RobustEntities entities, string command, params object[] parameters)
        {
            entities.Database.ExecuteSqlCommand(command, parameters);
        }
    }
}
