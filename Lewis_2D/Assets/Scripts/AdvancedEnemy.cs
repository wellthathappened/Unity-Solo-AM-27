using UnityEngine;

public class AdvancedEnemy : Enemy
{
    private void Start()
    {
        // Only copy initializers if you have a new start function in this class
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        speed = 10;
    }

    private void Update()
    {
        // Whatever put in here overrides stuff from enemy class
    }

    public void shooting()
    {
        // Put shooting code in here
    }
}
