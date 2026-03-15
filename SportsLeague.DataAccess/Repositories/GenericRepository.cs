using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class GenericRepository<T>: IGenericRepository<T> where T: AuditBase
{
    protected readonly LeagueDbContext _context;//_context Representa la conexion a la base de datos
    protected readonly DbSet<T> _dbSet;// _dbSet Representa la conexion a la base de dato
    //protected solo la pueden usar esta clase y las que hereden de ella
    //reandonly es para que una vez ingresado no se pueda cambiar

    public GenericRepository(LeagueDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();//duda???????
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();// es equivalente a una cosulta sql para encontrar una lista
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);// busca clave primaria equivalente a consulta sql
    }
    public async Task<T> CreateAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;//tiempo actual en utc
        entity.UpdateAt = null;// limpia el campo de actualizacion
        await _dbSet.AddAsync(entity);//agrega el objeto al contextob
        await _context.SaveChangesAsync();//guarda los cambios en la base de datos
        return entity;//retorna el objeto guardado
    }

    public async Task UpdateAsync (T entity)
    {
        entity.UpdateAt= DateTime.UtcNow;//actualiza la fecha de actualizacion
        _dbSet.Update(entity);//marca la entidad como modificada
        await _context.SaveChangesAsync();//guarda cambios
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);//busca id
        if(entity != null)//verifica que exista
        {
            _dbSet.Remove(entity);// verifica que exista
            await _context.SaveChangesAsync();//guarda cambios
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id);//verifica que existe almenos un registro que cumpla la condicion
    }

}
