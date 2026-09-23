using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isFollowing = false;

    public int health = 3;
    public int maxHealth = 3;

    public float speed = 3.5f;
    public float detectionDistance = 5;
    public float stoppingDistance = 1;

    public PlayerController player;
    public Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float targetDistance = Vector2.Distance(player.transform.position, transform.position);

        isFollowing = targetDistance <= detectionDistance;

        if (isFollowing)
        {
            if (player.transform.position.x < transform.position.x)
                rb.linearVelocityX = -speed;

            else if (player.transform.position.x > transform.position.x)
                rb.linearVelocityX = speed;

            else if (Mathf.Abs(player.transform.position.x - transform.position.x) <= stoppingDistance)
                rb.linearVelocityX = 0;
        }
        else
            rb.linearVelocityX = 0;
    }

    // TO DO: Attack player when you feel necessary
    // TO DO: Take damage when applied
}
