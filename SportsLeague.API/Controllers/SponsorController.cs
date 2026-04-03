using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SponsorController : ControllerBase
{
    private readonly ISponsorService _sponsorService;
    private readonly IMapper _mapper;

    public SponsorController(
        ISponsorService sponsorService,
        IMapper mapper)
    {
        _sponsorService = sponsorService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
    {
        var sponsors = await _sponsorService.GetAllAsync();
        var sponsorDto = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
        return Ok(sponsorDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
    {
        var sponsor = await _sponsorService.GetByIdAsync(id);
        if (sponsor == null)
            return NotFound(new { message = $"Sponsor con ID {id} no encontrado" });

        var sponsorDto = _mapper.Map<SponsorResponseDTO>(sponsor);
        return Ok(sponsorDto);
    }

    [HttpPost]
    public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO dto)
    {
        var sponsor = _mapper.Map<Sponsor>(dto);
        var created = await _sponsorService.CreateAsync(sponsor);

        var responseDto = _mapper.Map<SponsorResponseDTO>(created);

        return CreatedAtAction(
            nameof(GetById),
            new { id = responseDto.Id },
            responseDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            await _sponsorService.UpdateAsync(id, sponsor);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _sponsorService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/tournaments")]
    public async Task<ActionResult> AssociateToTournament(int id, TournamentSponsorRequestDTO dto)
    {
        try
        {
            // Forzamos que el SponsorId del DTO coincida con el ID de la URL
            dto.SponsorId = id;

            var tournamentSponsor = _mapper.Map<TournamentSponsor>(dto);
            await _sponsorService.AssociateToTournamentAsync(tournamentSponsor);

            return Ok(new { message = "Patrocinador vinculado exitosamente al torneo." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }


    [HttpGet("{id}/tournaments")]
    public async Task<ActionResult<IEnumerable<TournamentSponsorResponseDTO>>> GetSponsorTournaments(int id)
    {
        var associations = await _sponsorService.GetSponsorTournamentsAsync(id);

        // El Mapper usará la configuración de MappingProfile para traer el TournamentName
        var response = _mapper.Map<IEnumerable<TournamentSponsorResponseDTO>>(associations);

        return Ok(response);
    }

    [HttpDelete("{id}/tournaments/{tournamentId}")]
    public async Task<ActionResult> DissociateFromTournament(int id, int tournamentId)
    {
        try
        {
            await _sponsorService.DissociateFromTournamentAsync(tournamentId, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

}