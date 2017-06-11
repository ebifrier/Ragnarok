using System;
namespace Codehaus.Parsec
{
    using System.Collections.Generic;

    /// <summary> This class gives the default behavior of reporting parser errors. </summary>
    /// <author>  Ben Yu
    /// 
    /// Nov 18, 2004
    /// </author>
#if !NETSTANDARD
    [Serializable]
#endif
    sealed class DefaultShowError
    {
        internal static string Show(ParseError err, Pos pos)
        {
            return toErrorStr(err, pos);
        }
        static string toErrorStr(ParseError err, Pos? pos)
        {
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (pos.HasValue)
                buf.Append("line " + pos.Value.Line + ", column " + pos.Value.Column);
            if (err != null)
            {
                buf.Append(":\n");
                showExpecting(buf, err.getExpecting());
                showUnexpected(buf, err.getUnexpected());
                showMessages(buf, err.getMessages());
                showEncountered(buf, err.getEncountered());
            }
            return buf.ToString();
        }
        static void showEncountered(System.Text.StringBuilder buf, string s)
        {
            if (s == null)
                return;
            buf.Append(s).Append(" encountered.\n");
        }
        static string[] Dedup(string[] msgs)
        {
            return Misc.Dedup(msgs, Comparer<string>.Default);
        }
        /*
        static string[] unique (string[] msgs) {
          SupportClass.TreeSetSupport set_Renamed = new SupportClass.TreeSetSupport ((msgs));
          string[] umsgs = new string[set_Renamed.Count];
          SupportClass.ICollectionSupport.ToArray (set_Renamed, umsgs);
          return umsgs;
        }*/
        static void showList(System.Text.StringBuilder buf, string[] msgs)
        {
            if (msgs.Length == 0)
                return;
            for (int i = 0; i < msgs.Length - 1; i++)
            {
                buf.Append(msgs[i]).Append(' ');
            }
            if (msgs.Length > 1)
                buf.Append("or ");
            buf.Append(msgs[msgs.Length - 1]);
        }
        static void showExpecting(System.Text.StringBuilder buf, string[] msgs)
        {
            if (msgs == null || msgs.Length == 0)
                return;
            string[] umsgs = Dedup(msgs);
            buf.Append("expecting ");
            showList(buf, umsgs);
            buf.Append(".\n");
        }
        static void showUnexpected(System.Text.StringBuilder buf, string[] msgs)
        {
            if (msgs == null || msgs.Length == 0)
                return;
            showList(buf, Dedup(msgs));
            buf.Append(" unexpected.\n");
        }
        static void showMessages(System.Text.StringBuilder buf, string[] msgs)
        {
            if (msgs == null || msgs.Length == 0)
                return;
            buf.Append(msgs[0]);
            for (int i = 1; i < msgs.Length; i++)
            {
                buf.Append(" or \n").Append(msgs[i]);
            }
            buf.Append("\n");
        }
        private DefaultShowError()
        {
        }
    }
}
