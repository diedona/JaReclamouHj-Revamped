using MediatR;

namespace JaReclamouHoje.Application.Features.Complaints.CreateComplaint;

public record CreateComplaintCommand(
    string Title, 
    string Description
) : IRequest<CreateComplaintResponse>;
