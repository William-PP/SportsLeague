using SportsLeague.Domain.Enums;
namespace SportsLeague.API.DTOs.Request;
public class CreateMatchLineupDto
{
    public int PlayerId { get; set; }
    public bool IsStarter { get; set; }
    public MatchLineupPosition Position { get; set; }
}
