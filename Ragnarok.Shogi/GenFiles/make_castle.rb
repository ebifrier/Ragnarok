#!/usr/bin/ruby -Ku
#
# 囲い情報をC#のソースに変換します。
#

require 'yaml'
require 'kconv'

#
# utf8からsjisに変換します。
#
def tosjis(str)
  str.kconv(Kconv::SJIS, Kconv::UTF8)
end

#
# 駒の位置情報などを保持します。
#
class Piece
  attr_accessor :type, :file, :rank
  
  def initialize(pair)
    pos = pair[1].split(/\s*,\s*/)

    @type = pair[0]
    @file = pos[0].to_i
    @rank = pos[1].to_i
  end
end

#
# 囲い情報を保持します。
#
class Castle
  attr_accessor :name, :piece_list, :base_list
  attr_accessor :priority, :updated

  def initialize(node)
    @piece_list = []
    node["piece_list"].each do |hash|
      pair = hash.first
      
      @piece_list.push(Piece.new(pair))
    end

    @name = node["name"]
    @base_list = []
    @priority = 0
    @updated = false
  end

  #
  # 基本となる囲いを追加します。
  #
  def add_base(base_castle)
    @base_list.push(base_castle)
  end

  #
  # 定義ソースを返します。
  #
  def definition()
    str =  "new CastleInfo(\n"
    str << "    \"#{@name}\", #{@priority},\n"
    str << "    new []\n"
    str << "    {\n"
    
    @piece_list.each do |piece|
      str << "        new CastlePiece(PieceType.#{piece.type}, new Position(#{piece.file}, #{piece.rank})),\n"
    end

    str << "    },\n"
    str << "    new string[] { "

    @base_list.each do |base_castle|
      str << "\"#{base_castle.name}\", "
    end

    str << "}),"
    str
  end
end

#
# 囲いデータを読み込みます。
#
def load_castle_list()
  raw_castle_list = YAML.load_file("castle_list.yaml")

  raw_castle_list.map do |node|
    Castle.new(node)
  end
end

#
# 囲い同士の関係を判定します。
#
def is_base_castle(castle, base_castle)
  return false if base_castle.piece_list.size >= castle.piece_list.size
  
  base_castle.piece_list.each do |piece|
    index = castle.piece_list.index do |comp|
      piece.type == comp.type and
      piece.file == comp.file and
      piece.rank == comp.rank
    end

    return false if index == nil
  end
  
  return true
end

#
# 基本囲いを更新します。
#
def update_base_castle(castle, castle_list)
  castle_list.each do |base_castle|
    castle.add_base(base_castle) if is_base_castle(castle, base_castle)
  end
end

#
# 基本囲いを更新します。
#
def update_castle(castle)
  return if castle.updated

  castle.base_list.each do |base_castle|
    update_castle(base_castle)

    # 優先順位を更新
    pri = base_castle.priority + 10
    castle.priority = pri if castle.priority < pri
  end

  castle.updated = true
end

#
# 定義リストをソースに直します。
#
def make_castle_definition_list(castle_list, indent_num)
  indent = " " * indent_num

  (castle_list.map do |c|
    c.definition
  end).join("\n\n").gsub("\n", "\n" + indent)
end

castle_list = load_castle_list()

castle_list.each do |castle|
  update_base_castle(castle, castle_list)
end
castle_list.each do |castle|
  update_castle(castle)
end

i = 0
castle_list = castle_list.sort_by do |x|
  [-x.priority, i += 1]
end

fp = File.open("../CastleInfoTable.cs", "w")

fp.print <<EOD
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
                #{make_castle_definition_list(castle_list, 16)}
            };
    }
}
EOD
fp.close
