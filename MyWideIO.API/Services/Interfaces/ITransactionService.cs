namespace MyWideIO.API.Services.Interfaces
{
    public interface ITransactionService
    {
        public Task BeginTransactionAsync();
        public Task CommitAsync();
        public Task RollbackAsync();
    }
}
