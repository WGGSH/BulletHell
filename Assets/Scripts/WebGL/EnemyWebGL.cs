using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
// using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class EnemyWebGL : MonoBehaviour {
  [SerializeField]
  private BulletWebGL[] bulletPrefab; // 使用する弾のPrefab

  static public int frameCount; // フレーム数
  private int currentFuncIndex; // 実行中の弾幕番号

  [SerializeField]
  static private List<BulletWebGL> bulletList;
  static private int maxBullet;
  [SerializeField]
  private int MAXBULLET;

  // キャッシュデータ
  static public BulletWebGL bullet;
  private Vector3 pos;

  public static List<int> cacheBulletIndex;
  static private int findLastIndex = 0;

  // 弾幕リスト
  private delegate void DanmakuFunc ();
  private DanmakuFunc[] funcTables;
  private Transform transformComponent;

  // Lua
  LuaEnv lua;
  [SerializeField]
  TextAsset luaScript;
  private string luaText;
  [SerializeField, Multiline]
  private string luaHeaderText;

  // ローカルファイルのパス取得用フィールド
  [SerializeField]
  private InputField inputField;

  static public List<BulletWebGL> BulletList {
    get { return bulletList; }
  }

  // public int MaxBullet {
  //   get { return this.maxBullet; }
  // }

  // Use this for initialization
  void Start () {
    maxBullet = this.MAXBULLET;

    // 弾幕の登録
    this.funcTables = new DanmakuFunc[4];
    this.funcTables[0] = this.Func00;
    this.funcTables[1] = this.Func01;
    this.funcTables[2] = this.Func02;
    this.funcTables[3] = this.Func03;

    bulletList = new List<BulletWebGL> ();
    bulletList.Clear ();
    for (int i = 0; i < maxBullet; i++) {
      EnemyWebGL.bullet = Instantiate (this.bulletPrefab[0], this.transform.position, Quaternion.identity) as BulletWebGL;
      bulletList.Add (EnemyWebGL.bullet);
    }

    this.currentFuncIndex = 3;

    this.transformComponent = this.GetComponent<Transform> ();

    this.lua = new LuaEnv ();

    this.luaScript = new TextAsset ();
    this.luaScript = Resources.Load ("test.lua", typeof (TextAsset)) as TextAsset;

    EnemyWebGL.cacheBulletIndex = new List<int> ();
  }

  // Update is called once per frame
  void FixedUpdate () {
    frameCount++;
    float deltatime = Time.deltaTime;

    this.funcTables[this.currentFuncIndex] ();

    // this.lua = new LuaEnv ();
    // this.lua.DoString (this.luaScript.text);
    // this.lua.Dispose ();
    // this.lua.DoString ("require 'Resources/test'");
  }

  public IEnumerator LoadScript (string url) {
    WWW www = new WWW (url);
    yield return www;

    // 読み込み終了
    // Debug.Log (www.text);

    // 読み込んだファイルが正しく実行できるかチェックする
    this.luaText = www.text;
    try {
      this.lua.DoString (this.luaHeaderText + this.luaText);
    } catch (LuaException ex) {
      // MessageBox.ShowAlertBox (ex.Message, "Lua Script Error");
      this.luaText = "";
      Debug.Log ("Lua Script Error");
    }
    this.Restart ();

    // string path = EditorUtility.OpenFilePane ("Overwrite with png", "", "txt");
    // if (path.Length != 0) {
    //   // this.luaScript = new TextAsset (path);
    //   // this.luaScript = Resources.Load (path, typeof (TextAsset)) as TextAsset;
    //   FileInfo fiA = new FileInfo (path);
    //   StreamReader srA = new StreamReader (fiA.OpenRead (), Encoding.UTF8);
    //   this.luaText = srA.ReadToEnd ();
    // }
  }

  public void FileSelect () {
    StartCoroutine (LoadScript (this.inputField.text));
  }

  // 実行する弾幕の変更
  public void ChangeFunc (int num) {
    this.currentFuncIndex = num;
    this.Restart ();
  }

  // 弾幕を初期状態に戻す
  public void Restart () {
    frameCount = 0;
    findLastIndex = 0;
    for (int i = 0; i < bulletList.Count; i++) {
      bulletList[i].Diactivate ();
    }
  }

  // 使用可能な弾を探す
  static private BulletWebGL FindBullet () {
    int firstIndex = findLastIndex;
    int index = firstIndex;
    BulletWebGL targetBullet;
    while (true) {
      targetBullet = bulletList[index++];
      if (targetBullet.active == false) {
        findLastIndex = index;
        if (findLastIndex == maxBullet) {
          findLastIndex = 0;
        }
        return targetBullet;
      }
      if (index == maxBullet) {
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
    if (frameCount % 2 == 0) {
      // 弾の生成
      for (int x = 0; x < 6; x++) {
        for (int y = 0; y < 3; y++) {
          BulletWebGL targetBullet = FindBullet ();
          if (targetBullet == null) continue;

          // ----- ここから弾幕の設定 -----

          // 初期座標
          targetBullet.TransformCache.position = this.transformComponent.position;

          // 初期速度
          targetBullet.speed = 0.05f;

          // 初期角度
          targetBullet.Angle1 = 6.28f / 6 * (x + y / 7.0f) + frameCount * frameCount / 10000.0f;
          targetBullet.Angle2 = 6.28f / 3 * y + Mathf.Sin (frameCount / 100f * y) / 12.0f;

          // 色設定
          targetBullet.SetColor (Color.HSVToRGB (
            (frameCount / 100.0f + x * 0.08f) % 1.0f,
            0.5f,
            0.6f));

          // ----- 弾幕設定ここまで -----

          targetBullet.Activate ();
        }
      }
    }

    // 弾の移動処理
    // ここが一番重い
    BulletWebGL target;
    for (int i = 0; i < maxBullet; i++) {
      target = bulletList[i];
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
    if (frameCount % 180 == 1) {
      // 弾の生成
      float angle1 = Random.Range (0, 360);
      float angle2 = Random.Range (0, 360);
      float angle3 = Random.Range (0, 360);
      float col = Random.Range (0, 1.0f);
      for (int y = 0; y < 64; y++) {
        for (int x = 0; x < 32; x++) {
          bullet = Instantiate (this.bulletPrefab[0], this.transform.position, Quaternion.identity);
          bullet.SetAngle (6.28f / 64 * x, 6.28f / 64 * y);
          bullet.velocity = Quaternion.Euler (angle1, angle2, angle3) * bullet.velocity;
          bullet.SetColor (Color.HSVToRGB (col, 0.5f, 0.6f));
          bulletList.Add (bullet);
        }
      }
    }

    int count = bulletList.Count;
    for (int i = count - 1; i >= 0; i--) {
      bulletList[i].Move ();
      this.pos = bulletList[i].transform.position;
      if (this.pos.x < -10 || this.pos.x > 10 || this.pos.y < -10 || this.pos.y > 10 || this.pos.z < -10 || this.pos.z > 10) {
        Destroy (bulletList[i].gameObject);
        bulletList.Remove (bulletList[i]);
      }
    }

  }

  private void Func02 () {
    if (frameCount % 4 == 0) {
      // 弾の生成
      float col = 0.5f + 0.1f * Mathf.Sin (frameCount / 10.0f);
      for (int y = 0; y < 3; y++) {
        for (int x = 0; x < 6; x++) {
          bullet = Instantiate (this.bulletPrefab[0], this.transform.position, Quaternion.identity);
          bullet.SetAngle (6.28f / 6 * x + Mathf.Sin (frameCount / 16.0f) * 6.28f / 12, 6.28f / 16 * 5);
          bullet.SetSpeed (0.07f - 0.015f * y);
          bullet.accel.Set (0, -0.0003f, 0);
          bullet.SetColor (Color.HSVToRGB (col, 0.5f, 0.6f));
          bulletList.Add (bullet);
        }
      }
    }

    int count = bulletList.Count;
    for (int i = count - 1; i >= 0; i--) {
      bulletList[i].velocity += bulletList[i].accel;
      bulletList[i].Move ();
      this.pos = bulletList[i].transform.position;
      if (this.pos.x < -10 || this.pos.x > 10 || this.pos.y < -10 || this.pos.y > 10 || this.pos.z < -10 || this.pos.z > 10) {
        Destroy (bulletList[i].gameObject);
        bulletList.Remove (bulletList[i]);
      }
    }

  }

  // 弾幕
  public static int XCount, YCount;
  private void Func03 () {
    this.lua.DoString (this.luaHeaderText + this.luaText);
    // BulletCreate (0, 0, 0, 0.05f, 0, 0, 0, 0.5f, 0.5f);

    // 弾の移動処理
    // ここが一番重い
    BulletWebGL target;
    for (int i = 0; i < maxBullet; i++) {
      target = bulletList[i];
      if (target.active) {
        target.Move (); // 重い:要改善?
        this.pos = target.TransformCache.position;
        if (this.pos.x < -10 || this.pos.x > 10 || this.pos.y < -10 || this.pos.y > 10 || this.pos.z < -10 || this.pos.z > 10) {
          target.Diactivate ();
        }
      }
    }

    return;

    if (frameCount % 3 == 0) {
      // 弾の生成
      for (int x = 0; x < 12; x++) {
        XCount = x;
        for (int y = 0; y < 3; y++) {
          YCount = y;
          bullet = FindBullet ();
          if (bullet == null) continue;

          // ----- ここから弾幕の設定 -----

          // 初期座標
          bullet.TransformCache.position = this.transformComponent.position;

          // 初期速度
          bullet.speed = 0.05f;

          // this.lua = new LuaEnv ();
          this.lua.DoString (this.luaScript.text);
          // this.lua.DoString ("require 'test'");
          // this.lua.Dispose ();

          // 初期角度
          // targetBullet.Angle1 = 6.28f / 6 * (x + y / 7.0f) + this.frameCount * this.frameCount / 10000.0f;
          // targetBullet.Angle2 = 6.28f / 3 * y + Mathf.Sin (this.frameCount / 100f * y) / 12.0f;

          // 色設定
          // targetBullet.SetColor (Color.HSVToRGB (
          //   (this.frameCount / 100.0f + x * 0.08f) % 1.0f,
          //   0.5f,
          //   0.6f));

          // ----- 弾幕設定ここまで -----

          bullet.Activate ();
        }
      }
    }

    // 弾の移動処理
    // ここが一番重い
    for (int i = 0; i < maxBullet; i++) {
      target = bulletList[i];
      if (target.active) {
        target.Move (); // 重い:要改善?
        this.pos = target.TransformCache.position;
        if (this.pos.x < -10 || this.pos.x > 10 || this.pos.y < -10 || this.pos.y > 10 || this.pos.z < -10 || this.pos.z > 10) {
          target.Diactivate ();
        }
      }
    }

  }

  static public void BulletCreate (float px, float py, float pz, float speed, float angle1, float angle2, float h, float s, float v, float scale = 1) {
    bullet = FindBullet ();
    if (bullet == null) {
      return;
    }
    SetSpeed (speed);
    bullet.TransformCache.position = new Vector3 (px, py, pz);
    SetAngle1 (angle1);
    SetAngle2 (angle2);
    SetColor (h, s, v);
    SetState (1);
    SetScale (scale);
    SetTime (0);
    bullet.Activate ();
  }

  static public void BulletCreate (float px, float py, float pz, float speed, float anglev1, float anglev2, float accel, float anglea1, float anglea2, float h, float s, float v, float scale = 1) {
    bullet = FindBullet ();
    if (bullet == null) {
      return;
    }
    bullet.TransformCache.position = new Vector3 (px, py, pz);
    SetSpeed (speed);
    SetAngle1 (anglev1);
    SetAngle2 (anglev2);
    SetAccelSpeed (accel);
    SetAngleAccel1 (anglea1);
    SetAngleAccel2 (anglea2);
    SetColor (h, s, v);
    SetState (2);
    SetScale (scale);
    SetTime (0);
    bullet.Activate ();
  }

  static public void SetSpeed (float speed) {
    bullet.speed = speed;
  }

  static public void SetAccelSpeed (float speed) {
    bullet.accelSpeed = speed;
  }

  static public void SetAngle1 (float angle) {
    bullet.Angle1 = angle;
  }

  static public void SetAngle2 (float angle) {
    bullet.Angle2 = angle;
  }

  static public void SetAngleAccel1 (float angle) {
    bullet.angleAccel1 = angle;
  }

  static public void SetAngleAccel2 (float angle) {
    bullet.angleAccel2 = angle;
  }

  static public void SetColor (float h, float s, float v) {
    bullet.SetColor (Color.HSVToRGB (h, s, v));
  }

  static public void SetState (int value) {
    bullet.state = value;
  }

  static public void SetScale (float scale) {
    bullet.scale = scale;
    bullet.gameObject.transform.localScale = new Vector3 (scale, scale, scale);
  }

  static public void SetTime (int time) {
    bullet.time = time;
  }

  static public void AddCache () {
    int index = EnemyWebGL.findLastIndex;
    if (index == 0) {
      index = EnemyWebGL.maxBullet - 1;
    } else {
      index--;
    }
    EnemyWebGL.cacheBulletIndex.Add (index);
  }

  static public BulletWebGL GetCache (int index) {
    return EnemyWebGL.bulletList[EnemyWebGL.cacheBulletIndex[index]];
  }
}
