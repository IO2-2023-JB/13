using Microsoft.EntityFrameworkCore.Storage;
using MyWideIO.API.Data;
using MyWideIO.API.Services.Interfaces;

namespace MyWideIO.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public TransactionService(ApplicationDbContext context)
        {
            _dbContext = context;
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }
        public async Task CommitAsync()
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException("Transaction has not been started");
            }
            await _transaction.CommitAsync();
            _transaction = null;
        }
        public async Task RollbackAsync()
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException("Transaction has not been started");
            }
            await _transaction.RollbackAsync();
            _transaction = null;
        }
    }
}
