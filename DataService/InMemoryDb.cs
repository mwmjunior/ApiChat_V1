using System.Collections.Concurrent;
using WebChat.Models;

namespace WebChat.DataService
{
    public class InMemoryDb
    {
        public ConcurrentDictionary<string, UserConnection> Connections { get; } = new();
        public ConcurrentDictionary<string, Conversation> Conversations { get; } = new();  // Propriedade para acessar as conversas
    }
}
