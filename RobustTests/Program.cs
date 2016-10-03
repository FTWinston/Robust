using Robust;
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

            using (var entities = new RobustEntities())
            {
                type = new EntityType()
                {
                    Name = "Contact",
                };
                entities.EntityTypes.Add(type);
                
                name = new Field()
                {
                    Name = "Name",
                    MinNumber = 1,
                    MaxNumber = 1,
                    EntityType = type,
                    FieldType = entities.FieldTypes.First((ft) => ft.ID == 7),
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
                    FieldType = entities.FieldTypes.First((ft) => ft.ID == 7),
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
                    FieldType = entities.FieldTypes.First((ft) => ft.ID == 2),
                    SortOrder = 3,
                    Mandatory = true,
                };
                type.Fields.Add(dob);
                
                entities.SaveChanges();
            
                var mapping = new MappingBuilder<Contact>(type)
                .AddField(name, "Name")
                .AddField(email, "Email")
                .AddField(dob, "DateOfBirth")
                .GetResult();

                Contact c = new Contact();
                c.Name = "Test contact";
                c.Email = "test@email.com";
                c.DateOfBirth = DateTime.Now;

                var saveEntity = mapping.SaveNew(c);
                entities.Entities.Add(saveEntity);
                entities.SaveChanges();

                var entity = type.Entities.OnlyCurrent().FirstOrDefault();

                Contact c2 = mapping.Load(entity);

                Console.WriteLine("Before: {0} / {1} / {2}", c.Name, c.Email, c.DateOfBirth);
                Console.WriteLine("After:  {0} / {1} / {2}", c2.Name, c2.Email, c2.DateOfBirth);
            }
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
