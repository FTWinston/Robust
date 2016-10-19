using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Robust
{
    public static class Installation
    {
        #region core data model
        public static void CreateDataModel()
        {
            using (var entities = new RobustEntities())
            {
                string modelSchema = "[" + ConfigurationManager.AppSettings["ModelSchema"] + "]",
                        dataSchema = "[" + ConfigurationManager.AppSettings["DataSchema"] + "]",
                        viewSchema = "[" + ConfigurationManager.AppSettings["ViewSchema"] + "]";

                try
                {
                    RunCommand(entities, "create schema " + modelSchema);
                }
                catch { }

                try
                {
                    RunCommand(entities, "create schema " + dataSchema);
                }
                catch { }

                try
                {
                    RunCommand(entities, "create schema " + viewSchema);
                }
                catch { }
                
                var scriptStream = typeof(Installation).Assembly.GetManifestResourceStream("Robust.CreateModel.sql");
                var mainScript = new StreamReader(scriptStream).ReadToEnd()
                    .Replace("[model]", modelSchema)
                    .Replace("[data]", dataSchema);

                foreach (var script in SplitSqlStatements(mainScript))
                    RunCommand(entities, script);
            }
        }
        
        public static void DeleteRobustModel()
        {
            using (var entities = new RobustEntities())
            {
                DropDataViews();

                string modelSchema = "[" + ConfigurationManager.AppSettings["ModelSchema"] + "]",
                        dataSchema = "[" + ConfigurationManager.AppSettings["DataSchema"] + "]",
                        viewSchema = "[" + ConfigurationManager.AppSettings["ViewSchema"] + "]";
                
                var scriptStream = typeof(Installation).Assembly.GetManifestResourceStream("Robust.DeleteModel.sql");

                var script = new StreamReader(scriptStream).ReadToEnd()
                    .Replace("[model]", modelSchema)
                    .Replace("[data]", dataSchema);

                RunCommand(entities, script);

                try
                {
                    RunCommand(entities, "drop schema " + modelSchema);
                }
                catch { }

                try
                {
                    RunCommand(entities, "drop schema " + dataSchema);
                }
                catch { }

                try
                {
                    RunCommand(entities, "drop schema " + viewSchema);
                }
                catch { }
            }
        }
        #endregion core data model

        #region flattened data views, to assist with reporting etc
        public static void DropDataViews()
        {
            var query = "select name from sys.views where schema_id = (select schema_id from sys.schemas where name = @p0)";
            string schema = "[" + ConfigurationManager.AppSettings["ViewSchema"] + "]";
            using (var entities = new RobustEntities())
            {
                foreach (string viewName in Installation.RunQuery<string>(entities, query, schema))
                    Installation.RunCommand(entities, "drop view " + schema + ".[" + viewName + "]");
            }
        }

        public static void CreateDataViews()
        {
            using (var entities = new RobustEntities())
            {
                var allFields = entities.Fields.OrderBy(f => f.EntityTypeID).ThenBy(f => f.SortOrder).ToList();
                var entityTypes = entities.EntityTypes.Where(et => et.Active).OrderBy(et => et.Name).ToList();

                foreach (var entityType in entityTypes)
                    CreateDataView(entities, entityType, allFields.Where(f => f.EntityTypeID == entityType.ID));
            }
        }

        private static void CreateDataView(RobustEntities entities, EntityType entityType, IEnumerable<Field> fields)
        {
            string viewSchema = ConfigurationManager.AppSettings["ViewSchema"];
            string dataSchema = ConfigurationManager.AppSettings["DataSchema"];

            var sb = new StringBuilder();
            sb.Append("create view [");
            sb.Append(viewSchema);
            sb.Append("].[");
            sb.Append(entityType.PluralName());
            sb.AppendLine("] as");
            sb.AppendLine("select ID");

            foreach (var field in fields)
            {
                WriteViewField(sb, field, dataSchema);
            }

            sb.Append("from [");
            sb.Append(dataSchema);
            sb.Append("].CurrentEntities e where EntityTypeID = ");
            sb.Append(entityType.ID);

            Installation.RunCommand(entities, sb.ToString());
        }

        private static void WriteViewField(StringBuilder sb, Field field, string dataSchema)
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
                sb.Append(",(select top 1 Value from [");
                sb.Append(dataSchema);
                sb.Append("].");
                sb.Append(fieldType.ValueTable);
                sb.Append(" where FieldValueID = (select top 1 ID from [");
                sb.Append(dataSchema);
                sb.Append("].CurrentFieldValues where EntityID = e.ID and FieldID = ");
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
        #endregion flattened data views, to assist with reporting etc

        private static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            // Split by "GO" statements
            var statements = Regex.Split(
                    sqlScript,
                    @"^\s*GO\s*\d*\s*($|\-\-.*$)",
                    RegexOptions.Multiline |
                    RegexOptions.IgnorePatternWhitespace |
                    RegexOptions.IgnoreCase);

            // Remove empties, trim, and return
            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n'));
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
