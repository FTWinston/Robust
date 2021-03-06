﻿using Robust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobustTests
{
    class Program
    {
        static void Main(string[] args)
        {
            EntityType type;
            Field name, email, dob;

            try
            {
                Installation.DeleteDataModel();
            }
            catch
            {

            }

            Installation.CreateDataModel();

            using (var connection = new DataConnection())
            {
                type = new EntityType()
                {
                    Name = "Contact",
                    Active = true,
                };
                connection.EntityTypes.Add(type);
                
                name = new Field()
                {
                    Name = "Name",
                    MinNumber = 1,
                    MaxNumber = 1,
                    EntityType = type,
                    FieldType = connection.GetFieldType(BaseFieldType.Text),
                    SortOrder = 1,
                    Mandatory = true,
                };
                type.Fields.Add(name);

                email = new Field()
                {
                    Name = "Email",
                    MinNumber = 1,
                    MaxNumber = 1,
                    EntityType = type,
                    FieldType = connection.GetFieldType(BaseFieldType.Text),
                    SortOrder = 2,
                    Mandatory = true,
                };
                type.Fields.Add(email);


                dob = new Field()
                {
                    Name = "Date of Birth",
                    MinNumber = 1,
                    MaxNumber = 1,
                    EntityType = type,
                    FieldType = connection.GetFieldType(BaseFieldType.Date),
                    SortOrder = 3,
                    Mandatory = true,
                };
                type.Fields.Add(dob);
                connection.SaveChanges();

                var mapping = new MappingBuilder<Contact>(connection, type)
                .AddField(name, "Name")
                .AddField(email, "Email")
                .AddField(dob, "DateOfBirth")
                .GetResult();

                for (int i = 0; i < 100; i++)
                {
                    Contact c = new Contact();
                    c.Name = "Test contact #" + i;
                    c.Email = "test" + i + "@email.com";
                    c.DateOfBirth = DateTime.Now.AddDays(-i);

                    var saveEntity = mapping.SaveNew(c);
                    connection.Entities.Add(saveEntity);

                    if (i==0)
                        Console.WriteLine("Before: {0} / {1} / {2}", c.Name, c.Email, c.DateOfBirth);
                }

                connection.SaveChanges();

                var entity = type.Entities.OnlyCurrent().FirstOrDefault();

                Contact c2 = mapping.Load(entity);
                Console.WriteLine("After:  {0} / {1} / {2}", c2.Name, c2.Email, c2.DateOfBirth);
            }

            Installation.CreateDataViews();
            //Installation.DeleteDataModel();
            Console.ReadKey();
        }
    }

    class Contact
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
