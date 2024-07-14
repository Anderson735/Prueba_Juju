using DataAccess;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Business
{
    public class BaseService<TEntity> where TEntity : class, new()
    {
        protected BaseModel<TEntity> _BaseModel;
        private readonly CustomerService _customerService;


        public BaseService(BaseModel<TEntity> baseModel, CustomerService customerService)
        {
            _BaseModel = baseModel;
            _customerService = customerService;
        }

        #region Repository


        /// <summary>
        /// Consulta todas las entidades
        /// </summary>
        public virtual IQueryable<TEntity> GetAll()
        {
            return _BaseModel.GetAll;
        }

        /// <summary>
        /// Crea un entidad (Guarda)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TEntity CreateCustomer(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Obtener el nombre de la entidad TEntity utilizando GetPropertyName
            string entityName = GetPropertyName(entity);
            bool result = FindByName(entityName);

            // Verificar si ya existe una entidad con el mismo nombre
            if (result)
            {
                throw new ArgumentNullException($"La entidad {entityName} ya existe.");
            }

            return _BaseModel.Create(entity);
        }

        public string GetPropertyName(TEntity entity)
        {
            // Obtener el tipo de TEntity en tiempo de ejecución
            Type entityType = typeof(TEntity);

            // Buscar la propiedad "Name" en el tipo de TEntity
            PropertyInfo propertyInfo = entityType.GetProperty("Name");

            if (propertyInfo != null)
            {
                // Obtener el valor de la propiedad "Name" del objeto TEntity
                object value = propertyInfo.GetValue(entity);

                // Convertir el valor a string (o al tipo que necesites)
                if (value != null)
                {
                    return value.ToString();
                }
            }
            // Manejar el caso donde la propiedad "Name" no existe en TEntity
            throw new ArgumentException($"La entidad {entityType.Name} no tiene una propiedad 'Name'.");
        }

        public virtual bool FindByName(string name)
        {
            TEntity entity = _BaseModel.GetByCondition(e => GetPropertyName(e) == name);
            return entity != null;
        }

        /// <summary>
        /// Actualiza la entidad (GUARDA)
        /// </summary>
        /// <param name="editedEntity">Entidad editada</param>
        /// <param name="originalEntity">Entidad Original sin cambios</param>
        /// <param name="changed">Indica si se modifico la entidad</param>
        /// <returns></returns>
        public virtual TEntity Update(object id, TEntity editedEntity, out bool changed)
        {
            TEntity originalEntity = _BaseModel.FindById(id);

            if (originalEntity == null)
            {
                throw new ArgumentException($"No se encontró la entidad con el ID especificado: {id}");
            }

            TEntity updatedEntity = _BaseModel.Update(editedEntity, out changed);

            return updatedEntity;
        }


        /// <summary>
        /// Elimina una entidad (Guarda)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TEntity Delete(TEntity entity)
        {
            return _BaseModel.Delete(entity);
        }

        /// <summary>
        /// Guardar cambios
        /// </summary>
        public virtual void SaveChanges()
        {
            _BaseModel.SaveChanges();
        }
        #endregion

        public List<TEntity> CreateMultiplesPosts(List<TEntity> entities)
        {

            // Crear cada entidad utilizando el repositorio correspondiente
            var createdEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                var createdEntity = CreateMultiplePost(entity);
                createdEntities.Add(createdEntity);
            }

            return createdEntities;
        }


        public virtual TEntity CreateMultiplePost(TEntity entity)
        {
            return _BaseModel.Create(entity);
        }

        /// <summary>
        /// Elimina una entidad (Guarda)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TEntity CreatePost(TEntity entity)
        {
            ValidateCustomerExistence(entity);

            AdjustBodyLength(entity);

            AdjustCategory(entity);

            return _BaseModel.Create(entity);
        }

        private void ValidateCustomerExistence(TEntity entity)
        {
            int customerId = ExtractCustomerIdPost(entity);

            if (!_customerService.CustomerExists(customerId))
            {
                throw new ArgumentException($"No se encontró el cliente con el ID especificado: {customerId}");
            }
        }

        private void AdjustBodyLength(TEntity entity)
        {
            string body = ExtractBodyPost(entity);

            if (body.Length > 20)
            {
                // Cortar el texto a 9 caracteres y agregar "..."
                string shortenedBody = body.Substring(0, 9) + "...";

                // Actualizar la propiedad "Body" en la entidad TEntity
                var propertyInfo = entity.GetType().GetProperty("Body");
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(entity, shortenedBody);
                }
            }
        }

        private void AdjustCategory(TEntity entity)
        {
            int type = ExtractTypePost(entity);
            string category = null;

            switch (type)
            {
                case 1:
                    category = "Farándula";
                    break;
                case 2:
                    category = "Política";
                    break;
                case 3:
                    category = "Futbol";
                    break;
                default:
                    // No se hace nada, se mantiene la categoría ingresada por el usuario
                    break;
            }

            if (category != null)
            {
                entity.GetType().GetProperty("Category").SetValue(entity, category);
            }
        }

        public string ExtractBodyPost(TEntity entity)
        {
            // Obtener el tipo de TEntity en tiempo de ejecución
            Type entityType = typeof(TEntity);

            // Buscar la propiedad "Name" en el tipo de TEntity
            PropertyInfo propertyInfo = entityType.GetProperty("Body");

            if (propertyInfo != null)
            {
                // Obtener el valor de la propiedad "Name" del objeto TEntity
                object value = propertyInfo.GetValue(entity);

                // Convertir el valor a string (o al tipo que necesites)
                if (value != null)
                {
                    return value.ToString();
                }
            }
            // Manejar el caso donde la propiedad "Name" no existe en TEntity
            throw new ArgumentException($"La entidad {entityType.Name} no tiene una propiedad 'Body'.");
        }

        public int ExtractCustomerIdPost(TEntity entity)
        {
            // Obtener el tipo de TEntity en tiempo de ejecución
            Type entityType = typeof(TEntity);
            

            // Buscar la propiedad "Name" en el tipo de TEntity
            PropertyInfo propertyInfo = entityType.GetProperty("CustomerId");

            if (propertyInfo != null)
            {
                // Obtener el valor de la propiedad "CustomerId" del objeto TEntity
                object value = propertyInfo.GetValue(entity);

                // Convertir el valor a string (o al tipo que necesites)
                if (value != null)
                {
                    return int.Parse(value.ToString());
                }
            }
            // Manejar el caso donde la propiedad "IdCustomer" no existe en TEntity
            throw new ArgumentException($"La entidad {entityType.Name} no tiene una propiedad 'IdCustomer'.");
        }

        public int ExtractTypePost(TEntity entity)
        {
            // Obtener el tipo de TEntity en tiempo de ejecución
            Type entityType = typeof(TEntity);

            // Buscar la propiedad "Name" en el tipo de TEntity
            PropertyInfo propertyInfo = entityType.GetProperty("Type");

            if (propertyInfo != null)
            {
                // Obtener el valor de la propiedad "CustomerId" del objeto TEntity
                object value = propertyInfo.GetValue(entity);

                // Convertir el valor a string (o al tipo que necesites)
                if (value != null)
                {
                    return int.Parse(value.ToString());
                }
            }
            // Manejar el caso donde la propiedad "IdCustomer" no existe en TEntity
            throw new ArgumentException($"La entidad {entityType.Name} no tiene una propiedad 'IdCustomer'.");
        }

        public string ExtractCategoryPost(TEntity entity)
        {
            // Obtener el tipo de TEntity en tiempo de ejecución
            Type entityType = typeof(TEntity);

            // Buscar la propiedad "Category" en el tipo de TEntity
            PropertyInfo propertyInfo = entityType.GetProperty("Category");


            if (propertyInfo != null)
            {
                // Obtener el valor de la propiedad "Category" del objeto TEntity
                object value = propertyInfo.GetValue(entity);

                // Convertir el valor a string (o al tipo que necesites)
                if (value != null)
                {
                    return value.ToString();
                }
            }
            // Manejar el caso donde la propiedad "Category" no existe en TEntity
            throw new ArgumentException($"La entidad {entityType.Name} no tiene una propiedad 'Category'.");
        }

        public bool DeleteCustomerAndPosts(int customerId)
        {
            // Verificar si el cliente existe antes de intentar eliminarlo
            if (!_customerService.CustomerExists(customerId))
            {
                throw new ArgumentException($"No se encontró el cliente con el ID especificado: {customerId}");
            }

            // Llamar al método del servicio de cliente para eliminar cliente y posts
            _customerService.DeleteCustomerAndPosts(customerId);
            return true;
        }

    }
}
