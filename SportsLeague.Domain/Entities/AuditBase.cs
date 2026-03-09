namespace SportsLeague.Domain.Entities;
/*
namespace funciona como un contenedor logico

Esta clase funciona como estructura base comun es 
lo primero que se hace se usa para evitar repetir el codigo
que nace de crear otras clases con constructores 
garantiza que todas la entidades tengan identificador unico
Fecha de creacion y fecha de modificacion, aunque esto puede variar dependiendo las reglas de negocio
*/
public abstract class AuditBase
//el abstract no permite instanciar directamente pero si heredar
{
    public int Id{get; set;} //esta bien que se pueda cambiar el Id???
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow; //se asigna hora y fecha actual en UTC
    public DateTime? UpdateAt {get; set;}
}

//AuditBase no representa algo real del negocio solo es una base estructural