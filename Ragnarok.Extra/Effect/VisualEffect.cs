﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlintSharp;
using FlintSharp.Renderers;
using Ragnarok.MathEx;

namespace Ragnarok.Extra.Effect
{
    /// <summary>
    /// アニメーションのタイプです。
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// 通常のアニメーション
        /// </summary>
        Normal,
        /// <summary>
        /// ランダム
        /// </summary>
        Random,
    }

    /// <summary>
    /// ヴィジュアル表示などを可能にするエフェクトオブジェクトです。
    /// </summary>
    public class VisualEffect : EffectObject
    {
        /// <summary>
        /// デフォルトのサウンドマネージャを取得または設定します。
        /// </summary>
        public static IEffectSoundManager DefaultSoundManager
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VisualEffect()
        {
            Blend = BlendType.Diffuse;
            AnimationImageCount = 1;
            SoundManager = DefaultSoundManager;
            StartSoundVolume = 1.0;
        }

        /// <summary>
        /// BasePathが変更された時に呼ばれます。
        /// </summary>
        protected override void OnBasePathChanged(string basePath)
        {
            base.OnBasePathChanged(basePath);

            ParticleEmitters.ForEach(_ => _.BasePath = basePath);
        }

        #region パーティクル
        /// <summary>
        /// パーティクルのレンダラーを取得または設定します。
        /// </summary>
        public Renderer ParticleRenderer
        {
            get { return GetValue<Renderer>(nameof(ParticleRenderer)); }
            set { SetValue(nameof(ParticleRenderer), value); }
        }

        /// <summary>
        /// パーティクルの放出元リストを取得します。
        /// </summary>
        public EmitterCollection ParticleEmitters
        {
            get
            {
                if (ParticleRenderer == null)
                {
                    return null;
                }

                return ParticleRenderer.Emitters;
            }
        }
        #endregion

        #region イメージ関係
        /// <summary>
        /// このオブジェクトのメッシュを取得または設定します。
        /// </summary>
        public Mesh Mesh
        {
            get { return GetValue<Mesh>(nameof(Mesh)); }
            set { SetValue(nameof(Mesh), value); }
        }

        /// <summary>
        /// 描画種別を取得または設定します。
        /// </summary>
        public BlendType Blend
        {
            get { return GetValue<BlendType>(nameof(Blend)); }
            set { SetValue(nameof(Blend), value); }
        }
        #endregion
        
        #region アニメーションプロパティ
        /// <summary>
        /// 最初に画像をランダムで選ぶ場合、このリストから選択します。
        /// </summary>
        /// <remarks>
        /// XAML上での処理を考え、string型にしています。
        /// </remarks>
        public List<string> InitialImageUriList
        {
            get { return GetValue<List<string>>(nameof(InitialImageUriList)); }
            private set { SetValue(nameof(InitialImageUriList), value); }
        }

        /// <summary>
        /// 描画するイメージのURIを取得または設定します。
        /// </summary>
        /// <remarks>
        /// XAML上での処理を考え、string型にしています。
        /// </remarks>
        public string ImageUri
        {
            get { return GetValue<string>(nameof(ImageUri)); }
            set { SetValue(nameof(ImageUri), value); }
        }

        /// <summary>
        /// アニメーションのタイプを取得または設定します。
        /// </summary>
        public AnimationType AnimationType
        {
            get { return GetValue<AnimationType>(nameof(AnimationType)); }
            set { SetValue(nameof(AnimationType), value); }
        }

        /// <summary>
        /// アニメーションさせる画像の総数を取得または設定します。
        /// </summary>
        public int AnimationImageCount
        {
            get { return GetValue<int>(nameof(AnimationImageCount)); }
            set { SetValue(nameof(AnimationImageCount), value); }
        }

        /// <summary>
        /// アニメーション画像のインデックスを取得または設定します。
        /// </summary>
        public int AnimationImageIndex
        {
            get { return GetValue<int>(nameof(AnimationImageIndex)); }
            set { SetValue(nameof(AnimationImageIndex), value); }
        }
        #endregion

        #region サウンド
        /// <summary>
        /// サウンドマネージャを取得または設定します。
        /// </summary>
        public IEffectSoundManager SoundManager
        {
            get { return GetValue<IEffectSoundManager>(nameof(SoundManager)); }
            set { SetValue(nameof(SoundManager), value); }
        }

        /// <summary>
        /// 開始時に鳴らすサウンドのパスを取得または設定します。
        /// </summary>
        public string StartSoundPath
        {
            get { return GetValue<string>(nameof(StartSoundPath)); }
            set { SetValue(nameof(StartSoundPath), value); }
        }

        /// <summary>
        /// 開始時に鳴らすサウンドの音量を0.0～1.0の範囲で取得または設定します。
        /// </summary>
        public double StartSoundVolume
        {
            get { return GetValue<double>(nameof(StartSoundVolume)); }
            set { SetValue(nameof(StartSoundVolume), value); }
        }
        #endregion

        /// <summary>
        /// 音量に指定の係数をかけ、音の調整を行います。
        /// </summary>
        /// <remarks>
        /// この計算はすべての子オブジェクトで行われます。
        /// </remarks>
        public void MultiplyStartVolume(double rate)
        {
            StartSoundVolume *= rate;

            foreach (var child in Children)
            {
                var visual = child as VisualEffect;
                if (visual == null)
                {
                    continue;
                }

                visual.MultiplyStartVolume(rate);
            }
        }

        /// <summary>
        /// 開始時の効果音を再生します。
        /// </summary>
        public bool PlayStartSound(bool isPlay = true)
        {
            var soundManager = SoundManager;
            if (soundManager == null)
            {
                return false;
            }

            // 開始時のサウンドを鳴らします。
            var soundPath = StartSoundPath;
            if (string.IsNullOrEmpty(soundPath))
            {
                return false;
            }

            var path = MakeContentPath(soundPath);
            var sound = soundManager.PlayEffectSound(
                path,
                (isPlay ? StartSoundVolume : 0.0));
            if (sound == null)
            {
                return false;
            }

            return true;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            // ランダムイメージの設定を行います。
            if (InitialImageUriList != null && InitialImageUriList.Any())
            {
                var index = MathUtil.RandInt(0, InitialImageUriList.Count);

                ImageUri = InitialImageUriList[index];
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (ParticleRenderer != null)
            {
                foreach (var emitter in ParticleRenderer.Emitters)
                {
                    emitter.Start();
                }
            }

            PlayStartSound();
        }

        /// <summary>
        /// 追加の子要素があるかどうかを取得します。
        /// </summary>
        protected override bool HasChildren()
        {
            if (base.HasChildren())
            {
                return true;
            }

            if (ParticleRenderer == null)
            {
                return false;
            }

            return !ParticleRenderer.Emitters.All(_ => !_.Particles.Any());
        }

        /// <summary>
        /// アニメーションインデックスの更新を行います。
        /// </summary>
        protected override void OnEnterFrame(EnterFrameEventArgs e)
        {
            base.OnEnterFrame(e);

            // 必要ならアニメーションインデックスを変更
            switch (AnimationType)
            {
                case AnimationType.Normal:
                    AnimationImageIndex = Math.Min(
                        (int)(AnimationImageCount * e.ProgressRate),
                        AnimationImageCount - 1);
                    break;
                case AnimationType.Random:
                    AnimationImageIndex = MathUtil.RandInt(0, AnimationImageCount);
                    break;
            }

            // パーティクルの更新を行います。
            if (ParticleRenderer != null)
            {
                ParticleRenderer.OnUpdateFrame(e.ElapsedTime.TotalSeconds);
            }
        }

        /// <summary>
        /// フレーム毎の描画処理を行います。
        /// </summary>
        protected override void OnRender(EventArgs e)
        {
            base.OnRender(e);

            if (ParticleRenderer != null)
            {
                ParticleRenderer.OnRenderFrame();
            }
        }
    }
}
