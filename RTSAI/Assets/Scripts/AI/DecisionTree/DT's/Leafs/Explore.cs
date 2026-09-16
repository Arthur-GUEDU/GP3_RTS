using UnityEngine;
using UnityEngine.AI;

public class Explore : DecisionTreeLeaf
{
    [Header("Conditions")]
    public AIController controller;

    public DTSquadManagerRoot squadManagerRoot;

    public int exploreRadius = 100;

    public override void Execute()
    {
        Vector3 randomDirection = Random.insideUnitSphere * exploreRadius;
        randomDirection += squadManagerRoot.joblessSquad.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, exploreRadius, NavMesh.AllAreas))
            controller.AssignGoal(squadManagerRoot.joblessSquad, controller.ExploreGoal, new Target(hit.position));
    }
}
