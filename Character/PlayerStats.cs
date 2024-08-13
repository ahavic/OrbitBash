public class PlayerStats
{
    public int KillCount => killCount;
    public int DeathCount => deathCount;
    int killCount = 0;
    int deathCount = 0;

    public void AddKill()
    {
        killCount++;
    }

    public void AddDeath()
    {
        deathCount++;
    }
}
