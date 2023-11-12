using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour
{
    public GameObject enemy;
    public int enemyCount;
    public int CurrentEnemyCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentEnemyCount <= 0) Spawn(enemyCount);
    }
    public void Spawn(int enemyCount)
    {
        while (enemyCount > 0)
        {
            GameObject e = Instantiate(this.enemy, new Vector3(this.gameObject.transform.position.x + Random.onUnitSphere.x * 5, this.gameObject.transform.position.y + Random.onUnitSphere.y * 5, this.gameObject.transform.position.z + Random.onUnitSphere.z*5), Quaternion.identity);
            var enemy = e.GetComponent<EnemyScript>();
            enemy.summoner = this.gameObject;
            enemyCount--;
            CurrentEnemyCount++;
        }
    }
}
