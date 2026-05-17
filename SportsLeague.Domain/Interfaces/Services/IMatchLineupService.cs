using SportsLeague.Domain.Entities;
namespace SportsLeague.Domain.Interfaces.Services;
public interface IMatchLineupService
{
    Task<MatchLineup> RegisterLineupAsync(int matchId, MatchLineup lineup);
    Task<IEnumerable<MatchLineup>> GetLineupByMatchAsync(int matchId);
    Task<IEnumerable<MatchLineup>> GetLineupByTeamAsync(int matchId, int teamId);
    Task DeleteLineupAsync(int id);
}
