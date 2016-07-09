﻿using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour {

	public float springForce = 20f;
	public float dampingForce = 5f;

	private Mesh deformingMesh;
	private Vector3[] originalVertices, displacedVertices;
	private Vector3[] vertexVelocities;
	private float uniformScale = 1f;

	private void Start () {
		deformingMesh = GetComponent<MeshFilter> ().mesh;
		originalVertices = deformingMesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];
		for (int i = 0; i < originalVertices.Length; i++) {
			displacedVertices [i] = originalVertices [i];
		}
		vertexVelocities = new Vector3[originalVertices.Length];
	}

	private void Update () {
		uniformScale = transform.localScale.x;
		for (int i = 0; i < displacedVertices.Length; i++) {
			UpdateVertex (i);
		}
		deformingMesh.vertices = displacedVertices;
		deformingMesh.RecalculateNormals();
	}

	public void AddDeformingForce (Vector3 point, float force) {
		point = transform.InverseTransformPoint (point);
		//Debug.DrawLine (Camera.main.transform.position, point);
		for (int i = 0; i < displacedVertices.Length; i++) {
			AddForceToVertex (i, point, force);
		}
	}

	private void UpdateVertex (int i) {
		Vector3 velocity = vertexVelocities [i];
		Vector3 displacement = displacedVertices [i] - originalVertices [i];
		displacement *= uniformScale;
		velocity -= displacement * springForce * Time.deltaTime;
		velocity *= 1 - dampingForce * Time.deltaTime;
		vertexVelocities [i] = velocity;
		displacedVertices [i] += velocity * (Time.deltaTime / uniformScale);
	}

	private void AddForceToVertex (int i, Vector3 point, float force) {
		Vector3 pointToVertex = displacedVertices [i] - point;
		pointToVertex *= uniformScale;
		float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
		float velocity = attenuatedForce * Time.deltaTime;
		vertexVelocities [i] += pointToVertex.normalized * velocity;
	}

}
