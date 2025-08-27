using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulatedScene : MonoBehaviour
{
    // The scene used only for physics simulation (not rendered in the main scene)
    private Scene _simulatedScene;

    // Physics scene reference of the simulated scene
    private PhysicsScene _physicsScene;

    // Root environment transform containing all objects that should be cloned into the simulated scene
    [SerializeField] private Transform _environment;

    // Line renderer used to draw the simulated trajectory
    [SerializeField] private LineRenderer _lineRenderer;

    // Number of physics steps used when simulating the trajectory
    [SerializeField] private int _maxPhysicsInteraction;

    private void Awake()
    {
        // Create a new simulated scene for physics-only simulation
       // CreatePhysicsSimulatedScene();
    }

    private void Start()
    {
        CreatePhysicsSimulatedScene();

    }

    private void CreatePhysicsSimulatedScene()
    {
        // Create a new scene with its own physics world
        _simulatedScene = SceneManager.CreateScene("SimulatedPhysics", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulatedScene.GetPhysicsScene();
        Debug.Log($"_environment: {_environment.name}, figli trovati: {_environment.childCount}");

        foreach (Transform child in _environment)
        {
            if (child != null)
            {
                GameObject clone = Instantiate(child.gameObject);

                // Disabilita tutti i renderer nella gerarchia
                foreach (Renderer r in clone.GetComponentsInChildren<Renderer>())
                {
                    r.enabled = false;
                }

                // Sposta l'oggetto nella scena simulata
                SceneManager.MoveGameObjectToScene(clone, _simulatedScene);
            }
        }

    }

    public void SimulateTrajectory(Ball ballPrefab, Vector3 pos, Vector3 velocity)
    {
        // Istanzia la palla nella scena simulata
        var simulatedObject = Instantiate(ballPrefab, pos, Quaternion.identity);

        Rigidbody rb = simulatedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Resetta la velocità
            rb.angularVelocity = Vector3.zero; // Resetta la rotazione
            rb.useGravity = true; // Abilita la gravità
        }

        // Disabilita il rendering per rendere invisibile la palla simulata
        Renderer renderer = simulatedObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        // Sposta la palla nella scena simulata
        SceneManager.MoveGameObjectToScene(simulatedObject.gameObject, _simulatedScene);

        // Inizializza la velocità della palla
        simulatedObject.Init(velocity, pos);

        // Imposta il numero di punti per il LineRenderer
        _lineRenderer.positionCount = _maxPhysicsInteraction;

        // Simula la fisica passo dopo passo
        for (int i = 0; i < _maxPhysicsInteraction; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime ); // Avanza la simulazione
            _lineRenderer.SetPosition(i, simulatedObject.transform.position); // Salva la posizione
        }

        // Distruggi la palla simulata
        Destroy(simulatedObject.gameObject);
    }
}
