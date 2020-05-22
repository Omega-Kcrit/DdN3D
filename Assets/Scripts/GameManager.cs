using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //HEARTHRATE VARIABLES
    public float startHearthRate = 120;
    public float baseHearthRateLoss = 4;
    float hearthRate;
    float hearthRateLoss;

    //PLAYER & CAM
    GameObject player;
    GameObject camera;

    //CHECKPOINTS
    public Transform startPoint;
    Transform lastCheckpoint;

    //UI
    public Text hearthRateText;
    public Text deathsCountText;
    public GameObject winScreen;
    public GameObject deathScreen;
    public GameObject pauseScreen;

    //OTHER
    public GameObject gate;
    public float hearthRateTimer = 0.5f;
    float timer;
    bool gamePaused = false;
    bool playerDead = false;
    int deathsCount = 0;


    //SINGLETON
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        Cursor.visible = false;

        if (_instance != null && _instance != this)
            Destroy(this.gameObject);

        else _instance = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        lastCheckpoint = startPoint;

        hearthRate = startHearthRate;
        hearthRateLoss = baseHearthRateLoss;
        timer = hearthRateTimer;
    }

    void Update()
    {
        //CONTADOR DE PULSACIONES
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            hearthRate -= hearthRateLoss;
            timer = hearthRateTimer;
        }

        //GREEN
        if (hearthRate > 140)
            hearthRateText.color = Color.green;

        //YELLOW
        if (hearthRate < 140 && hearthRate > 120)
            hearthRateText.color = Color.yellow;

        //RED
        if (hearthRate < 120 && hearthRate > 100)
            hearthRateText.color = Color.red;

        //DEATH
        else if (hearthRate < 100 && !playerDead)
        {
            hearthRate = 0;
            GameOver();
        }

        //PAUSE
        if (Input.GetKeyDown(KeyCode.Escape) && !playerDead)
            PauseGame();
            
        //UPDATE UI
        hearthRateText.text = hearthRate.ToString("00");
    }

    public void ModifyHearthRate(float value)
    {
        hearthRateLoss = value;
    }

    public void OpenGate()
    {
        gate.GetComponent<Animation>().Play();
        gate.GetComponent<AudioSource>().Play();
    }

    public void PauseGame()
    {
        if (!gamePaused)
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);

            camera.GetComponent<CameraMovement>().enabled = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            gamePaused = true;
        }
           
        else 
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);

            Cursor.visible = false;

            camera.GetComponent<CameraMovement>().enabled = true;

            gamePaused = false;
        }
    }

    public void ReplayGame()
    {
        Time.timeScale = 1;

        camera.GetComponent<CameraMovement>().enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToLastCheckpoint()
    {
        Debug.Log("RESTAURANDO ULTIMO CHECKPOINT");
        Time.timeScale = 1;

        deathScreen.SetActive(false);
        
        Cursor.visible = false;

        camera.GetComponent<CameraMovement>().enabled = true;

        hearthRate = 120;
        playerDead = false;
        deathsCount++;

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = lastCheckpoint.position;
        player.GetComponent<CharacterController>().enabled = true;
    }

    public void SetLastCheckpoint(Transform cp)
    {
        lastCheckpoint = cp;
    }

    public void WinGame()
    {
        Time.timeScale = 0;

        camera.GetComponent<CameraMovement>().enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Confined;

        winScreen.SetActive(true);

        if (deathsCount == 1)
            deathsCountText.text = "TOU DIED " + deathsCount + " TIME.";

        else deathsCountText.text = "TOU DIED " + deathsCount + " TIMES.";

        playerDead = true;
    }

    public void GameOver()
    {
        Time.timeScale = 0;

        camera.GetComponent<CameraMovement>().enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Confined;

        deathScreen.SetActive(true);

        playerDead = true;
    }
}
