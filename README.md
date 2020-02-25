# mml2neutrino
MML (Music Macro Language) for NEUTRINO

## アプリ
- [v0.2](https://github.com/ksasao/mml2neutrino/files/4251015/mml2neutrino_v0.2.zip) 細かな修正と機能追加。文法エラーメッセージの改善。([リリースノート](https://github.com/ksasao/mml2neutrino/releases/tag/v0.2))
- [v0.1](https://github.com/ksasao/mml2neutrino/files/4243814/v0.1.zip) とにかく試したい方向け。α版です。

## 書式
- 「ドレミ」と音階の通りに歌わせる場合 ... "CドDレEミ"
- オクターブを指定する場合 __O(数字)__
- オクターブを1つ上げる __>__
- オクターブを1つ下げる __<__
- すべて四分音符で歌唱 ... "L4CドDレEミ"
- 最後だけ二分音符で歌唱 ... "L4CドDレE2ミ"
- テンポを ♩=120 に設定 "T120CドDレEミ"

## 使用方法
1. ```input.bat``` ```mml2neutrino.exe``` ```template.xml``` の3つのファイルを NEUTRINO の Run.bat と同じフォルダにコピーしてください。
2. コマンドプロンプトで、NEUTRINOと同じフォルダに移動(```cd c:\Users\ksasao\Desktop\NEUTRINO``` などのように自分の環境にあわせて入力)し、```mml2neutrino.exe "任意のMML"``` で実行します。必ず```"``` (ダブルクォーテーション)で囲むようにしてください。

## 実行例
```
mml2neutrino.exe "O4L4R4CあEさAだG2よ"
```
で、「朝だよー」と歌います。またその音声ファイル(output.wav)が生成されます。

## 制限事項
現時点では様々な制限事項があります。徐々に修正していきます。勢い大事。
- 1小節目の歌い始める直前には休符を挿入してください。これは音声の立ち上がりは(ブレスなど)実際の音符よりも早くから音が鳴るためです。
- 内部的には4/4拍子、八分音符単位で処理をしています。小節をまたがるような音の長さを指定するとおかしな動作をします。
- 多少エラーチェックはしていますが、予測できない結果になることがあります。
- 対応しているのは全音符、二分音符、四分音符、八分音符のみです。