using Microsoft.AspNetCore.Mvc;
using WebChat.DataService;
using WebChat.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly InMemoryDb _db;

        public ChatController(InMemoryDb db)
        {
            _db = db;
        }

        // Endpoint para enviar uma mensagem em uma conversa existente
        [HttpPost("send-message")]
        public IActionResult SendMessage([FromBody] MessageDto messageDto)
        {
            // Verifica se o DTO contém os dados necessários
            if (messageDto == null || string.IsNullOrEmpty(messageDto.ConversationId) || string.IsNullOrEmpty(messageDto.Content))
            {
                return BadRequest("Dados da mensagem inválidos.");
            }

            // Busca a conversa no banco de dados em memória
            if (!_db.Conversations.TryGetValue(messageDto.ConversationId, out var conversation))
            {
                return NotFound("Conversa não encontrada.");
            }

            // Aqui você poderia adicionar lógica para armazenar a mensagem na conversa, se necessário
            // Por exemplo: conversation.Messages.Add(new Message { Content = messageDto.Content, SenderId = messageDto.SenderId });

            // Simula o envio da mensagem para todos os participantes (lógica a ser adaptada para envio real)
            foreach (var participant in conversation.Participants)
            {
                // Lógica para notificar os participantes (via SignalR, por exemplo)
            }

            return Ok("Mensagem enviada com sucesso.");
        }

        // Endpoint para obter todas as conversas de um usuário
        [HttpGet("conversations/{userId}")]
        public ActionResult<IEnumerable<Conversation>> GetUserConversations(string userId)
        {
            var conversations = _db.Conversations.Values
                .Where(c => c.Participants.Contains(userId))
                .ToList();

            if (!conversations.Any())
            {
                return NotFound("Nenhuma conversa encontrada.");
            }

            return Ok(conversations);
        }

        // Método para buscar uma conversa específica pelo ID único
        [HttpGet("conversation/{conversationId}")]
        public ActionResult<Conversation> GetConversationById(string conversationId)
        {
            if (_db.Conversations.TryGetValue(conversationId, out var conversation))
            {
                return Ok(conversation);
            }

            return NotFound("Conversa não encontrada.");
        }

        // Endpoint para iniciar uma nova conversa ou retornar uma existente
        [HttpPost("start")]
        public ActionResult<Conversation> StartConversation([FromBody] List<string> userIds)
        {
            if (userIds == null || userIds.Count < 2)
            {
                return BadRequest("É necessário fornecer pelo menos dois IDs de usuários.");
            }

            // Verifica se já existe uma conversa com os mesmos participantes
            var existingConversation = _db.Conversations.Values
                .FirstOrDefault(c => c.Participants.All(userIds.Contains) && c.Participants.Count == userIds.Count);

            if (existingConversation != null)
            {
                return Ok(existingConversation);  // Retorna a conversa existente
            }

            // Cria uma nova conversa
            var conversation = new Conversation { Participants = userIds };
            _db.Conversations[conversation.Id] = conversation;

            return CreatedAtAction(nameof(GetConversationById), new { conversationId = conversation.Id }, conversation);
        }
    }

    // DTO para enviar mensagens
    public class MessageDto
    {
        public string ConversationId { get; set; }  // ID da conversa
        public string SenderId { get; set; }        // ID do remetente
        public string Content { get; set; }         // Conteúdo da mensagem
    }
}
