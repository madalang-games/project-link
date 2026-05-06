using ProjectLink.Domain.Entities;
using ProjectLink.Domain.Interfaces;

namespace ProjectLink.Application.Progress;

public record GetProgressQuery(string UserId, CancellationToken Ct);

public class GetProgressQueryHandler
{
    private readonly IProgressRepository _repo;

    public GetProgressQueryHandler(IProgressRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<StageProgress>> HandleAsync(GetProgressQuery query)
        => _repo.GetAllAsync(query.UserId, query.Ct);
}
