using Photon.Pun;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviourPun
{
    InternalUI playerInternalUI = null;

    [SerializeField] GameObject ExternalUI = null;
    [SerializeField] Image healthBarExternal = null;    

    private void Awake()
    {
        if (photonView.IsMine)
        {
            playerInternalUI = FindObjectOfType<InternalUI>();
            ExternalUI.SetActive(false);
        }
        else        
            ExternalUI.SetActive(true);        
    }

    public void UpdateHealthUI(CharacterData data)
    {
        healthBarExternal.fillAmount = data.health / data.maxHealth;
        playerInternalUI.HealthBar.fillAmount = data.health / data.maxHealth;
    }

    public void UpdateBoostEnergyUI(float boostEnergy, float maxBoostEnergy)
    {
        playerInternalUI.BoostEnergyBar.fillAmount = boostEnergy / maxBoostEnergy;
    }

    public void RespawnTimerUIUpdate(float seconds)
    {
        playerInternalUI.RespawnTimer.text = seconds.ToString("0.0");
    }

    public void ActivateRespawnTimer(bool activate)
    {
        playerInternalUI.RespawnTimer.gameObject.SetActive(activate);
    }
}
