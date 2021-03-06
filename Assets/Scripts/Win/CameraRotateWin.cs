﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateWin : MonoBehaviour {
  [SerializeField]
  private float rotateSpeed;
  private float angle1;
  private float angle2;

  private Vector3 screenPoint;
  private Vector3 offset;

  // Use this for initialization
  void Start () {

  }

  // Update is called once per frame
  void Update () {

    // マウスクリック時に座標を記録する
    if (Input.GetMouseButtonDown (0)) {
      this.screenPoint = Input.mousePosition;
    }
    if (Input.GetMouseButton (0)) {
      // マウスをドラッグすると，カメラを回転する
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
    }

  }
}
