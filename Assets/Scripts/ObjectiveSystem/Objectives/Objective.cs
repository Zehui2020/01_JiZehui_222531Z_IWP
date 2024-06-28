public class Objective
{
    public enum ObjectiveType { Normal, Progress }
    public ObjectiveType objectiveType;

    public string objectiveName;

    public Objective(ObjectiveType objectiveType, string objectiveName)
    {
        this.objectiveType = objectiveType;
        this.objectiveName = objectiveName;
    }

    public void UpdateObjectiveName(string newObjectiveName)
    {
        objectiveName = newObjectiveName;
    }
}