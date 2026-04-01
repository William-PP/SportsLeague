using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ISponsorRepository :IGenericRepository<Sponsor>
{
    Task<bool> ExistsByNameAsync(string name); // para verificar si un nombre esta duplicado
}
