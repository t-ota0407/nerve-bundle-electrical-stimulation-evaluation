public enum DrawingTasks
{
    GENERAL,
    HORIZONTAL,
    VERTICAL,
}

public class DrawingTasksConverter
{
    private const string stringGeneral = "GENERAL";
    private const string stringHorizontal = "HORIZONTAL";
    private const string stringVertical = "VERTICAL";
    public static string ToString(DrawingTasks drawingTask)
    {
        string stringExpression = stringGeneral;
        switch (drawingTask)
        {
            case DrawingTasks.GENERAL:
                stringExpression = stringGeneral;
                break;
            case DrawingTasks.HORIZONTAL:
                stringExpression = stringHorizontal;
                break;
            case DrawingTasks.VERTICAL:
                stringExpression = stringVertical;
                break;
        }
        return stringExpression;
    }
}