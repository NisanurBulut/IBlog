﻿using Medusa.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Medusa.DataAccess.Interface
{
    public interface IGenericDal<tEntity> where tEntity : class, ITable, new()
    {
        Task<List<tEntity>> GetAllAsync();
        Task<List<tEntity>> GetAllAsync(Expression<Func<tEntity, bool>> filter);
        Task<List<tEntity>> GetAllAsync<tKey>(Expression<Func<tEntity, tKey>> keySelector);
        Task<List<tEntity>> GetAllAsync<tKey>(Expression<Func<tEntity, bool>> filter, Expression<Func<tEntity, tKey>> keySelector);
        Task<tEntity> GetAsync(Expression<Func<tEntity, bool>> filter);
        Task<tEntity> FindByIdAsync(int id);
        Task AddAsync(tEntity item);

        Task UpdateAsync(tEntity item);
        Task RemoveAsync(tEntity item);

    }
}
