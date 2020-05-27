using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;
using MLAgents.SideChannels;

// 랜덤으로 큐브의 위치를 바꾸어 준다.
// 원래 알고 싶은 것 : EE position ? 어떤 위치에 있더라도 그냥 집는것이다. 
public class GripperAgentScript : Agent
{
    // 오브젝트 정의
    public GameObject left_obj;
    public GameObject right_obj;
    public GameObject target_obj;
    public GameObject connect_obj;

    // Rigidbody 정의
    public Rigidbody leftarm_body;
    public Rigidbody rightarm_body;
    public Rigidbody target_body;
    public Rigidbody connect_body;

    // 조인트 정의
    public HingeJoint left_hinge;
    public HingeJoint right_hinge;

    FloatPropertiesChannel m_ResetParams;

    public override void Initialize()
    {
        leftarm_body = left_obj.GetComponent<Rigidbody>();
        rightarm_body = right_obj.GetComponent<Rigidbody>();
        target_body = target_obj.GetComponent<Rigidbody>();
        connect_body = connect_obj.GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.FloatProperties;
        //파이썬 라이브러리랑 커넥션

        SetResetParameters();
    }
    public void SetResetParameters()
    {
        SetCube();
    }
    public void SetCube()
    {
        //Set the attributes of the ball by fetching the information from the academy
        target_body.mass = m_ResetParams.GetPropertyWithDefault("mass", 0.3f);
        var scale = m_ResetParams.GetPropertyWithDefault("scale", 0.8f);
        target_obj.transform.localScale = new Vector3(scale, scale, scale);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(left_obj.transform.rotation.z);
        sensor.AddObservation(right_obj.transform.rotation.z);
        sensor.AddObservation(target_obj.transform.position - connect_obj.transform.position);
        sensor.AddObservation(target_body.velocity);
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        var actionZ = Mathf.Clamp(vectorAction[0], 0, 0.01f);

        if (left_obj.transform.rotation.z > 8f && actionZ > 0f)

        {
            left_obj.transform.Rotate(new Vector3(0, 0, -1), actionZ);
        }
        if (right_obj.transform.rotation.z < -8f && actionZ > 0f)

        {
            right_obj.transform.Rotate(new Vector3(0, 0, 1), actionZ);
        }

        //상자와 커넥터의 거리계산
        if ((target_obj.transform.position.y - connect_obj.transform.position.y) < -3f ||
            Mathf.Abs(target_obj.transform.position.x - connect_obj.transform.position.x) > 3f ||
            Mathf.Abs(target_obj.transform.position.z - connect_obj.transform.position.z) > 3f)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(0.1f);
        }
    }
    public override void OnEpisodeBegin()
    {
        left_obj.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        left_obj.transform.Rotate(new Vector3(0, 0, -1), Random.Range(0f, 10f));

        right_obj.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        right_obj.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 0f));

        target_body.velocity = new Vector3(0f, 0f, 0f);
        target_obj.transform.position = new Vector3(0, 0, 0);
        //Reset the parameters when the Agent is reset.
        SetResetParameters();
    }
    // Start is called before the first frame update
    void Start()
    {
        left_obj = GameObject.Find("left_arm");
        right_obj = GameObject.Find("obj_right_arm");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
