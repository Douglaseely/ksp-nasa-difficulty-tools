using KspMissionPlanner.Planning;

namespace KspMissionPlanner.Integrations;

public interface IMissionMathService
{
    DirectTransferEstimate EstimateTransfer(MissionGoal fromGoal, MissionGoal toGoal, MissionPlanningContext context);
}