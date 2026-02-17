using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Robust.Shared.Prototypes;

namespace Content.Shared.Mind.Filters;

/// <summary>
/// A mind filter that requires minds to have a specific job.
/// This uses mind roles, not ID cards.
/// </summary>
public sealed partial class JobMindFilter : MindFilter
{
    [DataField] // L5 - don't make required + make optional for below
    public ProtoId<JobPrototype>? Job;

    /// <summary>
    /// L5 - porting of PickRandomPersonComponent.OnlyChoosableJobs to mind role
    /// refactor. This is kinda hacky it should be a new filer probably but meh.
    /// </summary>
    [DataField]
    public bool OnlyChoosableJobs;

    protected override bool ShouldRemove(Entity<MindComponent> mind, EntityUid? exclude, IEntityManager entMan, SharedMindSystem mindSys)
    {
        var jobSys = entMan.System<SharedJobSystem>();

        // Begin L5 additions - OnlyChoosableJobs support
        if (OnlyChoosableJobs
            && jobSys.MindTryGetJob(mind, out var jobProto)
            && !jobProto.SetPreference)
            return false;
        // End L5 additions

        return Job is not null && jobSys.MindHasJobWithId(mind, Job); // L5 null check
    }
}
