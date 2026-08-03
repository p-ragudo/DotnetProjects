namespace QueueServer.Common;

public class Utils
{
    public static int GenerateRandomMiliseconds(int min, int max, int step)
    {
        int maxSteps = (max - min) / step;
        int randomStep = Random.Shared.Next(0, maxSteps + 1);

        return min + (randomStep * step);
    }
}