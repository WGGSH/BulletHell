﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {
  [SerializeField]
  private Enemy enemy;
  private int count;
  private Text text;
  private float time;
  // Use this for initialization
  void Start () {
    Application.targetFrameRate = 30;
    this.count = 0;
    this.text = this.GetComponent<Text> ();
  }

  // Update is called once per frame
  void Update () {
    this.count++;
    this.time += Time.deltaTime;
    if (this.count % 60 == 0) {
      float fps = 1.0f / this.time * 60;
      this.time = 0;
      int num = 0;
      int max = Enemy.BulletList.Count;
      for (int i = 0; i < max; i++) {
        if (Enemy.BulletList[i].active == true) {
          num++;
        }
      }
      this.text.text =
        "FPS: " + fps + "\n" +
        "Bullet: " + num;
    }
  }
}
