﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Robust
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RobustEntities : DbContext
    {
        public RobustEntities()
            : base("name=RobustEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<FieldValue> FieldValues { get; set; }
        public virtual DbSet<FieldValue_Bit> FieldValue_Bit { get; set; }
        public virtual DbSet<FieldValue_Date> FieldValue_Date { get; set; }
        public virtual DbSet<FieldValue_Decimal> FieldValue_Decimal { get; set; }
        public virtual DbSet<FieldValue_ForeignKey> FieldValue_ForeignKey { get; set; }
        public virtual DbSet<FieldValue_FreeText> FieldValue_FreeText { get; set; }
        public virtual DbSet<FieldValue_Int> FieldValue_Int { get; set; }
        public virtual DbSet<FieldValue_Text> FieldValue_Text { get; set; }
        public virtual DbSet<EntityType> EntityTypes { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<FieldType> FieldTypes { get; set; }
        public virtual DbSet<CurrentEntity> CurrentEntities { get; set; }
        public virtual DbSet<CurrentFieldValue> CurrentFieldValues { get; set; }
    }
}
