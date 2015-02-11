#!/usr/local/bin/ruby -Ku
#
# Ragnarok.Updateに使う更新ファイルなどを作成します。
#

require 'rubygems'
require 'yaml'
require 'fileutils'
require 'image_size'
require 'digest/md5'
require 'rexml/document'
require 'zip'

$LocalImagePath = "E:/HP/garnet-alice.net/common/images"

#$HtmlImagePath = "http://garnet-alice.net/common/images/"
#$HtmlImagePath = "E:/HP/garnet-alice.net/common/images"
$HtmlImagePath = "/assets/img/thumbnail/"

#
# ファイル又はディレクトリを削除するメソッドを作成
#
def deleteall(delthem)
  if FileTest.directory?(delthem) then  # ディレクトリかどうかを判別
    Dir.foreach( delthem ) do |file|    # 中身を一覧
      next if /^\.+$/ =~ file           # 上位ディレクトリと自身を対象から外す
      deleteall(delthem.sub(/\/+$/,"") + "/" + file)
    end
    Dir.rmdir(delthem) rescue ""        # 中身が空になったディレクトリを削除
  else
    if File.exists?(delthem)
      File.delete(delthem)              # ディレクトリでなければ削除
    end
  end
end

#
# ファイルのMD5を計算します。
#
def compute_md5(filename)
  # binary mode必須
  File.open(filename, "rb") do |f|
    s = f.read
    
    return Digest::MD5.hexdigest(s)
  end
end

#
# サーバー上のサムネを一つ、バージョンと日付から選択します。
#
def select_thumbnail(rdata)
  return rdata.thumbnail if rdata.thumbnail != nil
  
  image_path = File.join($LocalImagePath, "*")
  filelist = Dir.glob(image_path).
    map do |path| File.basename(path) end.
    sort
    
  # md5から一意な画像を選択します。
  md5 = Digest::MD5.new()
  md5.update(rdata.version)
  md5.update(rdata.date)
  message = md5.digest
  
  digest = 0
  message.bytes do |b|
    digest += b
  end
  
  filelist[digest % filelist.length]
end

#
# ローカルのサムネイルのフルパスを取得します。
#
def local_thumbnail_fullpath(filename)
  File.join($LocalImagePath, filename)
end

#
# サーバー上のサムネイルのフルパスを取得します。
#
def remote_thumbnail_fullpath(filename)
  File.join($HtmlImagePath, filename)
end

#
# 画像のサイズを取得します。
#
def get_image_size(filename)
  image_path = local_thumbnail_fullpath(filename)

  File::open(image_path, "rb") do |f|
    return ImageSize.new(f.read)
  end
end

#
# 各リリース情報を保持します。
#
class ReleaseData
  attr_accessor :version, :thumbnail, :date, :content

  def initialize(node)
    @version = node["version"]

    @thumbnail = node["thumbnail"]
    @date = node["date"]
    @content = node["content"]
  end

  # infoタグをhtml表示用に変換したcontentを取得します。
  def html_content()
    root = REXML::Element.new("ul")

    @content.each do |info_node|
      new_node = REXML::Element.new("li")

      if info_node.instance_of?(String)
        # 文字列のみの場合は、それを直接設定。
        new_node.text = info_node
      else
        pair = info_node.first

        # infoのタイプ名です。
        text = "(" + pair[0] + ") "

        case pair[0]
        when "本体" then
          new_node.add_element("span", {"class" => "main"}).add_text(text)
        when "将棋" then
          new_node.add_element("span", {"class" => "shogi"}).add_text(text)
        when "バグ" then
          new_node.add_element("span", {"class" => "bugfix"}).add_text(text)
        else
          new_node.add_element("span", {"class" => "unknown"}).add_text(text)
        end

        new_node.add_text(pair[1])
      end
      root.add_element(new_node)
    end
    
    root
  end
end

#
# アプリのタイトルやバージョン情報を保持します。
#
# dist_path上にビルドファイル、zip、更新ファイルなどを作成します。
#
class AppData
  attr_accessor :html_dir, :dist_path, :title, :version, :release_list
  attr_accessor :outdir_name, :outdir_path, :zip_name, :zip_path
  attr_accessor :versioninfo_path, :releasenote_path, :template_path
  
  def initialize(html_dir, dist_path, assemblyinfo_path, history_path)
    @html_dir = html_dir
    @title = load_title(assemblyinfo_path)
    @version = load_version(assemblyinfo_path)
    @release_list = load_history(history_path) if history_path != nil

    @dist_path = dist_path
    @outdir_name = "#{@title}_#{version_}"
    @outdir_path = File.join(dist_path, outdir_name)
    @zip_name = outdir_name + ".zip"
    @zip_path = File.join(dist_path, zip_name)
    
    @versioninfo_path = File.join(dist_path, "versioninfo.xml")
    @releasenote_path = File.join(dist_path, "release_note.html")
    @template_path = File.dirname(__FILE__)
  end

  #
  # _で区切られたバージョン情報を取得します。
  #
  def version_()
    @version.gsub(".", "_")
  end

  #
  # クライアントのタイトルを取得します。
  #
  def load_title(assemblyinfo_path)
    str = File.open(assemblyinfo_path).read
  
    if /\[assembly: AssemblyTitle\("(.+)"\)\]/ =~ str then
      version = $1
    else
      puts "Not found client's assembly title."
      exit(-1)
    end
  end
  
  #
  # クライアントのバージョンを取得します。
  #
  def load_version(assemblyinfo_path)
    str = File.open(assemblyinfo_path).read
  
    if /\[assembly: AssemblyVersion\("([\d\.]+)"\)\]/ =~ str then
      version = $1
    else
      puts "Not found client's assembly version."
      exit(-1)
    end
  end
  
  #
  # 更新履歴を読み込みます。
  #
  def load_history(history_path)
    history = YAML.load_file(history_path)

    history.map do |node|
      ReleaseData.new(node)
    end
  end
  
  #
  # プロジェクトのリビルドを行います。
  #
  def build(solution_path, constants, options)
    # ディレクトリが存在したら削除します。
    if File.exists?(@outdir_path)
      deleteall(@outdir_path)
    end
    
    # ビルドコマンドを実行します。
    path_command = "call \"%VS120COMNTOOLS%vsvars32.bat\""
    build_command =
      "msbuild /nologo \"#{solution_path}\" /t:Rebuild " +
      "/p:DefineConstants=\"#{constants}\" " +
      "/p:OutputPath=\"#{@outdir_path}\" /p:Configuration=Release " +
      (options ? options : "")
    
    puts build_command
    system(path_command + " && " + build_command)
  end
  
  #
  # templateファイルを置き換えます。
  #
  def convert_template(input_path, output_path)
    data = File.open(input_path).read
    data = data.gsub("${DIRNAME}", @html_dir)
    data = data.gsub("${VERSION}", @version)
    data = data.gsub("${_VERSION}", version_)
    data = data.gsub("${ZIP_FILE}", @zip_name)
    data = data.gsub("${MD5}", compute_md5(@zip_name))
    data = data.gsub("${PUB_DATE}", Time.now.to_s)
    data = data.gsub("${RAND}", rand(100000).to_s)
    
    File.open(output_path, "w") do |f|
      f.write(data)
      printf("wrote %s\n", output_path)
    end
  end
  
  #
  # zipファイルを作成します。
  #
  def make_zip()
    deleteall(@zip_path)

    printf("begin to make zip: %s\n", @zip_path)

    Zip::Archive.open(@zip_path, Zip::CREATE) do |ar|
      Dir::chdir(@outdir_path) do
        Dir.glob("**/*") do |file|
          if File.directory?(file)
            ar.add_dir(file)
          else
            ar.add_file(file, file)
          end
        end
      end
    end
  end
  
  #
  # zipファイルを作成します。(コマンドライン版)
  #
  def make_zip_()
    deleteall(@zip_path)

    Dir::chdir(@outdir_path) do
      file_str = (Dir.glob("*").map do |name|
        '"' + name + '"'
      end).join(' ')
      
      zip_command = "zip -r -q \"#{@zip_path}\" " + files_str
      system(zip_command)
    end
  end
  
  #
  # versioninfo.xmlを更新します。
  #
  def make_versioninfo()
    tmpl_path = File.join(@template_path, "versioninfo_tmpl.xml")

    convert_template(tmpl_path, @versioninfo_path)
  end
  
  def contains_class(klass)
    "contains(concat(' ',@class,' '), ' #{klass} ')"
  end

  #
  # リリース情報(html)を出力します。
  #
  def make_release_note()
    #tmpl_path = File.join(@template_path, "release_note_tmpl.html")
    tmpl_path = File.join(@template_path, "history.html.erb.tmpl")

    # release_note.html のテンプレートファイルを読み込みます。
    fp = File.open(tmpl_path, "r")
    doc = REXML::Document.new(fp)
    
    top = doc.elements["//*[#{contains_class('history-root')}]"]
    rnote_templ = top.elements["//*[#{contains_class('history')}]"]
    
    # delete_allは実体を消すためコピーが必要になります。
    rnote_clone = rnote_templ.deep_clone
    top.elements.delete_all("*")
    
    @release_list.each_with_index do |rdata, i|
      break if i >= 3
      note_node = rnote_clone.deep_clone
      
      if i == 0 and rdata.version != @version
        raise StandardError, 'Release version isn\'t matched (history.xml)'
      end
      
      # header
      node = note_node.elements["//*[#{contains_class('history-version')}]"]
      node.text = rdata.version
      
      # thumbnail
      node = note_node.elements["//*[#{contains_class('history-thumbnail')}]"]
      filename = select_thumbnail(rdata)
      image_size = get_image_size(filename)
      attrs = {
        'src' => remote_thumbnail_fullpath(File.basename(filename)),
        'alt' => '更新情報',
        'class' => 'history-image',
        'width' => image_size.width,
        'height' => image_size.height,
      }
      node.add_element("img", attrs)
      select_thumbnail(rdata)
      
      # date
      node = note_node.elements["//*[#{contains_class('history-date')}]"]
      node.text = (rdata.date ? rdata.date : " ")
      
      # content
      node = note_node.elements["//*[#{contains_class('history-content')}]"]
      node.add_element(rdata.html_content)
      
      top.add_element(note_node)
    end
    
    File.open(@releasenote_path, "w") do |f|
      doc.write(f, -1, false, true)
    end
  end
end
