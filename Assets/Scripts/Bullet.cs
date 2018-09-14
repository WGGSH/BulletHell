using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
  [SerializeField]
  private GameObject plane;
  [SerializeField]
  private Material material;
  private Color color;

  [SerializeField]
  public float speed;
  public float accelSpeed;
  public float angle1, angle2;
  public Vector3 velocity;
  public Vector3 accel;

  // Use this for initialization

  void Awake () {

  }
  void Start () { }

  // Update is called once per frame
  void Update () {
    this.transform.LookAt (Camera.main.transform.position);
  }

  public void SetAngle (float _angle1, float _angle2) {
    this.angle1 = _angle1;
    this.angle2 = _angle2;
    this.velocity.Set (
      Mathf.Cos (this.angle1) * Mathf.Cos (this.angle2) * this.speed,
      Mathf.Sin (this.angle2) * this.speed,
      Mathf.Sin (this.angle1) * Mathf.Cos (this.angle2) * this.speed
    );
  }

  public void SetSpeed (float speed) {
    this.velocity.Normalize ();
    this.velocity *= speed;
  }

  public void Move () {
    this.transform.position += this.velocity;
  }

  public void SetColor (Color _color) {
    this.material = this.plane.GetComponent<Renderer> ().material;
    this.material.SetColor ("_TintColor", _color);
  }

}
