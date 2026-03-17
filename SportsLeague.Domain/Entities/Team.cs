namespace SportsLeague.Domain.Entities;
/*Organiza la clase dentro del modulo sistema domain y entities respectivamente
 */
public class Team :AuditBase
//Team es una entidad del dominio representa un objeto real del negocio un equipo deportivo
//hereda AuditBase por lo que obtiene los 3 atributos de esta por defecto

{


//string.empty pone por defecto "" cuando es vacio es basicamente prefierouna cadena vacia en lugar de null
    public string Name{get; set;}=string.Empty;
    public string City {get; set;}=string.Empty;
    public string Stadium {get;set;}=string.Empty;
    public string? LogoUrl {get; set;}

/*FoundedDate es un tipo valor 
Los tipo valor no pueden ser null a menos que se use ? siempre tienen un valor por defecto(01/01/0001 00:00:00)

*/
    public DateTime FoundedDate {get; set;}

//Navigation Property - coleccion de jugadores

    public ICollection<Player> Players {get; set;} = new List<Player>();
}