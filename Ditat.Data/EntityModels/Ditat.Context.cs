﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DitatEntities : DbContext
    {
        public DitatEntities()
            : base("name=DitatEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryProperty> CategoryProperties { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LanguageText> LanguageTexts { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductCondition> ProductConditions { get; set; }
        public virtual DbSet<ProductStatu> ProductStatus { get; set; }
    
        public virtual int AddMissingTranslations()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AddMissingTranslations");
        }
    
        public virtual ObjectResult<GetLanguageText_Result> GetLanguageText(Nullable<int> languageId)
        {
            var languageIdParameter = languageId.HasValue ?
                new ObjectParameter("LanguageId", languageId) :
                new ObjectParameter("LanguageId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetLanguageText_Result>("GetLanguageText", languageIdParameter);
        }
    
        public virtual int GetLanguageAsJSON(Nullable<int> languageId)
        {
            var languageIdParameter = languageId.HasValue ?
                new ObjectParameter("LanguageId", languageId) :
                new ObjectParameter("LanguageId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetLanguageAsJSON", languageIdParameter);
        }
    
        public virtual int SetPrimaryImage(Nullable<System.Guid> imageId)
        {
            var imageIdParameter = imageId.HasValue ?
                new ObjectParameter("ImageId", imageId) :
                new ObjectParameter("ImageId", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SetPrimaryImage", imageIdParameter);
        }
    }
}
