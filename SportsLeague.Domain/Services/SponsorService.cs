using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _SponsorRepository;
    private readonly ITournamentSponsorRepository _TournamentSponsorRepository;
    private readonly ITournamentRepository _TournamentRepository;
    private readonly ILogger<SponsorService> _logger;

    public SponsorService(ISponsorRepository sponsorRepository, 
        ITournamentSponsorRepository tournamentSponsorRepository,
        ITournamentRepository tournamentRepository,
        ILogger<SponsorService> logger)
    {

        _SponsorRepository = sponsorRepository;
        _TournamentSponsorRepository = tournamentSponsorRepository;
        _TournamentRepository = tournamentRepository;
        _logger = logger;

    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all Sponsors");
        return await _SponsorRepository.GetAllAsync();
    }

    public async Task<Sponsor?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving Sponsor with ID: {SponsorId}", id);

        var sponsor = await _SponsorRepository.GetByIdAsync(id);
        if (sponsor == null)
            _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);

        return sponsor;
    }

    public async Task<Sponsor> CreateAsync(Sponsor sponsor)
    {
        //validacion de name duplicado
        if (await _SponsorRepository.ExistsByNameAsync(sponsor.Name))
        {
            _logger.LogWarning("Attempted to create duplicate sponsor name: {Name}", sponsor.Name);
            throw new InvalidOperationException("No se puede crear un Sponsor con Name duplicado");
        }
        //validacion de formato email
        if (string.IsNullOrEmpty(sponsor.ContactEmail) || !sponsor.ContactEmail.Contains("@"))
        {
            _logger.LogWarning("Invalid email format: {Email}", sponsor.ContactEmail);
            throw new InvalidOperationException("ContactEmail debe ser un formato válido");
        }

        _logger.LogInformation(
            "Creating Sponsor: {Name}",
            sponsor.Name);
            
        return await _SponsorRepository.CreateAsync(sponsor);
    }

    public async Task UpdateAsync(int id, Sponsor sponsor)
    {
        var existing = await _SponsorRepository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"No se encontró el Sponsor con ID {id}");

        existing.Name = sponsor.Name;
        
        

        _logger.LogInformation("Updating Sponsor with ID: {SponsorId}", id);
        await _SponsorRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _SponsorRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"No se encontró el Sponsor con ID {id}");

        _logger.LogInformation("Deleting Sponsor with ID: {SponsorId}", id);
        await _SponsorRepository.DeleteAsync(id);
    }
    public async Task AssociateToTournamentAsync(TournamentSponsor tournamentSponsor)
    {

        // Valicacion de contract amount mayor a 0
        if (tournamentSponsor.ContractAmount <= 0)
        {
            throw new InvalidOperationException("El monto del contrato debe ser mayor a 0");
        }

        // validacion para saber si el torneo existe
        var tournament = await _TournamentRepository.GetByIdAsync(tournamentSponsor.TournamentId);
        if (tournament == null)
            throw new KeyNotFoundException($"No se encontró el torneo con ID {tournamentSponsor.TournamentId}");

        // validacion para saber si el sponsor existe
        var sponsorExists = await _SponsorRepository.ExistsAsync(tournamentSponsor.SponsorId);
        if (!sponsorExists)
            throw new KeyNotFoundException($"No se encontró el patrocinador con ID {tournamentSponsor.SponsorId}");

        // validacion para evitar relaciones duplicadas
        var existingLink = await _TournamentSponsorRepository.GetByTournamentAndSponsorAsync(
         tournamentSponsor.TournamentId,
         tournamentSponsor.SponsorId);

        if (existingLink != null)
        {
            throw new InvalidOperationException("No se puede duplicar la vinculación (este patrocinador ya está inscrito en este torneo).");
        }

        _logger.LogInformation("Associating sponsor {SponsorId} to tournament {TournamentId}",
            tournamentSponsor.SponsorId, tournamentSponsor.TournamentId);

        await _TournamentSponsorRepository.CreateAsync(tournamentSponsor);
    }

    public async Task DissociateFromTournamentAsync(int tournamentId, int sponsorId)
    {
        _logger.LogInformation(
            "Attempting to dissociate Sponsor {SponsorId} from Tournament {TournamentId}",
            sponsorId, tournamentId);

        // validacion para saber saber si existe la relacion
        var existingLink = await _TournamentSponsorRepository
            .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
        if (existingLink == null)
        {
            _logger.LogWarning(
                "Dissociation failed: No relationship found between Sponsor {SponsorId} and Tournament {TournamentId}",
                sponsorId, tournamentId);

            throw new KeyNotFoundException("No se encontró una vinculación activa entre este patrocinador y el torneo especificado.");
        }

        await _TournamentSponsorRepository.DeleteAsync(existingLink.Id);

        _logger.LogInformation(
            "Successfully dissociated Sponsor {SponsorId} from Tournament {TournamentId}",
            sponsorId, tournamentId);
    }

    public async Task<IEnumerable<TournamentSponsor>> GetSponsorTournamentsAsync(int sponsorId)
    {
        _logger.LogInformation("Consultando torneos vinculados al patrocinador {SponsorId}", sponsorId);

        // Usamos el método que ya creamos en tu Repository
        return await _TournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
    }

}

