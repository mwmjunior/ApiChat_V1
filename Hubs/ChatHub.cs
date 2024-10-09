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

        // M�todo para iniciar ou pegar uma conversa existente
        public async Task StartConversation(string targetUserId)
        {
            string connectionId = Context.ConnectionId;

            // Verifica se o usu�rio est� conectado
            if (_db.Connections.TryGetValue(connectionId, out UserConnection userConnection))
            {
                // Tenta encontrar uma conversa existente entre os dois usu�rios
                var conversation = _db.Conversations.Values
                    .FirstOrDefault(c => c.Participants.Contains(userConnection.UserId)
                                         && c.Participants.Contains(targetUserId));

                if (conversation == null)
                {
                    // Se n�o existir uma conversa, cria uma nova
                    conversation = new Conversation();
                    conversation.Participants.Add(userConnection.UserId);
                    conversation.Participants.Add(targetUserId);

                    // Armazena a conversa no banco de dados em mem�ria
                    _db.Conversations[conversation.Id] = conversation;
                }

                // Atualiza a conex�o do usu�rio com o ID da conversa
                userConnection.ConversationId = conversation.Id;

                // Adiciona o usu�rio ao grupo da conversa
                await Groups.AddToGroupAsync(connectionId, conversation.Id);

                // Notifica o usu�rio que a conversa foi iniciada
                await Clients.Caller.SendAsync("ConversationStarted", conversation.Id);
            }
        }

        // Envia uma mensagem dentro de uma conversa
        public async Task SendMessage(string message)
        {
            if (_db.Connections.TryGetValue(Context.ConnectionId, out UserConnection conn))
            {
                // Verifica se a conex�o est� associada a uma conversa
                if (!string.IsNullOrEmpty(conn.ConversationId))
                {
                    // Envia a mensagem para os usu�rios no grupo da conversa
                    await Clients.Group(conn.ConversationId).SendAsync("ReceiveMessage", conn.UserName, message);
                }
            }
        }
    }
}
