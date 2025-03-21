# PMX Reader for Unity

PMX Reader は、Unity で PMX モデルを読み込むためのツールであり、PMX ファイルの読み込みと解析を簡素化することを目的としています。MMD（MikuMikuDance）モデルをインポートおよび操作する必要がある開発者に適しています。

## 現在のバージョン

**バージョン：1.0.0**

## 機能

- PMX 形式のモデルの読み込みに対応。
- 頂点、法線、マテリアル、ボーンなどのデータを抽出。
- Unity の Mesh およびマテリアルシステムと統合。
- 拡張性と統合性が高い。

## インストール方法

Unity Package Manager を使用してこのツールをインポートします。

### Git URL を使用してパッケージをインポート

1. Unity の `Package Manager` を開きます。
2. 左上の `+` ボタンをクリックします。
3. `Add package from git URL...` を選択します。
4. 次の URL を入力します：
   ```plaintext
   https://github.com/ZSaltedFish/PmxReader.git?path=Assets/pmxreader/package.json
   ```
