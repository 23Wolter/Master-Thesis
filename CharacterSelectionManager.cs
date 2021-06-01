using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : NetworkBehaviour
{
    [SerializeField] GameObject characterSelection;
    [SerializeField] GameObject[] availableCharacters; 
    [SerializeField] Button nextButton; 
    [SerializeField] Button previousButton;
    [SerializeField] GameObject nextWarning;
    [SerializeField] GameObject prevWarning;
    [SerializeField] AudioSource clickSound; 

    [SyncVar(hook = nameof(SetCharacterIndex))]
    public int currentCharacterIndex = 0;


    private void Start()
    {
        if (!isLocalPlayer) Destroy(characterSelection);
    }



    private void SetCharacterIndex(int oldIndex, int newIndex)
    {
        availableCharacters[oldIndex].SetActive(false);
        availableCharacters[newIndex].SetActive(true);
    }

    public void NextCharacter()
    {
        clickSound.Play(); 
        CmdNextCharacter();
    }

    public void PreviousCharacter()
    {
        clickSound.Play(); 
        CmdPreviousCharacter();
    }


    [Command]
    private void CmdNextCharacter()
    {
        if(currentCharacterIndex < availableCharacters.Length-1) currentCharacterIndex++;  
        else RpcShowWarning(true); 
    }


    [Command]
    private void CmdPreviousCharacter()
    {
        if (currentCharacterIndex > 0) currentCharacterIndex--;
        else RpcShowWarning(false); 
    }


    [ClientRpc]
    private void RpcShowWarning(bool next)
    {
        StartCoroutine(ShowWarning(next)); 
    }

    private IEnumerator ShowWarning(bool next)
    {
        GameObject warning;
        if (next) warning = nextWarning;
        else warning = prevWarning;

        if (warning)
        {
            warning.SetActive(true);
            yield return new WaitForSeconds(1);
            warning.SetActive(false); 
        } 
    }
}