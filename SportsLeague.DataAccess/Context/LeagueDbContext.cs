using Microsoft.EntityFrameworkCore;
using SportsLeague.Domain.Entities;

namespace SportsLeague.DataAccess.Context;

public class LeagueDbContext : DbContext /*clase principal de entity framework core
                                          * conecta la app con la base de datos*/

{
    public LeagueDbContext(DbContextOptions<LeagueDbContext> options) :base(options)
        /*DbContextOptions es el constructor contiene la confi de la base datos 
         *base(options) pasa la configuracion al padre sin esto no funciona practicamente nada*/
    {
    }
    //Esto representa la tabla teams es Tabla:teams tipo Team
    public DbSet<Team> teams => Set<Team>();
    //Esto representa la tabla player es Tabla:player
    public DbSet<Player> Players => Set<Player>();
    //Esto representa la tabla referees es Tabla:referee
    public DbSet<Referee> Referees => Set<Referee>();

    //Esto representa la tabla Tournaments es Tabla:tournament
    public DbSet<Tournament> Tournaments => Set<Tournament>();

    //Esto representa la tabla Tournamentteams es Tabla:tournamentteam

    public DbSet<TournamentTeam> TournamentTeams => Set<TournamentTeam>();

    //Esto representa la tabla Sponsors es Tabla:Sponsors
    
    public DbSet<Sponsor> Sponsors => Set<Sponsor>();


    //Esto representa la tabla TournamentSponsors es Tabla:TournamentSponsor

    public DbSet<TournamentSponsor> TournamentSponsors => Set<TournamentSponsor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        //Metodo para configurar la base de datos: se define aca claves, indices, tamaños, restricciones
    {
        base.OnModelCreating(modelBuilder); //llama a la implementacion base del framework 
        //Team Configuration
        modelBuilder.Entity<Team>(entity =>// aca se configurar la entidad team
        {

            entity.HasKey(t => t.Id);//clave primaria
            entity.Property(t => t.Name)//configura name
            .IsRequired() // not null (no puede ser null)
            .HasMaxLength(100); // varchar(100)
            entity.Property(t => t.City) // configura city
            .IsRequired()//not null
            .HasMaxLength(100);//varchar (150)
            entity.Property(t => t.Stadium)// configura estadio
            .HasMaxLength(150);//varchar (150)
            entity.Property(t => t.LogoUrl)//configura url
            .HasMaxLength(500);//varchar 500
            entity.Property(t => t.CreatedAt)// configura createdat
            .IsRequired();//not null
            entity.Property(t => t.UpdateAt) //configura updatedat
            .IsRequired(false); // permite el null concuerda con datetime?
            entity.HasIndex(t =>t.Name) //indice unico para name
            .IsUnique(); // no permite mas con este nombre en la base de datos
        });

        //Player configuration
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.Id); 
            entity.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(80);
            entity.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(80);
            entity.Property(p => p.BirthDate)
            .IsRequired();
            entity.Property(p => p.Number)
            .IsRequired(); 
            entity.Property(p => p.Position)
            .IsRequired(); 
            entity.Property(p => p.CreatedAt)
            .IsRequired();
            entity.Property(p => p.UpdateAt)
            .IsRequired(false);

            // Relación 1:N con Team

            entity.HasOne(p => p.Team) 
            .WithMany(t => t.Players)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Cascade); 

            // Índice único compuesto: número de camiseta único por equipo
            entity.HasIndex(p => new { p.TeamId, p.Number }) 
            .IsUnique();
        });

        // ── Referee Configuration ──
        modelBuilder.Entity<Referee>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.FirstName)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(r => r.LastName)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(r => r.Nationality)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(r => r.CreatedAt)
                .IsRequired();

            entity.Property(r => r.UpdateAt)
                .IsRequired(false);
        });

        // ── Tournament Configuration ──
        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(t => t.Season)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(t => t.StartDate)
                .IsRequired();

            entity.Property(t => t.EndDate)
                .IsRequired();

            entity.Property(t => t.Status)
                .IsRequired();

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            entity.Property(t => t.UpdateAt)
                .IsRequired(false);
        });

        // ── TournamentTeam Configuration ──
        modelBuilder.Entity<TournamentTeam>(entity =>
        {
            entity.HasKey(tt => tt.Id);

            entity.Property(tt => tt.RegisteredAt)
                .IsRequired();

            entity.Property(tt => tt.CreatedAt)
                .IsRequired();

            entity.Property(tt => tt.UpdateAt)
                .IsRequired(false);

            // Relación con Tournament
            entity.HasOne(tt => tt.Tournament)
                .WithMany(t => t.TournamentTeams)
                .HasForeignKey(tt => tt.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Team
            entity.HasOne(tt => tt.Team)
                .WithMany(t => t.TournamentTeams)
                .HasForeignKey(tt => tt.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice único compuesto: un equipo solo una vez por torneo
            entity.HasIndex(tt => new { tt.TournamentId, tt.TeamId })
                .IsUnique();
        });

         modelBuilder.Entity<Sponsor>(entity =>// aca se configurar la entidad Sponsor
        {

            entity.HasKey(s => s.Id);//clave primaria
            entity.Property(s => s.Name)//configura name
            .IsRequired() // not null (no puede ser null)
            .HasMaxLength(100); // varchar(100)
            entity.Property(s => s.ContactEmail) 
            .IsRequired()//not null
            .HasMaxLength(100);//varchar (100)
            entity.Property(s => s.Phone)
            .HasMaxLength(150);//varchar (150)
            entity.Property(s => s.WebsiteUrl)//configura url
            .HasMaxLength(500);//varchar 500
            entity.Property(s => s.CreatedAt)// configura createdat
            .IsRequired();//not null
            entity.Property(s => s.UpdateAt) //configura updatedat
            .IsRequired(false); // permite el null concuerda con datetime?
            entity.HasIndex(s =>s.Name) //indice unico para name
            .IsUnique(); // no permite mas con este nombre en la base de datos
        });

        // ── TournamentSponsor Configuration ──
        modelBuilder.Entity<TournamentSponsor>(entity =>
        {
            entity.HasKey(ts => ts.Id);

            entity.Property(ts => ts.ContractAmount)
                .HasPrecision(18,2)
                .IsRequired();

            entity.Property(ts => ts.JoinedAt)
                .IsRequired();

            // Relación con Tournament
            entity.HasOne(ts => ts.Tournament)
                .WithMany(t => t.TournamentSponsors)
                .HasForeignKey(ts => ts.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Sponsor
            entity.HasOne(ts => ts.Sponsor)
                .WithMany(t => t.TournamentSponsors)
                .HasForeignKey(ts => ts.SponsorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice único compuesto
            entity.HasIndex(ts => new { ts.TournamentId, ts.SponsorId })
                .IsUnique();
        });

        


    }
    }
