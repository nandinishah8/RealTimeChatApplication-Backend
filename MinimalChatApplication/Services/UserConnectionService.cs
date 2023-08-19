//using MinimalChatApplication.Interfaces;
//using StackExchange.Redis;

//namespace MinimalChatApplication.Services
//{
//    public class UserConnectionService : IUserConnectionService
//    {
//        private readonly IDatabase _redisDb;

//        public UserConnectionService(ConnectionMultiplexer multiplexer)
//        {
//            _redisDb = multiplexer.GetDatabase();
//        }

//        public async Task AddConnectionAsync(string userId, string connectionId)
//        {
//            await _redisDb.StringSetAsync(userId, connectionId);
//        }

//        public async Task<string> GetConnectionIdAsync(string userId)
//        {
//            return await _redisDb.StringGetAsync(userId);
//        }
//    }
//}
