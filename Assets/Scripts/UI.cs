using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {
  [SerializeField]
  private Enemy enemy;
  private int count;
  private Text text;
  // Use this for initialization
  void Start () {
    Application.targetFrameRate = 30;
    this.count = 0;
    this.text = this.GetComponent<Text> ();
  }

  // Update is called once per frame
  void Update () {
    this.count++;
    if (this.count % 60 == 0) {
      this.text.text =
        "FPS: " + 1.0f / Time.deltaTime + "\n" +
        "Bullet: " + this.enemy.bulletList.Count;
    }
  }
}
