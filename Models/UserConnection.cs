using System.Collections.Generic;

namespace WebChat.Models
{
    public class Conversation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();  // ID �nico para a conversa
        public List<string> Participants { get; set; } = new List<string>();  // IDs dos participantes
        public List<Message> Messages { get; set; } = new List<Message>();  // Adiciona a lista de mensagens
    }

    public class Message
    {
        public string SenderId { get; set; }  // ID do remetente
        public string Content { get; set; }    // Conte�do da mensagem
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;  // Timestamp da mensagem
    }

    public class UserConnection
    {
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; } = string.Empty;
        public string ChatRoom { get; set; } = string.Empty;
        public string ConversationId { get; set; } = string.Empty; // Adiciona o ID da conversa aqui
    }
}
