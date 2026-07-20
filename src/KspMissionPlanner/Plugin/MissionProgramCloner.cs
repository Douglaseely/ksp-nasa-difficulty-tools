using System;
using System.Collections.Generic;
using KspMissionPlanner.Planning;

namespace KspMissionPlanner.Plugin;

public static class MissionProgramCloner
{
    public static MissionProgram Clone(MissionProgram source)
    {
        var stages = new List<MissionStage>();
        foreach (var stage in source.Stages)
        {
            var goals = new List<MissionGoal>();
            foreach (var goal in stage.Goals)
            {
                goals.Add(new MissionGoal
                {
                    GoalId = Guid.NewGuid().ToString("N"),
                    Name = goal.Name,
                    GoalType = goal.GoalType,
                    TargetBody = goal.TargetBody,
                    IsRequired = goal.IsRequired,
                    OrbitTarget = goal.OrbitTarget is null ? null : new OrbitTarget
                    {
                        BodyName = goal.OrbitTarget.BodyName,
                        PeriapsisAltitudeMeters = goal.OrbitTarget.PeriapsisAltitudeMeters,
                        ApoapsisAltitudeMeters = goal.OrbitTarget.ApoapsisAltitudeMeters,
                        InclinationRadians = goal.OrbitTarget.InclinationRadians,
                    },
                    SurfaceTarget = goal.SurfaceTarget is null ? null : new SurfaceTarget
                    {
                        BodyName = goal.SurfaceTarget.BodyName,
                        WaypointName = goal.SurfaceTarget.WaypointName,
                        LatitudeDegrees = goal.SurfaceTarget.LatitudeDegrees,
                        LongitudeDegrees = goal.SurfaceTarget.LongitudeDegrees,
                        AltitudeMeters = goal.SurfaceTarget.AltitudeMeters,
                    },
                });
            }

            stages.Add(new MissionStage
            {
                StageNumber = stage.StageNumber,
                Name = stage.Name,
                Goals = goals,
            });
        }

        return new MissionProgram
        {
            Name = source.Name,
            StartPoint = new MissionStartPoint
            {
                BodyName = source.StartPoint.BodyName,
                Orbit = source.StartPoint.Orbit is null ? null : new OrbitTarget
                {
                    BodyName = source.StartPoint.Orbit.BodyName,
                    PeriapsisAltitudeMeters = source.StartPoint.Orbit.PeriapsisAltitudeMeters,
                    ApoapsisAltitudeMeters = source.StartPoint.Orbit.ApoapsisAltitudeMeters,
                    InclinationRadians = source.StartPoint.Orbit.InclinationRadians,
                },
                Surface = source.StartPoint.Surface is null ? null : new SurfaceTarget
                {
                    BodyName = source.StartPoint.Surface.BodyName,
                    WaypointName = source.StartPoint.Surface.WaypointName,
                    LatitudeDegrees = source.StartPoint.Surface.LatitudeDegrees,
                    LongitudeDegrees = source.StartPoint.Surface.LongitudeDegrees,
                    AltitudeMeters = source.StartPoint.Surface.AltitudeMeters,
                },
            },
            Stages = stages,
        };
    }
}