using System;
using System.Collections.Generic;

namespace KspMissionPlanner.Planning;

public interface IMissionProgramStore
{
    void Save(MissionProgram missionProgram);
    bool TryGetById(string missionId, out MissionProgram missionProgram);
    IReadOnlyList<MissionProgram> GetAll();
    bool Delete(string missionId);
}

public interface IGravityAssistScout
{
    IReadOnlyList<GravityAssistOption> FindCandidates(
        MissionGoal fromGoal,
        MissionGoal toGoal,
        double universalTimeSeconds);
}

public sealed class InMemoryMissionProgramStore : IMissionProgramStore
{
    private readonly Dictionary<string, MissionProgram> _missionPrograms = new Dictionary<string, MissionProgram>(StringComparer.OrdinalIgnoreCase);

    public void Save(MissionProgram missionProgram)
    {
        if (missionProgram is null)
        {
            throw new ArgumentNullException(nameof(missionProgram));
        }

        missionProgram.UpdatedUtc = DateTimeOffset.UtcNow;
        _missionPrograms[missionProgram.MissionId] = missionProgram;
    }

    public bool TryGetById(string missionId, out MissionProgram missionProgram)
    {
        return _missionPrograms.TryGetValue(missionId, out missionProgram!);
    }

    public IReadOnlyList<MissionProgram> GetAll()
    {
        return new List<MissionProgram>(_missionPrograms.Values);
    }

    public bool Delete(string missionId)
    {
        return _missionPrograms.Remove(missionId);
    }
}

public sealed class NoOpGravityAssistScout : IGravityAssistScout
{
    public IReadOnlyList<GravityAssistOption> FindCandidates(
        MissionGoal fromGoal,
        MissionGoal toGoal,
        double universalTimeSeconds)
    {
        _ = fromGoal;
        _ = toGoal;
        _ = universalTimeSeconds;
        return Array.Empty<GravityAssistOption>();
    }
}

public static class ParkingOrbitAugmentor
{
    public static MissionProgram EnsureParkingOrbitsAroundLandingGoals(
        MissionProgram missionProgram,
        ParkingOrbitTemplate template,
        bool addBeforeLanding,
        bool addAfterLanding)
    {
        if (missionProgram is null)
        {
            throw new ArgumentNullException(nameof(missionProgram));
        }

        var updatedStages = new List<MissionStage>();
        foreach (var stage in missionProgram.Stages)
        {
            var updatedGoals = new List<MissionGoal>();
            for (var index = 0; index < stage.Goals.Count; index++)
            {
                var goal = stage.Goals[index];

                if (addBeforeLanding && IsLandingLike(goal) && !HasAdjacentParkingOrbit(stage.Goals, index, before: true))
                {
                    updatedGoals.Add(CreateParkingOrbitGoal(goal.TargetBody, ParkingOrbitRole.DescentStaging, template));
                }

                updatedGoals.Add(goal);

                if (addAfterLanding && IsLandingLike(goal) && !HasAdjacentParkingOrbit(stage.Goals, index, before: false))
                {
                    updatedGoals.Add(CreateParkingOrbitGoal(goal.TargetBody, ParkingOrbitRole.AscentStaging, template));
                }
            }

            updatedStages.Add(new MissionStage
            {
                StageNumber = stage.StageNumber,
                Name = stage.Name,
                Goals = updatedGoals,
            });
        }

        return new MissionProgram
        {
            MissionId = missionProgram.MissionId,
            Name = missionProgram.Name,
            StartPoint = missionProgram.StartPoint,
            CreatedUtc = missionProgram.CreatedUtc,
            UpdatedUtc = DateTimeOffset.UtcNow,
            Stages = updatedStages,
        };
    }

    private static bool IsLandingLike(MissionGoal goal)
    {
        return goal.GoalType == MissionGoalType.Landing || goal.GoalType == MissionGoalType.SurfaceWaypoint;
    }

    private static bool HasAdjacentParkingOrbit(IReadOnlyList<MissionGoal> goals, int index, bool before)
    {
        var adjacentIndex = before ? index - 1 : index + 1;
        if (adjacentIndex < 0 || adjacentIndex >= goals.Count)
        {
            return false;
        }

        return goals[adjacentIndex].GoalType == MissionGoalType.ParkingOrbit;
    }

    private static MissionGoal CreateParkingOrbitGoal(string bodyName, ParkingOrbitRole role, ParkingOrbitTemplate template)
    {
        return new MissionGoal
        {
            Name = role == ParkingOrbitRole.DescentStaging ? "Auto Parking Orbit (Pre-Landing)" : "Auto Parking Orbit (Post-Landing)",
            GoalType = MissionGoalType.ParkingOrbit,
            TargetBody = bodyName,
            OrbitTarget = new OrbitTarget
            {
                BodyName = bodyName,
                PeriapsisAltitudeMeters = template.DefaultPeriapsisAltitudeMeters,
                ApoapsisAltitudeMeters = template.DefaultApoapsisAltitudeMeters,
                InclinationRadians = template.DefaultInclinationRadians,
            },
        };
    }
}