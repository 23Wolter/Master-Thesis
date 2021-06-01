using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayName : NetworkBehaviour
{
    [SerializeField] Text displayName;



    public void SetDisplayName(string name)
    {
        CmdSetDisplayName(name);
    }

    [Command]
    private void CmdSetDisplayName(string name)
    {
        RpcSetDisplayName(name); 
    }

    [ClientRpc]
    private void RpcSetDisplayName(string name)
    {
        displayName.text = name; 
    }

    public string GetDisplayName()
    {
        return displayName.text; 
    }
}
