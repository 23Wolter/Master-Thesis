using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [Header("Networking")]
    [SerializeField] NetworkManager networkManager;
    [SerializeField] InputField networkAddressInput;
    [SerializeField] string defaultNetworkAddress;

    [Header("Graphics")]
    [SerializeField] GameObject hostButtonPointerEnterImage; 
    [SerializeField] GameObject hostButtonPointerExitImage; 
    [SerializeField] GameObject joinButtonPointerEnterImage; 
    [SerializeField] GameObject joinButtonPointerExitImage;

    [Header("Sounds")]
    [SerializeField] AudioSource buttonSoundFX; 

    private string networkAddress; 

    public void HostLobby()
    {
        networkManager.StartHost(); 
    }

    public void JoinLobby()
    {
        if (networkAddressInput.text.Length == 0) networkAddress = defaultNetworkAddress;
        else networkAddress = networkAddressInput.text;  
        
        networkManager.networkAddress = networkAddress; 
        networkManager.StartClient();
    }

    public void ChangeHostButtonImage(bool pointerEnter)
    {
        if(pointerEnter)
        {
            buttonSoundFX.Play();
            hostButtonPointerExitImage.SetActive(false); 
            hostButtonPointerEnterImage.SetActive(true); 
        }
        else
        {
            buttonSoundFX.Stop();
            hostButtonPointerEnterImage.SetActive(false);
            hostButtonPointerExitImage.SetActive(true);
        }
    }
    
    public void ChangeJoinButtonImage(bool pointerEnter)
    {
        if(pointerEnter)
        {
            buttonSoundFX.Play();
            joinButtonPointerExitImage.SetActive(false); 
            joinButtonPointerEnterImage.SetActive(true); 
        }
        else
        {
            buttonSoundFX.Stop();
            joinButtonPointerEnterImage.SetActive(false);
            joinButtonPointerExitImage.SetActive(true);
        }
    }


    public void QuitGame()
    {
        Application.Quit(); 
    }
}
