using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 囲いに関する情報を保持します。
    /// </summary>
    public partial class CastleInfo
    {
        /// <summary>
        /// 囲いに関する情報がテーブル化されています。
        /// </summary>
        public readonly static List<CastleInfo> CastleTable =
            new List<CastleInfo>
            {
                new CastleInfo(
                    "ダイアモンド美濃", 20,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(2, 8)),
                        new CastlePiece(PieceType.Gin, new Position(3, 8)),
                        new CastlePiece(PieceType.Kin, new Position(4, 9)),
                        new CastlePiece(PieceType.Kin, new Position(5, 8)),
                        new CastlePiece(PieceType.Gin, new Position(4, 7)),
                    },
                    new string[] { "片美濃囲い", "本美濃囲い", }),
                
                new CastleInfo(
                    "すごく固い穴熊", 20,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(1, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(1, 8)),
                        new CastlePiece(PieceType.Kei, new Position(2, 9)),
                        new CastlePiece(PieceType.Gin, new Position(2, 8)),
                        new CastlePiece(PieceType.Kin, new Position(3, 9)),
                        new CastlePiece(PieceType.Kin, new Position(3, 8)),
                    },
                    new string[] { "穴熊", "固い穴熊", "固い穴熊", }),
                
                new CastleInfo(
                    "超固い居飛車穴熊", 20,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(9, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(9, 8)),
                        new CastlePiece(PieceType.Kei, new Position(8, 9)),
                        new CastlePiece(PieceType.Gin, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 9)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                    },
                    new string[] { "居飛車穴熊", "固い居飛車穴熊", "固い居飛車穴熊", }),
                
                new CastleInfo(
                    "松尾流穴熊", 20,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(9, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(9, 8)),
                        new CastlePiece(PieceType.Kei, new Position(8, 9)),
                        new CastlePiece(PieceType.Gin, new Position(8, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 9)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Kin, new Position(6, 7)),
                    },
                    new string[] { "居飛車穴熊", "固い居飛車穴熊", }),
                
                new CastleInfo(
                    "本美濃囲い", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(2, 8)),
                        new CastlePiece(PieceType.Gin, new Position(3, 8)),
                        new CastlePiece(PieceType.Kin, new Position(4, 9)),
                        new CastlePiece(PieceType.Kin, new Position(5, 8)),
                    },
                    new string[] { "片美濃囲い", }),
                
                new CastleInfo(
                    "高美濃囲い", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(2, 8)),
                        new CastlePiece(PieceType.Gin, new Position(3, 8)),
                        new CastlePiece(PieceType.Kin, new Position(4, 9)),
                        new CastlePiece(PieceType.Kin, new Position(4, 7)),
                    },
                    new string[] { "片美濃囲い", }),
                
                new CastleInfo(
                    "銀美濃囲い", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(2, 8)),
                        new CastlePiece(PieceType.Gin, new Position(3, 8)),
                        new CastlePiece(PieceType.Kin, new Position(4, 9)),
                        new CastlePiece(PieceType.Gin, new Position(5, 8)),
                    },
                    new string[] { "片美濃囲い", }),
                
                new CastleInfo(
                    "ヒラメ", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(2, 8)),
                        new CastlePiece(PieceType.Gin, new Position(3, 8)),
                        new CastlePiece(PieceType.Kin, new Position(4, 9)),
                        new CastlePiece(PieceType.Kin, new Position(5, 9)),
                    },
                    new string[] { "片美濃囲い", }),
                
                new CastleInfo(
                    "銀冠", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(2, 8)),
                        new CastlePiece(PieceType.Gin, new Position(2, 7)),
                        new CastlePiece(PieceType.Kin, new Position(3, 8)),
                        new CastlePiece(PieceType.Kin, new Position(4, 7)),
                    },
                    new string[] { "片銀冠", }),
                
                new CastleInfo(
                    "固い穴熊", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(1, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(1, 8)),
                        new CastlePiece(PieceType.Kei, new Position(2, 9)),
                        new CastlePiece(PieceType.Gin, new Position(2, 8)),
                        new CastlePiece(PieceType.Kin, new Position(3, 9)),
                    },
                    new string[] { "穴熊", }),
                
                new CastleInfo(
                    "固い穴熊", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(1, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(1, 8)),
                        new CastlePiece(PieceType.Kei, new Position(2, 9)),
                        new CastlePiece(PieceType.Gin, new Position(2, 8)),
                        new CastlePiece(PieceType.Kin, new Position(3, 8)),
                    },
                    new string[] { "穴熊", }),
                
                new CastleInfo(
                    "総矢倉", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 7)),
                        new CastlePiece(PieceType.Kin, new Position(6, 7)),
                        new CastlePiece(PieceType.Gin, new Position(5, 7)),
                    },
                    new string[] { "矢倉", }),
                
                new CastleInfo(
                    "菱矢倉", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 7)),
                        new CastlePiece(PieceType.Kin, new Position(6, 7)),
                        new CastlePiece(PieceType.Gin, new Position(6, 6)),
                    },
                    new string[] { "矢倉", }),
                
                new CastleInfo(
                    "居飛車銀冠", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 8)),
                        new CastlePiece(PieceType.Gin, new Position(8, 7)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Kin, new Position(6, 7)),
                    },
                    new string[] { "居飛車片銀冠", }),
                
                new CastleInfo(
                    "固い居飛車穴熊", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(9, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(9, 8)),
                        new CastlePiece(PieceType.Kei, new Position(8, 9)),
                        new CastlePiece(PieceType.Gin, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 9)),
                    },
                    new string[] { "居飛車穴熊", }),
                
                new CastleInfo(
                    "固い居飛車穴熊", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(9, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(9, 8)),
                        new CastlePiece(PieceType.Kei, new Position(8, 9)),
                        new CastlePiece(PieceType.Gin, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                    },
                    new string[] { "居飛車穴熊", }),
                
                new CastleInfo(
                    "ビッグ４", 10,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(9, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(9, 8)),
                        new CastlePiece(PieceType.Kei, new Position(8, 9)),
                        new CastlePiece(PieceType.Kin, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(8, 7)),
                        new CastlePiece(PieceType.Gin, new Position(7, 7)),
                    },
                    new string[] { "居飛車銀冠穴熊", }),
                
                new CastleInfo(
                    "片美濃囲い", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(2, 8)),
                        new CastlePiece(PieceType.Gin, new Position(3, 8)),
                        new CastlePiece(PieceType.Kin, new Position(4, 9)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "金美濃囲い", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(2, 8)),
                        new CastlePiece(PieceType.Kin, new Position(3, 8)),
                        new CastlePiece(PieceType.Gin, new Position(4, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "木村美濃", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(2, 8)),
                        new CastlePiece(PieceType.Kin, new Position(3, 8)),
                        new CastlePiece(PieceType.Gin, new Position(4, 7)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "片銀冠", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(2, 8)),
                        new CastlePiece(PieceType.Gin, new Position(2, 7)),
                        new CastlePiece(PieceType.Kin, new Position(3, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "銀冠穴熊", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(1, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(1, 8)),
                        new CastlePiece(PieceType.Kei, new Position(2, 9)),
                        new CastlePiece(PieceType.Gin, new Position(2, 7)),
                        new CastlePiece(PieceType.Kin, new Position(3, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "穴熊", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(1, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(1, 8)),
                        new CastlePiece(PieceType.Kei, new Position(2, 9)),
                        new CastlePiece(PieceType.Gin, new Position(2, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "早囲い", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(3, 8)),
                        new CastlePiece(PieceType.Gin, new Position(4, 8)),
                        new CastlePiece(PieceType.Kin, new Position(4, 9)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "金無双", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(3, 8)),
                        new CastlePiece(PieceType.Kin, new Position(5, 8)),
                        new CastlePiece(PieceType.Kin, new Position(4, 8)),
                        new CastlePiece(PieceType.Gin, new Position(2, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "金無双", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(3, 8)),
                        new CastlePiece(PieceType.Kin, new Position(5, 8)),
                        new CastlePiece(PieceType.Kin, new Position(4, 8)),
                        new CastlePiece(PieceType.Gin, new Position(3, 9)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "カニ囲い", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(6, 9)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(6, 8)),
                        new CastlePiece(PieceType.Kin, new Position(5, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "矢倉", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 7)),
                        new CastlePiece(PieceType.Kin, new Position(6, 7)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "銀立ち矢倉", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Kin, new Position(6, 7)),
                        new CastlePiece(PieceType.Gin, new Position(7, 6)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "銀矢倉", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 7)),
                        new CastlePiece(PieceType.Gin, new Position(6, 7)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "ボナンザ囲い", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 7)),
                        new CastlePiece(PieceType.Kin, new Position(6, 8)),
                        new CastlePiece(PieceType.Kin, new Position(5, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "菊水矢倉", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 9)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(6, 7)),
                        new CastlePiece(PieceType.Kei, new Position(7, 7)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "凹み矢倉", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 7)),
                        new CastlePiece(PieceType.Kin, new Position(6, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "片矢倉", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 7)),
                        new CastlePiece(PieceType.Kin, new Position(6, 7)),
                        new CastlePiece(PieceType.Kin, new Position(6, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "居飛車片銀冠", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 8)),
                        new CastlePiece(PieceType.Gin, new Position(8, 7)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "居飛車銀冠穴熊", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(9, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(9, 8)),
                        new CastlePiece(PieceType.Kei, new Position(8, 9)),
                        new CastlePiece(PieceType.Gin, new Position(8, 7)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "居飛車穴熊", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(9, 9)),
                        new CastlePiece(PieceType.Kyo, new Position(9, 8)),
                        new CastlePiece(PieceType.Kei, new Position(8, 9)),
                        new CastlePiece(PieceType.Gin, new Position(8, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "ミレニアム", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 9)),
                        new CastlePiece(PieceType.Kei, new Position(7, 7)),
                        new CastlePiece(PieceType.Gin, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 9)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "船囲い", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 9)),
                        new CastlePiece(PieceType.Kin, new Position(6, 9)),
                        new CastlePiece(PieceType.Kin, new Position(5, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "箱入り娘", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 9)),
                        new CastlePiece(PieceType.Kin, new Position(6, 9)),
                        new CastlePiece(PieceType.Kin, new Position(6, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "セメント囲い", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(7, 8)),
                        new CastlePiece(PieceType.Gin, new Position(6, 7)),
                        new CastlePiece(PieceType.Gin, new Position(5, 7)),
                        new CastlePiece(PieceType.Kin, new Position(6, 8)),
                        new CastlePiece(PieceType.Kin, new Position(5, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "中住まい", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(5, 8)),
                        new CastlePiece(PieceType.Kin, new Position(3, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "中原囲い", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(6, 9)),
                        new CastlePiece(PieceType.Kin, new Position(5, 9)),
                        new CastlePiece(PieceType.Gin, new Position(4, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "左美濃", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 8)),
                        new CastlePiece(PieceType.Kin, new Position(6, 9)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "天守閣美濃", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(8, 7)),
                        new CastlePiece(PieceType.Kaku, new Position(8, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 8)),
                        new CastlePiece(PieceType.Kin, new Position(6, 9)),
                        new CastlePiece(PieceType.Kin, new Position(5, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "米長玉", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(9, 8)),
                        new CastlePiece(PieceType.Kyo, new Position(9, 9)),
                        new CastlePiece(PieceType.Gin, new Position(8, 7)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "串カツ囲い", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(9, 8)),
                        new CastlePiece(PieceType.Kyo, new Position(9, 9)),
                        new CastlePiece(PieceType.Gin, new Position(8, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 9)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "雁木", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(6, 9)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Kin, new Position(5, 8)),
                        new CastlePiece(PieceType.Gin, new Position(6, 7)),
                        new CastlePiece(PieceType.Gin, new Position(5, 7)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "無敵囲い", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(5, 9)),
                        new CastlePiece(PieceType.Kin, new Position(6, 9)),
                        new CastlePiece(PieceType.Kin, new Position(4, 9)),
                        new CastlePiece(PieceType.Gin, new Position(6, 8)),
                        new CastlePiece(PieceType.Gin, new Position(4, 8)),
                        new CastlePiece(PieceType.Hisya, new Position(5, 8)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "風車", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Kei, new Position(7, 7)),
                        new CastlePiece(PieceType.Gin, new Position(6, 7)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Kei, new Position(3, 7)),
                        new CastlePiece(PieceType.Gin, new Position(4, 7)),
                    },
                    new string[] { }),
                
                new CastleInfo(
                    "イチゴ囲い", 0,
                    new []
                    {
                        new CastlePiece(PieceType.Gyoku, new Position(6, 8)),
                        new CastlePiece(PieceType.Kin, new Position(7, 8)),
                        new CastlePiece(PieceType.Kin, new Position(5, 8)),
                        new CastlePiece(PieceType.Gin, new Position(7, 9)),
                    },
                    new string[] { }),
            };
    }
}
