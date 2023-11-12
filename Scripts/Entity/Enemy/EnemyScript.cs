using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject summoner;
    public HealthSystemScript health;
    // Start is called before the first frame update
    void Start()
    {
        health = new HealthSystemScript(10);
    }

    // Update is called once per frame
    void Update()
    {
        if (health.getHealth() <= 0f)
        {
            var summon = summoner.GetComponent<EnemySpawnScript>();
            summon.CurrentEnemyCount--;
            Destroy(this.gameObject);
        }
    }
}
