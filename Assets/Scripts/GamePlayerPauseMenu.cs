using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button backButton;
    public Button continueButton;
    public AudioSource songLoop;

    void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        });
        continueButton.onClick.AddListener(Unpause);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    void Unpause()
    {
        songLoop.UnPause();
        gameObject.SetActive(false);
        GameObject[] berries = GameObject.FindGameObjectsWithTag("Berry");
        GameObject[] poisonberries = GameObject.FindGameObjectsWithTag("PoisonBerry");
        GameObject[] ultraberries = GameObject.FindGameObjectsWithTag("UltraBerry");
        GameObject[] slownessberries = GameObject.FindGameObjectsWithTag("SlowBerry");

        foreach (GameObject b in berries)
        {
            b.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -3);
        }
        foreach (GameObject pb in poisonberries)
        {
            pb.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -3);
        }
        foreach (GameObject ub in ultraberries)
        {
            ub.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -3);
        }
        foreach (GameObject sb in slownessberries)
        {
            sb.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -3);
        }
    }
}
