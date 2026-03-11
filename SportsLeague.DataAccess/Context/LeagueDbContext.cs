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

    public DbSet<Team> teams => Set<Team>();
    //Esto representa la tabla teams es Tabla:teams tipo Team
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        //Metodo para configurar la base de datos: se define aca claves, indices, tamaños, restricciones
    {
        base.OnModelCreating(modelBuilder); //llama a la implementacion base del framework 
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
    }
}