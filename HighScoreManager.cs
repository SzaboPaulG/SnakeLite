namespace TheAdventure;

public static class HighScoreManager
{
    private const string FileName = "highscore.txt";

    public static int Load()
    {
        if (!File.Exists(FileName))
        {
            File.WriteAllText(FileName, "0");
            return 0;
        }

        var text = File.ReadAllText(FileName);

        if (int.TryParse(text, out var score))
        {
            return score;
        }

        return 0;
    }

    public static void Save(int score)
    {
        File.WriteAllText(FileName, score.ToString());
    }
}