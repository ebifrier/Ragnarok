using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private ImageSetInfo imageSet;

        /// <summary>
        /// 選択されている内部オブジェクトを取得します。
        /// </summary>
        public GLEvaluationElementInternal InternalObj
        {
            get { return GetValue<GLEvaluationElementInternal>("InternalObj"); }
            private set { SetValue("InternalObj", value); }
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
                    "'type'はGLEvaluationElement_Internalを継承した" +
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
        /// ImageSetを設定し、内部オブジェクトをそれと同期します。
        /// </summary>
        public void SetImageSet(ImageSetInfo imageSet)
        {
            using (LazyLock())
            {
                InternalObj = this.internalObjList.FirstOrDefault(_ =>
                    _.ImageSet == imageSet);
                this.imageSet = imageSet;
            }
        }

        /// <summary>
        /// ImageSetListを設定し、内部オブジェクトをそれと同期します。
        /// </summary>
        public void SetImageSetList(IEnumerable<ImageSetInfo> imageSetList)
        {
            using (LazyLock())
            {
                this.internalObjList = imageSetList.Select(_ =>
                {
                    try
                    {
                        if (_ == null)
                        {
                            return null;
                        }

                        var internalType = FindInternalType(_.TypeId);
                        if (internalType == null)
                        {
                            return null;
                        }

                        var obj = Activator.CreateInstance(internalType) as
                            GLEvaluationElementInternal;
                        if (obj == null)
                        {
                            return null;
                        }

                        obj.Initialize(_);
                        return obj;
                    }
                    catch (Exception ex)
                    {
                        Util.ThrowIfFatal(ex);
                        return null;
                    }
                }).Where(_ => _ != null).ToList();

                SetImageSet(this.imageSet);
            }
        }
    }
}
