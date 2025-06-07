using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePlayer : MonoBehaviour
{
    public static GamePlayer instance;
    private readonly float spawnRate = 1f;
    private float nextSpawnTime;
    private BigInteger score;
    private BigInteger highscore;
    private float boostLeft;
    private float slownessLeft;
    private float screenWidth;
    private bool isGrounded;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text boostText;
    public GameObject bird;
    public GameObject pausePanel;
    public Rigidbody2D rb;
    public AudioSource backgroundMusic;
    public TMP_Text fpsCounter;
    private float nextUpdate;
    private float fps;
    public SpriteRenderer overlayRender;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public GameObject jumpArrow;
    public GameObject restartButton;
    public GameObject backButton;

    void Awake()
    {
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
        instance = this;
        highscore = BigInteger.Parse(PlayerPrefs.GetString("HighScoreV2", "0"));
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SpriteRenderer component = bird.GetComponent<SpriteRenderer>();
        int num = PlayerPrefs.GetInt("icon", 1);
        int num2 = PlayerPrefs.GetInt("overlay", 0);
        if (num == 1)
        {
            component.sprite = Tools.GetIconForUser(PlayerPrefs.GetInt("userId", 0));
        }
        else
        {
            component.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + num);
        }
        if (num2 == 8)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
            overlayRender.transform.localPosition = new UnityEngine.Vector3(-0.37f, 0.32f, 0f);
        }
        else if (num2 == 11)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
            overlayRender.transform.localScale = new UnityEngine.Vector3(1.1f, 1.1f, 1.1f); //yea i didnt feel like doing it for all lmao
            overlayRender.transform.localPosition = new UnityEngine.Vector3(-0.3141809f, 0.4324968f, 0f);
        }
        else if (num2 == 13)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
            overlayRender.transform.localPosition = new UnityEngine.Vector3(-0.3559977f, 0.3179995f, 0f);
        }
        else
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
        }
        if (component.sprite == null)
        {
            component.sprite = Resources.Load<Sprite>("Icons/Icons/bird_1");
            PlayerPrefs.SetInt("icon", 1);
        }
        if (overlayRender.sprite == null && num2 != 0)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_1");
            PlayerPrefs.SetInt("overlay", 1);
        }
        PlayerPrefs.Save();
        backgroundMusic.volume = PlayerPrefs.GetFloat("musicVolume", 1f);
        screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        highScoreText.text = $"High Score: {Tools.FormatWithCommas(highscore)}";
        if (Application.isMobilePlatform)
        {
            leftArrow = new("LeftArrow");
            rightArrow = new("RightArrow");
            jumpArrow = new("JumpArrow");
            restartButton = new("RestartButton");
            backButton = new("BackButton");
            leftArrow.AddComponent<SpriteRenderer>();
            rightArrow.AddComponent<SpriteRenderer>();
            jumpArrow.AddComponent<SpriteRenderer>();
            restartButton.AddComponent<SpriteRenderer>();
            backButton.AddComponent<SpriteRenderer>();

            leftArrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arrows/Arrow");
            leftArrow.GetComponent<SpriteRenderer>().sortingOrder = 1000;
            rightArrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arrows/Arrow");
            rightArrow.GetComponent<SpriteRenderer>().sortingOrder = 1000;
            jumpArrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arrows/Arrow");
            jumpArrow.GetComponent<SpriteRenderer>().sortingOrder = 1000;
            restartButton.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arrows/Restart");
            restartButton.GetComponent<SpriteRenderer>().sortingOrder = 1000;
            backButton.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arrows/Back");
            backButton.GetComponent<SpriteRenderer>().sortingOrder = 1000;

            leftArrow.transform.rotation = UnityEngine.Quaternion.Euler(0f, 0f, 90f);
            rightArrow.transform.rotation = UnityEngine.Quaternion.Euler(0f, 0f, -90f);

            leftArrow.transform.position = new UnityEngine.Vector3(-screenWidth / 2.5f, -4f, 0f);
            rightArrow.transform.position = new UnityEngine.Vector3(screenWidth / 2.5f, -4f, 0f);
            restartButton.transform.position = new UnityEngine.Vector3(screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);
            backButton.transform.position = new UnityEngine.Vector3(-screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);

            leftArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
            rightArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
            jumpArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
            restartButton.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
            backButton.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
            jumpArrow.transform.position = new UnityEngine.Vector3(screenWidth / 2.5f, -1f, 0f);
        }
    }

    void MoveBird()
    {
        float screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        float baseSpeed = 0.18f * (screenWidth / 20.19257f);
        bool doMoveRight = false;
        bool doMoveLeft = false;
        bool doJump = false;
        bool doRestart = false;
        bool doBack = false;
        float movespeed = baseSpeed;
        if (boostLeft > 0f)
        {
            movespeed = baseSpeed * 1.39f;
        }
        else if (slownessLeft > 0f)
        {
            movespeed = baseSpeed * 0.56f;
        }
        CheckIfGrounded();
        bool controllerLeft = Gamepad.current != null && (Gamepad.current.leftStick.left.isPressed || Gamepad.current.dpad.left.isPressed || Gamepad.current.rightStick.left.isPressed);
        bool controllerRight = Gamepad.current != null && (Gamepad.current.leftStick.right.isPressed || Gamepad.current.dpad.right.isPressed || Gamepad.current.rightStick.right.isPressed);
        bool controllerJump = Gamepad.current != null && (Gamepad.current.leftStick.up.isPressed || Gamepad.current.dpad.up.isPressed || Gamepad.current.rightStick.up.isPressed);
        if (!Application.isMobilePlatform)
        {
            if (controllerLeft || Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            {
                doMoveLeft = true;
            }
            if (controllerRight || Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            {
                doMoveRight = true;
            }
            if (controllerJump || Keyboard.current.spaceKey.isPressed || Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed || (Mouse.current.leftButton.isPressed && !Application.isMobilePlatform || (Gamepad.current != null && Gamepad.current.buttonSouth.isPressed)))
            {
                doJump = true;
            }
            if (Keyboard.current.rKey.isPressed)
            {
                doRestart = true;
            }
        }
        if (Application.isMobilePlatform)
        {
            if (!Application.isMobilePlatform)
            {
                if (Mouse.current.leftButton.isPressed)
                {
                    UnityEngine.Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
                    touchPosition.z = 0f;
                    if (leftArrow.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {
                        doMoveLeft = true;
                    }
                    if (rightArrow.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {
                        doMoveRight = true;
                    }
                    if (jumpArrow.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {
                        doJump = true;
                    }
                    if (restartButton.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {
                        doRestart = true;
                    }
                    if (backButton.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                    {
                        doBack = true;
                    }
                }
            }
            else
            {
                var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
                for (int i = 0; i < touches.Count; i++)
                {
                    var pos = touches[i].screenPosition;
                    UnityEngine.Vector3 clickPosition = Camera.main.ScreenToWorldPoint(new UnityEngine.Vector3(pos.x, pos.y, 0f));
                    clickPosition.z = 0f;
                    if (leftArrow.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                    {
                        doMoveLeft = true;
                    }
                    if (rightArrow.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                    {
                        doMoveRight = true;
                    }
                    if (jumpArrow.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                    {
                        doJump = true;
                    }
                    if (restartButton.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                    {
                        doRestart = true;
                    }
                    if (backButton.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                    {
                        doBack = true;
                    }
                }
            }
        }
        if (doMoveLeft && !doMoveRight)
        {
            bird.transform.position += new UnityEngine.Vector3(-movespeed, 0f, 0f);
            ClampPosition(screenWidth, bird);
            bird.transform.localScale = new UnityEngine.Vector3(1.35f, 1.35f, 1.35f);
        }
        if (doMoveRight && !doMoveLeft)
        {
            bird.transform.position += new UnityEngine.Vector3(movespeed, 0f, 0f);
            ClampPosition(screenWidth, bird);
            bird.transform.localScale = new UnityEngine.Vector3(-1.35f, 1.35f, 1.35f);
        }
        if (doJump && isGrounded)
        {
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Jump"), Camera.main.transform.position, 0.75f * PlayerPrefs.GetFloat("sfxVolume", 1f));
            if (boostLeft > 0f)
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 12f;
            }
            else if (slownessLeft > 0f)
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 6f;
            }
            else
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 9f;
            }
        }
        if (doBack)
        {
            TogglePause();
        }
        if (doRestart)
        {
            Respawn();
        }
    }

    void ClampPosition(float screenWidth, GameObject bird)
    {
        float halfWidth = screenWidth / 2.17f;
        float clampedX = Mathf.Clamp(bird.transform.position.x, -halfWidth, halfWidth);
        bird.transform.position = new UnityEngine.Vector3(clampedX, bird.transform.position.y, bird.transform.position.z);
    }

    void FixedUpdate()
    {
        SpawnBerries();
        if (!pausePanel.activeSelf)
        {
            MoveBird();
            if (boostLeft > 0f)
            {
                boostLeft -= Time.deltaTime;
                boostText.text = "Boost expires in " + $"{boostLeft:0.0}" + "s";
            }
            else if (slownessLeft > 0f)
            {
                slownessLeft -= Time.deltaTime;
                boostText.text = "Slowness expires in " + $"{slownessLeft:0.0}" + "s";
            }
            else
            {
                boostText.text = "";
            }
        }
    }

    void SpawnBerries()
    {
        if (!(Time.time >= nextSpawnTime))
        {
            return;
        }
        nextSpawnTime = Time.time + 1f / spawnRate;
        float spawnProbability = Random.value;
        if (!pausePanel.activeSelf)
        {
            GameObject newBerry;
            SpriteRenderer spriteRenderer;
            if (spawnProbability <= 0.6f)
            {
                newBerry = new GameObject("Berry");
                spriteRenderer = newBerry.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/Berry");
                newBerry.tag = "Berry";
            }
            else if (spawnProbability <= 0.8f)
            {
                newBerry = new GameObject("PoisonBerry");
                spriteRenderer = newBerry.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/PoisonBerry");
                newBerry.tag = "PoisonBerry";
            }
            else if (spawnProbability <= 0.9f)
            {
                newBerry = new GameObject("SlowBerry");
                spriteRenderer = newBerry.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/SlowBerry");
                newBerry.tag = "SlowBerry";
            }
            else
            {
                newBerry = new GameObject("UltraBerry");
                spriteRenderer = newBerry.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/UltraBerry");
                newBerry.tag = "UltraBerry";
            }
            spriteRenderer.sortingOrder = -5;

            float screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
            float spawnPositionX = Random.Range(-screenWidth / 2.17f, screenWidth / 2.17f);
            newBerry.transform.position = new UnityEngine.Vector3(spawnPositionX, Camera.main.orthographicSize + 1f, 0f);

            Rigidbody2D rb = newBerry.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearVelocity = new UnityEngine.Vector2(0f, -4f);
        }
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("Setting2", 0) == 1 && Time.time > nextUpdate)
        {
            fps = 1f / Time.deltaTime;
            fpsCounter.text = "FPS: " + Mathf.Round(fps);
            nextUpdate = Time.time + 0.25f;
        }
        if (screenWidth != Camera.main.orthographicSize * 2f * Camera.main.aspect)
        {
            screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
            ClampPosition(screenWidth, bird);
            if (Application.isMobilePlatform)
            {
                leftArrow.transform.position = new UnityEngine.Vector3(-screenWidth / 2.5f, -4f, 0f);
                rightArrow.transform.position = new UnityEngine.Vector3(screenWidth / 2.5f, -4f, 0f);
                restartButton.transform.position = new UnityEngine.Vector3(screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);
                backButton.transform.position = new UnityEngine.Vector3(-screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);

                leftArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                rightArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                jumpArrow.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                restartButton.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                backButton.transform.localScale = new UnityEngine.Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                jumpArrow.transform.position = new UnityEngine.Vector3(screenWidth / 2.5f, -1f, 0f);
            }
        }
        GameObject[] berries = GameObject.FindGameObjectsWithTag("Berry");
        GameObject[] poisonberries = GameObject.FindGameObjectsWithTag("PoisonBerry");
        GameObject[] ultraberries = GameObject.FindGameObjectsWithTag("UltraBerry");
        GameObject[] slownessberries = GameObject.FindGameObjectsWithTag("SlowBerry");
        if (!pausePanel.activeSelf)
        {
            CheckIfGrounded();
            GameObject[] array5 = berries;
            foreach (GameObject berry in array5)
            {
                if (berry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(berry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, berry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * PlayerPrefs.GetFloat("sfxVolume", 1f));
                    Destroy(berry);
                    score++;
                    UpdateScore(score);
                }
                berry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
            }
            array5 = poisonberries;
            foreach (GameObject gameObject7 in array5)
            {
                if (gameObject7.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(gameObject7);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, gameObject7.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Death"), Camera.main.transform.position, 1.2f * PlayerPrefs.GetFloat("sfxVolume", 1f));
                    Respawn();
                }
                gameObject7.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
            }
            array5 = ultraberries;
            foreach (GameObject gameObject8 in array5)
            {
                if (gameObject8.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(gameObject8);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, gameObject8.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Powerup"), Camera.main.transform.position, 0.35f * PlayerPrefs.GetFloat("sfxVolume", 1f));
                    Destroy(gameObject8);
                    if (slownessLeft > 0f)
                    {
                        slownessLeft = 0f;
                        score++;
                        UpdateScore(score);
                    }
                    else
                    {
                        boostLeft += 10f;
                        score += 5;
                        UpdateScore(score);
                    }
                }
                gameObject8.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
            }
            array5 = slownessberries;
            foreach (GameObject gameObject9 in array5)
            {
                if (gameObject9.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(gameObject9);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, gameObject9.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Downgrade"), Camera.main.transform.position, 0.35f * PlayerPrefs.GetFloat("sfxVolume", 1f));
                    Destroy(gameObject9);
                    boostLeft = 0f;
                    slownessLeft = 10f;
                    if (score > 0)
                    {
                        score--;
                        UpdateScore(score);
                    }
                }
                gameObject9.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
            }
        }
        else
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = UnityEngine.Vector2.zero;
            GameObject[] array5 = berries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
            }
            array5 = poisonberries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
            }
            array5 = ultraberries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
            }
            array5 = slownessberries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
            }
        }
        if (!Application.isMobilePlatform && (Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current != null && (Gamepad.current.startButton.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame))))
        {
            TogglePause();
        }
    }

    void Respawn()
    {
        bird.transform.position = new UnityEngine.Vector3(0f, -4.3f, 0f);
        bird.transform.localScale = new UnityEngine.Vector3(1.35f, 1.35f, 1.35f);
        rb.gravityScale = 0f;
        rb.linearVelocity = UnityEngine.Vector2.zero;
        score = 0;
        boostLeft = 0f;
        slownessLeft = 0f;
        UpdateScore(score);
        GameObject[] berries = GameObject.FindGameObjectsWithTag("Berry");
        GameObject[] poisonberries = GameObject.FindGameObjectsWithTag("PoisonBerry");
        GameObject[] ultraberries = GameObject.FindGameObjectsWithTag("UltraBerry");
        GameObject[] slownessberries = GameObject.FindGameObjectsWithTag("SlowBerry");

        foreach (GameObject b in berries)
        {
            Destroy(b);
        }
        foreach (GameObject pb in poisonberries)
        {
            Destroy(pb);
        }
        foreach (GameObject ub in ultraberries)
        {
            Destroy(ub);
        }
        foreach (GameObject sb in slownessberries)
        {
            Destroy(sb);
        }
    }

    void UpdateScore(BigInteger score)
    {
        if (score > highscore)
        {
            highscore = score;
        }
        PlayerPrefs.SetString("HighScoreV2", highscore.ToString());
        PlayerPrefs.Save();
        scoreText.text = "Score: " + Tools.FormatWithCommas(score);
        highScoreText.text = "High Score: " + Tools.FormatWithCommas(highscore);
    }

    void CheckIfGrounded()
    {
        isGrounded = bird.transform.position.y <= -4.1299996f;

        rb.gravityScale = isGrounded ? 0f : 1.5f;

        if (bird.transform.position.y < -4.1359f)
        {
            bird.transform.position = new UnityEngine.Vector2(bird.transform.position.x, -4.1359f);
            rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocity.x, 0f);
        }
        if (jumpArrow != null && jumpArrow.GetComponent<Renderer>() != null)
        {
            jumpArrow.GetComponent<Renderer>().material.color = isGrounded ? Color.white : Color.red;
        }
    }

    public void TogglePause()
    {
        if (pausePanel.activeSelf)
        {
            DisablePause();
        }
        else
        {
            EnablePause();
        }
    }

    public void EnablePause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        backgroundMusic.Pause();
        pausePanel.SetActive(true);
    }

    public void DisablePause()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        backgroundMusic.Play();
        pausePanel.SetActive(false);
    }

    void OnApplicationPause(bool pause)
    {
        if (pause) EnablePause();
    }

    void OnApplicationQuit()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}