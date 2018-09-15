using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
  [SerializeField]
  private Bullet[] bulletPrefab;

  private int frameCount;
  private int currentFuncIndex;

  [SerializeField]
  public List<Bullet> bulletList;
  [SerializeField]
  private int maxBullet;

  private Bullet bullet;
  private Vector3 pos;

  private delegate void DanmakuFunc ();
  private DanmakuFunc[] funcTables;
  private Transform transformComponent;

  private int findLastIndex;

  // Use this for initialization
  void Start () {
    // this.bulletPrefab = (Bullet) Resources.Load ("Resources/Prefabs/Bullet");
    this.funcTables = new DanmakuFunc[3];
    this.funcTables[0] = this.Func00;
    this.funcTables[1] = this.Func01;
    this.funcTables[2] = this.Func02;

    this.bulletList.Clear ();
    for (int i = 0; i < this.maxBullet; i++) {
      this.bullet = Instantiate (this.bulletPrefab[0], this.transform.position, Quaternion.identity) as Bullet;
      this.bulletList.Add (this.bullet);
    }

    this.currentFuncIndex = 0;

    this.transformComponent = this.GetComponent<Transform> ();
  }

  // Update is called once per frame
  void FixedUpdate () {
    this.frameCount++;
    float deltatime = Time.deltaTime;

    this.funcTables[this.currentFuncIndex] ();
  }

  public void ChangeFunc (int num) {
    this.currentFuncIndex = num;
    this.Restart ();
  }

  public void Restart () {
    this.frameCount = 0;
    int size = this.bulletList.Count;
    for (int i = size - 1; i >= 0; i--) {
      Destroy (this.bulletList[i].gameObject);
    }
    this.bulletList.Clear ();
  }

  private Bullet FindBullet () {
    int firstIndex = this.findLastIndex;
    int index = this.findLastIndex;
    while (true) {
      if (this.bulletList[index].active == false) {
        return this.bulletList[index];
      }
      index++;
      if (index == this.maxBullet) {
        index = 0;
      }
      if (index == firstIndex) {
        break;
      }
    }
    // for (int i = this.findLastIndex; i < this.maxBullet; i++) {
    //   if (this.bulletList[i].active == false) {
    //     return this.bulletList[i];
    //   }
    // }
    return null;
  }

  private void Func00 () {
    if (this.frameCount % 2 == 0) {
      this.findLastIndex = 0;
      // 弾の生成
      for (int x = 0; x < 6; x++) {
        for (int y = 0; y < 3; y++) {
          Bullet targetBullet = this.FindBullet ();
          if (targetBullet == null) continue;
          // this.bullet = Instantiate (this.bulletPrefab[0], this.transform.position, Quaternion.identity) as Bullet;
          targetBullet.transformComponent.position = this.transformComponent.position;
          // targetBullet.active = true;
          // targetBullet.gameObject.SetActive (true);
          targetBullet.Activate ();

          // targetBullet.enabled = true;
          targetBullet.speed = 0.05f;
          targetBullet.SetAngle (
            6.28f / 6 * (x + y / 7.0f) + this.frameCount * this.frameCount / 10000.0f,
            Mathf.Sin (this.frameCount / 100f * y) / 12.0f
          );
          // this.bullet.SetColor (Color.HSVToRGB ((this.frameCount / 100) % 1.0f, 1.0f, 1));
          targetBullet.SetColor (Color.HSVToRGB (
            (this.frameCount / 100.0f + x * 0.08f) % 1.0f,
            0.5f,
            0.6f));
          // this.bulletList.Add (this.bullet);
        }
      }
    }

    int count = this.bulletList.Count;
    for (int i = 0; i < this.maxBullet; i++) {
      if (this.bulletList[i].active == true) {
        Bullet target = this.bulletList[i];
        target.Move ();
        this.pos = target.transformComponent.position;
        if (this.pos.x < -10 || this.pos.x > 10 || this.pos.y < -10 || this.pos.y > 10 || this.pos.z < -10 || this.pos.z > 10) {
          // target.active = false;
          // target.gameObject.SetActive (false);
          this.bulletList[i].Diactivate ();

          // Destroy (this.bulletList[i].gameObject);
          // this.bulletList.Remove (this.bulletList[i]);
        }
      }
    }
    // for (int i = count - 1; i >= 0; i--) {
    //   this.bulletList[i].Move ();
    //   this.pos = this.bulletList[i].transform.position;
    //   if (this.pos.x < -10 || this.pos.x > 10 || this.pos.y < -10 || this.pos.y > 10 || this.pos.z < -10 || this.pos.z > 10) {
    //     Destroy (this.bulletList[i].gameObject);
    //     this.bulletList.Remove (this.bulletList[i]);
    //   }
    // }
  }

  private void Func01 () {
    if (this.frameCount % 180 == 1) {
      // 弾の生成
      float angle1 = Random.Range (0, 360);
      float angle2 = Random.Range (0, 360);
      float angle3 = Random.Range (0, 360);
      float col = Random.Range (0, 1.0f);
      for (int y = 0; y < 64; y++) {
        for (int x = 0; x < 32; x++) {
          this.bullet = Instantiate (this.bulletPrefab[0], this.transform.position, Quaternion.identity);
          this.bullet.SetAngle (6.28f / 64 * x, 6.28f / 64 * y);
          this.bullet.velocity = Quaternion.Euler (angle1, angle2, angle3) * this.bullet.velocity;
          this.bullet.SetColor (Color.HSVToRGB (col, 0.5f, 0.6f));
          this.bulletList.Add (this.bullet);
        }
      }
    }

    int count = this.bulletList.Count;
    for (int i = count - 1; i >= 0; i--) {
      this.bulletList[i].Move ();
      this.pos = this.bulletList[i].transform.position;
      if (this.pos.x < -10 || this.pos.x > 10 || this.pos.y < -10 || this.pos.y > 10 || this.pos.z < -10 || this.pos.z > 10) {
        Destroy (this.bulletList[i].gameObject);
        this.bulletList.Remove (this.bulletList[i]);
      }
    }

  }

  private void Func02 () {
    if (this.frameCount % 4 == 0) {
      // 弾の生成
      float col = 0.5f + 0.1f * Mathf.Sin (this.frameCount / 10.0f);
      for (int y = 0; y < 3; y++) {
        for (int x = 0; x < 6; x++) {
          this.bullet = Instantiate (this.bulletPrefab[0], this.transform.position, Quaternion.identity);
          this.bullet.SetAngle (6.28f / 6 * x + Mathf.Sin (this.frameCount / 16.0f) * 6.28f / 12, 6.28f / 16 * 5);
          this.bullet.SetSpeed (0.07f - 0.015f * y);
          this.bullet.accel.Set (0, -0.0003f, 0);
          this.bullet.SetColor (Color.HSVToRGB (col, 0.5f, 0.6f));
          this.bulletList.Add (this.bullet);
        }
      }
    }

    int count = this.bulletList.Count;
    for (int i = count - 1; i >= 0; i--) {
      this.bulletList[i].velocity += this.bulletList[i].accel;
      this.bulletList[i].Move ();
      this.pos = this.bulletList[i].transform.position;
      if (this.pos.x < -10 || this.pos.x > 10 || this.pos.y < -10 || this.pos.y > 10 || this.pos.z < -10 || this.pos.z > 10) {
        Destroy (this.bulletList[i].gameObject);
        this.bulletList.Remove (this.bulletList[i]);
      }
    }

  }
}
