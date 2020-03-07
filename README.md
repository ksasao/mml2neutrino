# mml2neutrino
MML (Music Macro Language) for NEUTRINO

AIきりたんをMMLで歌わせるやつです。
## アプリ
- [v0.4](https://github.com/ksasao/mml2neutrino/releases/download/v0.4/mml2neutrino_v0.4.zip) オプション引数で WORLD のパラメータを含む様々な設定ができるようになりました([リリースノート](https://github.com/ksasao/mml2neutrino/releases/tag/v0.4))。
- [v0.3](https://github.com/ksasao/mml2neutrino/releases/download/v0.3/mml2neutrino_v0.3.zip) 十六分音符および八分音符三連符までの長さに対応。一度生成した歌をキャッシュする機構を追加。エラーチェックの強化。([リリースノート](https://github.com/ksasao/mml2neutrino/releases/tag/v0.3))
- [v0.2](https://github.com/ksasao/mml2neutrino/files/4251015/mml2neutrino_v0.2.zip) 細かな修正と機能追加。文法エラーメッセージの改善。([リリースノート](https://github.com/ksasao/mml2neutrino/releases/tag/v0.2))
- [v0.1](https://github.com/ksasao/mml2neutrino/files/4243814/v0.1.zip) とにかく試したい方向け。α版です。

## 書式
### 基本
(音階名)(シャープ → # または + /フラット → -)(長さ)(歌詞)のように続けて入力します。音階名と歌詞は必須です。例えば、

> C#4あ

のように入力すると ド#, 四分音符で「あ」と歌います。

|記号|意味|デフォルト値|例|
|---|---|---|---|
|L|デフォルトの音長(四分音符=4,八分音符=8など)を指定します。1,2,3,4,6,8,12,16,24,48 のみが指定できます。|L4|L8Cあ|
|.|付点 (設定された長さを1.5倍します。直前に数字が指定されていない場合は L で設定した値を引き継ぎます。)|-|G4.あ|
|C,D,E,F,G,A,B|音階(C=ド,D=レ,...,B=シ)|Lで指定した値を引き継ぎます|Cあ (「ド」の音程で「あ」と歌います)|
|R|休符|Lで指定した値を引き継ぎます。|L8R (八分休符)|
|O|オクターブ|O4|R2L2O5CドO4BシO5Cド|
|>|オクターブを一つ上げます。|-|R4CドEミGソ>C2ド|
|<|オクターブを一つ下げます。|-|R4O5Cド<GソEミC2ド|
|T|テンポを四分音符換算で指定します。|80|T120 (♩=120 に設定)|

## 使用方法
1. ```mml2neutrino.exe``` を NEUTRINO の Run.bat と同じフォルダにコピーしてください。
2. コマンドプロンプトで、NEUTRINOと同じフォルダに移動(```cd c:\Users\ksasao\Desktop\NEUTRINO``` などのように自分の環境にあわせて入力)し、```mml2neutrino.exe "任意のMML"``` で実行します。必ず```"``` (ダブルクォーテーション)で囲むようにしてください。

### オプション
|引数||意味|
|---|---|---|
|-i|--input|MMLが記載されたテキストファイル (UTF-8)|
|-o|--output|出力ファイル名 (*.wav)|
|-m|--model|モデル名 (省略時は KIRITAN)|
|-k|--keyshift|MML のキーを変更します。1 で半音高くなります。省略時は 0 です。 |
|-p|--pitchshift|ピッチを変更します。+1.0 で半音高くなります。省略時は 0.0 です。|
|-f|--formantshift|フォルマントを変更します。大きくすると声が高くなります。省略時は 1.0です。|
|-t|--threads|使用するスレッド数(省略時は最大スレッド数-1)|
|-s|--silent|自動的にファイルを再生しません|

### 実行例
```
mml2neutrino.exe "O4L4R4CあEさAだG2よ"
```
で、「朝だよー」と歌います。またその音声ファイル(output.wav)が生成されます。

```
mml2neutrino.exe -i "sample.txt" -o "sample.wav"
```
で、```sample.txt``` に記述された MML を利用して歌います。UTF-8形式のみサポートしています。また、歌唱ファイルとして、"sample.wav" を出力します。

```
mml2neutrino.exe "O4RCあDあEあ" -k 1 -p -1
```
内部的にMMLを "O4RC+あD+あFあ" のように半音キーを上げ(-k 1)、出力音声のピッチを半音下げ(-p -1)て元の音程に戻します。このようにすると歌い方が改善する場合があります(キー変更＋ピッチ変更ガチャ)。

## 調声
くろ州 歌声合成備忘録 [AIシンガー調声アイデア　NEUTRINOで使えるテクニック](https://km4osm.com/neutrino-idea/) に記載されているテクニックは概ね利用可能です。

## 制限事項
現時点では様々な制限事項があります。徐々に修正していきます。勢い大事。
- 1小節目の歌い始める直前には休符を挿入してください。これは音声の立ち上がりは(ブレスなど)実際の音符よりも早くから音が鳴るためです。
- 内部的には4/4拍子、四十八分音符単位で処理をしています。小節をまたがるような音の長さを指定するとおかしな動作をします。
- 三連符は　L の値を工夫することで実装できます。例えば、四分音符の三連符は L6 で指定します。