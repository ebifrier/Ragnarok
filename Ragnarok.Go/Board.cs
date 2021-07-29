using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ragnarok.Go
{
    /// <summary>
    /// 石の連なり＝連を管理します。
    /// </summary>
    public sealed class StoneGroup
    {
        public HashSet<Square> Stones = new HashSet<Square>();
        public HashSet<Square> Libs = new HashSet<Square>();
        public Square Atari = Square.Empty();

        public StoneGroup()
        {
        }

        public StoneGroup(Square sq)
        {
            Stones.Add(sq);
        }

        /// <summary>
        /// オブジェクトのDeepCopyを返します。
        /// </summary>
        public StoneGroup Clone()
        {
            return new StoneGroup
            {
                Stones = new HashSet<Square>(Stones),
                Libs = new HashSet<Square>(Libs),
                Atari = Atari,
            };
        }

        /// <summary>
        /// 連に登録されている医師の数を取得します。
        /// </summary>
        public int Count
        {
            get { return Stones.Count(); }
        }

        /// <summary>
        /// 呼吸点の数を取得します。
        /// </summary>
        public int LibCount
        {
            get { return Libs.Count(); }
        }

        /// <summary>
        /// すべての石をクリアします。
        /// </summary>
        public void Clear()
        {
            Stones.Clear();
            Libs.Clear();
            Atari = Square.Empty();
        }

        /// <summary>
        /// 呼吸点を追加します。
        /// </summary>
        public void AddLib(Square sq)
        {
            if (sq.IsPass) return;
            if (Libs.Contains(sq)) return;

            Libs.Add(sq);
            Atari = sq;
        }

        /// <summary>
        /// 呼吸点を削除します。
        /// </summary>
        public void RemoveLib(Square sq)
        {
            if (sq.IsPass) return;
            if (!Libs.Contains(sq)) return;

            Libs.Remove(sq);
        }

        /// <summary>
        /// 連同士の結合処理を行います。
        /// </summary>
        public void Merge(StoneGroup other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            // 重複していない交点の石のみ連に追加します。
            other.Stones.ForEach(_ => Stones.Add(_));

            // 石として登録されていない交点を改めて呼吸点として追加します。
            var copyLibs = new List<Square>(Libs);
            Libs.Clear();
            copyLibs.Concat(other.Libs)
                .Where(_ => !Stones.Contains(_))
                .ForEach(_ => Libs.Add(_));

            // 呼吸点が一つしかない場合はアタリを設定します。
            if (Libs.Count() == 1)
            {
                Atari = Libs.First();
            }
        }
    }

    /// <summary>
    /// 碁石のポジションを保持します。
    /// </summary>
    public class Board : IEquatable<Board>
    {
        private readonly int[] Dir4;
        private readonly int boardSize;
        private int[] captureCounts = new int[] { 0, 0 };
        private StoneGroup[] stoneGroups;
        private Stone[] stones;
        private Stone turn;
        private Square ko;
        private Square lastSq;
        private int removeCount;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Board(int boardSize, bool init=true)
        {
            if (boardSize < 3 || boardSize % 2 == 0)
            {
                throw new ArgumentException("invalid value", nameof(boardSize));
            }

            var boardSize2 = boardSize + 2;
            this.boardSize = boardSize;
            this.Dir4 = new int[] { -1, -boardSize2, +1, boardSize2 };

            if (init)
            {
                this.stones = new Stone[boardSize2 * boardSize2];
                this.stoneGroups = new StoneGroup[boardSize2 * boardSize2];
                this.turn = Stone.Black;
                this.ko = Square.Empty();

                // 盤の外側を使わない値で埋めます。
                for (var i = 0; i < boardSize2; ++i)
                {
                    this.stones[i * boardSize2 + 0] = Stone.Wall;
                    this.stones[0 * boardSize2 + i] = Stone.Wall;
                    this.stones[i * boardSize2 + (boardSize2 - 1)] = Stone.Wall;
                    this.stones[(boardSize2 - 1) * boardSize2 + i] = Stone.Wall;
                }
            }
        }

        /// <summary>
        /// object.ReferenceEqualsによるオブジェクトの比較を行います。
        /// </summary>
        private sealed class IdentityEqualityComparer<T> : IEqualityComparer<T>
            where T : class
        {
            public int GetHashCode(T value)
            {
                return RuntimeHelpers.GetHashCode(value);
            }

            public bool Equals(T left, T right)
            {
                return ReferenceEquals(left, right);
            }
        }

        /// <summary>
        /// 盤のDeepCopyを返します。
        /// </summary>
        public Board Clone()
        {
            // StoneGroupは同じものがstoneGroupsに複数含まれているため、
            // クローン先の盤でも同じように重複させる必要がある。
            var sgDic = this.stoneGroups
                .Where(_ => _ != null)
                .Distinct(new IdentityEqualityComparer<StoneGroup>())
                .ToDictionary(_ => _, _ => _.Clone(), new IdentityEqualityComparer<StoneGroup>());

            // sgDicに重複しないような形でオブジェクトのクローンが含まれている。
            var clonedStoneGroups = this.stoneGroups
                .Select(_ => _ == null ? null : sgDic[_])
                .ToArray();

            return new Board(BoardSize)
            {
                stones = (Stone[])this.stones.Clone(),
                stoneGroups = clonedStoneGroups,
                captureCounts = (int[])this.captureCounts.Clone(),
                turn = this.turn,
                ko = this.ko,
                lastSq = this.lastSq,
                removeCount = this.removeCount,
            };
        }

        /// <summary>
        /// 碁盤のサイズを取得します。
        /// </summary>
        public int BoardSize
        {
            get { return this.boardSize; }
        }

        /// <summary>
        /// 現在の手番を取得または設定します。
        /// </summary>
        public Stone Turn
        {
            get { return this.turn; }
            set
            {
                if (this.turn != value)
                {
                    this.turn = value;
                    this.ko = Square.Empty(); // コウはクリアします。
                }
            }
        }

        /// <summary>
        /// 現在の手番を取得または設定します。
        /// </summary>
        public Square Ko
        {
            get { return this.ko; }
        }

        /// <summary>
        /// 最後に打った石の交点を取得します。
        /// </summary>
        public Square LastSq
        {
            get { return this.lastSq; }
        }

        /// <summary>
        /// 黒番が取り上げた石の数を取得または設定します。
        /// </summary>
        public int BlackCaptureCount
        {
            get { return this.captureCounts[0]; }
            set { this.captureCounts[0] = value; }
        }

        /// <summary>
        /// 白番が取り上げた石の数を取得または設定します。
        /// </summary>
        public int WhiteCaptureCount
        {
            get { return this.captureCounts[1]; }
            set { this.captureCounts[1] = value; }
        }

        /// <summary>
        /// 指定の場所の石の色を取得または設定します。
        /// </summary>
        public Stone this[int file, int rank]
        {
            get { return this[Square.Create(file, rank, BoardSize)]; }
            set { this[Square.Create(file, rank, BoardSize)] = value; }
        }

        /// <summary>
        /// 指定の場所の石の色を取得または設定します。
        /// </summary>
        public Stone this[int index]
        {
            get { return this[Square.FromIndex(index, BoardSize)]; }
            set { this[Square.FromIndex(index, BoardSize)] = value; }
        }

        /// <summary>
        /// 指定の場所の石の色を取得または設定します。
        /// </summary>
        public Stone this[Square sq]
        {
            get { return this.stones[sq.Index]; }
            set { this.stones[sq.Index] = value; }
        }

        /// <summary>
        /// FEN形式の局面を読み込み新たなオブジェクトを作成します。
        /// </summary>
        public static Board FromFen(string fen)
        {
            if (string.IsNullOrEmpty(fen))
            {
                return null;
            }

            // fen文字列の最初に碁盤の路数が入っています。
            var split = fen.Split(' ');
            if (split.Count() < 2)
            {
                return null;
            }

            var boardSize = int.Parse(split[0]);
            if (boardSize < 3 || boardSize > 27)
            {
                return null;
            }

            var board = new Board(boardSize);
            int wcount = 0;
            int file = 0;
            int rank = 0;

            foreach (var c in split[1])
            {
                switch (c)
                {
                    case 'B':
                    case 'b':
                        // 'B'文字で黒石の指定となります。
                        if (wcount > 0) { file += wcount; wcount = 0; }
                        board[file, rank] = Stone.Black;
                        file += 1;
                        break;
                    case 'W':
                    case 'w':
                        // 'W'文字で白石の指定となります。
                        if (wcount > 0) { file += wcount; wcount = 0; }
                        board[file, rank] = Stone.White;
                        file += 1;
                        break;
                    case 'E':
                    case 'e':
                        // 'E'文字でエラーの指定となります。
                        if (wcount > 0) { file += wcount; wcount = 0; }
                        board[file, rank] = Stone.Error;
                        file += 1;
                        break;
                    case '/':
                        // '/'文字で段の区切りとなります。
                        if (boardSize != file + wcount)
                        {
                            return null;
                        }
                        wcount = 0;
                        file = 0;
                        rank += 1;

                        // 段数が列数より多いということはありえない
                        if (rank > boardSize)
                        {
                            return null;
                        }
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        // 数字は空白の指定となります。
                        wcount = wcount * 10 + (c - '0');
                        break;
                }
            }

            // 最後に列数の確認を行う。
            if (file + wcount != boardSize)
            {
                return null;
            }

            if (split.Count() > 2)
            {
                var turn = split[2][0];
                board.Turn = turn != 'W' ? Stone.Black : Stone.White;
            }

            if (split.Count() > 3)
            {
                if (!int.TryParse(split[3], out int capCount))
                {
                    return null;
                }
                board.captureCounts[0] = capCount;
            }

            if (split.Count() > 4)
            {
                if (!int.TryParse(split[4], out int capCount))
                {
                    return null;
                }
                board.captureCounts[1] = capCount;
            }

            return board;
        }

        public string ToFen()
        {
            var sb = new StringBuilder();
            sb.Append(BoardSize);
            sb.Append(" ");

            for (var row = 0; row < BoardSize; ++row)
            {
                var wcount = 0;
                for (var col = 0; col < BoardSize; ++col)
                {
                    switch (this[col, row])
                    {
                        case Stone.Black:
                            if (wcount != 0)
                            {
                                sb.Append(wcount);
                                wcount = 0;
                            }
                            sb.Append('B');
                            break;
                        case Stone.White:
                            if (wcount != 0)
                            {
                                sb.Append(wcount);
                                wcount = 0;
                            }
                            sb.Append('W');
                            break;
                        case Stone.Error:
                            if (wcount != 0)
                            {
                                sb.Append(wcount);
                                wcount = 0;
                            }
                            sb.Append('E');
                            break;
                        case Stone.Empty:
                            wcount += 1;
                            break;
                    }
                }

                if (wcount != 0)
                {
                    sb.Append(wcount);
                }

                if (row < BoardSize)
                {
                    sb.Append("/");
                }
            }

            sb.Append(" ");
            sb.Append(Turn == Stone.Black ? 'B' : 'W');
            sb.Append(" ");
            sb.Append(BlackCaptureCount);
            sb.Append(" ");
            sb.Append(WhiteCaptureCount);

            return sb.ToString();
        }

        /// <summary>
        /// 盤面の簡易表示を行います。
        /// </summary>
        public override string ToString()
        {
            var buf = new StringBuilder();

            buf.Append("  ");
            foreach (var x in Enumerable.Range(0, BoardSize))
            {
                buf.Append($"{x + 1,2}");
            }
            buf.AppendLine();

            foreach (var y in Enumerable.Range(0, BoardSize))
            {
                buf.Append($"{y + 1,2}");
                foreach (var x in Enumerable.Range(0, BoardSize))
                {
                    var stone = this[x, y];
                    var sign =
                        stone == Stone.Empty ? " +" :
                        stone == Stone.Black ? " @" :
                        stone == Stone.White ? " O" : " E";
                    buf.Append(sign);
                }
                buf.AppendLine();
            }

            buf.AppendLine(Turn == Stone.Black ? "black" : "white");

            return buf.ToString();
        }

        /// <summary>
        /// 局面を90 * <paramref name="times90"/>度回転させた局面を作成します。
        /// </summary>
        public Board Rotate(int times90)
        {
            var board = new Board(BoardSize);
            board.turn = Turn;
            board.ko = Ko.Rotate(times90);
            board.lastSq = LastSq.Rotate(times90);

            Square.All(BoardSize)
                .ForEach(_ => board[_.Rotate(times90)] = this[_]);
            return board;
        }

        /// <summary>
        /// 局面を180度回転させた局面を作成します。
        /// </summary>
        public Board Inv()
        {
            return Rotate(2);
        }

        /// <summary>
        /// 連に含まれる石をまとめて削除します。
        /// </summary>
        /// <returns>
        /// 削除した石の数を返します。
        /// </returns>
        private int RemoveStoneGroup(Square sq)
        {
            if (this.stoneGroups[sq.Index] == null)
            {
                // すでに取り上げられている可能性があります。
                return 0;
            }

            // 連に登録された医師をすべて削除します。
            var sg = this.stoneGroups[sq.Index];
            sg.Stones
                .ForEach(rsq =>
                {
                    this.stones[rsq.Index] = Stone.Empty;
                    this.stoneGroups[rsq.Index] = null;

                    // 削除される点の周囲の連に呼吸点を追加します。
                    Dir4.Select(_ => rsq + _)
                        .Select(_ => this.stoneGroups[_.Index])
                        .Where(_ => _ != null)
                        .ForEach(_ => _.AddLib(rsq));
                });

            return sg.Stones.Count();
        }

        /// <summary>
        /// <paramref name="sq"/>で指定された交点に<paramref name="stone"/>色の石を置きます。
        /// </summary>
        /// <remarks>
        /// 石の取り上げ処理も同時に行います。
        /// </remarks>
        public bool PlaceStone(Square sq, Stone stone)
        {
            if (stone != Stone.Black && stone != Stone.White)
            {
                return false;
            }

            if (this.stones[sq.Index] != Stone.Empty)
            {
                return false;
            }

            var stoneGroup = new StoneGroup(sq);
            this.stones[sq.Index] = stone;
            this.stoneGroups[sq.Index] = stoneGroup;

            // 呼吸点の調整
            Dir4.Select(_ => sq + _)
                .ForEach(nsq =>
                {
                    if (this.stones[nsq.Index] == Stone.Empty)
                    {
                        // この石の連に呼吸点を追加
                        stoneGroup.AddLib(nsq);
                    }
                    else if (this.stoneGroups[nsq.Index] != null)
                    {
                        // 周りの連から呼吸点を削除
                        this.stoneGroups[nsq.Index].RemoveLib(sq);
                    }
                });

            // 連の接続を行います。
            Dir4.Select(_ => sq + _)
                .Where(nsq => this.stones[nsq.Index] == stone)
                .ForEach(nsq =>
                {
                    // 石の色が同じ周囲の連をすべて接続します。
                    stoneGroup.Merge(this.stoneGroups[nsq.Index]);
                    this.stoneGroups[nsq.Index].Stones
                        .ForEach(_ => this.stoneGroups[_.Index] = stoneGroup);
                });

            // 削除された石の数をクリアしておきます。
            this.removeCount = 0;

            // 石の取り上げ処理
            Dir4.Select(_ => sq + _)
                .Where(nsq => this.stones[nsq.Index] == stone.Inv())
                .Where(nsq => this.stoneGroups[nsq.Index].LibCount == 0)
                .ForEach(nsq => this.removeCount += RemoveStoneGroup(nsq));

            this.captureCounts[(int)stone - 1] += this.removeCount;
            return true;
        }

        /// <summary>
        /// 次の手が合法手かどうか確認します。
        /// </summary>
        public bool IsLegal(Square sq, Stone color = Stone.Empty)
        {
            if (sq.IsPass)
            {
                return true;
            }

            if (sq.IsEmpty || sq == Ko || this[sq] != Stone.Empty)
            {
                return false;
            }

            var turn = (color != Stone.Empty ? color : Turn);
            var sgCounts = new int[] { 0, 0 };
            var atariCounts = new int[] { 0, 0 };

            // 上下左右の連を調べ、石が含まれる連の数と
            // 呼吸点が1の連の数を数えます。
            foreach (var dir in Dir4)
            {
                var nsq = sq + dir;
                var stone = this[nsq];

                if (stone == Stone.Empty)
                {
                    return true;
                }
                else if (stone == Stone.Black || stone == Stone.White)
                {
                    // 石が含まれる連の数と、呼吸点が1の連の数をカウント
                    sgCounts[(int)stone - 1] += 1;
                    if (this.stoneGroups[nsq.Index].LibCount == 1)
                    {
                        atariCounts[(int)stone - 1] += 1;
                    }
                }
            }

            // 置く石と反対の色の連にアタリが含まれれば
            // 石が取れるということなので合法手となる。
            if (atariCounts[(int)turn.Inv() - 1] > 0)
            {
                return true;
            }

            // また、自分と同じ色の連に呼吸点が2以上のものが含まれていれば
            // 石を置いても連が取られることはないので合法手となる。
            if (atariCounts[(int)turn - 1] < sgCounts[(int)turn - 1])
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 盤上に石を追加します。
        /// </summary>
        /// <remarks>
        /// 石の取り上げ処理に加え、コウの判定も行います。
        /// </remarks>
        public bool MakeMove(Square sq, Stone color = Stone.Empty)
        {
            if (!IsLegal(sq))
            {
                return false;
            }

            var turn = (color != Stone.Empty ? color : Turn);
            if (sq.IsPass)
            {
                this.ko = Square.Empty();
            }
            else
            {
                PlaceStone(sq, turn);
                this.ko = Square.Empty();

                // コウの判定を行います。
                var sg = this.stoneGroups[sq.Index];
                if (this.removeCount == 1 && sg.Count == 1 && sg.LibCount == 1)
                {
                    this.ko = sg.Atari;
                }
            }

            this.turn = turn.Inv();
            this.lastSq = sq;
            return true;
        }

        /*public void UndoMove()
        {
            if (!this.stList.Any())
            {
                return;
            }

            var st = this.stList.Last();

            // 打った手を元に戻します。
            if (!st.Move.IsSpecialMove)
            {
                var sq = st.Move.CPoint;
                this.stones[sq.Index] = Stone.None;
            }

            // 取った手を元に戻します。
            foreach (var sq in st.TookStones)
            {
                this.stones[sq.Index] = st.Move.Stone.Inv();
            }

            this.stList.RemoveAt(this.stList.Count() - 1);
            Turn = Turn.Inv();
        }*/

        public override int GetHashCode()
        {
            return (
                BoardSize.GetHashCode() ^
                Turn.GetHashCode() ^
                this.stones.GetHashCode());
        }

        /// <summary>
        /// 盤の比較を行います。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = Util.PreEquals(this, obj);
            if (result != null)
            {
                return result.Value;
            }

            return Equals((Board)obj);
        }

        /// <summary>
        /// 盤の比較を行います。
        /// </summary>
        public bool Equals(Board obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (BoardSize != obj.BoardSize || Turn != obj.Turn)
            {
                return false;
            }

            if (!this.stones.SequenceEqual(obj.stones))
            {
                return false;
            }

            return true;
        }
    }
}
