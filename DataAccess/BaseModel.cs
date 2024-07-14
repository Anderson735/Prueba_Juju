using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess
{
    public class BaseModel<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// Contexto
        /// </summary>
        JujuTestContext _context;
        /// <summary>
        /// Entidad
        /// </summary>
        protected DbSet<TEntity> _dbSet;

        private readonly CustomerService _customerService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public BaseModel(JujuTestContext context, CustomerService customerService)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _customerService = customerService;
        }


        /// <summary>
        /// Consulta todas las entidades
        /// </summary>
        public virtual IQueryable<TEntity> GetAll
        {
            get { return _dbSet; }
        }

        /// <summary>
        /// Consulta una entidad por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity FindById(object id)
        {
            return _dbSet.Find(id);
        }



        /// <summary>
        /// Crea un entidad (Guarda)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TEntity Create(TEntity entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();

            return entity;
        }


  

        /// <summary>
        /// Obtiene una entidad por su nombre.
        /// </summary>
        /// <param name="name">Nombre de la entidad a buscar</param>
        /// <returns>La entidad encontrada o null si no se encontró ninguna entidad con ese nombre.</returns>
        public TEntity GetByCondition(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Actualiza la entidad (GUARDA)
        /// </summary>
        /// <param name="editedEntity">Entidad editada</param>
        /// <param name="originalEntity">Entidad Original sin cambios</param>
        /// <param name="changed">Indica si se modifico la entidad</param>
        /// <returns></returns>
        public virtual TEntity Update(TEntity editedEntity, out bool changed)
        {
            changed = false;

            var entry = _context.Entry(editedEntity);
            var keyValues = entry.Metadata.FindPrimaryKey()
                                          .Properties
                                          .Select(x => entry.Property(x.Name).CurrentValue)
                                          .ToArray();

            TEntity originalEntity = _context.Set<TEntity>().Find(keyValues);

            if (originalEntity == null)
            {
                throw new ArgumentException($"No se encontró la entidad con la clave especificada: {keyValues}");
            }

            _context.Entry(originalEntity).CurrentValues.SetValues(editedEntity);      
            changed = _context.Entry(originalEntity).State == EntityState.Modified;
            _context.SaveChanges();

            return originalEntity;
        }


        /// <summary>
        /// Elimina una entidad (Guarda)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TEntity Delete(TEntity entity)
        {
            _dbSet.Remove(entity);

            _context.SaveChanges();

            return entity;
        }


        /// <summary>
        /// Guardar cambios
        /// </summary>
        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }


    }
}
