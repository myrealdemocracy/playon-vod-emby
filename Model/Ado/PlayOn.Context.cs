﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PlayOn.Model.Ado
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PlayOnEntities : DbContext
    {
        public PlayOnEntities()
            : base("name=PlayOnEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Serie> Series { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<VideoSerie> VideoSeries { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<VideoMovie> VideoMovies { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
    }
}
