using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSensor : MonoBehaviour
{
	[Min (0.1f)]
    public float radius = 1f;     // Radius of the base of the cone
	[Min (1f)]
    public float height = 2f;     // Height of the cone
	[Min (6)]
    public int segments = 6;     // Number of segments to create the cone

	//Public
	public bool Scan (Vector3 targetPos)
	{
		transform.forward = targetPos - transform.position;
		ConeMesh tmp = ConeMesh.CreateConeMesh (radius, height, segments);
		tmp.Translate (transform.position);
		tmp.RotateMesh (transform.rotation);

		Vector3 origin = tmp.mesh.vertices[segments +1];
		bool hit = ScanIntern (origin, targetPos);

		for (int i = 0; i <= segments; i++)
			hit |= ScanIntern (origin, tmp.mesh.vertices[i]);
			//Debug.DrawLine (origin, tmp.mesh.vertices[i]);

		return hit;
	}

	//Private

	bool ScanIntern (Vector3 origin, Vector3 target)
	{
		Vector3 direction = target - origin;
		Ray ray = new Ray (origin, direction);

		RaycastHit hit;

		bool inRange = false;

		if (Physics.Raycast(ray, out hit, height))
			inRange = hit.collider.CompareTag ("Player");

		Debug.DrawRay (ray.origin, ray.direction * height, inRange ? Color.green : Color.red);

		return inRange;
	}

	private void OnDrawGizmosSelected()
	{
		ConeMesh coneMesh = ConeMesh.CreateConeMesh (radius, height, segments);
		coneMesh.Translate (transform.position);
		coneMesh.RotateMesh (transform.rotation);

		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + (transform.forward * height));
		Gizmos.DrawWireMesh (coneMesh.mesh);
		//Gizmos.DrawWireMesh(coneMesh.mesh, transform.position, transform.rotation);
	}
}
