using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : NetworkBehaviour
{
    [Header("Player Attributes")]
    [SerializeField] Transform playerBody;
    
    [Header("Weapon Attributes")]
    [SerializeField] Transform weaponMuzzle;
    [SerializeField] GameObject betaPlusRay;
    [SerializeField] GameObject betaMinusRay;
    [SerializeField] GameObject alphaRay;
    [SerializeField] GameObject gammaRay;

    [Header("Script References")]
    [SerializeField] WeaponCore weaponCoreScript; 

    [Header("VFX")]
    [SerializeField] ParticleSystem wallHitParticles;
    [SerializeField] ParticleSystem playerHitParticles;
    [SerializeField] Animator cameraAnim; 
    [SerializeField] GameObject blueBeamReadyVFX; 
    [SerializeField] GameObject redBeamReadyVFX; 
    [SerializeField] GameObject yellowBeamReadyVFX; 
    [SerializeField] GameObject greenBeamReadyVFX; 

    [Header("Generel UI")]
    [SerializeField] Text playersLeft;
    [SerializeField] Text gameOverText;
    [SerializeField] GameObject mainGameUI;
    [SerializeField] Slider reloadBar; 

    [Header("Health UI")]
    [SerializeField] Slider healthbar;
    [SerializeField] Text healthText;
    [SerializeField] GameObject inGameHealthCanvas; 
    [SerializeField] Slider inGameHealth; 

    [Header("Isotope Chart UI")]
    [SerializeField] Text protonCountText;
    [SerializeField] Text neutronCountText;
    [SerializeField] RectTransform currentIsotopeIndicator;
    [SerializeField] Transform isotopeChartCamera;

    [Header("Special Weapon UI")]
    [SerializeField] Slider specialRayBar;
    [SerializeField] Image specialRayFillImage;
    [SerializeField] Sprite greyRayBarImage; 
    [SerializeField] Sprite blueRayBarImage; 
    [SerializeField] Sprite redRayBarImage; 
    [SerializeField] Sprite yellowRayBarImage;
    [SerializeField] GameObject betaPlusRayText; 
    [SerializeField] GameObject betaMinusRayText; 
    [SerializeField] GameObject alphaRayText; 
    [SerializeField] GameObject gammaRayText;
    [SerializeField] GameObject specialWeaponInteractionButton;
    [SerializeField] LayerMask alphaPenetrates;
    [SerializeField] LayerMask betaPenetrates;
    [SerializeField] LayerMask gammaPenetrates;

    [Header("SFX")]
    [SerializeField] AudioSource beamSound;
    [SerializeField] AudioSource lazerSound;
    [SerializeField] AudioSource pickupSound;
    [SerializeField] AudioSource beamReadySound;
    [SerializeField] AudioSource takeDamageSound;
    [SerializeField] AudioSource killedSound; 


    [SyncVar(hook = nameof(UpdateHealth))]
    public float health = 200; 

    private CharacterController controller;
    public LineRenderer bulletTrail;
    public bool canOpenLoot = false;

    private int protonCount = 0;
    private int neutronCount = 0;

    public bool betaPlusRayReady; 
    public bool betaMinusRayReady; 
    public bool alphaRayReady;

    public bool shootingRay = false;
    private LineRenderer lineRenderer;
    public GameObject rayBeam; 
    private Transform rayStart;
    private Transform rayEnd;
    private LayerMask rayLayerMask;
    private float rayShootingTime;

    private bool reloading = false; 

    /*** START AND UPDATE ***/
    #region
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        healthbar.value = health;
        gameOverText.text = "";
        if (isLocalPlayer)
        {
            CmdSetNetworkAttributes();
        }
    }


    [Command]
    private void CmdSetNetworkAttributes()
    {
        RpcSetNetworkAttributes(); 
    }

    [ClientRpc]
    private void RpcSetNetworkAttributes()
    {
        inGameHealthCanvas.SetActive(true);
        bulletTrail = weaponMuzzle.GetComponent<LineRenderer>();
    }


    private void Update()
    {
        if (!isLocalPlayer) return;

        #region
        if (shootingRay) CmdShootingRay();
        #endregion

        if (!shootingRay && !reloading) Shoot();
    }

    private void Shoot()
    {
        if (!controller.isGrounded) return;

        if (Input.GetMouseButtonDown(0)) CmdFire();

        #region
        if (Input.GetKeyDown(KeyCode.Q)) {
            specialWeaponInteractionButton.SetActive(false);
            if (betaPlusRayReady) CmdPrepareRay(1);
            else if (betaMinusRayReady) CmdPrepareRay(2);
            else if (alphaRayReady) CmdPrepareRay(3);
            else return;
        }
        #endregion
    }
    #endregion

    // *** REGULAR FIRE ***
    #region

    [Command]
    private void CmdFire()
    {
        RpcFire(); 
    }

    [ClientRpc]
    private void RpcFire()
    {
        lazerSound.Play(); 

        RaycastHit hit;
        if (Physics.Raycast(weaponMuzzle.position, playerBody.forward, out hit, Mathf.Infinity))
        {
            #region 
            StartCoroutine(MuzzleFlash(hit.point));

            if(hit.transform.gameObject.CompareTag("Wall"))
            {
                NetworkServer.Destroy(GameObject.Find("WallHitParticles(Clone)")); 
                ParticleSystem particles = Instantiate(wallHitParticles);
                particles.transform.position = hit.point;
                particles.Play();
            }
            else
            #endregion
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                NetworkServer.Destroy(GameObject.Find("BloodSpatterParticles(Clone)"));
                ParticleSystem particles = Instantiate(playerHitParticles);
                particles.transform.position = hit.point;
                particles.Play();

                NetworkIdentity targetIdentity = hit.transform.gameObject.GetComponent<NetworkIdentity>();              
                if(targetIdentity.isLocalPlayer)
                {
                    GameObject targetPlayer = NetworkIdentity.spawned[targetIdentity.netId].gameObject;
                    PlayerAction targetAction = targetPlayer.GetComponent<PlayerAction>();
                    targetAction.CmdTakeDamage(1); 
                }
            }
            #region
            else if (hit.transform.gameObject.CompareTag("Dummy"))
            {
                ParticleSystem particles = Instantiate(playerHitParticles);
                particles.transform.position = hit.point;
                particles.Play();
                hit.transform.gameObject.GetComponent<DummyHealth>().TakeDamage(10);
            }
            #endregion
        }

        StartCoroutine(Reload()); 
    }

    private IEnumerator Reload()
    {
        reloading = true;
        reloadBar.value = 0; 

        while(reloadBar.value < 100)
        {
            yield return new WaitForSeconds(0.001f);
            reloadBar.value += 1f; 
        }

        reloadBar.value = 100;
        reloading = false; 
    }


    private IEnumerator MuzzleFlash(Vector3 hit)
    {
        bulletTrail.SetPosition(0, weaponMuzzle.position);
        bulletTrail.SetPosition(1, hit); 
        bulletTrail.enabled = true; 

        yield return new WaitForSeconds(0.1f);
        
        bulletTrail.enabled = false; 
    }
#endregion // *** REGULAR FIRE ***

    // *** SPECIAL FIRE ***
                #region
    [Command]
    private void CmdPrepareRay(int rayType)
    {
        RpcPrepareRay(rayType);
    }

    [ClientRpc]
    private void RpcPrepareRay(int rayType)
    {
        if (rayType == 1)
        {
            rayShootingTime = 2;
            rayBeam = betaPlusRay;
            rayLayerMask = betaPenetrates;
        }
        else if (rayType == 2)
        {
            rayShootingTime = 2;
            rayBeam = betaMinusRay;
            rayLayerMask = betaPenetrates;
        }
        else if (rayType == 3)
        {
            rayShootingTime = 4;
            rayBeam = alphaRay;
            rayLayerMask = alphaPenetrates;
        }

        lineRenderer = rayBeam.GetComponent<LineRenderer>();
        rayStart = rayBeam.transform.GetChild(0).gameObject.transform;
        rayEnd = rayBeam.transform.GetChild(1).gameObject.transform;

        rayBeam.SetActive(true);
        shootingRay = true;
        StartCoroutine(RayBeamTimer(rayShootingTime, rayType));
    }

    private IEnumerator RayBeamTimer(float time, int rayType)
    {
        beamSound.Play();

        LimitPlayerRotation(false, 4); 

        yield return new WaitForSeconds(time);

        beamSound.Stop();

        LimitPlayerRotation(true, 8); 

        shootingRay = false;
        rayBeam.SetActive(false);

        if(rayType == 1)
        {
            ChangeProtonCount(-1); // lose 1 proton
            specialRayBar.value -= 10;  // reduce special bar 1 step
            ChangeNeutronCount(1); // gain 1 neutron
            CheckSpecialRay(1); // check if special ray is ready again
        }
        else if(rayType == 2)
        {
            ChangeNeutronCount(-1);
            specialRayBar.value -= 10;
            ChangeProtonCount(1);
            CheckSpecialRay(0); 
        }
        else if(rayType == 3)
        {
            ChangeProtonCount(-1);
            ChangeNeutronCount(-1);
            ChangeProtonCount(-1);
            ChangeNeutronCount(-1);
            specialRayBar.value -= 10;
            CheckSpecialRay(-1); 
        }
    }

    private void LimitPlayerRotation(bool limit, float newSpeed)
    {
        if (!isLocalPlayer) return; 

        PlayerRotation rotationScript = GetComponent<PlayerRotation>();
        PlayerMovement movementScript = GetComponent<PlayerMovement>();
        
        rotationScript.SetCanRotate(limit);
        movementScript.SetMovementSpeed(newSpeed);
    }


    [Command]
    private void CmdShootingRay()
    {
        RpcShootingRay(); 
    }

    [ClientRpc]
    private void RpcShootingRay()
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        rayStart.transform.position = weaponMuzzle.position;

        RaycastHit hit;
        if (Physics.Raycast(weaponMuzzle.position, playerBody.forward, out hit, Mathf.Infinity, ~rayLayerMask))
        {
            float distance = Vector3.Distance(hit.point, weaponMuzzle.position);
            Vector3 target = new Vector3(0, 0, distance + 1);
            lineRenderer.SetPosition(1, target);

            rayEnd.gameObject.SetActive(true);
            rayEnd.transform.position = hit.point;
            rayEnd.transform.position -= (transform.TransformDirection(Vector3.forward)) * 0.1f;

            if (hit.transform.gameObject.CompareTag("Player"))
            {
                NetworkServer.Destroy(GameObject.Find("BloodSpatterParticles(Clone)"));
                ParticleSystem particles = Instantiate(playerHitParticles);
                particles.transform.position = hit.point;
                particles.Play();

                NetworkIdentity targetIdentity = hit.transform.gameObject.GetComponent<NetworkIdentity>();
                if (targetIdentity.isLocalPlayer)
                {
                    GameObject targetPlayer = NetworkIdentity.spawned[targetIdentity.netId].gameObject;
                    PlayerAction targetAction = targetPlayer.GetComponent<PlayerAction>();
                    targetAction.CmdTakeDamage(10*Time.deltaTime);
                }
            }
            else if (hit.transform.gameObject.CompareTag("Dummy"))
            {
                ParticleSystem particles = Instantiate(playerHitParticles);
                particles.transform.position = hit.point;
                particles.Play();
                hit.transform.gameObject.GetComponent<DummyHealth>().TakeDamage(1); 
            }
        }
        else
        {
            rayEnd.gameObject.SetActive(false);
        }
    }
                #endregion


    // *** TAKE DAMGE ***
                #region
    [Command]
    private void CmdTakeDamage(float damage)
    {
        health -= damage;
        RpcShowInGameHealth(health);
    }

    [ClientRpc]
    private void RpcShowInGameHealth(float health)
    {
        if (!shootingRay) takeDamageSound.Play();
        inGameHealth.value = health; 
    }

    private void UpdateHealth(float oldHealth, float newHealth)
    {
        healthbar.value = Mathf.Round(newHealth);
        healthText.text = Mathf.Round(newHealth).ToString();

        if (newHealth <= 0) KillPlayer();
    }


    private void KillPlayer()
    {
        StartCoroutine(ShowDeathText()); 

        gameObject.tag = "Dead";

        if (isLocalPlayer) CmdDisablePlayer(); 
        
        GameObject.Find("GameManager").GetComponent<GameManager>().RemovePlayer();
    }

    private IEnumerator ShowDeathText()
    {
        gameOverText.text = "Du blev smadret! Men du kan stadig følge kampen..";
        yield return new WaitForSeconds(2); 
        gameOverText.text = "";
        mainGameUI.SetActive(false);
    }

    [Command]
    private void CmdDisablePlayer()
    {
        RpcDisablePlayer(); 
    }

    [ClientRpc]
    public void RpcDisablePlayer()
    {
        killedSound.Play(); 
        this.enabled = false;
        playerBody.gameObject.SetActive(false);
        this.GetComponent<CapsuleCollider>().enabled = false;
    }


    public void UpdatePlayersLeft(int currentNumberOfPlayers, int maxNumberOfPlayers)
    {
        playersLeft.text = "Spillere tilbage: " + currentNumberOfPlayers + "/" + maxNumberOfPlayers;
    }
                #endregion

    // *** PICKUP ITEMS ***
                #region
    public void OpenLootBox(Vector3[] spawnPoints, uint netId)
    {
        CmdOpenLootBox(spawnPoints, netId);
    }


    [Command]
    private void CmdOpenLootBox(Vector3[] spawnPoints, uint netId)
    {
        GameObject.Find("SpawnManager").GetComponent<SpawnManager>().SpawnPickups(spawnPoints, netId); 
    }


    public void PickupItem(string nucleonType, uint netId)
    {
        if (nucleonType == "Proton")
        {
            ChangeProtonCount(1);
            CheckSpecialRay(0); 
        }
        else if (nucleonType == "Neutron")
        {
            ChangeNeutronCount(1);
            CheckSpecialRay(1); 
        }
        else Debug.LogError("Wrong Nucleon Type"); 

        CmdPickupItem(netId);
    }

    [Command]
    private void CmdPickupItem(uint netId)
    {
        RpcPlayPickupSFX(); 

        GameObject pickup = NetworkIdentity.spawned[netId].gameObject;
        NetworkServer.Destroy(pickup);
    }

    [ClientRpc]
    private void RpcPlayPickupSFX()
    {
        pickupSound.Play(); 
    }


    private void ChangeProtonCount(int value)
    {
        protonCount += value;
        protonCountText.text = protonCount.ToString();

        int UIIncrementer = (value < 0) ? -22 : 22;

        Vector3 currentPos = currentIsotopeIndicator.position;
        currentIsotopeIndicator.position = new Vector3(currentPos.x, currentPos.y + UIIncrementer, currentPos.z);

        currentPos = isotopeChartCamera.position;
        isotopeChartCamera.position = new Vector3(currentPos.x, currentPos.y + UIIncrementer, currentPos.z);

        weaponCoreScript.SetProtonUI(protonCount);
    }

    private void ChangeNeutronCount(int value)
    {
        neutronCount += value;
        neutronCountText.text = neutronCount.ToString();

        int UIIncrementer = (value < 0) ? -22 : 22;

        Vector3 currentPos = currentIsotopeIndicator.position;
        currentIsotopeIndicator.position = new Vector3(currentPos.x + UIIncrementer, currentPos.y, currentPos.z);

        currentPos = isotopeChartCamera.position;
        isotopeChartCamera.position = new Vector3(currentPos.x + UIIncrementer, currentPos.y, currentPos.z);

        weaponCoreScript.SetNeutronUI(neutronCount); 
    }


    private void CheckSpecialRay(int specialType)
    {
        bool specialCase = CheckSpecialCases();

        if (!specialCase) CheckRayColor(); 

        IncreaseSpecialBar(specialType);

        CheckBarLimit();

        CheckReadyRay(specialCase); 
    }
    private bool CheckSpecialCases()
    {
        if (protonCount == 4 && neutronCount == 3
            || protonCount == 5 && neutronCount == 6
            || protonCount == 8 && neutronCount == 7
            || protonCount == 9 && neutronCount == 10)
        {
            specialRayFillImage.sprite = greyRayBarImage;
            return true;
        }
        else return false; 
    }

    private void CheckRayColor()
    {
        if (protonCount + neutronCount >= 20) specialRayFillImage.sprite = yellowRayBarImage;
        else if (protonCount > neutronCount) specialRayFillImage.sprite = redRayBarImage;
        else if (neutronCount > protonCount) specialRayFillImage.sprite = blueRayBarImage;
        else specialRayFillImage.sprite = greyRayBarImage; 
    }

    private void IncreaseSpecialBar(int specialType)
    {
        if(specialType == 0)
        {
            if(protonCount > neutronCount) specialRayBar.value += 10; 
        }
        else if(specialType == 1)
        {
            if(neutronCount > protonCount) specialRayBar.value += 10; 
        }
        else
        {
            specialRayBar.value -= 10; 
        }
    }


    private void CheckBarLimit()
    {
        if (protonCount > 15)
        {
            LosePickup(true);
        }
        else if (neutronCount > 15)
        {
            LosePickup(false);
        }
        else if (protonCount > (neutronCount * 2) + 1) LosePickup(true);
        else if (neutronCount > (protonCount * 2) + 1) LosePickup(false);
        
    }

    private void CheckReadyRay(bool specialCase)
    {
        if (protonCount >= 10 && !specialCase)
        {
            specialRayBar.value = 100;
            betaPlusRayReady = true;
            betaPlusRayText.SetActive(true);
            CmdShowBeamVFX(1); 
            specialWeaponInteractionButton.SetActive(true); 
        }
        else 
        { 
            betaPlusRayReady = false;
            betaPlusRayText.SetActive(false);
            CmdShowBeamVFX(2);
        }

        if (neutronCount >= 10 && !specialCase)
        {
            specialRayBar.value = 100;
            betaMinusRayReady = true;
            betaMinusRayText.SetActive(true);
            CmdShowBeamVFX(3);
            specialWeaponInteractionButton.SetActive(true);
        }
        else
        {
            betaMinusRayReady = false;
            betaMinusRayText.SetActive(false);
            CmdShowBeamVFX(4);
        }

        if (protonCount + neutronCount >= 20 && !specialCase)
        {
            specialRayBar.value = 100;

            betaPlusRayReady = false;
            betaPlusRayText.SetActive(false);

            betaMinusRayReady = false;
            betaMinusRayText.SetActive(false);

            alphaRayReady = true;
            alphaRayText.SetActive(true);
            specialWeaponInteractionButton.SetActive(true);
            CmdShowBeamVFX(5);
        }
        else
        {
            alphaRayReady = false;
            alphaRayText.SetActive(false);
            CmdShowBeamVFX(6);
        }
    }


    [Command]
    private void CmdShowBeamVFX(int rayCase)
    {
        RpcShowBeamVFX(rayCase); 
    }

    [ClientRpc]
    private void RpcShowBeamVFX(int rayCase)
    {
        switch(rayCase)
        {
            case 1:
                redBeamReadyVFX.SetActive(true);
                beamReadySound.Play();
                break;
            case 2:
                redBeamReadyVFX.SetActive(false);
                beamReadySound.Stop(); 
                break;
            case 3:
                blueBeamReadyVFX.SetActive(true);
                beamReadySound.Play();
                break;
            case 4:
                blueBeamReadyVFX.SetActive(false);
                beamReadySound.Stop();
                break;
            case 5:
                redBeamReadyVFX.SetActive(false);
                blueBeamReadyVFX.SetActive(false);
                yellowBeamReadyVFX.SetActive(true);
                beamReadySound.Play();
                break;
            case 6:
                yellowBeamReadyVFX.SetActive(false);
                beamReadySound.Stop();
                break;
        }
    }


    private void LosePickup(bool proton)
    {
        if (proton) ChangeProtonCount(-1);
        else ChangeNeutronCount(-1);

        specialRayBar.value -= 10;

        CmdLosePickup(proton); 
    }

    [Command]
    private void CmdLosePickup(bool proton)
    {
        GameObject.Find("SpawnManager").GetComponent<SpawnManager>().DropPickup(proton, this.transform, playerBody);
    }
                #endregion
}
