﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Session6
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Session6Entities : DbContext
    {
        public Session6Entities()
            : base("name=Session6Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Aircrafts> Aircrafts { get; set; }
        public virtual DbSet<Airports> Airports { get; set; }
        public virtual DbSet<Amenities> Amenities { get; set; }
        public virtual DbSet<AmenitiesTickets> AmenitiesTickets { get; set; }
        public virtual DbSet<CabinTypes> CabinTypes { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<Offices> Offices { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Routes> Routes { get; set; }
        public virtual DbSet<Schedules> Schedules { get; set; }
        public virtual DbSet<Tickets> Tickets { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}