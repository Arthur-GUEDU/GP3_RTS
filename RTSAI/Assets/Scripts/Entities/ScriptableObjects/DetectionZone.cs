using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public EntityVisibility unitVisibility;
    public SphereCollider detectionSphere;

    private AIController controller;

    void Start()
    {
        controller = FindAnyObjectByType<AIController>();

        if (unitVisibility.Team == ETeam.Red)
        {
            detectionSphere.radius = unitVisibility.Range;
            detectionSphere.isTrigger = true;
        }
        else
        {
            detectionSphere.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EntityVisibility>() && unitVisibility.Team != other.gameObject.GetComponent<EntityVisibility>().Team && other.gameObject.GetComponent<Factory>())
        {
            foreach(GameObject factory in controller.playerFactorys)
            {
                if (factory.gameObject == other.gameObject)
                {
                    return;
                }
            }
            controller.playerFactorys.Add(other.gameObject);
        }

        if (other.gameObject.GetComponent<EntityVisibility>().Team == ETeam.Neutral && other.gameObject.GetComponent<TargetBuilding>())
        {
            foreach (Objective objective in controller.objectives)
            {
                if (objective.Target.CaptureTarget == other.gameObject.GetComponent<TargetBuilding>())
                {
                    return;
                }
            }

            Objective o = new Objective(controller.CaptureGoal, new Target(other.gameObject.GetComponent<TargetBuilding>()), 10);

            controller.AddObjective(o);
        }
    }
}
