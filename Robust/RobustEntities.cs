using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Robust
{
    internal partial class RobustEntities
    {
        public RobustEntities()
       : base(CreateConnection(), true)
        {
        }

        private static DbProviderFactory connectionFactory = null;
        private static MetadataWorkspace workspace = null;
        private static string connectionString = null;

        private static EntityConnection CreateConnection()
        {
            if (connectionFactory == null)
            {
                var entityConnectionString = ConfigurationManager.ConnectionStrings["RobustEntities"].ConnectionString;
                var connectionData = new EntityConnectionStringBuilder(entityConnectionString);
                connectionFactory = DbProviderFactories.GetFactory(connectionData.Provider);
                connectionString = entityConnectionString.Split('\"')[1]; // retrieve the "inner" connection string
            }

            var connection = connectionFactory.CreateConnection();
            connection.ConnectionString = connectionString;
            return new EntityConnection(GetWorkspace(), connection);
        }

        private static MetadataWorkspace GetWorkspace()
        {
            if (workspace != null)
                return workspace;

            string modelSchema = ConfigurationManager.AppSettings["ModelSchema"];
            string dataSchema = ConfigurationManager.AppSettings["DataSchema"];

            var assembly = typeof(DataConnection).Assembly;
            var storageReader = XmlReader.Create(assembly.GetManifestResourceStream("RobustDataModel.ssdl"));
            var conceptualReader = new XmlReader[] { XmlReader.Create(assembly.GetManifestResourceStream("RobustDataModel.csdl")) };
            var mappingReader = new XmlReader[] { XmlReader.Create(assembly.GetManifestResourceStream("RobustDataModel.msl")) };

            var storageXml = XDocument.Load(storageReader);

            var tablesInModelSchema = new string[] { "FieldTypes", "EntityTypes", "Fields" };

            XNamespace storageNamespace = "http://schemas.microsoft.com/ado/2009/11/edm/ssdl";
            foreach (var item in storageXml.Descendants(storageNamespace + "EntitySet"))
            {
                var nameAttr = item.Attribute("Name");
                if (nameAttr == null)
                    continue;

                var schemaAttr = item.Attribute("Schema");
                if (schemaAttr == null)
                    continue;

                schemaAttr.Value = tablesInModelSchema.Contains(nameAttr.Value) ? modelSchema : dataSchema;
            }

            storageReader = storageXml.CreateReader();

            var storageCollection = new StoreItemCollection(new XmlReader[] { storageReader });
            var conceptualCollection = new EdmItemCollection(conceptualReader);
            var mappingCollection = new StorageMappingItemCollection(conceptualCollection, storageCollection, mappingReader);

            workspace = new MetadataWorkspace(() => conceptualCollection, () => storageCollection, () => mappingCollection);
            return workspace;
        }
    }
}
