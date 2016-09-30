//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class FieldType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FieldType()
        {
            this.Fields = new HashSet<Field>();
            this.ChildFieldTypes = new HashSet<FieldType>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public Nullable<int> ParentFieldTypeID { get; set; }
        public string DisplayFormat { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Field> Fields { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FieldType> ChildFieldTypes { get; set; }
        public virtual FieldType ParentFieldType { get; set; }
    }
}
