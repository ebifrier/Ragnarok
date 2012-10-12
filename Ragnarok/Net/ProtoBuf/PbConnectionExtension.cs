using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Net.ProtoBuf
{
#if false
    /// <summary>
    /// <see cref="PbConnection"/>の拡張メソッド
    /// </summary>
    public static class PbConnectionExtension
    {
        /// <summary>
        /// オブジェクトのプロパティ変更通知を送信します。
        /// </summary>
        public static void SendObjectChangedCommand(
            this PbConnection connection,
            string objectName, string propertyName,
            Type propertyType, object propertyValue)
        {
            if (string.IsNullOrEmpty(objectName))
            {
                throw new ArgumentNullException("objectName");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            if (propertyType == null)
            {
                throw new ArgumentNullException("propertyType");
            }

            if (propertyValue == null)
            {
                throw new ArgumentNullException("propertyValue");
            }

            var command = new PbPropertyChanged()
            {
                ObjectId = objectName,
                PropertyName = propertyName,
                PropertyType = propertyType,
                PropertyValue = propertyValue,
            };

            // プロパティ値をデシリアライズします。
            command.SerializePropertyValue();

            connection.SendCommand(command);
        }
    }
#endif
}
