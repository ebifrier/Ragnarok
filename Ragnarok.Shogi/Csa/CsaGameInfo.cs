using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.Csa
{
    /// <summary>
    /// CSA将棋サーバーから送られる対局情報を管理します。
    /// </summary>
    public sealed class CsaGameInfo
    {
        /*private static readonly string BeginGameSummary = "BEGIN Game_Summary";
        private static readonly string EndGameSummary = "END Game_Summary";*/

        /// <summary>
        /// Protocol_Version:(バージョン)
        /// </summary>
        private static readonly Regex ProtocolVersionRegex = new Regex(
            @"Protocol_Version:\s*([\d._\-]+)$");
        /// <summary>
        /// Protocol_Mode:(Server|Direct|無し)
        /// </summary>
        private static readonly Regex ProtocolModeRegex = new Regex(
            @"Protocol_Mode:\s*([\w]+)?$");
        /// <summary>
        /// Format:(Shogi 1.0 などで必須)
        /// </summary>
        private static readonly Regex FormatRegex = new Regex(
            @"Format:\s*([\w\d .]+)$");
        /// <summary>
        /// Declaration:(Jishogi 1.1 など)
        /// </summary>
        private static readonly Regex DeclarationRegex = new Regex(
            @"Declaration:\s*([\w\d .]+)?$");
        /// <summary>
        /// Game_ID:(20060505-CSA14-3-5-7 など)
        /// </summary>
        private static readonly Regex GameIdRegex = new Regex(
            @"Game_ID:\s*([\w\d .\-]+)?$");
        /// <summary>
        /// Name+:(foobar などで必須)
        /// </summary>
        private static readonly Regex BlackNameRegex = new Regex(
            @"Name\+:\s*([\w\d_\-]+)$");
        /// <summary>
        /// Name-:(foobar などで必須)
        /// </summary>
        private static readonly Regex WhiteNameRegex = new Regex(
            @"Name\-:\s*([\w\d_\-]+)$");
        /// <summary>
        /// Your_Turn:(+|-)
        /// </summary>
        private static readonly Regex YourTurnRegex = new Regex(
            @"Your_Turn:\s*(\+|\-)$");
        /// <summary>
        /// Rematch_On_Draw:NO
        /// </summary>
        private static readonly Regex RematchOnDrawRegex = new Regex(
            @"Rematch_On_Draw:\s*(YES|NO)?$");
        /// <summary>
        /// To_Move:(+|-)
        /// </summary>
        private static readonly Regex ToMoveRegex = new Regex(
            @"To_Move:\s*(\+|\-)$");

        /// <summary>
        /// Time_Unit:(1sec など)
        /// </summary>
        private static readonly Regex TimeUnitRegex = new Regex(
            @"Time_Unit:\s*(?:([\d]+)(sec|min))?$");
        /// <summary>
        /// Least_Time_Per_Move:(1 など)
        /// </summary>
        private static readonly Regex LeastTimePerMoveRegex = new Regex(
            @"Least_Time_Per_Move:\s*([\d]+)?$");
        /// <summary>
        /// Time_Roundup:(YES|NO|なし)
        /// </summary>
        private static readonly Regex TimeRoundupRegex = new Regex(
            @"Time_Roundup:\s*(YES|NO)?$");
        /// <summary>
        /// Total_Time:(1500c など)
        /// </summary>
        private static readonly Regex TotalTimeRegex = new Regex(
            @"Total_Time:\s*([\d]+)?$");
        /// <summary>
        /// Byoyomi:(60 など)
        /// </summary>
        private static readonly Regex ByoyomiRegex = new Regex(
            @"Byoyomi:\s*([\d]+)?$");

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CsaGameInfo()
        {
            ProtocolMode = "Server";
            Declaration = string.Empty;
            GameId = string.Empty;

            TimeUnit = TimeSpan.FromSeconds(1.0);
            TimeRoundup = false;
            LeastTimePerMove = TimeSpan.Zero;
            TotalTime = TimeSpan.Zero;
            Byoyomi = TimeSpan.Zero;
        }

        public string ProtocolVersion
        {
            get;
            set;
        }

        public string ProtocolMode
        {
            get;
            set;
        }

        public string Format
        {
            get;
            set;
        }

        public string Declaration
        {
            get;
            set;
        }

        public string GameId
        {
            get;
            set;
        }

        public string BlackName
        {
            get;
            set;
        }

        public string WhiteName
        {
            get;
            set;
        }

        public BWType MyTurn
        {
            get;
            set;
        }

        public BWType BeginTurn
        {
            get;
            set;
        }

        public TimeSpan TimeUnit
        {
            get;
            set;
        }

        public bool TimeRoundup
        {
            get;
            set;
        }

        public TimeSpan LeastTimePerMove
        {
            get;
            set;
        }

        public TimeSpan TotalTime
        {
            get;
            set;
        }

        public TimeSpan Byoyomi
        {
            get;
            set;
        }

        public bool IsGameSummaryEnded
        {
            get;
            set;
        }

        private bool isInPosition;

        /// <summary>
        /// 対局情報を解釈します。
        /// </summary>
        /// <returns>
        /// 未解釈のまま残った文字列を返します。
        /// </returns>
        public string Parse(string str)
        {
            using (var reader = new StringReader(str))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        return null;
                    }

                    if (!ParseLine(line))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// 文字列オプションを与える行を解釈します。
        /// </summary>
        private static string ParseString(Regex re, string line,
                                          string defaultValue = "")
        {
            var m = re.Match(line);
            if (!m.Success)
            {
                return null;
            }

            return (m.Groups[1].Success ? m.Groups[1].Value : defaultValue);
        }

        /// <summary>
        /// 数値オプションを与える行を解釈します。
        /// </summary>
        private static int? ParseNumber(Regex re, string line,
                                        int defaultValue)
        {
            var m = re.Match(line);
            if (!m.Success)
            {
                return null;
            }

            if (!m.Groups[1].Success)
            {
                return defaultValue;
            }

            return int.Parse(m.Groups[1].Value);
        }

        /// <summary>
        /// YES|NOオプションを与える行を解釈します。
        /// </summary>
        private static bool? ParseBool(Regex re, string line,
                                       bool defaultValue)
        {
            var m = re.Match(line);
            if (!m.Success)
            {
                return null;
            }

            if (!m.Groups[1].Success)
            {
                return defaultValue;
            }

            var c = m.Groups[1].Value;
            return (
                c == "YES" ? true :
                c == "NO" ? false :
                defaultValue);
        }

        /// <summary>
        /// 手番オプションを与える行を解釈します。
        /// </summary>
        private static BWType? ParseTurn(Regex re, string line,
                                         BWType defaultValue = BWType.None)
        {
            var m = re.Match(line);
            if (!m.Success)
            {
                return null;
            }

            if (!m.Groups[1].Success)
            {
                return defaultValue;
            }

            var c = m.Groups[1].Value;
            return (
                c == "+" ? BWType.Black :
                c == "-" ? BWType.White :
                BWType.None);
        }

        /// <summary>
        /// Unit_Time行を解釈します。
        /// </summary>
        private static TimeSpan? ParseTimeUnit(Regex re, string line,
                                               TimeSpan defaultValue)
        {
            var m = re.Match(line);
            if (!m.Success)
            {
                return null;
            }

            if (!m.Groups[1].Success || !m.Groups[2].Success)
            {
                return defaultValue;
            }

            // Unit_Timeは 1min や 2sec などの形式です。
            var n = int.Parse(m.Groups[1].Value);
            var u = (
                m.Groups[2].Value == "sec" ? 1 :
                m.Groups[2].Value == "min" ? 60 :
                1);
            return TimeSpan.FromSeconds(n * u);
        }

        public bool ParseLine(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return false;
            }

            if (line.EndsWith("\n"))
            {
                line = line.Remove(line.Length - 1);
            }

            if (line == "BEGIN Game_Summary" ||
                line == "BEGIN Time" ||
                line == "END Time")
            {
                return true;
            }

            if (line == "END Game_Summary")
            {
                IsGameSummaryEnded = true;
                return true;
            }

            if (line == "BEGIN Position")
            {
                this.isInPosition = true;
                return true;
            }

            if (line == "END Position")
            {
                this.isInPosition = false;
                return true;
            }

            var result = ParseString(ProtocolVersionRegex, line);
            if (result != null)
            {
                ProtocolVersion = result;
                return true;
            }

            result = ParseString(ProtocolModeRegex, line, "Server");
            if (result != null)
            {
                ProtocolMode = result;
                return true;
            }

            result = ParseString(FormatRegex, line);
            if (result != null)
            {
                Format = result;
                return true;
            }

            result = ParseString(DeclarationRegex, line, string.Empty);
            if (result != null)
            {
                Declaration = result;
                return true;
            }

            result = ParseString(GameIdRegex, line, string.Empty);
            if (result != null)
            {
                GameId = result;
                return true;
            }

            result = ParseString(BlackNameRegex, line, string.Empty);
            if (result != null)
            {
                BlackName = result;
                return true;
            }

            result = ParseString(WhiteNameRegex, line, string.Empty);
            if (result != null)
            {
                WhiteName = result;
                return true;
            }

            var turn = ParseTurn(YourTurnRegex, line);
            if (turn != null)
            {
                MyTurn = turn.Value;
                return true;
            }

            var flag = ParseBool(RematchOnDrawRegex, line, false);
            if (flag != null)
            {
                return true;
            }

            turn = ParseTurn(ToMoveRegex, line);
            if (turn != null)
            {
                BeginTurn = turn.Value;
                return true;
            }

            var span = ParseTimeUnit(TimeUnitRegex, line, TimeSpan.FromSeconds(1));
            if (span != null)
            {
                TimeUnit = span.Value;
                return true;
            }

            var value = ParseNumber(LeastTimePerMoveRegex, line, 0);
            if (value != null)
            {
                LeastTimePerMove = TimeSpan.FromSeconds(value.Value);
                return true;
            }

            flag = ParseBool(TimeRoundupRegex, line, false);
            if (flag != null)
            {
                TimeRoundup = flag.Value;
                return true;
            }

            value = ParseNumber(TotalTimeRegex, line, 0);
            if (value != null)
            {
                TotalTime = TimeSpan.FromSeconds(
                    value.Value * TimeUnit.TotalSeconds);
                return true;
            }

            value = ParseNumber(ByoyomiRegex, line, 0);
            if (value != null)
            {
                Byoyomi = TimeSpan.FromSeconds(
                    value.Value * TimeUnit.TotalSeconds);
                return true;
            }

            // Position階層の情報は無視します。
            return this.isInPosition;
        }
    }
}
