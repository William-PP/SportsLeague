using SportsLeague.Domain.Entities;
namespace SportsLeague.Domain.Interfaces.Services;

public interface ISponsorService
{
    Task<IEnumerable<Sponsor>> GetAllAsync();
    Task<Sponsor?> GetByIdAsync(int id);
    Task<Sponsor> CreateAsync(Sponsor sponsor);
    Task UpdateAsync(int id, Sponsor sponsor);
    Task DeleteAsync(int id);

    //Metodo para vincular sponsors con torneos
    Task AssociateToTournamentAsync(TournamentSponsor tournamentSponsor);

    //Metodo para desvincular sponsors de torneos
    Task DissociateFromTournamentAsync(int tournamentId, int sponsorId);

    //Metodo para listar los torneos de un sponsor
    Task<IEnumerable<TournamentSponsor>> GetSponsorTournamentsAsync(int sponsorId);
}