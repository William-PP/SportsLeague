using SportsLeague.Domain.Entities;
namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
{
    //Metodo para obtener todos los patrocinadores por torneo especifico
    Task<IEnumerable<TournamentSponsor>> GetByTournamentIdAsync(int tournamentId);

    //Metodo para obtener todos los patrocinadores por torneo especifico
    Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId);

    //Metodo para evitar relaciones repetidas
    Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId);
}