using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Ragnarok.ObjectModel;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 投票が行われた指し手を時刻と共に保持します。
    /// </summary>
    public sealed class RegistereredMove
    {
        /// <summary>
        /// 投票された指し手を取得します。
        /// </summary>
        public LiteralMove Move
        {
            get;
            set;
        }

        /// <summary>
        /// 投票時刻を取得または設定します。
        /// </summary>
        public DateTime Timestamp
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 指し手の統計を扱うためのクラスです。
    /// </summary>
    public class MoveStatistics : NotifyObject
    {
        private int defaultSkillPoint = 10;
        private readonly Dictionary<SkillLevel, int> skillPointTable =
            new Dictionary<SkillLevel, int>();
        private readonly Dictionary<ShogiPlayer, RegistereredMove> moveDatas =
            new Dictionary<ShogiPlayer, RegistereredMove>();
        private LiteralMove opponentMove;

        /// <summary>
        /// デフォルトの得票ポイントを取得または設定します。
        /// </summary>
        public int DefaultSkillPoint
        {
            get { return this.defaultSkillPoint; }
            set { SetValue("DefaultSkillPoint", value, ref this.defaultSkillPoint); }
        }

        /// <summary>
        /// 対局相手の指し手を取得または設定します。
        /// </summary>
        public LiteralMove OpponentMove
        {
            get { return this.opponentMove; }
            set { SetValue(nameof(OpponentMove), value, ref this.opponentMove); }
        }

        /// <summary>
        /// 指し手を得票ポイントが高い順に取得します。
        /// </summary>
        [DependOnProperty(nameof(OpponentMove))]
        [DependOnProperty(nameof(DefaultSkillPoint))]
        public List<MovePointPair> MoveList
        {
            get
            {
                using (LazyLock())
                {
                    // 各指し手をポイントに変換し、その後
                    // 指し手を得票ポイント順にソートします。
                    return
                        (from pair in this.moveDatas
                         let move = ModifyMove(pair.Value.Move)
                         group pair by move into g

                         let total = g.Sum(_ => GetSkillPoint(_.Key.SkillLevel))
                         let timestamp = g.Max(_ => _.Value.Timestamp)
                         orderby total descending, timestamp descending
                         select new MovePointPair()
                         {
                             Move = g.Key,
                             Point = total,
                             Timestamp = timestamp,
                         }).ToList();
                }
            }
        }

        /// <summary>
        /// 直前の相手の手を考慮して、与えられた指し手を修正します。
        /// </summary>
        private LiteralMove ModifyMove(LiteralMove move)
        {
            if (move == null)
            {
                throw new ArgumentNullException(nameof(move));
            }

            if (!move.Validate())
            {
                throw new ArgumentException(
                    "与えられた指し手が正しくありません。", nameof(move));
            }

            using (LazyLock())
            {
                if (this.opponentMove == null)
                {
                    return move;
                }
                
                var newMove = move.Clone();

                // 同○○なら、相手の直前の手と列段を合わせます。
                if (newMove.SameAsPrev)
                {
                    newMove.File = this.opponentMove.File;
                    newMove.Rank = this.opponentMove.Rank;
                }
                else if (newMove.File == this.opponentMove.File &&
                         newMove.Rank == this.opponentMove.Rank)
                {
                    newMove.SameAsPrev = true;
                }

                return newMove;
            }
        }

        /// <summary>
        /// 棋力別の得票ポイントを設定します。
        /// </summary>
        public void SetSkillPoint(SkillLevel skillLevel, int point)
        {
            using (LazyLock())
            {
                this.skillPointTable[skillLevel] = point;

                this.RaisePropertyChanged(nameof(MoveList));
            }
        }

        /// <summary>
        /// 棋力別の得票ポイントを取得します。
        /// </summary>
        public int GetSkillPoint(SkillLevel skillLevel)
        {
            using (LazyLock())
            {
                int point;
                if (!this.skillPointTable.TryGetValue(skillLevel, out point))
                {
                    return this.defaultSkillPoint;
                }

                return point;
            }
        }

        /// <summary>
        /// 指し手に投票します。
        /// </summary>
        public void Vote(ShogiPlayer player, LiteralMove move, DateTime timestamp)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (move == null)
            {
                throw new ArgumentNullException(nameof(move));
            }

            using (LazyLock())
            {
                // プレイヤーの棋力が変わっている場合があるので、
                // プレイヤーも必ず設定し直します。
                if (this.moveDatas.ContainsKey(player))
                {
                    this.moveDatas.Remove(player);
                }

                this.moveDatas[player] = new RegistereredMove()
                {
                    Move = move,
                    Timestamp = timestamp,
                };

                this.RaisePropertyChanged(nameof(MoveList));
            }
        }

        /// <summary>
        /// 投票された指し手があれば、それを取得します。
        /// </summary>
        public RegistereredMove GetVote(string userId)
        {
            var player = new ShogiPlayer()
            {
                PlayerId = userId,
            };

            using (LazyLock())
            {
                RegistereredMove move;
                if (!this.moveDatas.TryGetValue(player, out move))
                {
                    return null;
                }

                return move;
            }
        }

        /// <summary>
        /// 投票された指し手をすべて破棄します。
        /// </summary>
        public void ClearVote()
        {
            using (LazyLock())
            {
                this.moveDatas.Clear();

                this.RaisePropertyChanged(nameof(MoveList));
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MoveStatistics()
        {
            /*var point = this.defaultSkillPoint;

            for (var grade = 15; grade >= 1; --grade)
            {
                var skillLevel = new SkillLevel(SkillKind.Kyu, grade);

                point += 2;
                SetSkillPoint(skillLevel, point);
            }

            for (var grade = 1; grade < 10; ++grade)
            {
                var skillLevel = new SkillLevel(SkillKind.Dan, grade);

                point += 2;
                SetSkillPoint(skillLevel, point);
            }*/
        }
    }
}
