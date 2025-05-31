using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    float spawnRate = 1f;
    float nextSpawnTime;
    int score;
    int highscore;
    float boostLeft;
    float slownessLeft;
    float screenWidth;
    bool isGrounded;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text boostText;
    public GameObject bird;
    public GameObject pausePanel;
    public Rigidbody2D rb;
    public AudioSource backgroundMusic;
    //public TMP_Text fpsCounter;
    float nextUpdate;
    float fps;
    //public SpriteRenderer overlayRender;

    void Awake()
    {
        highscore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SpriteRenderer component = bird.GetComponent<SpriteRenderer>();
        int num = PlayerPrefs.GetInt("icon", 1);
        int num2 = PlayerPrefs.GetInt("overlay", 1);
        if (num == 1)
        {
            if (PlayerPrefs.GetInt("userID", 0) == 1)
            {
                component.sprite = Resources.Load<Sprite>("icons/icons/bird_-1");
            }
            else if (PlayerPrefs.GetInt("userID", 0) == 2)
            {
                component.sprite = Resources.Load<Sprite>("icons/icons/bird_-2");
            }
            else if (PlayerPrefs.GetInt("userID", 0) == 4)
            {
                component.sprite = Resources.Load<Sprite>("icons/icons/bird_-3");
            }
            else if (PlayerPrefs.GetInt("userID", 0) == 6)
            {
                component.sprite = Resources.Load<Sprite>("icons/icons/bird_-4");
            }
            else
            {
                component.sprite = Resources.Load<Sprite>("icons/icons/bird_1");
            }
        }
        else
        {
            component.sprite = Resources.Load<Sprite>("icons/icons/bird_" + num);
        }
        if (num2 == 8)
        {
            //overlayRender.sprite = Resources.Load<Sprite>("icons/overlays/overlay_8");
            //overlayRender.transform.localPosition = new Vector3(-0.35f, 0.3f, 0f);
        }
        else
        {
            //overlayRender.sprite = Resources.Load<Sprite>("icons/overlays/overlay_" + num2);
        }
        if (component.sprite == null)
        {
            component.sprite = Resources.Load<Sprite>("icons/icons/bird_1");
            PlayerPrefs.SetInt("icon", 1);
        }
        //if (overlayRender.sprite == null && num2 != 0)
        //{
            //overlayRender.sprite = Resources.Load<Sprite>("icons/overlays/overlay_1");
            //PlayerPrefs.SetInt("overlay", 1);
        //}
        PlayerPrefs.Save();
        backgroundMusic.volume = PlayerPrefs.GetFloat("musicVolume", 1f);
        screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        GameObject.Find("HighScoreText").GetComponent<TMP_Text>().text = $"High Score: {highscore}";
        if (PlayerPrefs.GetInt("Setting2", 0) == 1)
        {
            GameObject leftArrow = new GameObject("LeftArrow");
            GameObject rightArrow = new GameObject("RightArrow");
            GameObject jumpArrow = new GameObject("JumpArrow");
            GameObject restartButton = new GameObject("RestartButton");
            GameObject backButton = new GameObject("BackButton");
            leftArrow.AddComponent<SpriteRenderer>();
            rightArrow.AddComponent<SpriteRenderer>();
            jumpArrow.AddComponent<SpriteRenderer>();
            restartButton.AddComponent<SpriteRenderer>();
            backButton.AddComponent<SpriteRenderer>();

            leftArrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arrows/Arrow");
            rightArrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arrows/Arrow");
            jumpArrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arrows/Arrow");
            restartButton.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arrows/Restart");
            backButton.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arrows/Back");

            leftArrow.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            rightArrow.transform.rotation = Quaternion.Euler(0f, 0f, -90f);

            leftArrow.transform.position = new Vector3(-screenWidth / 2.5f, -4f, 0f);
            rightArrow.transform.position = new Vector3(screenWidth / 2.5f, -4f, 0f);
            restartButton.transform.position = new Vector3(screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);
            backButton.transform.position = new Vector3(-screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);
            if (PlayerPrefs.GetInt("Setting3", 0) == 1)
            {
                leftArrow.transform.localScale = new Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                rightArrow.transform.localScale = new Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                jumpArrow.transform.localScale = new Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                restartButton.transform.localScale = new Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                backButton.transform.localScale = new Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                jumpArrow.transform.position = new Vector3(screenWidth / 2.5f, -1f, 0f);
            }
            else
            {
                leftArrow.transform.localScale = new Vector3(screenWidth / 20f, screenWidth / 20f, 1f);
                rightArrow.transform.localScale = new Vector3(screenWidth / 20f, screenWidth / 20f, 1f);
                jumpArrow.transform.localScale = new Vector3(screenWidth / 20f, screenWidth / 20f, 1f);
                restartButton.transform.localScale = new Vector3(screenWidth / 20f, screenWidth / 20f, 1f);
                backButton.transform.localScale = new Vector3(screenWidth / 20f, screenWidth / 20f, 1f);
                jumpArrow.transform.position = new Vector3(screenWidth / 2.5f, -2f, 0f);
            }
        }
    }

    void MoveBird()
    {
        float screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        float baseSpeed = 0.18f * (screenWidth / 20.19257f);
        bool doMoveRight = false;
        bool doMoveLeft = false;
        bool doJump = false;
        bool flag4 = false;
        bool flag5 = false;
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
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (!Application.isMobilePlatform)
        {
            if (horizontalInput < 0f || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                doMoveLeft = true;
            }
            if (horizontalInput > 0f || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                doMoveRight = true;
            }
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || (Input.GetMouseButton(0) && PlayerPrefs.GetInt("Setting2", 0) == 0) || Input.GetKey(KeyCode.JoystickButton0))
            {
                doJump = true;
            }
            if (Input.GetKey(KeyCode.R))
            {
                flag5 = true;
            }
        }
        if (PlayerPrefs.GetInt("Setting2", 0) == 1)
        {
            GameObject leftArrow = GameObject.Find("LeftArrow");
            GameObject rightArrow = GameObject.Find("RightArrow");
            GameObject jumpArrow = GameObject.Find("JumpArrow");
            GameObject restartButton = GameObject.Find("RestartButton");
            GameObject backButton = GameObject.Find("BackButton");
            if (!Application.isMobilePlatform)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 point2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    point2.z = 0f;
                    if (restartButton.GetComponent<SpriteRenderer>().bounds.Contains(point2))
                    {
                        flag5 = true;
                    }
                    if (backButton.GetComponent<SpriteRenderer>().bounds.Contains(point2))
                    {
                        flag4 = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touchPosition = Input.GetTouch(i);
                    Vector3 clickPosition = Camera.main.ScreenToWorldPoint(touchPosition.position);
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
                        flag5 = true;
                    }
                    if (backButton.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                    {
                        flag4 = true;
                    }
                }
            }
        }
        if (doMoveLeft && !doMoveRight)
        {
            bird.transform.position += new Vector3(-movespeed, 0f, 0f);
            ClampPosition(screenWidth, bird);
            bird.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
        }
        if (doMoveRight && !doMoveLeft)
        {
            bird.transform.position += new Vector3(movespeed, 0f, 0f);
            ClampPosition(screenWidth, bird);
            bird.transform.localScale = new Vector3(-1.35f, 1.35f, 1.35f);
        }
        if (doJump && isGrounded)
        {
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Jump"), Camera.main.transform.position, 0.75f * PlayerPrefs.GetFloat("sfxVolume", 1f));
            if (boostLeft > 0f)
            {
                rb.linearVelocity = Vector2.up * 12f;
            }
            else if (slownessLeft > 0f)
            {
                rb.linearVelocity = Vector2.up * 6f;
            }
            else
            {
                rb.linearVelocity = Vector2.up * 9f;
            }
        }
        if (flag4)
        {
            TogglePause();
        }
        if (flag5)
        {
            Respawn();
        }
    }

    void ClampPosition(float screenWidth, GameObject bird)
    {
        float halfWidth = screenWidth / 2.17f;
        float clampedX = Mathf.Clamp(bird.transform.position.x, -halfWidth, halfWidth);
        bird.transform.position = new Vector3(clampedX, bird.transform.position.y, bird.transform.position.z);
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
                boostText.GetComponent<TMP_Text>().text = "Boost expires in " + $"{boostLeft:0.0}" + "s";
            }
            else if (slownessLeft > 0f)
            {
                slownessLeft -= Time.deltaTime;
                boostText.GetComponent<TMP_Text>().text = "Slowness expires in " + $"{slownessLeft:0.0}" + "s";
            }
            else
            {
                boostText.GetComponent<TMP_Text>().text = "";
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
            newBerry.transform.position = new Vector3(spawnPositionX, Camera.main.orthographicSize + 1f, 0f);

            Rigidbody2D rb = newBerry.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(0f, -4f);
        }
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("Setting4", 0) == 1 && Time.time > nextUpdate)
        {
            fps = 1f / Time.deltaTime;
            //fpsCounter.text = "FPS: " + Mathf.Round(fps);
            nextUpdate = Time.time + 0.25f;
        }
        if (screenWidth != Camera.main.orthographicSize * 2f * Camera.main.aspect)
        {
            screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
            ClampPosition(screenWidth, bird);
            if (PlayerPrefs.GetInt("Setting2", 0) == 1)
            {
                GameObject leftArrow = GameObject.Find("LeftArrow");
                GameObject rightArrow = GameObject.Find("RightArrow");
                GameObject jumpArrow = GameObject.Find("JumpArrow");
                GameObject restartButton = GameObject.Find("RestartButton");
                GameObject backButton = GameObject.Find("BackButton");
                leftArrow.transform.position = new Vector3(screenWidth / 2.5f, -4f, 0f);
                rightArrow.transform.position = new Vector3(screenWidth / 2.5f, -4f, 0f);
                restartButton.transform.position = new Vector3(screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);
                backButton.transform.position = new Vector3(-screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0f);
                if (PlayerPrefs.GetInt("Setting3", 0) == 1)
                {
                    leftArrow.transform.localScale = new Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                    rightArrow.transform.localScale = new Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                    jumpArrow.transform.localScale = new Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                    restartButton.transform.localScale = new Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                    backButton.transform.localScale = new Vector3(screenWidth / 14f, screenWidth / 14f, 1f);
                    jumpArrow.transform.position = new Vector3(screenWidth / 2.5f, -1f, 0f);
                }
                else
                {
                    leftArrow.transform.localScale = new Vector3(screenWidth / 20f, screenWidth / 20f, 1f);
                    rightArrow.transform.localScale = new Vector3(screenWidth / 20f, screenWidth / 20f, 1f);
                    jumpArrow.transform.localScale = new Vector3(screenWidth / 20f, screenWidth / 20f, 1f);
                    restartButton.transform.localScale = new Vector3(screenWidth / 20f, screenWidth / 20f, 1f);
                    backButton.transform.localScale = new Vector3(screenWidth / 20f, screenWidth / 20f, 1f);
                    jumpArrow.transform.position = new Vector3(screenWidth / 2.5f, -2f, 0f);
                }
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
                else if (Vector3.Distance(bird.transform.position, berry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * PlayerPrefs.GetFloat("sfxVolume", 1f));
                    Destroy(berry);
                    score++;
                    UpdateScore(score);
                }
                berry.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, -4f);
            }
            array5 = poisonberries;
            foreach (GameObject gameObject7 in array5)
            {
                if (gameObject7.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(gameObject7);
                }
                else if (Vector3.Distance(bird.transform.position, gameObject7.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Death"), Camera.main.transform.position, 1.2f * PlayerPrefs.GetFloat("sfxVolume", 1f));
                    Respawn();
                }
                gameObject7.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, -4f);
            }
            array5 = ultraberries;
            foreach (GameObject gameObject8 in array5)
            {
                if (gameObject8.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(gameObject8);
                }
                else if (Vector3.Distance(bird.transform.position, gameObject8.transform.position) < 1.5f)
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
                gameObject8.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, -4f);
            }
            array5 = slownessberries;
            foreach (GameObject gameObject9 in array5)
            {
                if (gameObject9.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(gameObject9);
                }
                else if (Vector3.Distance(bird.transform.position, gameObject9.transform.position) < 1.5f)
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
                gameObject9.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, -4f);
            }
        }
        else
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            GameObject[] array5 = berries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
            array5 = poisonberries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
            array5 = ultraberries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
            array5 = slownessberries;
            for (int i = 0; i < array5.Length; i++)
            {
                array5[i].GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
        }
        if (!Application.isMobilePlatform && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)))
        {
            TogglePause();
        }
    }

    void Respawn()
    {
        bird.transform.position = new Vector3(0f, -4.3f, 0f);
        bird.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
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

    void UpdateScore(int score)
    {
        if (score > highscore)
        {
            highscore = score;
        }
        PlayerPrefs.SetInt("HighScore", highscore);
        PlayerPrefs.Save();
        scoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + highscore;
    }

    void CheckIfGrounded()
    {
        GameObject jumpArrow = GameObject.Find("JumpArrow");
        isGrounded = bird.transform.position.y <= -4.1299996f;

        rb.gravityScale = (isGrounded ? 0f : 1.5f);

        if (bird.transform.position.y < -4.18f)
        {
            bird.transform.position = new Vector2(bird.transform.position.x, -4.18f);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
        if (jumpArrow != null && jumpArrow.GetComponent<Renderer>() != null)
        {
            jumpArrow.GetComponent<Renderer>().material.color = (isGrounded ? Color.white : Color.red);
        }
    }

    void TogglePause()
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

    void EnablePause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        backgroundMusic.Pause();
        pausePanel.SetActive(value: true);
    }

    void DisablePause()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        backgroundMusic.Play();
        pausePanel.SetActive(value: false);
    }

    void OnApplicationPause()
    {
        EnablePause();
    }

    void OnApplicationQuit()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
