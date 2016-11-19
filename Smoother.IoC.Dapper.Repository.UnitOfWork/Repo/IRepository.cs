﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Smoother.IoC.Dapper.Repository.UnitOfWork.Connection;
using Smoother.IoC.Dapper.Repository.UnitOfWork.UoW;

namespace Smoother.IoC.Dapper.Repository.UnitOfWork.Repo
{
    public interface IRepository<TSession, TEntity, TPk>
        where TEntity : class, ITEntity<TPk>
        where TSession : ISession
    {
        TEntity Get(TPk key, IUnitOfWork<TSession> unitOfWork = null);
        Task<TEntity> GetAsync(TPk key, IUnitOfWork<TSession> unitOfWork = null);
        IEnumerable<TEntity>  GetAll(IUnitOfWork<TSession> unitOfWork = null);
        Task<IEnumerable<TEntity>> GetAllAsync(IUnitOfWork<TSession> unitOfWork = null);
        int SaveOrUpdate(TEntity entity, IUnitOfWork<TSession> unitOfWork = null);
        Task<int> SaveOrUpdateAsync(TEntity entity, IUnitOfWork<TSession> unitOfWork = null);
    }
}