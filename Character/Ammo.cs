[System.Serializable]
public struct eAmmoAmount
{
	public int Bombs;
	public int Missiles;
}

public class Ammo
{
	eAmmoAmount ammo;

    public int MaxMissileCount { get => maxMissileCount; private set => maxMissileCount = value; }
    int maxMissileCount;

	public int MaxBombCount
	{
		get => maxBombCount;
		private set => maxBombCount = value;
	}
	int maxBombCount;

	public int BombCount
	{
		get => bombCount;
		set
		{
			if (value >= maxBombCount)
				bombCount = maxBombCount;
			else
				bombCount = value;
		}
	}
	int bombCount;	

	public int MissileCount
	{
		get => missileCount;
		set
		{
			if (value >= maxMissileCount)
				missileCount = maxMissileCount;
			else
				missileCount = value;
		}
	}
	int missileCount;

	public Ammo(int missileCount, int bombCount)
	{
		maxBombCount = bombCount;
		maxMissileCount = missileCount;

		this.bombCount = maxBombCount;
		this.missileCount = maxMissileCount;
	}

	public eAmmoAmount CheckAmmo()
	{
		return new eAmmoAmount()
		{
			Bombs = BombCount,
			Missiles = missileCount
		};
	}	
}
