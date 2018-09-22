using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWin : MonoBehaviour {
  [SerializeField]
  private EnemyWin enemy;
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
    // 60フレームに1回UIを更新する
    if (this.count % 60 == 0) {
      float fps = 1.0f / this.time * 60;
      this.time = 0;
      this.text.text =
        "FPS: " + fps + "\n" +
        "Bullet: " + this.enemy.GetBulletNum();
    }
  }
}
