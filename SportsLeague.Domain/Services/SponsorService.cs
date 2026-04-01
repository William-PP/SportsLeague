using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _SponsorRepository;
    private readonly ILogger<SponsorService> _logger;

    public SponsorService(ISponsorRepository sponsorRepository, ILogger<SponsorService> logger)
    {

        _SponsorRepository = sponsorRepository;
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

}

