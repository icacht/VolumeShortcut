# Volume Shortcut Key Hooker
VolumeUp/Downキーのショートカットを設定するWindows用常駐ソフトウェアです.

## 使用方法
VolumeShortcut.exeを起動すると常駐を始めます。常駐している間, 設定したキーの入力をVolumeUp/Downキーの入力に置き換えます.

### キーの設定
タスクトレイ上のVolume Shortcutアイコンを右クリックしSettingを選択, もしくはアイコンをダブルクリックします.
開いたSettingウインドウでVolumeUp/Downそれぞれの入力に対応させたいキーを入力します.
その後, Applyボタンを押すことで設定が適用されます。

キーはSystem.Windows.Input.Key列挙子の名称で指定してください。
例として, F8キーを設定する場合は`F8`としてください。
Shift, Ctrl, Altのチェックボックスにチェックを付けることにより, 同時押しが設定されます。

キーの設定はexeファイルと同階層に作成されるSetting.jsonに保存されます。

### 終了方法
タスクトレイ上のVolume Shortcutアイコンを右クリックし, Exitを選択します.

## ビルド
.NET Coreを使用してビルドします.

### 必要環境
* .NET Core 3.1

### 手順
1. リポジトリをクローンする.
1. assets/に16x16サイズのアイコンファイルをicon.icoとして置く.
アイコンファイルはお好みのものを使用してください.
1. リポジトリルートで`dotnet build -c Release`を実行する.

## LICENSE
This software is released under the MIT License, see LICENSE.