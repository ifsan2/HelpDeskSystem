namespace HelpDesk.BLL.DTOs.Tickets
{
    public class TicketCommentDto
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsInternal { get; set; }
    }
}