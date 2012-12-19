using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// ProtoBuf用の固定長のヘッダを持つオブジェクトです。
    /// </summary>
    /// <remarks>
    /// ProtoBufの送受信に使われ、固定長のデータサイズなどを持ちます。
    /// </remarks>
    internal sealed class PbPacketHeader
    {
        /// <summary>
        /// データのマジックナンバーです。
        /// </summary>
        public const int MAGIC = 0xfe2345;

        /// <summary>
        /// 送信用ヘッダのバイト長です。
        /// </summary>
        public const int HeaderLength = 20;

        /// <summary>
        /// 送信データＩＤを取得または設定します。
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// レスポンスかどうかを取得または設定します。
        /// </summary>
        public bool IsResponse
        {
            get;
            set;
        }

        /// <summary>
        /// 型名のバイト長を取得または設定します。
        /// </summary>
        public int TypeNameLength
        {
            get;
            set;
        }

        /// <summary>
        /// 送受信データのバイト長を取得または設定します。
        /// </summary>
        public int PayloadLength
        {
            get;
            set;
        }

        /// <summary>
        /// デコード済みのバイナリヘッダをエンコードし
        /// このオブジェクトに設定します。
        /// </summary>
        public void SetDecodedHeader(byte[] header)
        {
            if (header == null || header.Length < HeaderLength)
            {
                throw new RagnarokNetException(
                    "パケットのヘッダデータが不正です。");
            }

            // ヘッダーデータをエンディアンを入れ替えながら設定します。
            var value = BitConverter.ToInt32(header, 0);
            var magic = IPAddress.NetworkToHostOrder(value);
            if (magic != MAGIC)
            {
                throw new RagnarokNetException(
                    "The magic of the packet header isn't valid.");
            }

            value = BitConverter.ToInt32(header, 4);
            Id = IPAddress.NetworkToHostOrder(value);

            value = BitConverter.ToInt32(header, 8);
            IsResponse = (IPAddress.NetworkToHostOrder(value) != 0);

            value = BitConverter.ToInt32(header, 12);
            TypeNameLength = IPAddress.NetworkToHostOrder(value);

            value = BitConverter.ToInt32(header, 16);
            PayloadLength = IPAddress.NetworkToHostOrder(value);
        }

        /// <summary>
        /// 送信用のエンコード済みバイナリパケットを取得します。
        /// </summary>
        public byte[] GetEncodedPacket()
        {
            // 内容が無いときはサイズが０になります。
            if (PayloadLength < 0)
            {
                throw new RagnarokNetException(
                    "送信用データを設定してください。");
            }

            // エンコード済みのヘッダ用バッファです。
            var packet = new byte[HeaderLength];

            var value = IPAddress.HostToNetworkOrder(MAGIC);
            var bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(packet, 0);

            value = IPAddress.HostToNetworkOrder(Id);
            bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(packet, 4);

            value = IPAddress.HostToNetworkOrder(IsResponse ? 1 : 0);
            bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(packet, 8);

            value = IPAddress.HostToNetworkOrder(TypeNameLength);
            bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(packet, 12);

            value = IPAddress.HostToNetworkOrder(PayloadLength);
            bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(packet, 16);

            return packet;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbPacketHeader()
        {
            PayloadLength = -1;
        }
    }
}
