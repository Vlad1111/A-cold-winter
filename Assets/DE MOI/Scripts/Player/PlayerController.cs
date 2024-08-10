using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static float mapSize = 10;
    public static PlayerController instance;

    public int nrHunterKilled = 0;

    public float walkingSpeed;
    public float lookSpeed;
    public Camera playerCamera;
    public Transform playerSpine;


    public Transform axeTransform;
    private AxeBehaviour axeBehaviour;
    public Transform bowTransform;
    private BowBehaviour bowBehaviour;
    public PickabelObjectDetection pickabelObjectDetection;
    public Text inventarText;

    private Vector2 walkingDirection;
    private Vector2 fires;
    private Vector2 interaction;
    private Vector3 lookingDirection = Vector3.zero;
    private Vector3 originalLocalPlayerRotation;
    private Vector3 originalLocalCameraRotation;

    private Animator playerAnimator;
    public Dictionary<string, int> inventar = new Dictionary<string, int>();

    private float stopMovindTime = -1;
    private bool stopMovingIndfinitly = false;

    public int nrArrows = 15;

    private SoundManager playerSoundManager;

    void Start()
    {
        instance = this;
        originalLocalPlayerRotation = transform.localRotation.eulerAngles;
        //originalLocalCameraRotation = playerCamera.transform.parent.localRotation.eulerAngles;
        originalLocalCameraRotation = playerSpine.localRotation.eulerAngles;
        playerAnimator = GetComponent<Animator>();
        bowBehaviour = bowTransform.GetComponent<BowBehaviour>();
        axeBehaviour = axeTransform.GetComponent<AxeBehaviour>();
        playerSoundManager = GetComponent<SoundManager>();

        //inventarText.text = Screen.width + " " + Screen.height;
    }

    public void getInput()
    {
        walkingDirection.x = Input.GetAxis("Vertical");
        walkingDirection.y = Input.GetAxis("Horizontal");

        lookingDirection.x = -Input.GetAxis("Mouse Y");
        lookingDirection.y = Input.GetAxis("Mouse X");

        fires.x = Input.GetAxis("Fire1");
        fires.y = Input.GetAxis("Fire2");

        interaction.x = Input.GetAxis("Interact1");
        interaction.y = Input.GetAxis("Interact2");
    }
    private float wasWalking = 0;
    public void move()
    {
        var locCamRot = originalLocalCameraRotation + Vector3.right * lookingDirection.x * lookSpeed * Time.deltaTime * MySettings.MouseSensitivity;
        if (locCamRot.x > 80)
            locCamRot.x = 80;
        else if (locCamRot.x < -75)
            locCamRot.x = -75;
        //playerCamera.transform.parent.localRotation = Quaternion.Euler(locCamRot);
        playerSpine.localRotation = Quaternion.Euler(locCamRot);
        originalLocalCameraRotation = locCamRot;

        var locPlayRot = originalLocalPlayerRotation + Vector3.up * lookingDirection.y * lookSpeed * Time.deltaTime * MySettings.MouseSensitivity;
        transform.localRotation = Quaternion.Euler(locPlayRot);
        originalLocalPlayerRotation = locPlayRot;

        //Debug.Log(lookingDirection + " " + locCamRot);

        float walkingPower = walkingSpeed;
        if (Vector2.Distance(Vector2.zero, walkingDirection) > 0.1f)
        {
            if (TerrainGenerator.instance)
            {
                float snowHeight = TerrainGenerator.instance.getShowHeight(transform.position) * 4;
                //Debug.Log(snowHeight);
                walkingPower -= Mathf.Sqrt(snowHeight) * 25;
                if (walkingPower < 0)
                    walkingPower = 0;
                TerrainGenerator.instance.walkedOver(transform.position, Time.deltaTime * snowHeight, true);
            }
            //Debug.Log(walkingSpeed + " " + walkingPower + " " + snowHeight);
            float angle = locPlayRot.y * Mathf.PI / 180f;
            Vector3 right = new Vector3(Mathf.Cos(angle), 0, -Mathf.Sin(angle));
            Vector3 fowrord = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            transform.localPosition += (fowrord * walkingDirection.x + right * walkingDirection.y) * walkingPower * Time.deltaTime;

            if (wasWalking <= 0)
                playerSoundManager.UnPause("walk");
            wasWalking = 1;
        }
        else
        {
            walkingPower = 0;
            if (wasWalking > 0)
                playerSoundManager.Pause("walk");
            wasWalking = 0;
        }

        playerAnimator.SetFloat("walking_speed", walkingPower);
        playerAnimator.SetFloat("walking_direction_x", walkingDirection.x * walkingPower);
    }
    internal void playSound(string soundToPlayOnPick)
    {
        playerSoundManager.Play(soundToPlayOnPick);
    }

    internal void craftArrow()
    {
        if (!inventar.ContainsKey("Wood"))
            return;
        int val = inventar["Wood"];
        if (val >= 2)
        {
            nrArrows++;
            inventar["Wood"] -= 2;
        }
        inventarText.text = "";
        foreach (string key in inventar.Keys)
        {
            inventarText.text += key + ":  " + inventar[key] + "\n";
        }
    }

    static float lastFire = 0;
    public void fire()
    {
        if (fires.y > 0.1 && nrArrows > 0)
        {
            playerAnimator.SetBool("fire1", false);
            if (lastFire < 0)
            {
                bowBehaviour.start();
                if (fires.x > 0.1)
                {
                    nrArrows--;
                    lastFire = 1f;
                    bowBehaviour.shoot();
                    playerAnimator.SetBool("fire1", true);
                    playerSoundManager.Play("fire");
                }
            }
            else
            {
                bowBehaviour.stop();
                lastFire -= Time.deltaTime;
                //if (lastFire < 0)
            }
            playerAnimator.SetBool("fire2", true);
            axeTransform.gameObject.SetActive(false);
            bowTransform.gameObject.SetActive(true);
        }
        else
        {
            if (lastFire < 0)
            {
                if (fires.x > 0.1)
                {
                    lastFire = 0.5f;
                    playerAnimator.SetBool("fire1", true);
                    axeTransform.gameObject.SetActive(true);
                    bowTransform.gameObject.SetActive(false);
                    axeBehaviour.isMakingContact = true;
                }
            }
            else
            {
                lastFire -= Time.deltaTime;
                if (lastFire < 0)
                {
                    playerAnimator.SetBool("fire1", false);
                    axeBehaviour.isMakingContact = false;
                }
            }
            bowBehaviour.stop();
            playerAnimator.SetBool("fire2", false);
        }
    }

    public void activatePickUpAction()
    {
        axeTransform.gameObject.SetActive(false);
        bowTransform.gameObject.SetActive(false);
        playerAnimator.SetBool("isPicking", true);
    }
    private float lastInteractionPressed = 0;
    private void pickUp()
    {
        if(interaction.x > 0.1)
        {
            if(lastInteractionPressed == 0)
                if(pickabelObjectDetection.pickabel != null)
                {
                    pickabelObjectDetection.pick();
                    lastInteractionPressed = 10;
                }
        }
        else if (interaction.y > 0.1)
        {
            if (lastInteractionPressed == 0)
            {
                MenuBehaviour.instance.toggleMenu();
                lastInteractionPressed = 10;
            }
        }
        else
        {
            playerAnimator.SetBool("isPicking", false);
            lastInteractionPressed = 0;
        }
    }

    public void addToInventar(Dictionary<string, int> toAdd)
    {
        foreach(string key in toAdd.Keys)
        {
            if (inventar.ContainsKey(key))
                inventar[key] += toAdd[key];
            else inventar.Add(key, toAdd[key]);
        }
        inventarText.text = "";
        foreach(string key in inventar.Keys)
        {
            inventarText.text += key + ":  " + inventar[key] + "\n";
        }
    }

    public void stopController(float time = -1)
    {
        if (time < 0)
        {
            stopMovingIndfinitly = true;
        }
        else
        {
            stopMovindTime = time;
            stopMovingIndfinitly = false;
        }
    }
    public void startController()
    {
        stopMovindTime = -1;
        stopMovingIndfinitly = false;
        playerSoundManager.PauseAll();
        playerAnimator.SetFloat("walking_speed", 0);
        playerAnimator.SetBool("fire1", false);
        playerAnimator.SetBool("fire2", false);
        playerAnimator.SetBool("isPicking", false);
    }
    internal void startTrading(HunterBehaviou hunterBehaviou)
    {
        if (!inventar.ContainsKey("Food") || inventar["Food"] < 5)
        {
            pickabelObjectDetection.centerText.text = "You don't have enought food to trade with";
            return;
        }
        if (hunterBehaviou.inventory.ContainsKey("Wood"))
        {
            int val = hunterBehaviou.inventory["Wood"];
            if(val > 1)
            {
                Dictionary<string, int> hunterTrade = new Dictionary<string, int>();
                Dictionary<string, int> playerTrade = new Dictionary<string, int>();
                hunterTrade.Add("Wood", -1);
                hunterTrade.Add("Food", 5);
                playerTrade.Add("Wood", 1);
                playerTrade.Add("Food", -5);
                hunterBehaviou.trade(hunterTrade);
                this.addToInventar(playerTrade);
            }
            else
                pickabelObjectDetection.centerText.text = "The hunter does not have enought wood";
        }
    }

    void Update()
    {
        getInput();
        pickUp();
        if (stopMovindTime < 0 && !stopMovingIndfinitly)
        {
            fire();
        }
        else
        {
            stopMovindTime -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (stopMovindTime < 0 && !stopMovingIndfinitly)
            move();
    }
}
