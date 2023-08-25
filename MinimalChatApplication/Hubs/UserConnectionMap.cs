//namespace MinimalChatApplication.Hubs
//{
//    public class UserConnectionMap
//    {
//        private readonly Dictionary<string, List<string>> _userConnections = new Dictionary<string, List<string>>();

//        public void AddConnection(string userId, string connectionId)
//        {
//            if (!_userConnections.ContainsKey(userId))
//            {
//                _userConnections[userId] = new List<string>();
//            }

//            if (!_userConnections[userId].Contains(connectionId))
//            {
//                _userConnections[userId].Add(connectionId);
//            }
//        }

//        //public void RemoveConnection(string userId, string connectionId)
//        //{
//        //    if (_userConnections.ContainsKey(userId))
//        //    {
//        //        _userConnections[userId].Remove(connectionId);

//        //        if (_userConnections[userId].Count == 0)
//        //        {
//        //            _userConnections.Remove(userId);
//        //        }
//        //    }
//        //}

//        public IReadOnlyList<string> GetConnections(string userId)
//        {
//            if (_userConnections.ContainsKey(userId))
//            {
//                return _userConnections[userId];
//            }

//            return new List<string>();
//        }
//    }
//}

