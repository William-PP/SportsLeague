using SportsLeague.Domain.Entities;
namespace SportsLeague.Domain.Interfaces.Repositories;

/*La funcion de un repositorio es acceder a la base de datos, guardar entidades
 * consultar entidades y eliminar entidades
 * la idea es no escribir el mismo codigo para cada entidad
*/
public interface IGenericRepository<T> where T : AuditBase //<T> t es generico funcionara para cualqueir entidad
   //where es una restriccion t debe ser si o si alguien que herede de auditbase la razon por que todas las entidades tienen lo mismos de auditbae
    //la interface obliga a implementar los metodos a quien la use
{

    //Todo esto e sprogramacion asincrona
    //task representa una operacio que se ejecuta de manera asincrona(significa que la operacion puede tardar)
    //esto es a fin de que no se ejecuten muchas cosas y se bloquee el programa
    //por eso se usa Async
    Task<IEnumerable<T>> GetAllAsync();//obtiene todos los registros de tipo t
    Task<T?> GetByIdAsync(int id);// busca una entidad por su id el ? es porque puede devolver null
    Task<T> CreateAsync (T entity);// crea un registro en base de datos
    Task UpdateAsync (T entity);//actualiza una entidad existente no devuelve nada
    Task DeleteAsync (int id); //Elimina un registro por Id.
    Task<bool> ExistsAsync(int id); //Verifica si el registro existe.
}
