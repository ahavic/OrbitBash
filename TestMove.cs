using Photon.Chat.UtilityScripts;
using System.Threading;
using UnityEngine;

[ExecuteInEditMode]
public class TestMove : MonoBehaviour
{
	public float moveSpeed = 10f;
	public float rotationSpeed = 10f;
	public float rayMagnitude = 3;

	public Vector3 myVect;
	public Vector3 rot;

	private float rotation;
	private Rigidbody rb;

	Quaternion q;
	Vector3 v;

	[SerializeField] Transform target = null;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		Vector3 targetVector = target.position - transform.position;

		Debug.DrawRay(transform.position, transform.forward * rayMagnitude, Color.blue);
		Debug.DrawRay(transform.position, transform.right * rayMagnitude, Color.red);
		Debug.DrawRay(transform.position, transform.up, Color.green);
		Debug.DrawRay(transform.position, targetVector, Color.yellow);


		Vector3 v = Vector3.Project(targetVector, transform.up);
		v = targetVector - v;



		Debug.DrawRay(transform.position, v, Color.magenta);

		print(Vector3.Dot(v.normalized, transform.forward));

		if (Input.GetKeyDown(KeyCode.O))
		{
			print(q.eulerAngles.y);

		}


		//Debug.DrawRay(transform.position, v, Color.green);
	}

	void LateUpdate()
	{


	}
}
