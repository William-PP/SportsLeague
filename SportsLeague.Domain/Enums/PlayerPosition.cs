namespace SportsLeague.Domain.Enums;

public enum PlayerPosition
{
    Goalkeeper =0,
    Defender = 1,
    Midfielder = 2,
    Forward = 3
    //es buena practica asignar valores , asi si alguien reordena los elements del enum, los valores de la base de datos no se corrompen
}