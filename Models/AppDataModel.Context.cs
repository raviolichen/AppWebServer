﻿//------------------------------------------------------------------------------
// <auto-generated>
//    這個程式碼是由範本產生。
//
//    對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//    如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppWebServer.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AppDataBaseEntities : DbContext
    {
        public AppDataBaseEntities()
            : base("name=AppDataBaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Banner> Banner { get; set; }
        public DbSet<EventPage> EventPage { get; set; }
        public DbSet<SignForm> SignForm { get; set; }
        public DbSet<SignRecords> SignRecords { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserSlv> UserSlv { get; set; }
        public DbSet<Gold> Gold { get; set; }
        public DbSet<messagePublish> messagePublish { get; set; }
        public DbSet<store> store { get; set; }
        public DbSet<StoreType> StoreType { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<UserCoinReord> UserCoinReord { get; set; }
        public DbSet<EventSlvs> EventSlvs { get; set; }
        public DbSet<Slvs> Slvs { get; set; }
        public DbSet<ownerSlvsdetail> ownerSlvsdetail { get; set; }
        public DbSet<Vote> Vote { get; set; }
        public DbSet<VoteItem> VoteItem { get; set; }
        public DbSet<ProxyDateLog> ProxyDateLog { get; set; }
        public DbSet<Cache> Cache { get; set; }
    }
}
