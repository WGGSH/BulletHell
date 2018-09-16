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
  public Transform cameraTransformComponent;

  [SerializeField]
  public float speed;
  public float accelSpeed;
  public float angle1, angle2;
  public Vector3 velocity;
  public Vector3 accel;
  public bool active;
  public Vector3 positionCache;

  private Mesh mesh;

  // Use this for initialization

  void Awake () {
    this.active = false;
    this.rendererComponent = this.plane.GetComponent<Renderer> ();
    this.material = this.rendererComponent.material;
    this.transformComponent = this.GetComponent<Transform> ();
    // this.gameObject.SetActive (false);
    this.positionCache = this.transformComponent.position;
    this.cameraTransformComponent = Camera.main.transform;
    this.rendererComponent.enabled = false;

    // 動的Mesh生成
    this.mesh = new Mesh ();
    float size = 0.01f;
    this.mesh.vertices = new Vector3[] {
      new Vector3 (-size, -size, 0),
      new Vector3 (size, -size, 0),
      new Vector3 (size, size, 0),
      new Vector3 (-size, size, 0),
    };
    this.mesh.uv = new Vector2[] {
      new Vector2 (0, 0),
      new Vector2 (1f, 0),
      new Vector2 (1f, 1f),
      new Vector2 (0, 1f),
    };
    this.mesh.triangles = new int[] {
      0,
      1,
      2,
      0,
      2,
      3,
    };
    this.mesh.RecalculateNormals ();
    this.mesh.RecalculateBounds ();
  }

  void Start () { }

  // Update is called once per frame
  void Update () {
    // this.transform.LookAt (Camera.main.transform.position);
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

  public void Draw () {
    // Graphics.DrawMesh()
    // this.transform.LookAt (this.cameraTransformComponent);
    // Graphics.DrawMesh (this.mesh, this.transformComponent.position,
    //   Quaternion.identity, this.material, 0);

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
    this.transformComponent.LookAt (this.cameraTransformComponent.position);
  }

  public void Diactivate () {
    this.active = false;
    this.rendererComponent.enabled = false;
  }

}
