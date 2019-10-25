using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public Movement movement; // Skapar en referens till movement skriptet för spelaren.

    // Det här skriptet hanterar kollisioner av spelaren.
    // Spelaren explodera till ett 100-tal mindre bitar som då sprider sig i en radie runt spelaren.
    public Material playerMaterial; // Sätter en variabel för materialet för dessa mindre kuber.

    public float cubeSize = 0.2f; // Offentliga variablar för att kunna ändra storlek och hur många kuber som ska skapas. 
    public int cubesInRow = 5;

    public float cubesPivotDistance;
    public Vector3 cubesPivot;

    // Egenskaper för att bestämma hur stark explosionen ska vara. Dessa kan ändras i Unity.
    public float explosionForce = 50f;
    public float explosionRadius = 4f;
    public float explosionUpward = 0.4f;

    void Start()
    {
        GetComponent<AudioSource>();

        // Kalkylerar Pivot distance (Sväng avstånd)
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);
    }

    void OnCollisionEnter(Collision collisionInfo) // "Trigger"n för kollisionen.
    {
        if (collisionInfo.collider.tag == "Obstacle") // Explosionen händer bara om det spelaren kolliderar med har en tag "Obstacle"
        {
            explode(); // Kallar explode metoden.
            movement.enabled = false; // Stänger av movement skriptet för spelaren.
            FindObjectOfType<GameManager>().GameOver(); // Kallar GameOver metoden/funktionen i GameManager
        }
    }

    private void explode() // Metoden/funktionen som hanterar själva explosionen.
    {
        // Tar bort spelar objektet.
        gameObject.SetActive(false);

        // Itererar igenom det 3 gånger för varje axis. x, y, z
        for (int x = 0; x < cubesInRow; x++)
        {
            for (int y = 0; y < cubesInRow; y++)
            {
                for (int z = 0; z < cubesInRow; z++)
                {
                    createPiece(x, y, z); // Skapar kuberna
                }
            }
        }

        // Hämtar positionen för explosionen.
        Vector3 explosionPos = transform.position;
        // Hämtar en collider för explosionen.
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);

        // Lägger till en kraft för varje collider.
        foreach (Collider hit in colliders)
        {
            // Hämtar RigidBody komponenten från Collidern
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null) // Om RigidBody inte är null
            {
                // Tillämpar en explosionskraft med givna parametrar.
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }
    }

    void createPiece(int x, int y, int z)
    {
        // Skapar mindre bitarna av spelaren.
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Cube); // Vilken form bitarna har.

        // Sätter bitarna position och skala
        piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        // Lägger till en RigidBody till varje bit.
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = cubeSize;

        // Sätter material för varje bit.
        piece.GetComponent<Renderer>().material = playerMaterial;
    }
}
