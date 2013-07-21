#!/usr/local/bin/ruby -Ku
#
# WeakEventHandlerのEventArgsを消去し、
# EventHandler型で使えるソースを作成します。
#

def modify_content(text)
  text.gsub!(/<TEventArgs>/, "")
  text.gsub!(/\s*where TEventArgs : EventArgs/, "")
  text.gsub!(/, TEventArgs>/, ">")
  text.gsub!(/TEventArgs/, "EventArgs")
  
  text
end

text = File.open("WeakEventHandler.cs").read

output = "WeakEventHandler.basic.cs"
File.open(output, "w") do |fp|
  fp.puts modify_content(text)
end
