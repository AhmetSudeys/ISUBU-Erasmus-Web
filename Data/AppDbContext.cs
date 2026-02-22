using Microsoft.EntityFrameworkCore;
using deneme.Models;

namespace deneme.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // madde 3 - ERD'den gelen tablolar - First Code Yaklaşımı
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Ulke> Ulkeler { get; set; }
        public DbSet<Okul> Okullar { get; set; }
        public DbSet<EgitimSeviyesi> EgitimSeviyeleri { get; set; }
        public DbSet<Dil> Diller { get; set; }
        public DbSet<ErasmusProgrami> ErasmusProgramlari { get; set; }
        public DbSet<Yorum> Yorumlar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Admin - AdminKullaniciAdi unique constraint
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.AdminKullaniciAdi)
                .IsUnique();

            // Okul - InternetSitesi index (unique değil, çünkü aynı website'e sahip farklı okullar olabilir)
            modelBuilder.Entity<Okul>()
                .HasIndex(o => o.InternetSitesi);

            // ErasmusProgrami - ErasmusKodu unique constraint
            modelBuilder.Entity<ErasmusProgrami>()
                .HasIndex(e => e.ErasmusKodu)
                .IsUnique();

            // Foreign Key ilişkileri ve cascade delete ayarları
            // Okul -> Ulke
            modelBuilder.Entity<Okul>()
                .HasOne(o => o.Ulke)
                .WithMany(u => u.Okullar)
                .HasForeignKey(o => o.UlkeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Dil -> EgitimSeviyesi
            modelBuilder.Entity<Dil>()
                .HasOne(d => d.EgitimSeviyesi)
                .WithMany(e => e.Diller)
                .HasForeignKey(d => d.EgitimSeviyesiId)
                .OnDelete(DeleteBehavior.Restrict);

            // ErasmusProgrami -> Okul
            modelBuilder.Entity<ErasmusProgrami>()
                .HasOne(e => e.Okul)
                .WithMany(o => o.ErasmusProgramlari)
                .HasForeignKey(e => e.OkulId)
                .OnDelete(DeleteBehavior.Restrict);

            // ErasmusProgrami -> Dil
            modelBuilder.Entity<ErasmusProgrami>()
                .HasOne(e => e.Dil)
                .WithMany(d => d.ErasmusProgramlari)
                .HasForeignKey(e => e.DilId)
                .OnDelete(DeleteBehavior.Restrict);

            // ErasmusProgrami <-> EgitimSeviyesi (Many-to-Many)
            // Ara tablo: ErasmusProgramiEgitimSeviyesi
            modelBuilder.Entity<ErasmusProgrami>()
                .HasMany(e => e.EgitimSeviyeleri)
                .WithMany(es => es.ErasmusProgramlari)
                .UsingEntity<Dictionary<string, object>>(
                    "ErasmusProgramiEgitimSeviyesi",
                    j => j.HasOne<EgitimSeviyesi>()
                        .WithMany()
                        .HasForeignKey("EgitimSeviyesiId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<ErasmusProgrami>()
                        .WithMany()
                        .HasForeignKey("ErasmusId")
                        .OnDelete(DeleteBehavior.Cascade)
                );

            // Yorum -> ErasmusProgrami
            modelBuilder.Entity<Yorum>()
                .HasOne(y => y.ErasmusProgrami)
                .WithMany(e => e.Yorumlar)
                .HasForeignKey(y => y.ErasmusId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

