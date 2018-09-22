using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateWin : MonoBehaviour {
  [SerializeField]
  private float rotateSpeed;
  // [SerializeField]
  // private Enemy enemy;
  private float angle1;
  private float angle2;

  private Vector3 screenPoint;
  private Vector3 offset;

  // Use this for initialization
  void Start () {

  }

  // Update is called once per frame
  void Update () {
    // this.angle1 += Input.GetAxis ("Vertical") * this.rotateSpeed;
    // this.angle2 += Input.GetAxis ("Horizontal") * this.rotateSpeed;

    if (Input.GetMouseButtonDown (0)) {
      this.screenPoint = Input.mousePosition;
    }
    if (Input.GetMouseButton (0)) {
      Vector3 currentScreenPoint = Input.mousePosition;

      this.angle1 += (currentScreenPoint.y - this.screenPoint.y) * this.rotateSpeed;
      this.angle2 += (currentScreenPoint.x - this.screenPoint.x) * this.rotateSpeed;

      this.screenPoint = currentScreenPoint;

      if (this.angle1 > 180) {
        this.angle1 = 180;
      }
      if (this.angle1 < -180) {
        this.angle1 = -180;
      }
      this.transform.rotation = Quaternion.Euler (this.angle1, this.angle2, 0);

      // 全ての弾の向きをカメラに合わせる
      // int count = this.enemy.BulletList.Count;
      // for (int i = 0; i < count; i++) {
      //   Bullet targetBullet = this.enemy.BulletList[i];
      //   if (targetBullet.active == false) {
      //     continue;
      //   }
      //   targetBullet.TransformCache.LookAt (targetBullet.CameraTransformCache.position);
      // }

    }

  }
}
