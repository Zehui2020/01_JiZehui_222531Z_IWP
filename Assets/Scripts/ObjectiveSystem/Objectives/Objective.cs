public class Objective
{
    public enum ObjectiveType { Normal, Progress }
    public ObjectiveType objectiveType;

    public string objectiveName;
    public ObjectiveUI objectiveUI;
    public event System.Action OnObjectiveComplete;

    public Objective(ObjectiveType objectiveType, string objectiveName)
    {
        this.objectiveType = objectiveType;
        this.objectiveName = objectiveName;
    }

    public void SetObjectiveUI(ObjectiveUI objectiveUI)
    {
        this.objectiveUI = objectiveUI;
    }

    public void UpdateObjectiveName(string newObjectiveName)
    {
        objectiveName = newObjectiveName;
    }

    public void CompleteObjective()
    {
        OnObjectiveComplete?.Invoke();
        OnObjectiveComplete = null;

        if (objectiveUI != null)
            objectiveUI.RemoveObjective();
    }
}