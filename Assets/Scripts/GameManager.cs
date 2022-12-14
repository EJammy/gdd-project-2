using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

#region Serialized Variables
    [SerializeField]
    public BeeDefender defender;

    [SerializeField]
    GameObject defenderPlaceholder;

    [SerializeField]
    Bug spawnBug;

    public void SpawnBug()
    {
        Instantiate(spawnBug);
    }

    public TextMeshProUGUI beeCount;
    public TextMeshProUGUI honeyCount;
    public TextMeshProUGUI scoreText;
    public GameObject endScreen;
#endregion

#region Unity Functions
    void Awake()
    {
        // DontDestroyOnLoad(gameObject);
        Singletons.gameManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        beeCount.text = "x  " + Singletons.hivemind.GetBugAmount();
        honeyCount.text = "Honey:  $ " + Statistics.honey;
    }

    // Update is called once per frame
    void Update()
    {
        beeCount.text = "x  " + Singletons.hivemind.GetBugAmount();
        honeyCount.text = "Honey:  $ " + Statistics.honey;
        if (!Singletons.isUpgradeMenuOpen && Input.GetKeyDown(KeyCode.Mouse0)) trySpawnBee();
    }
#endregion

#region Helper Functions
    bool checkPlacement(Vector2 pos)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(pos, 0.1f, Vector2.up, 0);
        foreach (var obj in hits)
        {
            // Debug.Log(obj.transform.tag);
            if (obj.transform.tag == "Room" || obj.transform.tag == "Occupied") return false;
        }
        return true;
    }

    void trySpawnBee()
    {
        var spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPos.z = Camera.main.nearClipPlane;
        var target = Singletons.hivemind.LastBug(BugType.lvl0);
        if (checkPlacement(spawnPos) && target != null)
        {
            // target.WorkRoom = null;
            // Instantiate(defender, spawnPos, Quaternion.identity);
            GameObject placeholder = Instantiate(defenderPlaceholder, spawnPos, Quaternion.identity);
            if (spawnPos.x > 0)
            {
                placeholder.GetComponent<SpriteRenderer>().flipX = true;
            }
            target.Deploy(spawnPos, () => {
                Destroy(placeholder.GetComponent<SpriteRenderer>());
                placeholder.transform.parent = Instantiate(defender, spawnPos, Quaternion.identity).transform;
            });
        }
    }
#endregion

#region Scene Management?
    public void EndGame()
    {
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.GetComponent<RectTransform>() == null && obj != this && obj.GetComponent<Camera>() == null)
                Destroy(obj);
        }
        scoreText.text = "Score: " + (int)Time.timeSinceLevelLoad*10/3;
        endScreen.SetActive(true);
    }
#endregion
}
