using System.IO;
using System.Runtime.InteropServices;
// using UnityEditor;
using UnityEngine;
using XLua; //read write filestream
using System.Text; //Encoding

#if UNITY_STANDALONE_WIN // Windows用
using System.Windows.Forms; //OpenFileDialog用に使う
#endif

// 弾データの構造体
public struct BulletWin {
  public Vector3 pos; // 座標
  public Vector3 velocity; // 速度
  public Vector3 acceleration; // 加速度
  public Color color; // 色
  public int state; // 状態保持パラメータ
  public float angle; // 角度

  // コンストラクタ
  public BulletWin (Vector3 pos, Vector3 velocity, Color color) {
    this.pos = pos;
    this.velocity = velocity;
    this.acceleration = Vector3.zero;
    this.color = color;
    this.state = 1;
    this.angle = Random.Range (0, 6.28f);
  }

  // コンストラクタ 2
  // 加速度の登録が可能
  public BulletWin (Vector3 pos, Vector3 velocity, Vector3 acceleration, Color color) {
    this.pos = pos;
    this.velocity = velocity;
    this.acceleration = acceleration;
    this.color = color;
    this.state = 2; // 加速度使用フラグ
    this.angle = Random.Range (0, 6.28f);
  }
}

// 敵クラス
public class EnemyWin : MonoBehaviour {

  [SerializeField]
  private Shader bulletsShader; // 弾シェーダ
  [SerializeField]
  private Texture[] bulletsTextureList; // 弾のテクスチャ
  [SerializeField]
  private ComputeShader bulletsComputeShader; // コンピュートシェーダ

  private Material bulletsMaterial; // 弾のマテリアル

  public static ComputeBuffer bulletsBuffer; // 弾のコンピュートバッファ

  static private int bulletMax; // 弾の最大数(弾幕スクリプトからの参照用)
  static public int frameCount = 0; // フレーム数

  [SerializeField]
  private int BULLETMAX; // 弾の最大数(UnityEditorからの設定用)
  public static BulletWin[] bulletList; // 弾リスト
  static private int findLastIndex; // 弾検索用インデックス

  // Lua
  private LuaEnv lua;
  [SerializeField]
  // private TextAsset luaScript;
  private string luaText; // 実行するスクリプトのテキストデータ
  [SerializeField, Multiline]
  private string luaHeaderText; // 読み込んだスクリプトを実行する前に呼び出すヘッダ

  // 終了時処理
  void OnDisable () {
    bulletsBuffer.Release ();
  }

  // 起動時処理
  void Start () {
    EnemyWin.bulletMax = this.BULLETMAX;

    this.bulletsMaterial = new Material (bulletsShader);
    this.InitializeComputeBuffer ();

    EnemyWin.findLastIndex = 0;

    // Luaのインスタンス作成
    this.lua = new LuaEnv ();
  }

  // スクリプトの読み込み
  public void LoadScript () {
#if UNITY_STANDALONE_WIN

    OpenFileDialog open_file_dialog = new OpenFileDialog ();

    // 拡張子Luaを指定
    open_file_dialog.Filter = "lua file|*.lua";

    open_file_dialog.CheckFileExists = false;

    open_file_dialog.ShowDialog ();

    string path = open_file_dialog.FileName;

    if (path.Length != 0) {
      FileInfo fi = new FileInfo (path);
      StreamReader sr = new StreamReader (fi.OpenRead (), Encoding.UTF8);
      this.luaText = sr.ReadToEnd ();

      // 読み込んだファイルが正しく実行できるかチェックする
      try {
        this.lua.DoString (this.luaHeaderText + this.luaText);
      } catch (LuaException ex) {
        MessageBox.Show (ex.Message);
        this.luaText = "";
      }
      this.BulletsInitialize ();
    }
#endif
  }

  void Update () {
    // コンピュートシェーダが行った計算結果を取得する
    bulletsBuffer.GetData (bulletList);

    frameCount++; // 経過フレーム数を進める

    // 弾幕の実行
    this.BulletsUpdate ();

    // たま情報をコンピュートシェーダに送る
    bulletsBuffer.SetData (bulletList);
    bulletsComputeShader.SetBuffer (0, "Bullets", bulletsBuffer);
    bulletsComputeShader.Dispatch (0, bulletsBuffer.count / 8 + 1, 1, 1);
  }

  // 弾情報の更新
  public static void BulletUpdate () {
    bulletsBuffer.SetData (bulletList);
  }

  // 弾幕の初期化
  private void BulletsInitialize () {
    frameCount = 0;
    bulletsBuffer.GetData (bulletList);
    for (int i = 0; i < EnemyWin.bulletMax; i++) {
      bulletList[i].state = 0;
      bulletList[i].pos = Vector3.zero;
      bulletList[i].velocity = Vector3.zero;
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
        int index = FindUnusedBullet ();
        if (index >= 0) {
          // 弾生成処理
          angle1 = Mathf.PI / XDIV * x + frameCount / 100.0f * ((x % 2) * 2 - 1);
          angle2 = Mathf.PI * 2 / YDIV * y + Mathf.PI / 6 * Mathf.Sin (frameCount / 100.0f) + Mathf.PI / YDIV / 2;
          bulletList[index] = new BulletWin (
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

  // 弾幕生成処理
  // Luaスクリプトを実行する
  private void BulletsUpdate () {
    this.lua.DoString (this.luaHeaderText + this.luaText);
  }

  static public void SetPosition (int index, Vector3 pos) {
    bulletList[index].pos = pos;
  }

  // 弾生成処理
  // 加速度無し
  static public void BulletCreate (float px, float py, float pz, float speed, float angle1, float angle2, float h, float s, float v) {
    int index = FindUnusedBullet ();
    if (index == -1) {
      return;
    }
    bulletList[index] = new BulletWin (
      new Vector3 (px, py, pz),
      new Vector3 (
        speed * Mathf.Cos (angle1) * Mathf.Cos (angle2),
        speed * Mathf.Sin (angle2),
        speed * Mathf.Sin (angle1) * Mathf.Cos (angle2)
      ),
      Color.HSVToRGB (h, s, v)
    );
  }

  // 弾生成処理
  // 加速度有り
  static public void BulletCreate (float px, float py, float pz, float speed, float anglev1, float anglev2, float accel, float anglea1, float anglea2, float h, float s, float v) {
    int index = FindUnusedBullet ();
    if (index == -1) {
      return;
    }
    bulletList[index] = new BulletWin (
      new Vector3 (px, py, pz),
      new Vector3 (
        speed * Mathf.Cos (anglev1) * Mathf.Cos (anglev2),
        speed * Mathf.Sin (anglev2),
        speed * Mathf.Sin (anglev1) * Mathf.Cos (anglev2)
      ),
      new Vector3 (
        accel * Mathf.Cos (anglea1) * Mathf.Cos (anglea2),
        accel * Mathf.Sin (anglea2),
        accel * Mathf.Sin (anglea1) * Mathf.Cos (anglea2)
      ),
      Color.HSVToRGB (h, s, v)
    );
  }

  // 現在表示されている弾数を取得
  public int GetBulletNum () {
    int count = 0;
    for (int i = 0; i < EnemyWin.bulletMax; i++) {
      if (bulletList[i].state >= 1) {
        count++;
      }
    }
    return count;
  }

  // 未使用の弾を一つ取得する(配列のインデックスを返す)
  // 同一フレーム内で複数回使用した場合，ループ回数は最大でbulletMax回になる
  // 返り値-1は全ての弾が使用する
  private static int FindUnusedBullet () {
    // リストの初めからではなく，最後に調べた箇所からループを始める
    int index = EnemyWin.findLastIndex;
    while (true) {
      // 未使用の弾を検索する
      if (bulletList[index].state == 0) {
        EnemyWin.findLastIndex = index + 1;
        if (EnemyWin.findLastIndex == EnemyWin.bulletMax) {
          EnemyWin.findLastIndex = 0;
        }
        return index;
      }
      if (index == EnemyWin.bulletMax) {
        index = 0;
      }
      if (index == EnemyWin.findLastIndex) {
        break;
      }
    }
    return -1;
  }

  // コンピュートバッファの初期化
  void InitializeComputeBuffer () {
    bulletsBuffer = new ComputeBuffer (EnemyWin.bulletMax, Marshal.SizeOf (typeof (BulletWin)));

    // 配列に初期値を代入する
    bulletList = new BulletWin[bulletsBuffer.count];

    for (int i = 0; i < EnemyWin.bulletMax; i++) {
      bulletList[i] = new BulletWin (Vector3.zero, Vector3.zero, new Color ());
      bulletList[i].state = 0;
    }

    // バッファに適応
    bulletsBuffer.SetData (bulletList);
  }

  // レンダリング
  void OnRenderObject () {

    // テクスチャ、バッファをマテリアルに設定
    for (int i = 0; i < this.bulletsTextureList.Length; i++) {
      bulletsMaterial.SetTexture ("_Tex" + i, bulletsTextureList[i]);
    }
    bulletsMaterial.SetBuffer ("Bullets", bulletsBuffer);

    // レンダリングを開始
    bulletsMaterial.SetPass (0);

    // 弾のオブジェクトを描画
    Graphics.DrawProcedural (MeshTopology.Points, bulletsBuffer.count);
  }

}
