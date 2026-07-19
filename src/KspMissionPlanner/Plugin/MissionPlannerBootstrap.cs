using KspIntegration.Mission;
using KspMissionPlanner.Planning;

namespace KspMissionPlanner.Plugin;

public sealed class MissionPlannerBootstrap
{
    private readonly IKspGameContext _gameContext;
    private readonly IWaypointCatalog? _waypointCatalog;
    private readonly IMissionProgramStore _missionStore;
    private readonly IGravityAssistScout _gravityAssistScout;

    public MissionPlannerBootstrap(
        IKspGameContext gameContext,
        IWaypointCatalog? waypointCatalog = null,
        IMissionProgramStore? missionStore = null,
        IGravityAssistScout? gravityAssistScout = null)
    {
        _gameContext = gameContext;
        _waypointCatalog = waypointCatalog;
        _missionStore = missionStore ?? new InMemoryMissionProgramStore();
        _gravityAssistScout = gravityAssistScout ?? new NoOpGravityAssistScout();
    }

    public MissionPlan BuildReversePlan(MissionObjective objective)
    {
        var missionProgram = CreateSingleObjectiveMissionProgram(objective);
        return BuildReversePlan(missionProgram);
    }

    public MissionPlan BuildReversePlan(MissionProgram missionProgram)
    {
        if (missionProgram is null)
        {
            throw new System.ArgumentNullException(nameof(missionProgram));
        }

        // This is the integration seam where KSP state is read and your math modules are called.
        var universalTimeSeconds = _gameContext.UniversalTimeSeconds;

        ValidateWaypointTargets(missionProgram);

        var allGoals = FlattenGoals(missionProgram.Stages);
        var transitionAssessments = new System.Collections.Generic.List<MissionTransitionAssessment>();
        var legs = new System.Collections.Generic.List<MissionLeg>();

        for (var index = 0; index < allGoals.Count - 1; index++)
        {
            var fromGoal = allGoals[index];
            var toGoal = allGoals[index + 1];
            var gravityAssistCandidates = _gravityAssistScout.FindCandidates(fromGoal, toGoal, universalTimeSeconds);

            transitionAssessments.Add(new MissionTransitionAssessment
            {
                FromGoalId = fromGoal.GoalId,
                ToGoalId = toGoal.GoalId,
                DirectTransferDeltaVMetersPerSecond = 0.0,
                DirectTransferFlightTimeSeconds = 0.0,
                GravityAssistCandidates = gravityAssistCandidates,
            });

            legs.Add(new MissionLeg
            {
                LegType = InferLegType(toGoal),
                Description = $"{fromGoal.Name} -> {toGoal.Name}",
                FromGoalId = fromGoal.GoalId,
                ToGoalId = toGoal.GoalId,
                DepartureSurfaceTarget = fromGoal.SurfaceTarget,
                ArrivalSurfaceTarget = toGoal.SurfaceTarget,
                EstimatedDeltaVMetersPerSecond = 0.0,
                EstimatedCoastTimeSeconds = 0.0,
                GravityAssistCandidates = gravityAssistCandidates,
            });
        }

        var terminalGoal = allGoals.Count > 0 ? allGoals[allGoals.Count - 1] : null;
        var objective = terminalGoal is null ? new MissionObjective() : new MissionObjective
        {
            Name = terminalGoal.Name,
            TargetBody = terminalGoal.TargetBody,
            ObjectiveType = ConvertGoalType(terminalGoal.GoalType),
            SurfaceTarget = terminalGoal.SurfaceTarget,
        };

        return new MissionPlan
        {
            MissionId = missionProgram.MissionId,
            MissionName = missionProgram.Name,
            Objective = objective,
            Stages = missionProgram.Stages,
            TransitionAssessments = transitionAssessments,
            Legs = legs,
        };
    }

    public MissionProgram SaveMissionProgram(MissionProgram missionProgram)
    {
        _missionStore.Save(missionProgram);
        return missionProgram;
    }

    public System.Collections.Generic.IReadOnlyList<MissionProgram> GetMissionPrograms()
    {
        return _missionStore.GetAll();
    }

    public bool DeleteMissionProgram(string missionId)
    {
        return _missionStore.Delete(missionId);
    }

    public MissionProgram EnsureParkingOrbitGoals(
        MissionProgram missionProgram,
        ParkingOrbitTemplate template,
        bool addBeforeLanding,
        bool addAfterLanding)
    {
        return ParkingOrbitAugmentor.EnsureParkingOrbitsAroundLandingGoals(
            missionProgram,
            template,
            addBeforeLanding,
            addAfterLanding);
    }

    private void ValidateWaypointTargets(MissionProgram missionProgram)
    {
        if (_waypointCatalog is null)
        {
            return;
        }

        foreach (var stage in missionProgram.Stages)
        {
            foreach (var goal in stage.Goals)
            {
                if (goal.GoalType == MissionGoalType.SurfaceWaypoint && goal.SurfaceTarget is not null)
                {
                    _waypointCatalog.TryGetWaypoint(
                        goal.SurfaceTarget.BodyName,
                        goal.SurfaceTarget.WaypointName,
                        out _);
                }
            }
        }
    }

    private static System.Collections.Generic.List<MissionGoal> FlattenGoals(System.Collections.Generic.IReadOnlyList<MissionStage> stages)
    {
        var goals = new System.Collections.Generic.List<MissionGoal>();
        foreach (var stage in stages)
        {
            foreach (var goal in stage.Goals)
            {
                goals.Add(goal);
            }
        }

        return goals;
    }

    private static MissionObjectiveType ConvertGoalType(MissionGoalType goalType)
    {
        return goalType switch
        {
            MissionGoalType.Orbit => MissionObjectiveType.Orbit,
            MissionGoalType.Flyby => MissionObjectiveType.Flyby,
            MissionGoalType.Landing => MissionObjectiveType.Landing,
            MissionGoalType.SurfaceWaypoint => MissionObjectiveType.SurfaceWaypoint,
            MissionGoalType.SurfaceHop => MissionObjectiveType.SurfaceHop,
            MissionGoalType.ParkingOrbit => MissionObjectiveType.Orbit,
            _ => MissionObjectiveType.Orbit,
        };
    }

    private static MissionLegType InferLegType(MissionGoal goal)
    {
        return goal.GoalType switch
        {
            MissionGoalType.Orbit => MissionLegType.OrbitalTransfer,
            MissionGoalType.Flyby => MissionLegType.Flyby,
            MissionGoalType.Landing => MissionLegType.Landing,
            MissionGoalType.SurfaceWaypoint => MissionLegType.Landing,
            MissionGoalType.SurfaceHop => MissionLegType.SurfaceHop,
            MissionGoalType.ParkingOrbit => MissionLegType.Capture,
            _ => MissionLegType.OrbitalTransfer,
        };
    }

    private static MissionProgram CreateSingleObjectiveMissionProgram(MissionObjective objective)
    {
        var terminalGoal = new MissionGoal
        {
            Name = objective.Name,
            GoalType = objective.ObjectiveType switch
            {
                MissionObjectiveType.Orbit => MissionGoalType.Orbit,
                MissionObjectiveType.Flyby => MissionGoalType.Flyby,
                MissionObjectiveType.Landing => MissionGoalType.Landing,
                MissionObjectiveType.SurfaceWaypoint => MissionGoalType.SurfaceWaypoint,
                MissionObjectiveType.SurfaceHop => MissionGoalType.SurfaceHop,
                _ => MissionGoalType.Orbit,
            },
            TargetBody = objective.TargetBody,
            SurfaceTarget = objective.SurfaceTarget,
        };

        return new MissionProgram
        {
            Name = objective.Name,
            Stages = new[]
            {
                new MissionStage
                {
                    StageNumber = 1,
                    Name = "Single objective",
                    Goals = new[] { terminalGoal },
                },
            },
        };

    }
}