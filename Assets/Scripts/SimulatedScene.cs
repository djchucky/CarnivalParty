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

    void Start()
    {
        // Get the root transform of this GameObject
        _environment = gameObject.transform.root;

        // Create a new simulated scene for physics-only simulation
        CreatePhysicsSimulatedScene();
    }

    private void CreatePhysicsSimulatedScene()
    {
        // Create a new scene with its own physics world
        _simulatedScene = SceneManager.CreateScene("SimulatedPhysics", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulatedScene.GetPhysicsScene();

        // Clone every object from the environment into the simulated scene
        foreach (Transform child in _environment)
        {
            GameObject item = child.gameObject;
            if (item != null && item.transform.childCount > 0)
            {
                // If object has children, clone it and disable renderers (so it won’t be visible)
                GameObject parent = Instantiate(item, item.transform.position, Quaternion.identity);
                foreach (Transform t in parent.transform)
                {
                    Renderer r = t.gameObject.GetComponent<Renderer>();
                    if (r != null)
                    {
                        r.enabled = false; // Disable mesh rendering
                    }
                    // Move the cloned parent into the simulated scene
                    SceneManager.MoveGameObjectToScene(parent, _simulatedScene);
                }
            }
            else if (item != null)
            {
                // If the object has no children, just clone and disable its renderer
                GameObject itemClone = Instantiate(item, item.transform.position, item.transform.rotation);
                Renderer renderer = itemClone.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }

                // Move the cloned object into the simulated scene
                SceneManager.MoveGameObjectToScene(itemClone, _simulatedScene);
            }
        }
    }

    // Simulates the trajectory of a ball and draws it using a line renderer
    public void SimulateTrajectory(Ball ballPrefab, Vector3 pos, Vector3 velocity)
    {
        // Instantiate the ball inside the main scene
        var simulatedObject = Instantiate(ballPrefab, pos, Quaternion.identity);

        // Disable rendering so the simulated ball is invisible
        if (simulatedObject.GetComponent<Renderer>() != null)
        {
            simulatedObject.GetComponent<Renderer>().enabled = false;
        }

        // Move ball into the simulated physics scene
        SceneManager.MoveGameObjectToScene(simulatedObject.gameObject, _simulatedScene);

        // Initialize ball velocity
        simulatedObject.Init(velocity);

        // Set the number of points for the trajectory line
        _lineRenderer.positionCount = _maxPhysicsInteraction;

        // Run physics simulation step by step and record positions
        for (int i = 0; i < _maxPhysicsInteraction; i++)
        {
            // Advance the simulation (scaled for faster progression)
            _physicsScene.Simulate(Time.fixedDeltaTime * 3f);

            // Save the ball’s current position into the line renderer
            _lineRenderer.SetPosition(i, simulatedObject.transform.position);
        }

        // Destroy the simulated ball after the trajectory has been calculated
        Destroy(simulatedObject.gameObject);
    }
}
