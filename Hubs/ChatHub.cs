using Microsoft.AspNetCore.SignalR;
using WebChat.DataService;
using WebChat.Models;
using System.Linq;
using System.Threading.Tasks;

namespace WebChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly InMemoryDb _db;

        public ChatHub(InMemoryDb db)
        {
            _db = db;
        }

        // Método para iniciar ou pegar uma conversa existente
        public async Task StartConversation(string targetUserId)
        {
            string connectionId = Context.ConnectionId;

            // Verifica se o usuário está conectado
            if (_db.Connections.TryGetValue(connectionId, out UserConnection userConnection))
            {
                // Tenta encontrar uma conversa existente entre os dois usuários
                var conversation = _db.Conversations.Values
                    .FirstOrDefault(c => c.Participants.Contains(userConnection.UserId)
                                         && c.Participants.Contains(targetUserId));

                if (conversation == null)
                {
                    // Se não existir uma conversa, cria uma nova
                    conversation = new Conversation();
                    conversation.Participants.Add(userConnection.UserId);
                    conversation.Participants.Add(targetUserId);

                    // Armazena a conversa no banco de dados em memória
                    _db.Conversations[conversation.Id] = conversation;
                }

                // Atualiza a conexão do usuário com o ID da conversa
                userConnection.ConversationId = conversation.Id;

                // Adiciona o usuário ao grupo da conversa
                await Groups.AddToGroupAsync(connectionId, conversation.Id);

                // Notifica o usuário que a conversa foi iniciada
                await Clients.Caller.SendAsync("ConversationStarted", conversation.Id);
            }
        }

        // Envia uma mensagem dentro de uma conversa
        public async Task SendMessage(string message)
        {
            if (_db.Connections.TryGetValue(Context.ConnectionId, out UserConnection conn))
            {
                // Verifica se a conexão está associada a uma conversa
                if (!string.IsNullOrEmpty(conn.ConversationId))
                {
                    // Envia a mensagem para os usuários no grupo da conversa
                    await Clients.Group(conn.ConversationId).SendAsync("ReceiveMessage", conn.UserName, message);
                }
            }
        }
    }
}
