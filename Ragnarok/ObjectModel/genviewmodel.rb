#!/usr/local/bin/ruby

def output_content(copy_lines)
  str =<<EOD
#if CLR_V4
#define RGN_DYNAMICVIEWMODEL
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;
using System.ComponentModel;
using System.Reflection;

// ViewModelBaseの一部です。
// このファイルはModelBaseから自動生成されています。

namespace Ragnarok.ObjectModel
{
    using Ragnarok.Utility;

    public partial class DynamicViewModel
    {
#{copy_lines}
    }
}

#endif
EOD
end

def modify_content(copy_lines)
  copy_lines.gsub!(/(public|private) static/, "\\1")
  copy_lines.gsub!(/this IModel self,?\s*/, "")
  copy_lines.gsub!(/this ILazyModel self,?\s*/, "")
  copy_lines.gsub!(/this IParentModel self,?\s*/, "")
  copy_lines.gsub!(/lazyModel,\s*/, "")
  copy_lines.gsub!("self", "this")
  copy_lines.gsub!("lock (this)", "using (new DebugLock(SyncRoot))")
  
  copy_lines
end

input = "ModelExtensions.cs"
output = "DynamicViewModel.part.cs"

in_copy_line = 0
copy_lines = ""

File.open(input).each_line do |line|
  if /^\s*#endregion/ =~ line
    in_copy_line -= 1
  end

  if in_copy_line > 0
    copy_lines << line
  end

  if /^\s*#region/ =~ line
    in_copy_line += 1
  end
end

copy_lines = modify_content(copy_lines)

outfp = File.open(output, "w")
outfp.puts output_content(copy_lines)
