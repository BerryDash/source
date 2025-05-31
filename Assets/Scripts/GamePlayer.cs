using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    private float spawnRate = 1f;
    private float nextSpawnTime = 0f;
    private int score = 0;
    private int highscore = 0;
    private float boostLeft = 0f;
    private float slownessLeft = 0f;
    private float screenWidth = 0f;
    private bool isGrounded = false;
    private readonly float groundYPosition = -4.1359f;
    private GameObject bird;
    private Rigidbody2D rb;
    public GameObject pausePanel;
    public AudioSource songLoop;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text boostText;

    void Awake()
    {
        bird = GameObject.Find("Bird");
        rb = bird.GetComponent<Rigidbody2D>();

        highscore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Start()
    {
        screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        highScoreText.text = $"High Score: {highscore}";

        if (PlayerPrefs.GetInt("Setting2", 0) == 1 || Application.isMobilePlatform)
        {
            GameObject leftArrow = new("LeftArrow");
            GameObject rightArrow = new("RightArrow");
            GameObject jumpArrow = new("JumpArrow");
            GameObject restartButton = new("RestartButton");
            GameObject backButton = new("BackButton");

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

            leftArrow.transform.rotation = Quaternion.Euler(0, 0, 90);
            rightArrow.transform.rotation = Quaternion.Euler(0, 0, -90);

            leftArrow.transform.position = new Vector3(-screenWidth / 2.5f, -4f, 0);
            rightArrow.transform.position = new Vector3(screenWidth / 2.5f, -4f, 0);
            restartButton.transform.position = new Vector3(screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0);
            backButton.transform.position = new Vector3(-screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0);
            if (PlayerPrefs.GetInt("Setting3", 0) == 1)
            {
                leftArrow.transform.localScale = new Vector3(screenWidth / 14, screenWidth / 14, 1);
                rightArrow.transform.localScale = new Vector3(screenWidth / 14, screenWidth / 14, 1);
                jumpArrow.transform.localScale = new Vector3(screenWidth / 14, screenWidth / 14, 1);
                restartButton.transform.localScale = new Vector3(screenWidth / 14, screenWidth / 14, 1);
                backButton.transform.localScale = new Vector3(screenWidth / 14, screenWidth / 14, 1);
                jumpArrow.transform.position = new Vector3(screenWidth / 2.5f, -1f, 0);
            }
            else
            {
                leftArrow.transform.localScale = new Vector3(screenWidth / 20, screenWidth / 20, 1);
                rightArrow.transform.localScale = new Vector3(screenWidth / 20, screenWidth / 20, 1);
                jumpArrow.transform.localScale = new Vector3(screenWidth / 20, screenWidth / 20, 1);
                restartButton.transform.localScale = new Vector3(screenWidth / 20, screenWidth / 20, 1);
                backButton.transform.localScale = new Vector3(screenWidth / 20, screenWidth / 20, 1);
                jumpArrow.transform.position = new Vector3(screenWidth / 2.5f, -2f, 0);
            }
        }
    }

    void MoveBird()
    {
        float screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float baseSpeed = 0.18f * (screenWidth / 20.19257f);
        bool doMoveRight = false;
        bool doMoveLeft = false;
        bool doJump = false;

        float movespeed = baseSpeed;

        if (boostLeft > 0)
        {
            movespeed = baseSpeed * 1.39f;
        }
        else if (slownessLeft > 0)
        {
            movespeed = baseSpeed * 0.56f;
        }

        CheckIfGrounded();

        float horizontalInput = Input.GetAxis("Horizontal");

        if (!Application.isMobilePlatform)
        {
            if (horizontalInput < 0 || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                doMoveLeft = true;
            }
            if (horizontalInput > 0 || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                doMoveRight = true;
            }
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || (Input.GetMouseButton(0) && PlayerPrefs.GetInt("Setting2", 0) == 0) || Input.GetKey(KeyCode.JoystickButton0))
            {
                doJump = true;
            }
            if (Input.GetKey(KeyCode.R))
            {
                Respawn();
            }
        }
        if (PlayerPrefs.GetInt("Setting2", 0) != 0 || Application.isMobilePlatform)
        {
            GameObject leftArrow = GameObject.Find("LeftArrow");
            GameObject rightArrow = GameObject.Find("RightArrow");
            GameObject jumpArrow = GameObject.Find("JumpArrow");
            GameObject restartButton = GameObject.Find("RestartButton");
            GameObject backButton = GameObject.Find("BackButton");

            if (Application.isMobilePlatform)
            {
                foreach (Touch touch in Input.touches)
                {
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    touchPosition.z = 0;

                    if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                    {
                        if (leftArrow.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                        {
                            doMoveLeft = true;
                        }
                        else if (rightArrow.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                        {
                            doMoveRight = true;
                        }
                        else if (jumpArrow.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                        {
                            doJump = true;
                        }
                    }

                    if (touch.phase == TouchPhase.Began)
                    {
                        if (restartButton.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                        {
                            Respawn();
                        }
                        else if (backButton.GetComponent<SpriteRenderer>().bounds.Contains(touchPosition))
                        {
                            Pause();
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    clickPosition.z = 0;

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
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    clickPosition.z = 0;

                    if (restartButton.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                    {
                        Respawn();
                    }
                    if (backButton.GetComponent<SpriteRenderer>().bounds.Contains(clickPosition))
                    {
                        Pause();
                    }
                }
            }
        }

        if (doMoveLeft && !doMoveRight)
        {
            bird.transform.position += new Vector3(-movespeed, 0, 0);
            ClampPosition(screenWidth, bird);
            bird.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
            doMoveLeft = false;
        }
        if (doMoveRight && !doMoveLeft)
        {
            bird.transform.position += new Vector3(movespeed, 0, 0);
            ClampPosition(screenWidth, bird);
            bird.transform.localScale = new Vector3(-1.35f, 1.35f, 1.35f);
        }
        if (doJump && isGrounded)
        {
            if (boostLeft > 0)
            {
                rb.linearVelocity = Vector2.up * 12;
            }
            else if (slownessLeft > 0)
            {
                rb.linearVelocity = Vector2.up * 6;
            }
            else
            {
                rb.linearVelocity = Vector2.up * 9;
            }
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
        if (pausePanel.activeSelf) return;
        MoveBird();
        SpawnBerries();

        if (boostLeft > 0)
        {
            boostLeft -= Time.deltaTime;
            boostText.text = "Boost expires in " + string.Format("{0:0.0}", boostLeft) + "s";
        }
        else if (slownessLeft > 0)
        {
            slownessLeft -= Time.deltaTime;
            boostText.text = "Slowness expires in " + string.Format("{0:0.0}", slownessLeft) + "s";
        }
        else
        {
            boostText.text = "";
        }

        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.JoystickButton7) || Input.GetKey(KeyCode.Joystick2Button7))
        {
            Pause();
        }
    }

    void SpawnBerries()
    {
        if (Time.time >= nextSpawnTime)
        {
            nextSpawnTime = Time.time + 1f / spawnRate;

            float spawnProbability = Random.value;
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
            newBerry.transform.position = new Vector3(spawnPositionX, Camera.main.orthographicSize + 1, 0);

            Rigidbody2D rb = newBerry.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(0, -3);
        }
    }

    void Update()
    {
        if (pausePanel.activeSelf) return;
        CheckIfGrounded();
        if (screenWidth != Camera.main.orthographicSize * 2 * Camera.main.aspect)
        {
            screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
            ClampPosition(screenWidth, GameObject.Find("Bird"));
            if (PlayerPrefs.GetInt("Setting2", 0) == 1)
            {
                GameObject leftArrow = GameObject.Find("LeftArrow");
                GameObject rightArrow = GameObject.Find("RightArrow");
                GameObject jumpArrow = GameObject.Find("JumpArrow");
                GameObject restartButton = GameObject.Find("RestartButton");
                GameObject backButton = GameObject.Find("BackButton");

                leftArrow.transform.position = new Vector3(-screenWidth / 2.5f, -4f, 0);
                rightArrow.transform.position = new Vector3(screenWidth / 2.5f, -4f, 0);
                restartButton.transform.position = new Vector3(screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0);
                backButton.transform.position = new Vector3(-screenWidth / 2.3f, Camera.main.orthographicSize - 1.2f, 0);
                if (PlayerPrefs.GetInt("Setting3", 0) == 1)
                {
                    leftArrow.transform.localScale = new Vector3(screenWidth / 14, screenWidth / 14, 1);
                    rightArrow.transform.localScale = new Vector3(screenWidth / 14, screenWidth / 14, 1);
                    jumpArrow.transform.localScale = new Vector3(screenWidth / 14, screenWidth / 14, 1);
                    restartButton.transform.localScale = new Vector3(screenWidth / 14, screenWidth / 14, 1);
                    backButton.transform.localScale = new Vector3(screenWidth / 14, screenWidth / 14, 1);
                    jumpArrow.transform.position = new Vector3(screenWidth / 2.5f, -1f, 0);
                }
                else
                {
                    leftArrow.transform.localScale = new Vector3(screenWidth / 20, screenWidth / 20, 1);
                    rightArrow.transform.localScale = new Vector3(screenWidth / 20, screenWidth / 20, 1);
                    jumpArrow.transform.localScale = new Vector3(screenWidth / 20, screenWidth / 20, 1);
                    restartButton.transform.localScale = new Vector3(screenWidth / 20, screenWidth / 20, 1);
                    backButton.transform.localScale = new Vector3(screenWidth / 20, screenWidth / 20, 1);
                    jumpArrow.transform.position = new Vector3(screenWidth / 2.5f, -2f, 0);
                }
            }
        }
        GameObject[] berries = GameObject.FindGameObjectsWithTag("Berry");
        GameObject[] poisonberries = GameObject.FindGameObjectsWithTag("PoisonBerry");
        GameObject[] ultraberries = GameObject.FindGameObjectsWithTag("UltraBerry");
        GameObject[] slownessberries = GameObject.FindGameObjectsWithTag("SlowBerry");

        foreach (GameObject berry in berries)
        {
            if (berry.transform.position.y < -Camera.main.orthographicSize - 1)
            {
                Destroy(berry);
            }
            else if (Vector3.Distance(bird.transform.position, berry.transform.position) < 1.5f) {
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position);
                Destroy(berry);
                score++;
                UpdateScore(score);
            }
        }

        foreach (GameObject poisonberry in poisonberries)
        {
            if (poisonberry.transform.position.y < -Camera.main.orthographicSize - 1)
            {
                Destroy(poisonberry);
            }
            else if (Vector3.Distance(bird.transform.position, poisonberry.transform.position) < 1.5f) {
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Death"), Camera.main.transform.position);
                Respawn();
            }
        }

        foreach (GameObject ultraberry in ultraberries)
        {
            if (ultraberry.transform.position.y < -Camera.main.orthographicSize - 1)
            {
                Destroy(ultraberry);
            }
            else if (Vector3.Distance(bird.transform.position, ultraberry.transform.position) < 1.5f) {
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Powerup"), Camera.main.transform.position, 0.5f);
                Destroy(ultraberry);
                if (slownessLeft > 0)
                {
                    slownessLeft = 0;
                    score += 1;
                    UpdateScore(score);
                }
                else
                {
                    boostLeft += 10f;
                    score += 5;
                    UpdateScore(score);
                }
            }
        }

        foreach (GameObject slownessberry in slownessberries)
        {
            if (slownessberry.transform.position.y < -Camera.main.orthographicSize - 1)
            {
                Destroy(slownessberry);
            }
            else if (Vector3.Distance(bird.transform.position, slownessberry.transform.position) < 1.5f) {
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Slowness"), Camera.main.transform.position, 0.5f);
                Destroy(slownessberry);
                boostLeft = 0f;
                slownessLeft = 10f;
                if (score > 0)
                {
                    score -= 1;
                    UpdateScore(score);
                }
            }
        }
    }

    void Respawn()
    {
        bird.transform.position = new Vector3(0, groundYPosition, 0);
        bird.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        score = 0;
        boostLeft = 0;
        slownessLeft = 0;
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

    private void CheckIfGrounded()
    {
        GameObject jumpArrow = GameObject.Find("JumpArrow");
        isGrounded = bird.transform.position.y <= groundYPosition + 0.05f;

        rb.gravityScale = isGrounded ? 0 : 1.5f;

        if (bird.transform.position.y < groundYPosition)
        {
            bird.transform.position = new Vector2(bird.transform.position.x, groundYPosition);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }

        if (jumpArrow != null)
        {
            Renderer jumpArrowRenderer = jumpArrow.GetComponent<Renderer>();
            if (jumpArrowRenderer != null)
            {
                jumpArrowRenderer.material.color = isGrounded ? Color.white : Color.red;
            }
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("HighScore", highscore);
        PlayerPrefs.Save();
    }

    void Pause()
    {
        songLoop.Pause();
        pausePanel.SetActive(true);

        GameObject[] berries = GameObject.FindGameObjectsWithTag("Berry");
        GameObject[] poisonberries = GameObject.FindGameObjectsWithTag("PoisonBerry");
        GameObject[] ultraberries = GameObject.FindGameObjectsWithTag("UltraBerry");
        GameObject[] slownessberries = GameObject.FindGameObjectsWithTag("SlowBerry");

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        foreach (GameObject b in berries)
        {
            b.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
        foreach (GameObject pb in poisonberries)
        {
            pb.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
        foreach (GameObject ub in ultraberries)
        {
            ub.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
        foreach (GameObject sb in slownessberries)
        {
            sb.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
}