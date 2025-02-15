using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GestionDesRetardsEtAbseneces.Model;

public partial class DbGestgrhContext : DbContext
{
    public DbGestgrhContext()
    {
    }

    public DbGestgrhContext(DbContextOptions<DbGestgrhContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Absence> Absences { get; set; }

    public virtual DbSet<Authentification> Authentifications { get; set; }

    public virtual DbSet<Demandeconge> Demandeconges { get; set; }

    public virtual DbSet<Employe> Employes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Rapportassiduite> Rapportassiduites { get; set; }

    public virtual DbSet<Retard> Retards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Server=127.0.0.1;Database=db_gestgrh ;Uid=root;Pwd=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Absence>(entity =>
        {
            entity.HasKey(e => e.IdAbsence).HasName("PRIMARY");

            entity.ToTable("absence");

            entity.HasIndex(e => e.IdEmploye, "idEmploye");

            entity.Property(e => e.IdAbsence).HasColumnName("idAbsence");
            entity.Property(e => e.DateAbsence)
                .HasColumnType("date")
                .HasColumnName("dateAbsence");
            entity.Property(e => e.IdEmploye).HasColumnName("idEmploye");
            entity.Property(e => e.Justification)
                .HasColumnType("text")
                .HasColumnName("justification");
            entity.Property(e => e.TypeAbsence)
                .HasMaxLength(50)
                .HasColumnName("typeAbsence");
            entity.Property(e => e.Valide).HasColumnName("valide");

            entity.HasOne(d => d.IdEmployeNavigation).WithMany(p => p.Absences)
                .HasForeignKey(d => d.IdEmploye)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("absence_ibfk_1");
        });

        modelBuilder.Entity<Authentification>(entity =>
        {
            entity.HasKey(e => e.IdAuthentification).HasName("PRIMARY");

            entity.ToTable("authentification");

            entity.HasIndex(e => e.IdEmploye, "idEmploye");

            entity.Property(e => e.IdAuthentification)
                .HasComment("Primary Key")
                .HasColumnName("idAuthentification");
            entity.Property(e => e.DateDerniereConnexion)
                .HasComment("Last Connection Date")
                .HasColumnType("datetime");
            entity.Property(e => e.DateExpiration)
                .HasComment("Expiration Date")
                .HasColumnType("datetime");
            entity.Property(e => e.IdEmploye)
                .HasComment("Foreign Key")
                .HasColumnName("idEmploye");
            entity.Property(e => e.MotDePasseHash)
                .HasMaxLength(255)
                .HasComment("Hashed Password");
            entity.Property(e => e.NiveauAcces).HasComment("Access Level");
            entity.Property(e => e.TokenSession)
                .HasMaxLength(255)
                .HasComment("Session Token");

            entity.HasOne(d => d.IdEmployeNavigation).WithMany(p => p.Authentifications)
                .HasForeignKey(d => d.IdEmploye)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("authentification_ibfk_1");
        });

        modelBuilder.Entity<Demandeconge>(entity =>
        {
            entity.HasKey(e => e.IdDemande).HasName("PRIMARY");

            entity.ToTable("demandeconge");

            entity.HasIndex(e => e.IdEmploye, "idEmploye");

            entity.Property(e => e.IdDemande).HasColumnName("idDemande");
            entity.Property(e => e.DateDebut)
                .HasColumnType("date")
                .HasColumnName("dateDebut");
            entity.Property(e => e.DateFin)
                .HasColumnType("date")
                .HasColumnName("dateFin");
            entity.Property(e => e.IdEmploye).HasColumnName("idEmploye");
            entity.Property(e => e.Justification)
                .HasColumnType("text")
                .HasColumnName("justification");
            entity.Property(e => e.Statut)
                .HasMaxLength(50)
                .HasColumnName("statut");
            entity.Property(e => e.TypeConge)
                .HasMaxLength(50)
                .HasColumnName("typeConge");

            entity.HasOne(d => d.IdEmployeNavigation).WithMany(p => p.Demandeconges)
                .HasForeignKey(d => d.IdEmploye)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("demandeconge_ibfk_1");
        });

        modelBuilder.Entity<Employe>(entity =>
        {
            entity.HasKey(e => e.IdEmploye).HasName("PRIMARY");

            entity.ToTable("employe");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.IdEmploye).HasColumnName("idEmploye");
            entity.Property(e => e.Email)
                .HasMaxLength(60)
                .HasColumnName("email");
            entity.Property(e => e.MotDePasse).HasMaxLength(255);
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .HasColumnName("nom");
            entity.Property(e => e.Prenom)
                .HasMaxLength(60)
                .HasColumnName("prenom");
            entity.Property(e => e.RoleEmploye)
                .HasMaxLength(50)
                .HasColumnName("roleEmploye");
            entity.Property(e => e.Statut)
                .HasMaxLength(50)
                .HasColumnName("statut");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.IdNotification).HasName("PRIMARY");

            entity.ToTable("notification");

            entity.HasIndex(e => e.IdEmploye, "idEmploye");

            entity.Property(e => e.IdNotification).HasColumnName("idNotification");
            entity.Property(e => e.DateEnvoi)
                .HasColumnType("datetime")
                .HasColumnName("dateEnvoi");
            entity.Property(e => e.IdEmploye).HasColumnName("idEmploye");
            entity.Property(e => e.MessageNotification)
                .HasColumnType("text")
                .HasColumnName("messageNotification");
            entity.Property(e => e.Statut).HasColumnName("statut");
            entity.Property(e => e.TypeNotification)
                .HasMaxLength(50)
                .HasColumnName("typeNotification");

            entity.HasOne(d => d.IdEmployeNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.IdEmploye)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notification_ibfk_1");
        });

        modelBuilder.Entity<Rapportassiduite>(entity =>
        {
            entity.HasKey(e => e.IdRapport).HasName("PRIMARY");

            entity.ToTable("rapportassiduite");

            entity.HasIndex(e => e.IdEmploye, "idEmploye");

            entity.Property(e => e.IdRapport).HasColumnName("idRapport");
            entity.Property(e => e.ContenuRapport)
                .HasColumnType("text")
                .HasColumnName("contenuRapport");
            entity.Property(e => e.DateGeneration)
                .HasColumnType("date")
                .HasColumnName("dateGeneration");
            entity.Property(e => e.IdEmploye).HasColumnName("idEmploye");
            entity.Property(e => e.PeriodeRapport)
                .HasMaxLength(50)
                .HasColumnName("periodeRapport");

            entity.HasOne(d => d.IdEmployeNavigation).WithMany(p => p.Rapportassiduites)
                .HasForeignKey(d => d.IdEmploye)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rapportassiduite_ibfk_1");
        });

        modelBuilder.Entity<Retard>(entity =>
        {
            entity.HasKey(e => e.IdRetard).HasName("PRIMARY");

            entity.ToTable("retard");

            entity.HasIndex(e => e.IdEmploye, "idEmploye");

            entity.Property(e => e.IdRetard).HasColumnName("idRetard");
            entity.Property(e => e.DateRetard)
                .HasColumnType("date")
                .HasColumnName("dateRetard");
            entity.Property(e => e.HeureDebut)
                .HasColumnType("time")
                .HasColumnName("heureDebut");
            entity.Property(e => e.HeureFin)
                .HasColumnType("time")
                .HasColumnName("heureFin");
            entity.Property(e => e.IdEmploye).HasColumnName("idEmploye");
            entity.Property(e => e.Justification)
                .HasColumnType("text")
                .HasColumnName("justification");
            entity.Property(e => e.Valide).HasColumnName("valide");

            entity.HasOne(d => d.IdEmployeNavigation).WithMany(p => p.Retards)
                .HasForeignKey(d => d.IdEmploye)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("retard_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
