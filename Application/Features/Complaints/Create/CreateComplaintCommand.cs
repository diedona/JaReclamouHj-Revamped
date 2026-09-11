using MediatR;

namespace JaReclamouHoje.Application.Features.Complaints.Create;

public record CreateComplaintCommand(
    string Title, 
    string Description
) : IRequest<CreateComplaintResponse>;
