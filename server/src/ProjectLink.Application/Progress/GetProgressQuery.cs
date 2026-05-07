using ProjectLink.Contracts.Progress;
using ProjectLink.Domain.Interfaces;

namespace ProjectLink.Application.Progress;

public record GetProgressQuery(string UserId, CancellationToken Ct);

public class GetProgressQueryHandler
{
    private readonly IProgressRepository _repo;
    private readonly IStaticDataService  _staticData;

    public GetProgressQueryHandler(IProgressRepository repo, IStaticDataService staticData)
    {
        _repo       = repo;
        _staticData = staticData;
    }

    public async Task<ProgressResponse> HandleAsync(GetProgressQuery query)
    {
        var records     = (await _repo.GetAllAsync(query.UserId, query.Ct)).ToList();
        var clearedIds  = records.Select(r => r.StageId).ToHashSet();
        var allStages   = _staticData.GetAllStages();

        var entries = allStages
            .OrderBy(s => s.StageId)
            .Select(s =>
            {
                var record     = records.FirstOrDefault(r => r.StageId == s.StageId);
                var isUnlocked = s.StageId == 1 || clearedIds.Contains(s.StageId - 1);
                return new StageProgressEntry
                {
                    StageId    = s.StageId,
                    Stars      = record?.Stars ?? 0,
                    IsUnlocked = isUnlocked,
                    ClearedAt  = record?.ClearedAt.ToString("O"),
                };
            })
            .ToList();

        return new ProgressResponse { Stages = entries };
    }
}
