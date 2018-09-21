using System.IO;
using System.Runtime.InteropServices;
// using UnityEditor;
using UnityEngine;
using XLua; //read write filestream
using System.Text; //Encoding

#if UNITY_STANDALONE_WIN // Windows用
  using System.Windows.Forms; //OpenFileDialog用に使う
#endif

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
  /// 角度
  /// </summary>
  public float angle;

  /// <summary>
  /// コンストラクタ
  /// </summary>
  public BulletDX (Vector3 pos, Vector3 accel, Color color) {
    this.pos = pos;
    this.accel = accel;
    this.color = color;
    this.count = 1;
    this.angle = Random.Range (0, 6.28f);
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
  public Texture[] bulletsTextureList;

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

  static private int bulletMax;
  [SerializeField]
  private int BULLETMAX;
  static BulletDX[] bulletList;
  static private int findLastIndex;

  // Lua
  private LuaEnv lua;
  [SerializeField]
  private TextAsset luaScript;
  private string luaText;
  [SerializeField, Multiline]
  private string luaHeaderText;

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
    bulletMax = this.BULLETMAX;

    bulletsMaterial = new Material (bulletsShader);
    InitializeComputeBuffer ();

    findLastIndex = 0;

    this.lua = new LuaEnv ();

  }

  public void LoadScript () {
    #if UNITY_STANDALONE_WIN

    OpenFileDialog open_file_dialog = new OpenFileDialog ();

    //InputFieldの初期値を代入しておく(こうするとダイアログがその場所から開く)

    //csvファイルを開くことを指定する
    open_file_dialog.Filter = "lua file|*.lua";

    //ファイルが実在しない場合は警告を出す(true)、警告を出さない(false)
    open_file_dialog.CheckFileExists = false;

    //ダイアログを開く
    open_file_dialog.ShowDialog ();

    //取得したファイル名をstringに代入する
    string path = open_file_dialog.FileName;

    // string path = EditorUtility.OpenFilePanel ("Overwrite with png", "", "txt");
    if (path.Length != 0) {
      // this.luaScript = new TextAsset (path);
      // this.luaScript = Resources.Load (path, typeof (TextAsset)) as TextAsset;
      FileInfo fiA = new FileInfo (path);
      StreamReader srA = new StreamReader (fiA.OpenRead (), Encoding.UTF8);
      this.luaText = srA.ReadToEnd ();

      // 読み込んだファイルが正しく実行できるかチェックする
      try {
        this.lua.DoString (this.luaHeaderText + this.luaText);
      } catch (LuaException ex) {
        // MessageBox.ShowAlertBox (ex.Message, "Lua Script Error");
        MessageBox.Show (ex.Message);
        this.luaText = "";
      }
      this.DanmakuInitialize ();
    }
#endif
  }

  /// <summary>
  /// 更新処理
  /// </summary>
  static public int frameCount = 0;
  void Update () {

    bulletsBuffer.GetData (bulletList);
    frameCount++;

    // 弾幕の実行
    this.Func01 ();

    bulletsBuffer.SetData (bulletList);
    bulletsComputeShader.SetBuffer (0, "Bullets", bulletsBuffer);
    bulletsComputeShader.Dispatch (0, bulletsBuffer.count / 8 + 1, 1, 1);
  }

  private void DanmakuInitialize () {
    frameCount = 0;
    bulletsBuffer.GetData (bulletList);
    for (int i = 0; i < bulletMax; i++) {
      bulletList[i].count = 0;
      bulletList[i].pos = Vector3.zero;
      bulletList[i].accel = Vector3.zero;
    }
    bulletsBuffer.SetData (bulletList);

  }

  private void Func00 () {
    float angle1, angle2;
    int XDIV = 12;
    int YDIV = 12;
    float speed = 0.15f;
    for (int y = 0; y < YDIV; y++) {
      for (int x = 0; x < XDIV; x++) {
        int index = FindBullet ();
        if (index >= 0) {
          // 弾生成処理
          angle1 = Mathf.PI / XDIV * x + frameCount / 100.0f * ((x % 2) * 2 - 1);
          angle2 = Mathf.PI * 2 / YDIV * y + Mathf.PI / 6 * Mathf.Sin (frameCount / 100.0f) + Mathf.PI / YDIV / 2;
          bulletList[index] = new BulletDX (
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
  }

  private void Func01 () {
    this.lua.DoString (this.luaHeaderText + this.luaText);
  }

  static public void BulletCreate (float px, float py, float pz, float speed, float angle1, float angle2, float h, float s, float v) {
    int index = FindBullet ();
    if (index == -1) {
      return;
    }
    bulletList[index] = new BulletDX (
      new Vector3 (px, py, pz),
      new Vector3 (
        speed * Mathf.Cos (angle1) * Mathf.Cos (angle2),
        speed * Mathf.Sin (angle2),
        speed * Mathf.Sin (angle1) * Mathf.Cos (angle2)
      ),
      Color.HSVToRGB (h, s, v)
    );
    // bulletList[index].angle = angle1;

  }

  // 現在表示されている弾数を取得
  public int GetBulletNum () {
    int count = 0;
    for (int i = 0; i < bulletMax; i++) {
      if (bulletList[i].count == 1) {
        count++;
      }
    }
    return count;
  }

  private static int FindBullet () {
    int firstIndex = findLastIndex;
    int index = firstIndex;
    while (true) {
      if (bulletList[index].count == 0) {
        findLastIndex = index + 1;
        if (findLastIndex == bulletMax) {
          findLastIndex = 0;
        }
        return index;
      }
      if (index == bulletMax) {
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
    bulletsBuffer = new ComputeBuffer (bulletMax, Marshal.SizeOf (typeof (BulletDX)));

    // 配列に初期値を代入する
    bulletList = new BulletDX[bulletsBuffer.count];

    for (int i = 0; i < bulletMax; i++) {
      bulletList[i] = new BulletDX (Vector3.zero, Vector3.zero, new Color ());
      bulletList[i].count = 0;
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
    bulletsBuffer.SetData (bulletList);
  }

  /// <summary>
  /// レンダリング
  /// </summary>
  void OnRenderObject () {

    // テクスチャ、バッファをマテリアルに設定
    for (int i = 0; i < this.bulletsTextureList.Length; i++) {
      bulletsMaterial.SetTexture ("_Tex" + i, bulletsTextureList[i]);
    }
    bulletsMaterial.SetBuffer ("Bullets", bulletsBuffer);

    // レンダリングを開始
    bulletsMaterial.SetPass (0);

    // 1万個のオブジェクトをレンダリング
    Graphics.DrawProcedural (MeshTopology.Points, bulletsBuffer.count);
  }

}
