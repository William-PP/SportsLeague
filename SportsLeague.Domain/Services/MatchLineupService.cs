using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Helpers;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class MatchLineupService : IMatchLineupService
{
    private readonly IMatchLineupRepository _matchLineupRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly MatchValidationHelper _validationHelper;
    private readonly ILogger<MatchLineupService> _logger;

    public MatchLineupService(
        IMatchLineupRepository matchLineupRepository,
        IMatchRepository matchRepository,
        MatchValidationHelper validationHelper,
        ILogger<MatchLineupService> logger)
    {
        _matchLineupRepository = matchLineupRepository;
        _matchRepository = matchRepository;
        _validationHelper = validationHelper;
        _logger = logger;
    }

    public async Task<MatchLineup> RegisterLineupAsync(int matchId, MatchLineup lineup)
    {
        // V1: El partido debe existir
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

        // V6: El partido debe estar en estado Scheduled
        if (match.Status != MatchStatus.Scheduled)
            throw new InvalidOperationException("Solo se pueden registrar alineaciones en partidos Scheduled");

        // V2 y V3: El jugador debe existir y pertenecer al HomeTeam o AwayTeam
        var player = await _validationHelper.ValidatePlayerInMatchAsync(lineup.PlayerId, match);

        // V4: El jugador no puede estar registrado dos veces en la misma alineación
        var existingLineup = await _matchLineupRepository.GetByMatchAndPlayerAsync(matchId, lineup.PlayerId);
        if (existingLineup != null)
            throw new InvalidOperationException("El jugador ya está registrado en la alineación de este partido");

        // V5: Máximo 11 titulares por equipo por partido
        if (lineup.IsStarter)
        {
            var teamId = player.TeamId;
            var startersCount = await _matchLineupRepository.CountStartersByTeamAsync(matchId, teamId);
            if (startersCount >= 11)
                throw new InvalidOperationException("El equipo ya tiene 11 titulares registrados en este partido");
        }

        lineup.MatchId = matchId;

        _logger.LogInformation("Registering lineup: Match {MatchId}, Player {PlayerId}, IsStarter {IsStarter}, Position {Position}",
            matchId, lineup.PlayerId, lineup.IsStarter, lineup.Position);

        return await _matchLineupRepository.CreateAsync(lineup);
    }

    public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAsync(int matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

        return await _matchLineupRepository.GetByMatchWithDetailsAsync(matchId);
    }

    public async Task<IEnumerable<MatchLineup>> GetLineupByTeamAsync(int matchId, int teamId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

        return await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
    }

    public async Task DeleteLineupAsync(int id)
    {
        var exists = await _matchLineupRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"No se encontró la alineación con ID {id}");

        await _matchLineupRepository.DeleteAsync(id);
    }
}
