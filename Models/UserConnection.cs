using System.Collections.Generic;

namespace WebChat.Models
{
    public class Conversation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();  // ID único para a conversa
        public List<string> Participants { get; set; } = new List<string>();  // IDs dos participantes
    }

    public class UserConnection
    {
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; } = string.Empty;
        public string ChatRoom { get; set; } = string.Empty;
        public string ConversationId { get; set; } = string.Empty; // Adiciona o ID da conversa aqui
    }
}
