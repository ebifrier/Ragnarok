#!/usr/local/bin/ruby

#
# 与えられたファイルの一部分を切り出します。
#
def get_copy_string(input_file)
  in_copy_line = 0
  copy_lines = ""

  File.open(input_file).each_line do |line|
    if /^\s*#endregion copy to DynamicViewModel/ =~ line
      in_copy_line -= 1
    end
    
    if in_copy_line > 0
      copy_lines << line
    end
    
    if /^\s*#region copy to DynamicViewModel/ =~ line
      in_copy_line += 1
    end
  end

  copy_lines
end

#
# 実際にファイルに出力する内容を取得します。
#
def output_content(copy_lines)
  str =<<EOD
#if CLR_V4
#define RGN_DYNAMICVIEWMODEL
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Runtime.Serialization;

// DynamicViewModelの一部です。
// このファイルは自動生成されています。

namespace Ragnarok.ObjectModel
{
    using Ragnarok.Utility;

    public partial class DynamicViewModel
    {
#{copy_lines}    }
}
#endif
EOD
end

#
# ModelExtensionsのソースを変換します。
#
def modify_extension_content(copy_lines)
  copy_lines.gsub!(/(public|private) static/, "\\1")
  copy_lines.gsub!(/this IModel self,?\s*/, "")
  copy_lines.gsub!(/this ILazyModel self,?\s*/, "")
  copy_lines.gsub!(/this IParentModel self,?\s*/, "")
  copy_lines.gsub!(/lazyModel,\s*/, "")
  copy_lines.gsub!("self", "this")
  copy_lines.gsub!("lock (this)", "using (new DebugLock(SyncRoot))")
  copy_lines.gsub!("LazyModelObject", "lazyModelObject")
  copy_lines.gsub!("DependModelList", "dependModelList")
  
  copy_lines
end

#
# NotifyObjectのソースを変換します。
#
def modify_notifyobject_content(copy_lines)
#  copy_lines.gsub!(/(public|private) static/, "\\1")
#  copy_lines.gsub!("LazyModelObject", "lazyModelObject")#
#  copy_lines.gsub!("self", "this")
#  copy_lines.gsub!("lock (this)", "using (new DebugLock(SyncRoot))")
  
  copy_lines
end

output = "DynamicViewModel.Extensions.cs"
copy_lines = get_copy_string("ModelExtensions.cs")
copy_lines = modify_extension_content(copy_lines)

File.open(output, "w") do |fp|
  fp.puts output_content(copy_lines)
end

output = "DynamicViewModel.NotifyObject.cs"
copy_lines = get_copy_string("NotifyObject.cs")
copy_lines = modify_notifyobject_content(copy_lines)

File.open(output, "w") do |fp|
  fp.puts output_content(copy_lines)
end
