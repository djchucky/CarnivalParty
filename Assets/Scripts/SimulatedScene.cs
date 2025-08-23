using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulatedScene : MonoBehaviour
{
    private Scene _simulatedScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _environment;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private int _maxPhysicsInteraction;

    void Start()
    {
        _environment = gameObject.transform.root;
        CreatePhysicsSimulatedScene();
    }

    private void CreatePhysicsSimulatedScene()
    {
        _simulatedScene = SceneManager.CreateScene("SimulatedPhysics",new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulatedScene.GetPhysicsScene();

        foreach (Transform child in _environment)
        {
            GameObject item = child.gameObject;
            if(item != null && item.transform.childCount > 0)
            {        
                GameObject parent = Instantiate(item, item.transform.position, Quaternion.identity);
                foreach (Transform t in parent.transform)
                {
                    Renderer r = t.gameObject.GetComponent<Renderer>();
                    if (r != null)
                    {
                        r.enabled = false;
                    }
                    SceneManager.MoveGameObjectToScene(parent, _simulatedScene);
                }

            }
            else if (item != null)
            {
                GameObject itemClone = Instantiate(item, item.transform.position, item.transform.rotation);
                Renderer renderer = itemClone.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }

                SceneManager.MoveGameObjectToScene(itemClone, _simulatedScene);
            }
        }
    }

    public void SimulateTrajectory(Ball ballPrefab,Vector3 pos, Vector3 velocity)
    {
        var simulatedObject = Instantiate(ballPrefab, pos, Quaternion.identity);

        if(simulatedObject.GetComponent<Renderer>() != null)
        {
            simulatedObject.GetComponent<Renderer>().enabled = false;
        }

        SceneManager.MoveGameObjectToScene(simulatedObject.gameObject,_simulatedScene);
        simulatedObject.Init(velocity);

        _lineRenderer.positionCount = _maxPhysicsInteraction;       

        for (int i = 0; i < _maxPhysicsInteraction; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime * 3f);
            _lineRenderer.SetPosition(i, simulatedObject.transform.position);
        }

        Destroy(simulatedObject.gameObject);
    }
}
