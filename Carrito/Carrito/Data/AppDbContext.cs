using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Carrito.Models;

namespace Carrito.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Persona> Personas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Autor> Autors { get; set; }

    public virtual DbSet<Editorial> Editorials { get; set; }

    public virtual DbSet<Genero> Generos { get; set; }

    public virtual DbSet<Libro> Libros { get; set; }

    public DbSet<Carrito.Models.Carrito> Carritos { get; set; }

    public DbSet<CarritoLibro> CarritoLibros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder.UseSqlServer("Server=GUS-PC\\MSSQLSERVERINSTA;Database=ClubLibros;Trusted_Connection=True;TrustServerCertificate=True");
       //=> optionsBuilder.UseSqlServer("Server=DESKTOP-2BU8S5T;Database=ClubLibros;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
                modelBuilder.Entity<Persona>()
            .HasDiscriminator<string>("TipoPersona")
            .HasValue<Persona>("Persona")
            .HasValue<Usuario>("Usuario");

        modelBuilder.Entity<Autor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authors__3214EC0741AF68BB");

            entity.ToTable("Autor");

            entity.HasIndex(e => e.FullName, "UQ__Authors__89C60F119BF29D3E").IsUnique();

            entity.Property(e => e.Country).HasMaxLength(80);
            entity.Property(e => e.FullName).HasMaxLength(160);
        });

        modelBuilder.Entity<Editorial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Publishe__3214EC07B1F2861C");

            entity.ToTable("Editorial");

            entity.HasIndex(e => e.Name, "UQ__Publishe__737584F67DA8EECF").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(160);
        });

        modelBuilder.Entity<Genero>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genres__3214EC07250568B3");

            entity.ToTable("Genero");

            entity.HasIndex(e => e.Name, "UQ__Genres__737584F6C2A08496").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(80);
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Books__3214EC07AFED810F");

            entity.ToTable("Libro");

            entity.HasIndex(e => e.AuthorId, "IX_Books_AuthorId");

            entity.HasIndex(e => e.GenreId, "IX_Books_GenreId");

            entity.HasIndex(e => e.Title, "IX_Books_Title");

            entity.Property(e => e.CoverUrl).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Author).WithMany(p => p.Libros)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Authors");

            entity.HasOne(d => d.Genre).WithMany(p => p.Libros)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Genres");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Libros)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK_Books_Publishers");
        });

        modelBuilder.Entity<CarritoLibro>()
         .HasKey(cl => new { cl.CarritoId, cl.LibroId });

        base.OnModelCreating(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
