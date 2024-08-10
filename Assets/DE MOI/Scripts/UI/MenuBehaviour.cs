using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    public static MenuBehaviour instance;

    public Image textToShowBackground;
    public Text textToShowText;
    public Transform menu;
    public Transform[] menuTabs;
    public int curentMenuTab = 0;

    private string[] textToShow = new string[0];
    private int curentTextTOShow = 100;
    private float lastTextToShowChange = 0;

    public Font PixelFont;
    public Volume globalVolume;
    public Light mainLight;
    public RenderPipelineAsset curentRenderPipeline;
    private bool isFullScreen = true;
    private Vector2Int rezolution;
    private int rezolutionScalling = 3;
    private bool useScallinf = false;

    public Text inventartextDisplay;
    public Text arrayTextDisplay;

    public AudioClip endMusic;
    public Transform credits;
    public Text finalVeridictText;
    
    //private motionBlur = null;

    public void showText(string[] text)
    {
        textToShow = text;
        curentTextTOShow = 0;
        textToShowText.text = textToShow[curentTextTOShow];
        textToShowBackground.gameObject.SetActive(true);
        textToShowText.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        rezolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        //Screen.SetResolution(rezolution.x, rezolution.y, isFullScreen);
        updateinvetnarDesplay();
        Cursor.visible = false;
    }

    public void updateinvetnarDesplay()
    {
        if (PlayerController.instance == null)
            return;

        var inv = PlayerController.instance.inventar;
        if (inv.ContainsKey("Wood") && inv.ContainsKey("Food") && inv["Wood"] >= 100 && inv["Food"] >= 200)
        {
            inventartextDisplay.text = "I have all that I need. I must return home now.";
        }
        else
        {
            inventartextDisplay.text = "I have collencted:\n";
            if (PlayerController.instance.inventar.Count == 0)
                inventartextDisplay.text += "Nothing";
            else
            {
                foreach (string key in PlayerController.instance.inventar.Keys)
                {
                    inventartextDisplay.text += key + " - " + PlayerController.instance.inventar[key] + "\n";
                }
            }
            arrayTextDisplay.text = "I have " + PlayerController.instance.nrArrows + " arrows";
        }
    }

    public void craftArrow()
    {
        PlayerController.instance.craftArrow();
        updateinvetnarDesplay();
    }
    // Update is called once per frame
    void Update()
    {
        if(curentTextTOShow < textToShow.Length )
        {
            lastTextToShowChange += Time.deltaTime;
            if (lastTextToShowChange > 2)
            {
                textToShowText.text = textToShow[curentTextTOShow];
                curentTextTOShow++;
                lastTextToShowChange = 0;
            }
        }
        else if(lastTextToShowChange < 2)
        {
            lastTextToShowChange += Time.deltaTime;
            if (lastTextToShowChange > 2)
            {
                textToShowBackground.gameObject.SetActive(false);
                textToShowText.gameObject.SetActive(false);
            }
        }


        //Vector2Int newR = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
        //if(newR.x != rezolution.x || newR.y != rezolution.y)
        //{
        //    rezolution = newR;
        //    toggleDynamicRezolution(useScallinf);
        //}
    }

    public void nextTab(int direction)
    {
        curentMenuTab += direction;
        if (curentMenuTab < 0)
            curentMenuTab = menuTabs.Length - 1;
        else if (curentMenuTab >= menuTabs.Length)
            curentMenuTab = 0;
        for (int i = 0; i < menuTabs.Length; i++)
            if (i != curentMenuTab)
                menuTabs[i].gameObject.SetActive(false);
        menuTabs[curentMenuTab].gameObject.SetActive(true);

        PlayerController.instance.playSound("paheFlip");
    }

    public void toggleShadow(bool bol)
    {
        if (!bol)
            mainLight.shadows = LightShadows.None;
        else
            mainLight.shadows = LightShadows.Soft;
    }
    public void toggleDynamicRezolution(bool bol)
    {
        Debug.Log("DING");
        useScallinf = bol;
        if (bol)
            Screen.SetResolution(rezolution.x / rezolutionScalling, rezolution.y / rezolutionScalling, isFullScreen);
        else
            Screen.SetResolution(rezolution.x, rezolution.y, isFullScreen);
    }

    public void toggleFullscreen(bool bol)
    {
        isFullScreen = bol;
        Screen.fullScreen = bol;
    }

    public void toggleEasyToReadText(bool bol)
    {
        Text[] texts = menu.GetComponentsInChildren<Text>(true);
        if (bol)
        {
            Font ariel = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].font = ariel;
            }
        }
        else
        {
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].font = PixelFont;
            }
        }
    }

    internal void toggleMenu()
    {
        if (menu.gameObject.activeSelf)
        {
            menu.gameObject.SetActive(false);
            PlayerController.instance.startController();
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
            PlayerController.instance.stopController();
            updateinvetnarDesplay();
            menu.gameObject.SetActive(true);
            for (int i = 0; i < menuTabs.Length; i++)
                if (i != curentMenuTab)
                    menuTabs[i].gameObject.SetActive(false);
            menuTabs[curentMenuTab].gameObject.SetActive(true);
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void changeMauseSensitivity(Slider slider)
    {
        MySettings.MouseSensitivity = slider.value;
    }
    public void changeMasterSoundVolume(Slider slider)
    {
        MySettings.OverallVolumes = slider.value;
    }
    public void changeEffectSoundVolume(Slider slider)
    {
        MySettings.EffectsVolume = slider.value;
    }
    public void changeMusicSoundVolume(Slider slider)
    {
        MySettings.MusicVolume = slider.value;
    }

    public void playEnd()
    {
        Cursor.visible = true;
        menu.gameObject.SetActive(true);
        foreach (Transform chl in menu)
            chl.gameObject.SetActive(false);
        credits.gameObject.SetActive(true);
        PlatformBehaviour crPB = credits.GetComponent<PlatformBehaviour>();
        crPB.isMoving = true;

        if (PlayerController.instance.nrHunterKilled == 0)
        {
            TerrainGenerator.instance.goodEndTree.gameObject.SetActive(true);
            finalVeridictText.text = "You got the good ending";
        }
        else
        {
            TerrainGenerator.instance.badEndTree.gameObject.SetActive(true);
            finalVeridictText.text = "You got the bad ending";
        }

        BackgroundMusicScript.instance.playSound(endMusic, BackgroundMusicScript.instance.volumeMultiplyer, BackgroundMusicScript.instance.transitionSpeed);
        PlayerController.instance.playerCamera.gameObject.AddComponent<FinalCameraBehaviour>();
        PlayerController.instance.startController();
        Destroy(PlayerController.instance);
        PlayerController.instance = null;

        TerrainGenerator.instance.enabled = false;
    }
}
