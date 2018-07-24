using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class MoodBox : MonoBehaviour {

    void OnDrawGizmos() {
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Gizmos.color = new Color(0.5f, 0.9f, 1.0f, 0.35f);
    Gizmos.DrawCube(GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
  }
}
