using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

using Live2DSharp;
using Live2DSharp.Framework;

using Ragnarok;
using Ragnarok.Utility;

namespace Live2DSharp.Framework
{
    /// <summary>
    /// モーションの優先順位
    /// </summary>
    public enum Priority
    {
        None,
        Idle,
        Normal,
        Force,
    }

    /// <summary>
    /// モデル用のクラスです。
    /// </summary>
    /// <remarks>
    /// Live2Dではモデルを、原点中心・右方向と上方向をプラスとする
    /// 幅2.0の座標系に対して描画します。
    /// モデルにはLive2Dデータの座標系を上記の座標系に
    /// 変換する行列を設定する必要があります。
    /// 
    /// また、OpenGLに対して設定された描画用行列や透視変換行列は
    /// モデルの描画時にすべて初期化され新しい行列が設定されます。
    /// そのため、モデルの描画位置やサイズの変更はOpenGLではなく
    /// モデルの行列で設定する必要があります。
    /// </remarks>
    public class L2DModel : IDisposable
    {
        public const string MOTION_GROUP_IDLE	 = "idle";// アイドリング
        public const string MOTION_GROUP_TAP_BODY = "tap_body";// 体をタップしたとき

		private readonly List<uint> textures = new List<uint>();
        private readonly PriorityMotionQueueManager motionManager =
            new PriorityMotionQueueManager();
        private readonly PriorityMotionQueueManager expressionManager =
            new PriorityMotionQueueManager();
        private readonly L2DEyeBlink eyeBlink = new L2DEyeBlink();
        private readonly L2DTargetPoint targetManager = new L2DTargetPoint();
        private readonly Dictionary<string, AMotion> motions =
            new Dictionary<string, AMotion>();
        private readonly Dictionary<string, AMotion> expressions =
            new Dictionary<string, AMotion>();

        private Live2DModelOpenGL live2DModel;
        private L2DModelMatrix modelMatrix;
        private L2DPhysics physics;
        private L2DPose pose;

        private readonly OpenGL gl;
        private L2DModelData root;
        private string modelHomeDir;
        private bool disposed;

        //private float alpha;
        /*private bool useLipSync;			// リップシンクが有効かどうか
        private float lastLipSyncValue;*/	// 基本は0～1
        private bool debugMode = true;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DModel(OpenGL gl, string filepath)
        {
            var root = L2DModelData.Load(filepath);

            this.gl = gl;
            this.root = root;
            this.modelHomeDir = Path.GetDirectoryName(filepath);

            Initialize(root);
        }

        /// <summary>
        /// ファイナライザ
        /// </summary>
        ~L2DModel()
        {
            Dispose(false);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.live2DModel != null)
                    {
                        this.textures.ForEach(_ =>
                            this.live2DModel.releaseModelTextureNo((int)_));
                        this.textures.Clear();
                    }

                    this.live2DModel = null;
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// モデルファイルからのパスを取得します。
        /// </summary>
        public string MakeHomePath(string leaf)
        {
            return Path.Combine(this.modelHomeDir, leaf);
        }

        /// <summary>
        /// モデルファイルを初期化します。
        /// </summary>
        private void Initialize(L2DModelData root)
        {
            // モデルファイルの読み込み
            if (!string.IsNullOrEmpty(root.ModelFile))
            {
                LoadModel(MakeHomePath(root.ModelFile));

                for (var i = 0; i < root.Textures.Count(); ++i)
                {
                    LoadTexture(i, MakeHomePath(root.Textures[i]));
                }
            }

            foreach (var exp in root.Expressions)
            {
                LoadExpression(exp.Name, MakeHomePath(exp.File));
            }

            if (!string.IsNullOrEmpty(root.PhysicsFile))
            {
                LoadPhysics(MakeHomePath(root.PhysicsFile));
            }

            if (!string.IsNullOrEmpty(root.PoseFile))
            {
                LoadPose(MakeHomePath(root.PoseFile));
            }

            if (root.Layout != null)
            {
                this.modelMatrix.SetupLayout(root.Layout);
            }

            this.live2DModel.PremultipliedAlpha = false;
            this.live2DModel.saveParam();

            foreach (var pair in root.Motions)
            {
                PreloadMotionGroup(pair.Key);
            }

            this.motionManager.stopAllMotions();
        }

        /// <summary>
        /// モデルファイルの読み込みを行います。
        /// </summary>
        private void LoadModel(string path)
        {
            if (debugMode)
            {
                Log.Info("Load model: '{0}'", path);
            }

            // mocファイルの読み込みを行います。
            var l2dPath = new LDString(path, MemoryParam.__CreateInstance(IntPtr.Zero));
            this.live2DModel = Live2DModelOpenGL.loadModel(l2dPath);
            this.live2DModel.saveParam();

            if (Live2D.Error != Live2D.L2D_NO_ERROR)
            {
                Log.Error("Error : Failed to loadModelData().");
                return;
            }

            // とりあえずモデル用の行列を初期化します。
            this.modelMatrix = new L2DModelMatrix(
                live2DModel.CanvasWidth, live2DModel.CanvasHeight);
            this.modelMatrix.SetWidth(2.0);
            this.modelMatrix.SetCenterPosition(0, 0);
        }

        /// <summary>
        /// テクスチャの読み込みを行います。
        /// </summary>
        private void LoadTexture(int no, string path)
        {
            uint glTexName = L2DTextureLoader.LoadTexture(gl, path);

            this.live2DModel.setTexture(no, glTexName);
            this.textures.Add(glTexName);
        }

        /// <summary>
        /// 表情用のエクスプレッションファイルを読み込みます。
        /// </summary>
        private void LoadExpression(string name, string path)
        {
            var emotion = L2DExpressionMotionLoader.Load(path);

            this.expressions[name] = emotion;
        }

        /// <summary>
        /// 髪を揺らすためのファイルを読み込みます。
        /// </summary>
        private void LoadPhysics(string path)
        {
            this.physics = L2DPhysics.Load(path);
        }

        /// <summary>
        /// ポーズデータを読み込みます。
        /// </summary>
        private void LoadPose(string path)
        {
            this.pose = L2DPose.Load(path);
        }

        /// <summary>
        /// モーションファイルの読み込みを行います。
        /// </summary>
        private AMotion LoadMotion(string name, string path)
		{
            var l2dPath = new LDString(path, MemoryParam.__CreateInstance(IntPtr.Zero));
			var motion = Live2DMotion.loadMotion(l2dPath);

			if (!string.IsNullOrEmpty(name))
			{
				this.motions[name] = motion;
			}

			return motion;
		}

        /// <summary>
        /// モーショングループの事前読み込みを行います。
        /// </summary>
        private void PreloadMotionGroup(string group)
        {
            List<L2DMotionData> motions;
            if (!this.root.Motions.TryGetValue(group, out motions))
            {
                return;
            }

            for (var i = 0; i < motions.Count(); ++i)
            {
                var motion = motions[i];
                var name = string.Format("{0}_{1}", group, i);
                LoadMotion(name, MakeHomePath(motion.File));

                if (!string.IsNullOrEmpty(motion.Sound))
                {
                    //LoadSound(MakeHomePath(motion.Sound));
                }
            }
        }

        /// <summary>
        /// モーショングループを開放します。
        /// </summary>
        private void ReleaseMotionGroup(string group)
        {
            List<L2DMotionData> motions;
            if (!this.root.Motions.TryGetValue(group, out motions))
            {
                return;
            }
            
            foreach(var motion in motions)
            {
                if (!string.IsNullOrEmpty(motion.Sound))
                {
                    //UnloadSound(MakeHomePath(motion.Sound));
                }
            }
        }

        /// <summary>
        /// モデルの管理用オブジェクトを取得します。
        /// </summary>
        public L2DModelManager ModelManager
        {
            get;
            internal set;
        }

        /// <summary>
        /// 顔の向く方向を設定します。
        /// </summary>
        public void SetTarget(double x, double y)
        {
            this.targetManager.FaceTargetX = x;
            this.targetManager.FaceTargetY = y;
        }

        /// <summary>
        /// モデルの行列を更新します。
        /// </summary>
        public void UpdateMatrix(Matrix44d projection)
        {
            if (projection == null)
            {
                throw new ArgumentNullException("projection");
            }

            UpdateMatrix_(projection);
        }

        /// <summary>
        /// unsafe用
        /// </summary>
        private unsafe void UpdateMatrix_(Matrix44d projection)
        {
            var m = projection.Clone();
            m.Multiply(this.modelMatrix.Transform);

            var floatArray = m.AsColumnMajorArray
                .Select(_ => Convert.ToSingle(_))
                .ToArray();

            // 行列の値を設定します。
            fixed (float* fixedValues = floatArray)
            {
                this.live2DModel.Matrix = fixedValues;
            }
        }

        /// <summary>
        /// フレームごとの更新処理を行います。
        /// </summary>
        public virtual void Update(TimeSpan elapsed)
        {
            // 前回セーブされた状態をロード
            this.live2DModel.loadParam();
            if (this.motionManager.isFinished())
            {
                // モーション再生がない場合は、待機モーションの中からランダムに再生する。
                StartRandomMotion(MOTION_GROUP_IDLE, Priority.Idle);
            }
            else
            {
                this.motionManager.updateParam(this.live2DModel);
            }
            this.live2DModel.saveParam(); // 状態を保存

            // 表情でパラメータ更新（相対変化）
            this.expressionManager.updateParam(this.live2DModel);

            this.targetManager.Update(elapsed);
            var dragX = (float)this.targetManager.FaceCurrentX;
            var dragY = (float)this.targetManager.FaceCurrentY;

            // ドラッグによる変化
            // ドラッグによる顔の向きの調整
            this.live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_X, dragX * 30, 1.0f);
            this.live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_Y, dragY * 30, 1.0f);
            this.live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_Z, dragX * dragY, 1.0f);

            // ドラッグによる体の向きの調整
            this.live2DModel.addToParamFloat(L2DStandardID.PARAM_BODY_ANGLE_X, dragX * 10, 1.0f);

            // ドラッグによる目の向きの調整
            this.live2DModel.addToParamFloat(L2DStandardID.PARAM_EYE_BALL_X, dragX, 1.0f);
            this.live2DModel.addToParamFloat(L2DStandardID.PARAM_EYE_BALL_Y, dragY, 1.0f);
            
            // 呼吸など
            var t = (Environment.TickCount / 1000.0) * 2 * Math.PI;
            this.live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_X, (float)(15 * Math.Sin(t / 6.5345)), 0.5f);
            this.live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_Y, (float)(8 * Math.Sin(t / 3.5345)), 0.5f);
            this.live2DModel.addToParamFloat(L2DStandardID.PARAM_ANGLE_Z, (float)(10 * Math.Sin(t / 5.5345)), 0.5f);
            this.live2DModel.addToParamFloat(L2DStandardID.PARAM_BODY_ANGLE_X, (float)(4 * Math.Sin(t / 15.5345)), 0.5f);
            this.live2DModel.addToParamFloat(L2DStandardID.PARAM_BREATH, (float)(0.5 + 0.5 * Math.Sin(t / 3.2345)), 1.0f);
            
            // 物理演算でパラメータを更新
            if (this.physics != null)
            {
                this.physics.UpdateParam(this.live2DModel, elapsed);
            }

            if (this.eyeBlink != null)
            {
                this.eyeBlink.UpdateParam(this.live2DModel, elapsed);
            }

            // リップシンクの設定
            if (true) //this.useLipSync)
            {
                // リアルタイムでリップシンクを行う場合、
                // システムから音量を取得して0～1の範囲で入力してください。
                var value = 0.0f;
                this.live2DModel.addToParamFloat(L2DStandardID.PARAM_MOUTH_OPEN_Y, value, 0.8f);
            }

            // ポーズの設定
            if (this.pose != null)
            {
                this.pose.UpdateParam(this.live2DModel, elapsed);
            }

            this.live2DModel.update();
        }

        /// <summary>
        /// モーションを開始します。
        /// </summary>
        public int StartMotion(string group, int no, Priority priority)
        {
            if (priority == Priority.Force)
            {
                this.motionManager.ReservePriority = (int)priority;
            }

            List<L2DMotionData> motionDatas;
            if (!this.root.Motions.TryGetValue(group, out motionDatas) ||
                no >= motionDatas.Count())
            {
                return -1;
            }

            var motionData = motionDatas[no];
            var name = string.Format("{0}_{1}", group, no);
            var autoDelete = false;
            AMotion motion;
            if (!this.motions.TryGetValue(name, out motion))
            {
                motion = LoadMotion(null, MakeHomePath(motionData.File));
                autoDelete = true;
            }

            // フェードイン／アウトの設定
            motion.FadeIn = motionData.FadeIn;
            motion.FadeOut = motionData.FadeOut;

            // もしサウンドがあればそれを再生します。
            if (!string.IsNullOrEmpty(motionData.Sound))
            {
                OnPlayEffect(MakeHomePath(motionData.Sound));
            }

            Log.Info("motion[{0}] started", motionData.File);

            return this.motionManager.startMotionPriority(
                motion, autoDelete, (int)priority);
        }

        /// <summary>
        /// SEの再生を行います。
        /// </summary>
        protected virtual void OnPlayEffect(string path)
        {
            if (ModelManager == null)
            {
                return;
            }

            var soundManager = ModelManager.SoundManager;
            if (soundManager == null)
            {
                return;
            }

            path = Path.GetFullPath(path);
            soundManager.PlayEffectSound(path, 1.0);
        }

        /// <summary>
        /// モーションをランダムに開始します。
        /// </summary>
        public int StartRandomMotion(string group, Priority priority)
        {
            List<L2DMotionData> motionDatas;
            if (!this.root.Motions.TryGetValue(group, out motionDatas) ||
                !motionDatas.Any())
            {
                return -1;
            }

            var no = MathEx.RandInt(0, motionDatas.Count());
            return StartMotion(group, no, priority);
        }

        /// <summary>
        /// モデルの描画を行います。
        /// </summary>
        public void Render()
        {
            if (this.live2DModel == null)
            {
                return;
            }

            this.live2DModel.draw();
        }

        /// <summary>
        /// 論理座標で指定された点がヒットしているか調べます。
        /// </summary>
        public bool HitTest(string pid, double testX, double testY)
        {
            return this.root.HitAreas
                .Where(_ => _.Name == pid)
                .Any(_ => HitTestInternal(_.Id, testX, testY));
        }

        /// <summary>
        /// 論理座標で指定された点がヒットしているか調べます。
        /// </summary>
        private unsafe bool HitTestInternal(string drawID, double testX, double testY)
        {
            int drawIndex = this.live2DModel.getDrawDataIndex(drawID);
            if (drawIndex < 0)
            {
                return false;// 存在しない場合はfalse
            }

            var points = new List<float>();
            var count = 0;

            float* fpoints = live2DModel.getTransformedPoints(drawIndex, &count);
            for (int i = 0; i < count * 2; ++i)
            {
                points.Add(fpoints[i]);
            }

            float left = this.live2DModel.CanvasWidth;
            float right = 0;
            float top = this.live2DModel.CanvasHeight;
            float bottom = 0;

            for (int j = 0; j < count; ++j)
            {
                float x = points[DEF.VERTEX_OFFSET + j * DEF.VERTEX_STEP];
                float y = points[DEF.VERTEX_OFFSET + j * DEF.VERTEX_STEP + 1];
                left = Math.Min(left, x);     // Min x
                right = Math.Max(right, x);   // Max x
                top = Math.Min(top, y);	      // Min y
                bottom = Math.Max(bottom, y); // Max y
            }

            var inv = this.modelMatrix.Transform.Invert();
            var tx = inv.TransformX(testX);
            var ty = inv.TransformY(testY);
            if (debugMode)
            {
                Log.Debug("hittest: ({0:F2}, {1:F2}) - ({2:F2}, {3:F2})",
                    left, top, right, bottom);
                Log.Debug("hittest: ({0:F2}, {1:F2})", tx, ty);
            }

            return (left <= tx && tx <= right && top <= ty && ty <= bottom);
        }

        /// <summary>
        /// 式形式のモーションを開始します。
        /// </summary>
        public void SetExpression(string expressionId)
        {
            var motion = expressions[expressionId];
            if (motion != null)
            {
                this.expressionManager.startMotion(motion, false);
                Log.Info("expression[{0}] started", expressionId);
            }
            else
            {
                Log.Error("expression[{0}] is null.", expressionId);
            }
        }

        /// <summary>
        /// ランダムに式形式のモーションを開始します。
        /// </summary>
        public void SetRandomExpression()
        {
            var no = MathEx.RandInt(0, this.expressions.Count());
            var expression = this.expressions
                .WhereWithIndex((_, __) => __ == no)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(expression.Key))
            {
                SetExpression(expression.Key);
            }
        }
    }
}
