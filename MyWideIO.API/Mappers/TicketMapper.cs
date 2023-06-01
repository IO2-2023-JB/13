using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;

namespace MyWideIO.API.Mappers
{
    public static class TicketMapper
    {
        public static GetTicketDto MapTicketModelToGetTicketDto(TicketModel ticketModel)
        {
            return new GetTicketDto
            {
                TicketId = ticketModel.Id,
                Reason = ticketModel.Reason,
                TargetId = ticketModel.TargetId,
                TargetType = ticketModel.TargetType,
                Status =  new GetTicketStatusDto{Status = ticketModel.Status}, // ?
                Response = ticketModel.Response ?? "",
                SubmitterId = ticketModel.SubmitterId
            };
        }
    }
}
