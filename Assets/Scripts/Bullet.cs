using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
  [SerializeField]
  private GameObject plane;
  [SerializeField]
  private Material material;
  [SerializeField]
  private Renderer rendererComponent;
  private Color color;
  public Transform transformComponent;

  [SerializeField]
  public float speed;
  public float accelSpeed;
  public float angle1, angle2;
  public Vector3 velocity;
  public Vector3 accel;
  public bool active;
  public Vector3 positionCache;

  // Use this for initialization

  void Awake () {
    this.active = false;
    this.rendererComponent = this.plane.GetComponent<Renderer> ();
    this.material = this.rendererComponent.material;
    this.transformComponent = this.GetComponent<Transform> ();
    // this.gameObject.SetActive (false);
    this.positionCache = this.transformComponent.position;
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

  public void Draw(){
    // Graphics.DrawMesh()
  }

  public void Move () {
    this.transformComponent.position += this.velocity;
    // this.transformComponent.position.Set (
    //   this.transformComponent.position.x + this.velocity.x,
    //   this.transformComponent.position.y + this.velocity.y,
    //   this.transformComponent.position.z + this.velocity.z);
  }

  public void SetColor (Color _color) {
    this.material.SetColor ("_TintColor", _color);
  }

  public void Activate () {
    this.active = true;
    this.rendererComponent.enabled = true;
  }

  public void Diactivate () {
    this.active = false;
    this.rendererComponent.enabled = false;
  }

}
