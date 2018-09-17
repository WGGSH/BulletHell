using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
  [SerializeField]
  private Bullet[] bulletPrefab; // 使用する弾のPrefab

  private int frameCount; // フレーム数
  private int currentFuncIndex; // 実行中の弾幕番号

  [SerializeField]
  private List<Bullet> bulletList;
  [SerializeField]
  private int maxBullet;

  // キャッシュデータ
  private Bullet bullet;
  private Vector3 pos;
  private int findLastIndex;

  // 弾幕リスト
  private delegate void DanmakuFunc ();
  private DanmakuFunc[] funcTables;
  private Transform transformComponent;

  public List<Bullet> BulletList {
    get { return this.bulletList; }
  }

  // public int MaxBullet {
  //   get { return this.maxBullet; }
  // }

  // Use this for initialization
  void Start () {
    // 弾幕の登録
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

  // 実行する弾幕の変更
  public void ChangeFunc (int num) {
    this.currentFuncIndex = num;
    this.Restart ();
  }

  // 弾幕を初期状態に戻す
  public void Restart () {
    this.frameCount = 0;
    this.findLastIndex = 0;
    for (int i = 0; i < this.bulletList.Count; i++) {
      this.bulletList[i].Diactivate ();
    }
    this.bulletList.Clear ();
  }

  // 使用可能な弾を探す
  private Bullet FindBullet () {
    int firstIndex = this.findLastIndex;
    int index = firstIndex;
    Bullet targetBullet;
    while (true) {
      targetBullet = this.bulletList[index++];
      if (targetBullet.active == false) {
        this.findLastIndex = index;
        if (this.findLastIndex == this.maxBullet) {
          this.findLastIndex = 0;
        }
        return targetBullet;
      }
      if (index == this.maxBullet) {
        index = 0;
      }
      // ループを1周した場合,生成可能な弾のスペースが無いため終了
      if (index == firstIndex) {
        break;
      }
    }
    return null;
  }

  // 弾幕
  private void Func00 () {
    if (this.frameCount % 2 == 0) {
      // 弾の生成
      for (int x = 0; x < 6; x++) {
        for (int y = 0; y < 3; y++) {
          Bullet targetBullet = this.FindBullet ();
          if (targetBullet == null) continue;

          // ----- ここから弾幕の設定 -----

          // 初期座標
          targetBullet.TransformCache.position = this.transformComponent.position;

          // 初期速度
          targetBullet.speed = 0.05f;

          // 初期角度
          targetBullet.Angle1 = 6.28f / 6 * (x + y / 7.0f) + this.frameCount * this.frameCount / 10000.0f;
          targetBullet.Angle2 = 6.28f / 3 * y + Mathf.Sin (this.frameCount / 100f * y) / 12.0f;

          // 色設定
          targetBullet.SetColor (Color.HSVToRGB (
            (this.frameCount / 100.0f + x * 0.08f) % 1.0f,
            0.5f,
            0.6f));

          // ----- 弾幕設定ここまで -----

          targetBullet.Activate ();
        }
      }
    }

    // 弾の移動処理
    // ここが一番重い
    Bullet target;
    for (int i = 0; i < this.maxBullet; i++) {
      target = this.bulletList[i];
      if (target.active) {
        target.Move (); // 重い:要改善?
        this.pos = target.TransformCache.position;
        if (this.pos.x < -10 || this.pos.x > 10 || this.pos.y < -10 || this.pos.y > 10 || this.pos.z < -10 || this.pos.z > 10) {
          target.Diactivate ();
        }
      }
    }

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
