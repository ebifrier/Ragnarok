using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Ragnarok.Utility
{
    /// <summary>
    /// バイナリデータを区切り文字で区切りながら読み込みます。
    /// </summary>
    /// <remarks>
    /// BufferSize 以上のデータは扱えませんので、ReadXxxなどで
    /// データを読み取りながら処理してください。
    /// </remarks>
    public class BinarySplitReader : IDisposable
    {
        //private Stream baseStream;
        private MemoryStream outputStream;
        private byte[] buffer;
        private int readPosition = 0;
        private int writePosition = 0;
        private bool disposed = false;

        /// <summary>
        /// リングバッファ中の位置を前にずらします。
        /// </summary>
        private int IncreamentPosition(int position, int offset)
        {
            var newPosition = position + offset;

            return (newPosition % this.buffer.Length);
        }

        /// <summary>
        /// リングバッファ中の位置を後ろにずらします。
        /// </summary>
        private int DecrementPosition(int position, int offset)
        {
            var newPosition = position - offset;

            while (newPosition < 0)
            {
                newPosition += this.buffer.Length;
            }

            return newPosition;
        }

        /// <summary>
        /// 与えられた位置が読み込み可能な位置かどうか調べます。
        /// </summary>
        private bool CanReadPosition(int position)
        {
            if (this.readPosition <= this.writePosition)
            {
                return (
                    this.readPosition <= position &&
                    position < this.writePosition);
            }
            else
            {
                return (
                    (this.readPosition <= position &&
                     position < this.buffer.Length) ||
                    (0 <= position &&
                     position < this.writePosition));
            }
        }

        /*/// <summary>
        /// 基本になるストリームオブジェクトを取得します。
        /// </summary>
        public virtual Stream BaseStream
        {
            get { return this.baseStream; }
        }*/

        /// <summary>
        /// 現在読み取り中の文字を一文字取得します。
        /// </summary>
        public virtual int PeekChar()
        {
            if (this.buffer == null || !this.buffer.Any())
            {
                return -1;
            }

            return this.buffer[this.readPosition];
        }

        /// <summary>
        /// 読み取り用のデータを追加で書き込みます。
        /// </summary>
        public int Write(byte[] buffer)
        {
            return Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 読み取り用のデータを追加で書き込みます。
        /// </summary>
        public virtual int Write(byte[] buffer, int offset, int count)
        {
            if (this.writePosition < this.readPosition)
            {
                // BUFFER: =======WP----------RP=======
                // (= -> 読み取り中のデータ, - -> 読み込み可能範囲)
                // 
                // 書き込み位置の方が読み込み位置よりも前にあるので、
                // データは読み込み位置までしか書き込むことができません。
                // (RP ～ WP までに読み込み可能なデータがあります)

                // 現在のポジションから、要求されたサイズ分だけデータを書き込みます。
                var writeBytes = 0;

                if (this.readPosition > 0)
                {
                    writeBytes = Math.Min(
                        (this.readPosition - 1) - this.writePosition,
                        count);

                    Buffer.BlockCopy(
                        buffer, offset,
                        this.buffer, this.writePosition,
                        writeBytes);
                    this.writePosition = IncreamentPosition(
                        this.writePosition, writeBytes);
                }

                return writeBytes;
            }
            else
            {
                // BUFFER: -------RP==========WP-------
                // (= -> 読み取り中のデータ, - -> 読み込み可能範囲)
                // 
                // 書き込み位置の方が読み込み位置よりも後にあるので、
                // データはバッファの終端までと、開始位置から読み込み位置まで
                // 書き込むことができます。
                // (RP ～ WP までに読み込み可能なデータがあります)

                // 現在のポジションから、要求されたサイズ分データを読み取ります。
                var writeBytes = Math.Min(
                    this.buffer.Length - this.writePosition
                      - (this.readPosition == 0 ? 1 : 0),
                    count);
                Buffer.BlockCopy(
                    buffer, offset,
                    this.buffer, this.writePosition,
                    writeBytes);
                this.writePosition = IncreamentPosition(
                    this.writePosition, writeBytes);

                // まだ読み込み可能なデータがあれば、バッファの先頭に読み取ります。
                if (count > writeBytes && this.readPosition > 0)
                {
                    var writeBytes2 = Math.Min(
                        this.readPosition - 1,
                        count - writeBytes);

                    Buffer.BlockCopy(
                        buffer, offset + writeBytes,
                        this.buffer, this.writePosition,
                        writeBytes2);
                    this.writePosition = IncreamentPosition(
                        this.writePosition, writeBytes2);
                    writeBytes += writeBytes2;
                }

                return writeBytes;
            }
        }

        /*/// <summary>
        /// 指定したバイト数分だけストリームから読み取ったバイトを
        /// 内部バッファーに格納します。
        /// </summary>
        /// <remarks>
        /// リングバッファ特有の事情などは一切考えません。
        /// </remarks>
        protected virtual int FillBufferInternal(int numBytes)
        {
            // 現在のポジションから、要求されたサイズ分データを読み取ります。
            var readBytes = this.baseStream.Read(
                this.buffer, this.writePosition, numBytes);

            // 内部バッファに書き込んだので、その分だけ書き込み位置をずらします。
            this.writePosition = IncreamentPosition(
                this.writePosition, readBytes);

            return readBytes;
        }

        /// <summary>
        /// ストリームから読み取ったバイトを内部バッファーに格納します。
        /// </summary>
        protected virtual int FillBuffer()
        {
            /*if (numBytes > this.buffer.Length)
            {
                throw new ArgumentOutOfRangeException(
                    "要求されたサイズは内部バッファサイズよりも大きくなっています。",
                    "numBytes");
            }

            if (this.writePosition < this.readPosition)
            {
                // BUFFER: =======RP----------WP=======
                // (= -> 読み取り中のデータ, - -> 読み込み可能範囲)
                // 
                // 書き込み位置の方が読み込み位置よりも前にあるので、
                // データは読み込み位置までしか書き込むことができません。
                // (RP ～ WP までに読み込み可能なデータがあります)

                // 現在のポジションから、要求されたサイズ分データを読み取ります。
                var readBytes = 0;

                if (this.readPosition > 0)
                {
                    var needBytes = (this.readPosition - 1) - this.writePosition;
                    readBytes = FillBufferInternal(needBytes);
                }

                return readBytes;
            }
            else
            {
                // BUFFER: -------WP==========RP-------
                // (= -> 読み取り中のデータ, - -> 読み込み可能範囲)
                // 
                // 書き込み位置の方が読み込み位置よりも後にあるので、
                // データはバッファの終端までと、開始位置から読み込み位置まで
                // 書き込むことができます。
                // (RP ～ WP までに読み込み可能なデータがあります)

                // 現在のポジションから、要求されたサイズ分データを読み取ります。
                var needBytes =  this.buffer.Length - this.writePosition
                    - (this.readPosition == 0 ? 1 : 0);
                var readBytes = FillBufferInternal(needBytes);

                // まだ読み込み可能なデータがあれば、バッファの先頭に読み取ります。
                if (readBytes >= needBytes && this.readPosition > 0)
                {
                    readBytes += FillBufferInternal(this.readPosition - 1);
                }

                return readBytes;
            }
        }*/

        /// <summary>
        /// 指定のサイズ分読み取りが完了したことを設定します。
        /// </summary>
        /// <remarks>
        /// リングバッファ特有の事情を考慮しません。
        /// </remarks>
        protected virtual void ReadBufferDoneInternal(int numBytes)
        {
            this.outputStream.Write(
                this.buffer, this.readPosition, numBytes);

            // 内部バッファを
            this.readPosition = IncreamentPosition(
                this.readPosition, numBytes);
        }

        /// <summary>
        /// 現在の読み込み位置から<paramref name="position"/>まで
        /// 読み込みが完了したことを伝えます。
        /// </summary>
        /// <param name="position">
        /// 読み込みが完了した位置の次の位置(次に読み込みを開始する位置)
        /// を指定します。
        /// </param>
        protected virtual void ReadBufferDone(int position)
        {
            if (this.readPosition <= position)
            {
                var writeBytes = position - this.readPosition;
                ReadBufferDoneInternal(writeBytes);
            }
            else
            {
                var writeBytes = this.buffer.Length - this.readPosition;
                ReadBufferDoneInternal(writeBytes);

                ReadBufferDoneInternal(position);
            }
        }

        /// <summary>
        /// 出力結果を取得します。
        /// </summary>
        private byte[] GetOutputResult()
        {
            var result = this.outputStream.ToArray();

            this.outputStream.SetLength(0);
            return result;
        }

        /// <summary>
        /// 区切り文字が来るまでデータを読み取ります。
        /// </summary>
        /// <remarks>
        /// 最後までデータが来なかった場合は、nullを返します。
        /// separatorが変わるとデータが正しく読み込めなく可能性があります。
        /// </remarks>
        public virtual byte[] ReadUntil(byte[] separator)
        {
            if (separator == null || !separator.Any())
            {
                throw new ArgumentNullException(
                    "セパレーターが正しくありません。", "separator");
            }

            var rp = this.readPosition;
            while (CanReadPosition(rp))
            {
                if (this.buffer[rp] == separator[0])
                {
                    ReadBufferDone(rp + separator.Length);
                    return GetOutputResult();
                }

                rp = IncreamentPosition(rp, 1);
            }

            ReadBufferDone(rp);
            /*if (FillBuffer() == 0)
            {
                return null;
            }*/

            return null; // ReadUntil(separator);
        }

        /// <summary>
        /// 区切り文字が来るまでデータを読み取ります。
        /// </summary>
        public byte[] ReadUntil(byte separator)
        {
            return ReadUntil(new byte[] { separator });
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BinarySplitReader(int bufferSize)
        {
            this.buffer = new byte[bufferSize];
            //this.baseStream = stream;
            this.outputStream = new MemoryStream();
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
        /// ベースストリームを含むすべてのオブジェクトを解放します。
        /// </summary>
        public virtual void Close()
        {
            /*if (this.baseStream != null)
            {
                this.baseStream.Dispose();
                this.baseStream = null;
            }*/

            if (this.outputStream != null)
            {
                this.outputStream.Dispose();
                this.outputStream = null;
            }

            this.buffer = null;
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Close();
                }

                this.disposed = true;
            }
        }
    }
}
