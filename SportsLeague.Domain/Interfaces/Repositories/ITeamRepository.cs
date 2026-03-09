using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ITeamRepository :IGenericRepository<Team>
/*ITeamRepository hereda todos los metodos de IgenerecRepository
 por lo tanto ya contiene todos sus metodos como es
IGenericRepository<Team> el generico T se reemplaza por team*/
{
    Task<Team?> GetByNameAsync (string name);//busca un equipo por nombre
    Task<IEnumerable<Team>> GetByCityAsync (string city);//busca varios equipos que pertenezcan a una ciudad
    //IEnumerable es una coleccion que puede iterarse elemento por elemento
}
