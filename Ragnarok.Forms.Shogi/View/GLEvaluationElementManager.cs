using System;
using System.Collections.Generic;
using System.Linq;

using Ragnarok.ObjectModel;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// GLEvaluationElementクラスで表示するオブジェクトを管理するクラスです。
    /// </summary>
    public sealed class GLEvaluationElementManager : NotifyObject
    {
        private readonly Dictionary<string, Type> internalTypeDic =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        private List<GLEvaluationElementInternal> internalObjList =
            new List<GLEvaluationElementInternal>();
        private List<ImageSetInfo> imageSetList =
            new List<ImageSetInfo>();

        /// <summary>
        /// 選択されている内部オブジェクトを取得します。
        /// </summary>
        public GLEvaluationElementInternal InternalObj
        {
            get { return GetValue<GLEvaluationElementInternal>("InternalObj"); }
            private set { SetValue("InternalObj", value); }
        }

        /// <summary>
        /// ImageSetListを取得または設定します。
        /// </summary>
        public List<ImageSetInfo> ImageSetList
        {
            get
            {
                return this.imageSetList;
            }
            set
            {
                using (LazyLock())
                {
                    this.internalObjList = value
                        .Select(_ => CreateInternal(_))
                        .Where(_ => _ != null)
                        .ToList();
                    this.imageSetList = value;

                    // デフォルト値に設定しておきます。
                    InternalObj = this.internalObjList.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// 表示する評価値画像のセットを取得または設定します。
        /// </summary>
        [DependOnProperty("InternalObj")]
        public ImageSetInfo ImageSet
        {
            get
            {
                using (LazyLock())
                {
                    if (InternalObj == null)
                    {
                        return null;
                    }

                    return InternalObj.ImageSet;
                }
            }
            set
            {
                using (LazyLock())
                {
                    InternalObj = this.internalObjList.FirstOrDefault(_ =>
                        _.ImageSet == value);
                    if (InternalObj == null)
                    {
                        // 見つからない場合は、最初の要素を使います。
                        InternalObj = this.internalObjList.FirstOrDefault();
                    }

                    this.RaisePropertyChanged("ImageSet");
                }
            }
        }

        /// <summary>
        /// 評価値セットのタイトルを取得または設定します。
        /// </summary>
        [DependOnProperty("ImageSet")]
        public string ImageSetTitle
        {
            get
            {
                return (ImageSet != null ? ImageSet.Title : string.Empty);
            }
            set
            {
                var imageSet = ImageSetList.FirstOrDefault(_ => _.Title == value);
                if (imageSet != null)
                {
                    // 合致するタイトルが見つかった場合のみ、
                    // 評価値セットを更新します。
                    ImageSet = imageSet;
                }
            }
        }

        /// <summary>
        /// 内部の描画に使う型の追加を行います。
        /// </summary>
        public void AddInternalType(string typeId, Type type)
        {
            if (string.IsNullOrEmpty(typeId))
            {
                throw new ArgumentException("typeId");
            }

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (!typeof(GLEvaluationElementInternal).IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    "'type'はGLEvaluationElementInternalを継承した" +
                    "クラスである必要があります。");
            }

            using (LazyLock())
            {
                this.internalTypeDic.Add(typeId, type);
            }
        }

        /// <summary>
        /// 型IDから対象となる型を取得します。
        /// </summary>
        public Type FindInternalType(string typeId)
        {
            using (LazyLock())
            {
                Type type;
                if (!this.internalTypeDic.TryGetValue(typeId, out type))
                {
                    return null;
                }

                return type;
            }
        }

        /// <summary>
        /// 評価値画像などを描画する内部オブジェクトを作成します。
        /// </summary>
        private GLEvaluationElementInternal CreateInternal(ImageSetInfo imageSet)
        {
            try
            {
                if (imageSet == null)
                {
                    return null;
                }

                var internalType = FindInternalType(imageSet.TypeId);
                if (internalType == null)
                {
                    return null;
                }

                // internalType型を実体化します。
                var obj = Activator.CreateInstance(internalType) as
                    GLEvaluationElementInternal;
                if (obj == null)
                {
                    return null;
                }

                obj.Initialize(imageSet);
                return obj;
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);
                return null;
            }
        }
    }
}
