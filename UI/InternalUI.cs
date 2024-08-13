using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InternalUI : MonoBehaviour
{
	[SerializeField] Image healthBar = null;
	public Image HealthBar
	{
		get => healthBar;
		private set { }
	}

	[SerializeField] Image boostEnergyBar = null;
	public Image BoostEnergyBar
	{
		get => boostEnergyBar;
		private set { }
	}

	[SerializeField] Text respawnTimer = null;
	public Text RespawnTimer { get => respawnTimer; private set { } }
}
