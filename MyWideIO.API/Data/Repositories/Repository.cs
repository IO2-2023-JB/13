﻿using MyWideIO.API.Data.IRepositories;

namespace MyWideIO.API.Data.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task AddAsync(T entity, bool saveChanges)
        {
            await _dbContext.AddAsync(entity);
            if (saveChanges)
                await _dbContext.SaveChangesAsync();
        }

        public virtual async Task AddAsync(IEnumerable<T> entities, bool saveChanges)
        {
            await _dbContext.AddRangeAsync(entities);
            if (saveChanges)
                await _dbContext.SaveChangesAsync();
        }

        public virtual async Task RemoveAsync(T entity, bool saveChanges)
        {
            _dbContext.Remove(entity);
            if (saveChanges)
                await _dbContext.SaveChangesAsync();
        }

        public virtual async Task RemoveAsync(IEnumerable<T> entities, bool saveChanges)
        {
            _dbContext.RemoveRange(entities);
            if (saveChanges)
                await _dbContext.SaveChangesAsync();
        }


        public virtual async Task<T?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.FindAsync<T>(id, cancellationToken);
        }

        public virtual async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity, bool saveChanges)
        {
            _dbContext.Update(entity);
            if (saveChanges)
                await _dbContext.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(IEnumerable<T> entities, bool saveChanges)
        {
            _dbContext.UpdateRange(entities);
            if (saveChanges)
                await _dbContext.SaveChangesAsync();
        }
    }
}
