//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ditat.Data.EntityModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductImage
    {
        public System.Guid Id { get; set; }
        public System.Guid ProductId { get; set; }
        public string Path { get; set; }
        public bool PrimaryImage { get; set; }
        public Nullable<System.DateTime> Inserted { get; set; }
        public string Barcode { get; set; }
        public string Category { get; set; }
        public string Properties { get; set; }
        public Nullable<double> ClassifierPropability { get; set; }
        public Nullable<bool> SelectedForExport { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
