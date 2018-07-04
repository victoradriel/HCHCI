using UnityEngine;

namespace CompleteProject
{
    public class EnemyManager : MonoBehaviour {
        public PlayerHealth playerHealth;       // Reference to the player's heatlh.
        public EnemyHealth enemyHealth;
        public GameObject[] enemy;                // The enemy prefab to be spawned.
        //public float spawnTime = 3f;            // How long between each spawn.
        public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
        public GameObject enemyalive;
        public GameObject enemyrender;
        public GameObject enemylight;
        public string enemyname;
        public string renderenemyname;
        public Renderer rend;
        public Light light;
        public bool renderisoff = false;


        void Start () {
            // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
            // InvokeRepeating ("Spawn", spawnTime, spawnTime);
            Spawn();
        }

        void Update() {
            if (!GameObject.Find(enemyname + "(Clone)")) {
                Spawn();
            } else if(!renderisoff) {
                // disable rendering of enemy
                enemyrender = GameObject.Find(renderenemyname);
                rend = enemyrender.GetComponent<Renderer>();
                rend.enabled = false;

                // disable light
                enemylight = GameObject.Find("Point light");
                light = enemylight.GetComponent<Light>();
                light.enabled = false;

                renderisoff = true;
            }
            else {
                enemyalive = GameObject.Find(enemyname + "(Clone)");
                enemyHealth = enemyalive.GetComponent<EnemyHealth>();

                if (enemyHealth.currentHealth <= 40f) {
                    rend.enabled = true;
                } else if (enemyHealth.currentHealth <= 80f) {
                    light.enabled = true;
                }
            }
        }

        void adjustName() {
            if (renderenemyname == "Zombear")
                renderenemyname = "ZomBear";

            if (renderenemyname == "ZomBunny")
                renderenemyname = "Zombunny";
        }

        void Spawn ()  {
            // If the player has no health left...
            if(playerHealth.currentHealth <= 0f)
            {
                // ... exit the function.
                return;
            }

            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range (0, spawnPoints.Length);
            int enemyIndex = Random.Range(0, enemy.Length);

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            Instantiate (enemy[enemyIndex], spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
            enemyname = enemy[enemyIndex].name;
            renderenemyname = enemyname;
            adjustName();
            renderisoff = false;
        }
    }
}