using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MyAgent : Agent
{
    public void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    public GameObject Target;
    public GameObject BomB;
    private Rigidbody rb;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(0, 0.8f, 0);
        Target.transform.localPosition = new Vector3(Random.value * 8 - 4, 0.55f, Random.value * 8 - 4);
        BomB.transform.localPosition = new Vector3(Random.value * 8 - 4, 0.383f, Random.value * 8 - 4);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;


    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(Target.transform.localPosition);
        sensor.AddObservation(BomB.transform.localPosition);
        sensor.AddObservation(rb.velocity);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float moveSpeed = 5f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            Debug.Log("Collision Detected");
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
        
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Target") 
        {
            Debug.Log("Collision Detected");
            SetReward(+1f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if (collision.gameObject.name == "BomB") 
        {
            Debug.Log("Collision Detected");
            AddReward(-0.5f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
        
    }
}
