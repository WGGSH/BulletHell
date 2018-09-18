using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// 弾の構造体
/// </summary>
struct BulletDX {

  /// <summary>
  /// 座標
  /// </summary>
  public Vector3 pos;

  /// <summary>
  /// 速度
  /// </summary>
  public Vector3 accel;

  /// <summary>
  /// 色
  /// </summary>
  public Color color;

  /// <summary>
  /// 使用中フラグ
  /// </summary>
  public int count;

  /// <summary>
  /// コンストラクタ
  /// </summary>
  public BulletDX (Vector3 pos, Vector3 accel, Color color) {
    this.pos = pos;
    this.accel = accel;
    this.color = color;
    this.count = 1;
  }
}

/// <summary>
/// 沢山の弾を管理するクラス
/// </summary>
public class EnemyDX : MonoBehaviour {

  /// <summary>
  /// 弾をレンダリングするシェーダー
  /// </summary>
  public Shader bulletsShader;

  /// <summary>
  /// 弾のテクスチャ
  /// </summary>
  public Texture bulletsTexture;

  /// <summary>
  /// 弾の更新を行うコンピュートシェーダー
  /// </summary>
  public ComputeShader bulletsComputeShader;

  /// <summary>
  /// 弾のマテリアル
  /// </summary>
  Material bulletsMaterial;

  /// <summary>
  /// 弾のコンピュートバッファ
  /// </summary>
  ComputeBuffer bulletsBuffer;

  [SerializeField]
  private int bulletMax;
  private BulletDX[] bulletList;
  private int findLastIndex;

  /// <summary>
  /// 破棄
  /// </summary>
  void OnDisable () {
    // コンピュートバッファは明示的に破棄しないと怒られます
    bulletsBuffer.Release ();
  }

  /// <summary>
  /// 初期化
  /// </summary>
  void Start () {
    bulletsMaterial = new Material (bulletsShader);
    InitializeComputeBuffer ();

    this.findLastIndex = 0;
  }

  /// <summary>
  /// 更新処理
  /// </summary>
  int frameCount = 0;
  void Update () {

    bulletsBuffer.GetData (this.bulletList);

    this.frameCount++;
    float angle1, angle2;
    int XDIV = 12;
    int YDIV = 12;
    float speed = 0.15f;
    for (int y = 0; y < YDIV; y++) {
      for (int x = 0; x < XDIV; x++) {
        int index = this.FindBullet ();
        if (index >= 0) {
          // 弾生成処理
          angle1 = Mathf.PI / XDIV * x + this.frameCount / 100.0f * ((x % 2) * 2 - 1);
          angle2 = Mathf.PI * 2 / YDIV * y + Mathf.PI / 6 * Mathf.Sin (this.frameCount / 100.0f) + Mathf.PI / YDIV / 2;
          this.bulletList[index] = new BulletDX (
            new Vector3 (0, 0, 0),
            new Vector3 (
              speed * Mathf.Cos (angle1) * Mathf.Cos (angle2),
              speed * Mathf.Sin (angle2),
              speed * Mathf.Sin (angle1) * Mathf.Cos (angle2)
            ),
            Color.HSVToRGB (1.0f / XDIV * x, 0.6f, 0.7f)
          );
        }

      }
    }

    // this.bulletList[this.c++] = new BulletDX (
    //   new Vector3 (0, 0, 0),
    //   new Vector3 (0.05f, 0, 0),
    //   Color.HSVToRGB (Random.Range (0.0f, 1.0f), 1, 1)
    // );
    bulletsBuffer.SetData (this.bulletList);

    bulletsComputeShader.SetBuffer (0, "Bullets", bulletsBuffer);
    // bulletsComputeShader.SetFloat ("DeltaTime", Time.deltaTime);
    // bulletsComputeShader.SetFloat ("TotalTime", Time.time);
    bulletsComputeShader.Dispatch (0, bulletsBuffer.count / 8 + 1, 1, 1);
  }

  public int GetBulletNum () {
    int count = 0;
    for (int i = 0; i < this.bulletMax; i++) {
      if (this.bulletList[i].count == 1) {
        count++;
      }
    }
    return count;
  }

  private int FindBullet () {
    int firstIndex = this.findLastIndex;
    int index = firstIndex;
    while (true) {
      if (this.bulletList[index].count == 0) {
        this.findLastIndex = index + 1;
        if (this.findLastIndex == this.bulletMax) {
          this.findLastIndex = 0;
        }
        return index;
      }
      if (index == this.bulletMax) {
        index = 0;
      }
      if (index == firstIndex) {
        break;
      }
    }
    return -1;
  }

  /// <summary>
  /// コンピュートバッファの初期化
  /// </summary>
  void InitializeComputeBuffer () {
    // 弾数は1万個
    int XDIV = 100;
    int YDIV = 100;
    float PI = Mathf.PI;
    int NUM = XDIV * YDIV;
    bulletsBuffer = new ComputeBuffer (this.bulletMax, Marshal.SizeOf (typeof (BulletDX)));

    // 配列に初期値を代入する
    this.bulletList = new BulletDX[bulletsBuffer.count];

    for (int i = 0; i < this.bulletMax; i++) {
      this.bulletList[i] = new BulletDX (Vector3.zero, Vector3.zero, new Color ());
      this.bulletList[i].count = 0;
    }

    // int index = 0;
    // float angle1, angle2;
    // float speed = 5.0f;
    // for (int y = 0; y < YDIV; y++) {
    //   for (int x = 0; x < XDIV; x++) {
    //     angle1 = PI * 2 / XDIV * x + PI * 5 / YDIV * y;
    //     angle2 = PI / YDIV * y - PI / 2;
    //     this.bulletList[index] = new BulletDX (new Vector3 (0, 0, 0),
    //       new Vector3 (
    //         speed * Mathf.Cos (angle1) * Mathf.Cos (angle2),
    //         speed * Mathf.Sin (angle2),
    //         speed * Mathf.Sin (angle1) * Mathf.Cos (angle2)
    //       ),
    //       // Color.HSVToRGB (1.0f / XDIV * x, 1.0f, 0.3f)
    //       Color.HSVToRGB (1.0f / XDIV * x, 0.5f, 0.5f)
    //     );
    //     this.bulletList[index].count = 0;

    //     index++;
    //   }
    // }

    // バッファに適応
    bulletsBuffer.SetData (this.bulletList);
  }

  /// <summary>
  /// レンダリング
  /// </summary>
  void OnRenderObject () {

    // テクスチャ、バッファをマテリアルに設定
    bulletsMaterial.SetTexture ("_MainTex", bulletsTexture);
    bulletsMaterial.SetBuffer ("Bullets", bulletsBuffer);

    // レンダリングを開始
    bulletsMaterial.SetPass (0);

    // 1万個のオブジェクトをレンダリング
    Graphics.DrawProcedural (MeshTopology.Points, bulletsBuffer.count);
  }

}
