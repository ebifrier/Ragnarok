using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Ragnarok.Utility
{
    using Ragnarok.ObjectModel;

    /// <summary>
    /// 設定の読み込み時に使われます。
    /// </summary>
    public class SettingsLoadedEventArgs : EventArgs
    {
    }

    /// <summary>
    /// 設定の保存時に使われます。
    /// </summary>
    public class SettingsSavingEventArgs : CancelEventArgs
    {
    }

    /// <summary>
    /// 設定ファイルを保存するための基底ファイルです。
    /// </summary>
    /// <remarks>
    /// ApplicationSettingsBaseクラスは便利なのですが、
    /// ファイルを指定位置に保存できないなどの弊害があるので、
    /// 自作しています。
    /// 
    /// また、XmlSerializerはTimeSpanなどいくつかのクラスが
    /// 扱えない仕様になっているので、このクラスの使用は
    /// 見送りました。
    /// </remarks>
    [Serializable()]
    public class AppSettingsBase :
        IXmlSerializable, IModel, INotifyPropertyChanging
    {
        /// <summary>
        /// 以前はRootNodeを設定してシリアライズ/デシリアライズをしていたのですが、
        /// これを使うと処理速度が異様に遅くなることが判明したため、
        /// 今は使っていません。
        /// </summary>
        /*private static readonly XmlRootAttribute RootNode =
            new XmlRootAttribute("configuration");*/

        private const string SETTING_TAG = "Setting";
        private const string VALUE_TAG = "Value";
        private const string NAME_ATTRIBUTE = "name";
        private const string SERIALIZEAS_ATTRIBUTE = "serialize-as";

        private readonly object SyncRoot = new object();
        private Dictionary<string, object> propertyStorage =
            new Dictionary<string, object>();

        /// <summary>
        /// プロパティの変更を通知します。
        /// </summary>
        [field: NonSerialized()]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティの変更前に通知を行います。
        /// </summary>
        [field: NonSerialized()]
        public virtual event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// 設定が読み込まれたことを通知します。
        /// </summary>
        [field: NonSerialized()]
        public virtual event EventHandler<SettingsLoadedEventArgs> SettingsLoaded;

        /// <summary>
        /// 設定が読み込まれる前に通知します。
        /// </summary>
        [field:NonSerialized()]
        public virtual event EventHandler<SettingsSavingEventArgs> SettingsSaving;

        /// <summary>
        /// AutoSave機能を使うかどうかを取得または設定します。
        /// </summary>
        public bool IsAutoSave
        {
            get;
            set;
        }

        /// <summary>
        /// プロパティ値の変更を通知します。
        /// </summary>
        public virtual void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, e));
            }

            OnPropertyChanged(this, e);
        }

        /// <summary>
        /// プロパティ値が変わる前に通知します。
        /// </summary>
        public virtual void NotifyPropertyChanging(PropertyChangingEventArgs e)
        {
            var handler = PropertyChanging;

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, e));
            }

            OnPropertyChanging(this, e);
        }

        /// <summary>
        /// 設定が読み込まれた後に通知します。
        /// </summary>
        protected virtual void NotifySettingsLoaded(SettingsLoadedEventArgs e)
        {
            var handler = SettingsLoaded;

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, e));
            }

            OnSettingsLoaded(this, e);
        }

        /// <summary>
        /// 設定が保存される前に通知します。
        /// </summary>
        protected virtual void NotifySettingsSaving(SettingsSavingEventArgs e)
        {
            var handler = SettingsSaving;

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, e));
            }

            OnSettingsSaving(this, e);
        }

        /// <summary>
        /// 設定ファイルのパスを取得または設定します。
        /// </summary>
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// プロパティ名から値を取得または設定します。
        /// </summary>
        public object this[string propertyName]
        {
            get
            {
                lock (SyncRoot)
                {
                    object value = null;

                    if (!this.propertyStorage.TryGetValue(propertyName, out value))
                    {
                        return null;
                    }

                    return value;
                }
            }
            set
            {
                lock (SyncRoot)
                {
                    NotifyPropertyChanging(new PropertyChangingEventArgs(propertyName));
                    this.propertyStorage[propertyName] = value;
                    NotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// プロパティ名から値を取得または設定します。
        /// </summary>
        public T GetValue<T>(string propertyName)
        {
            lock (SyncRoot)
            {
                object value;
                if (!this.propertyStorage.TryGetValue(propertyName, out value))
                {
                    return default(T);
                }

                return (T)value;
            }
        }

        /// <summary>
        /// プロパティ名から値を取得または設定します。
        /// </summary>
        public void SetValue<T>(string propertyName, T value)
        {
            lock (SyncRoot)
            {
                NotifyPropertyChanging(new PropertyChangingEventArgs(propertyName));
                this.propertyStorage[propertyName] = value;
                NotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// プロパティが変更されたときに呼ばれます。
        /// </summary>
        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsAutoSave)
            {
                if (string.IsNullOrEmpty(e.PropertyName))
                {
                    return;
                }

                // AS_ (AutoSave)で始まるプロパティ値が変更されたら
                // 自動的に設定を保存します。
                // 付いてないプロパティは明示的にロードorセーブを
                // しないと保存されません。
                if (e.PropertyName.StartsWith("AS_"))
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// プロパティが変更される前に呼ばれます。
        /// </summary>
        protected virtual void OnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
        }

        /// <summary>
        /// 設定が読み込まれた後に呼ばれます。
        /// </summary>
        protected virtual void OnSettingsLoaded(object sender, SettingsLoadedEventArgs e)
        {
        }

        /// <summary>
        /// 設定が保存される前に呼ばれます。
        /// </summary>
        protected virtual void OnSettingsSaving(object sender, SettingsSavingEventArgs e)
        {
        }

        /// <summary>
        /// default(type)を取得します。
        /// </summary>
        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        /// <summary>
        /// デフォルト値を指定のプロパティに設定します。
        /// </summary>
        private void SetDefaultValue(IPropertyObject propertyObj)
        {
            var dvalue = GetDefault(propertyObj.PropertyInfo.PropertyType);

            propertyObj.SetValue(this, dvalue);
        }

        /// <summary>
        /// 文字列から値の変換を試みます。
        /// </summary>
        protected virtual bool TryConvertToValue(Type type, string str,
                                                 out object result)
        {
            if (StringConverter.CanConvert(type))
            {
                result = StringConverter.Convert(type, str);
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// 値から文字列に変換を試みます。
        /// </summary>
        protected virtual bool TryConvertToString(Type type, object value,
                                                  out string result)
        {
            if (StringConverter.CanConvert(type))
            {
                result = (value != null ? value.ToString() : "");
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// 各プロパティをデフォルト値に初期化します。
        /// </summary>
        public void Reset()
        {
            var propertyDic = MethodUtil.GetPropertyDic(GetType());
            if (propertyDic == null)
            {
                return;
            }

            // DefaultSettingValueAttributeを持つプロパティに
            // デフォルト値を設定します。
            foreach (var propertyObj in propertyDic.Values)
            {
                var property = propertyObj.PropertyInfo;

                try
                {
                    if (property.DeclaringType == typeof(AppSettingsBase))
                    {
                        continue;
                    }
                    if (!propertyObj.CanWrite)
                    {
                        continue;
                    }

                    // デフォルト値を属性から設定します。
                    var attributes = property.GetCustomAttributes(
                        typeof(DefaultValueAttribute),
                        false);
                    if (!attributes.Any())
                    {
                        SetDefaultValue(propertyObj);
                        continue;
                    }

                    // デフォルト値を取得。
                    var attr = (DefaultValueAttribute)attributes[0];
                    var value = attr.Value;

                    // 値が文字列なら値への変換を試みます。
                    var valueStr = value as string;
                    if (valueStr != null)
                    {
                        if (!TryConvertToValue(property.PropertyType, valueStr,
                                               out value))
                        {
                            SetDefaultValue(propertyObj);
                            continue;
                        }
                    }

                    // 値を設定します。
                    propertyObj.SetValue(this, value);
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex,
                        "{0}.{1}: 値の設定に失敗しました。",
                        GetType(),
                        property.Name);

                    // デフォルト値の設定を行います。
                    SetDefaultValue(propertyObj);
                }
            }
        }

        /// <summary>
        /// ファイルから読み込みます。
        /// </summary>
        public void Reload()
        {
            try
            {
                List<string> changedPropertyList = null;

                lock (SyncRoot)
                {
                    if (!File.Exists(Location))
                    {
                        Reset();
                    }
                    else
                    {
                        var oldPropertyStorage = this.propertyStorage;
                        var newInstance = DoDeserialize(Location);

                        this.propertyStorage = (
                            newInstance.propertyStorage ??
                            new Dictionary<string, object>());

                        // 値が変更されたプロパティの一覧を取得します。
                        changedPropertyList =
                            (from pair in this.propertyStorage
                             where CheckPropertyChanged(pair, oldPropertyStorage)
                             select pair.Key)
                                .ToList();
                    }
                }

                // lockの外、ファイルを閉じた後に
                // 必要なプロパティの更新を通知します。
                this.RaisePropertyChanged(changedPropertyList);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "{0}: 設定ファイルの読み込みに失敗しました。", Location);
            }
            
            NotifySettingsLoaded(new SettingsLoadedEventArgs());
        }

        /// <summary>
        /// 実際のデシリアライズ処理を行います。
        /// </summary>
        private AppSettingsBase DoDeserialize(string filepath)
        {
            using (var reader = new XmlTextReader(filepath))
            {
                var f = new DataContractSerializer(GetType());

                return (AppSettingsBase)f.ReadObject(reader);
            }
        }

        /// <summary>
        /// プロパティ値が新旧で変わっているか調べます。
        /// </summary>
        private bool CheckPropertyChanged(KeyValuePair<string, object> propertyPair,
                                          Dictionary<string, object> oldPropertyStorage)
        {
            // 古いプロパティセットにプロパティが存在しないか、
            // 値が変わっていたらそのプロパティは"変更された"
            // と判断されます。
            object oldValue;
            if (!oldPropertyStorage.TryGetValue(propertyPair.Key,
                                                out oldValue))
            {
                return true;
            }

            if (propertyPair.Value == null)
            {
                return (oldValue != null);
            }

            return !propertyPair.Value.Equals(oldValue);
        }

        /// <summary>
        /// スキーマを返します。
        /// </summary>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 単純なシリアライズオブジェクトを読み込みます。
        /// </summary>
        private void ReadStringValue(XmlReader reader,
                                     IPropertyObject propertyObj)
        {
            try
            {
                var str = reader.ReadElementString(VALUE_TAG);
                object value;

                // 文字列からの変換を試みます。
                var property = propertyObj.PropertyInfo;
                if (!TryConvertToValue(property.PropertyType, str, out value))
                {
                    Log.Error(
                        "{0}プロパティ値の変換に失敗しました。",
                        property.Name);
                    return;
                }

                this.propertyStorage[property.Name] = value;
            }
            catch (XmlException ex)
            {
                Log.ErrorException(ex,
                    "{0}プロパティ値の読み込みに失敗しました。",
                    propertyObj.PropertyInfo.Name);
            }
        }

        /// <summary>
        /// 単純ではないシリアライズオブジェクトを読み込みます。
        /// </summary>
        private void ReadXmlValue(XmlReader reader, IPropertyObject propertyObj)
        {
            try
            {
                var property = propertyObj.PropertyInfo;
                var serializer = new DataContractSerializer(property.PropertyType);

                reader.ReadStartElement(VALUE_TAG);
                var value = serializer.ReadObject(reader);
                reader.ReadEndElement();

                this.propertyStorage[property.Name] = value;
            }
            catch (XmlException ex)
            {
                Log.ErrorException(ex,
                    "{0}プロパティ値の読み込みに失敗しました。",
                    propertyObj.PropertyInfo.Name);
            }
        }

        /// <summary>
        /// デシリアライズします。
        /// </summary>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var propertyDic = MethodUtil.GetPropertyDic(GetType());

            reader.ReadStartElement();
            while (!reader.EOF)
            {
                // settingタグの開始ノードまで移動します。
                if (!reader.IsStartElement() ||
                    reader.LocalName != SETTING_TAG)
                {
                    if (!reader.ReadToFollowing(SETTING_TAG))
                    {
                        return;
                    }
                }

                // <Setting name="prop name" serialize-as="string or xml">
                var name = reader.GetAttribute(NAME_ATTRIBUTE);
                var serializeAs = reader.GetAttribute(SERIALIZEAS_ATTRIBUTE);
                if (string.IsNullOrEmpty(name) ||
                    string.IsNullOrEmpty(serializeAs))
                {
                    reader.Read();
                    continue;
                }
                reader.ReadStartElement(SETTING_TAG);

                // プロパティから名前を検索します。
                IPropertyObject propertyObj;
                if (!propertyDic.TryGetValue(name, out propertyObj))
                {
                    continue;
                }

                // 種別によって読み込み方を変えます。
                if (serializeAs == "string")
                {
                    ReadStringValue(reader, propertyObj);
                }
                else
                {
                    ReadXmlValue(reader, propertyObj);
                }

                reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }

        /// <summary>
        /// 設定ファイルに保存します。
        /// </summary>
        public void Save()
        {
            try
            {
                // 保存はキャンセルできます。
                var e = new SettingsSavingEventArgs();
                NotifySettingsSaving(e);
                if (e.Cancel)
                {
                    return;
                }

                lock (SyncRoot)
                {
                    // ファイルのあるディレクトリを事前に作成します。
                    var dirName = Path.GetDirectoryName(Location);

                    // 例外が帰って来たら諦める。
                    Util.CreateDirectoryRecursive(dirName);

                    using (var tmp = new PassingTmpFile(Location))
                    {
                        DoSerialize(tmp.TmpFileName);

                        tmp.Success();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "{0}: 設定ファイルの保存に失敗しました。", Location);
            }
        }

        /// <summary>
        /// 実際のシリアライズ処理を行います。
        /// </summary>
        private void DoSerialize(string filepath)
        {
            // ファイルに設定内容を出力します。
            using (var writer = new XmlTextWriter(filepath, Encoding.UTF8))
            {
                // インデントの指定をしないとインデントされません;;
                writer.Formatting = Formatting.Indented;

                var f = new DataContractSerializer(GetType());
                f.WriteObject(writer, this);
            }
        }

        /// <summary>
        /// 値をシリアライズして書き込みます。
        /// </summary>
        private void WriteXmlValue(XmlWriter writer, string propertyName,
                                   Type valueType, object value)
        {
            try
            {
                var serializer = new DataContractSerializer(valueType);
                
                serializer.WriteObject(writer, value);
            }
            catch (XmlException ex)
            {
                Log.ErrorException(ex,
                    "{0}プロパティの出力に失敗しました。",
                    propertyName);
            }
        }

        /// <summary>
        /// シリアライズします。
        /// </summary>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var propertyDic = MethodUtil.GetPropertyDic(GetType());

            foreach (var propertyObj in propertyDic.Values)
            {
                var property = propertyObj.PropertyInfo;
                if (!property.CanRead || !property.CanWrite)
                {
                    continue;
                }
                if (property.DeclaringType == typeof(AppSettingsBase))
                {
                    continue;
                }

                // ターゲットとなるプロパティの値を取得します。
                object value;
                if (!this.propertyStorage.TryGetValue(property.Name, out value))
                {
                    continue;
                }

                var valueType = (value == null
                    ? property.PropertyType
                    : value.GetType());

                // 単純型は値をそのまま書き出します。
                writer.WriteStartElement(SETTING_TAG);
                writer.WriteAttributeString(NAME_ATTRIBUTE, property.Name);

                // 文字列に変換できれば、それをそのまま出力します。
                string result;
                if (TryConvertToString(valueType, value, out result))
                {
                    writer.WriteAttributeString(SERIALIZEAS_ATTRIBUTE, "string");
                    writer.WriteElementString(VALUE_TAG, result);
                }
                else
                {
                    writer.WriteAttributeString(SERIALIZEAS_ATTRIBUTE, "xml");

                    writer.WriteStartElement(VALUE_TAG);
                    WriteXmlValue(writer, property.Name, valueType, value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// アセンブリから保存場所を設定します。
        /// </summary>
        public void SetLocationFromAssembly(Assembly asm)
        {
            if (asm == null)
            {
                throw new ArgumentNullException("asm");
            }

            // AssemblyCompanyの取得
            var asmcmp = (AssemblyCompanyAttribute)
                Attribute.GetCustomAttribute(
                    asm, typeof(AssemblyCompanyAttribute));

            // AssemblyProductの取得
            var asmprd = (AssemblyProductAttribute)
                Attribute.GetCustomAttribute(
                    asm, typeof(AssemblyProductAttribute));

            var basePath = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

            Location = Path.Combine(
                Path.Combine(
                    Path.Combine(basePath, asmcmp.Company),
                    asmprd.Product),
                "user.config");
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected AppSettingsBase()
        {
            var asm = Assembly.GetEntryAssembly();
            SetLocationFromAssembly(asm);

            // プロパティ値をDictionaryで管理しているので、
            // デフォルト値がないと呼び出しに失敗することがあります。
            Reset();
        }

        /// <summary>
        /// AppSettingsBaseの継承クラスを作成します。
        /// </summary>
        public static T CreateSettings<T>()
            where T : AppSettingsBase, new()
        {
            var settings = new T();
            
            settings.Reload();
            settings.IsAutoSave = true;

            return settings;
        }
    }
}
