namespace HelpDesk.BLL.DTOs.Tickets
{
    public class CreateTicketCommentDto
    {
        public int TicketId { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}