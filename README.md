# JankenGame

Blazor Webアプリケーションで実装された、ジャンケンとブラックジャックのゲームです。

## 概要

このプロジェクトは、ASP.NET Core 9.0とBlazor Interactive Server Renderingを使用したウェブアプリケーションです。複数のカジュアルゲームをプレイできます。

## 機能

- **ジャンケンゲーム**: コンピューターとのジャンケン対戦
- **ブラックジャック**: 21ゲーム（ブラックジャック）

## プロジェクト構成

```
Components/
├── Pages/              # ゲームページコンポーネント
│   ├── Home.razor      # ホームページ
│   ├── Janken.razor    # ジャンケンゲームページ
│   └── Blackjack.razor # ブラックジャックゲームページ
├── Layout/             # レイアウトコンポーネント
│   ├── MainLayout.razor
│   └── NavMenu.razor
└── App.razor           # ルートコンポーネント

Models/
├── Janken/            # ジャンケン関連モデル
│   ├── JankenHand.cs
│   ├── JankenPlayer.cs
│   ├── JankenRecord.cs
│   └── JankenResult.cs
└── BlackJack/         # ブラックジャック関連モデル
    ├── Card.cs
    ├── Deck.cs
    └── Hand.cs
```

## 技術スタック

- **フレームワーク**: ASP.NET Core 9.0
- **UI**: Blazor Interactive Server Rendering
- **言語**: C# 13

## セットアップ方法

### 前提条件

- .NET 9.0 SDK

### インストール

1. リポジトリをクローンします
   ```bash
   git clone <repository-url>
   ```

2. プロジェクトディレクトリに移動します
   ```bash
   cd JankenGame
   ```

3. プロジェクトを復元してビルドします
   ```bash
   dotnet restore
   dotnet build
   ```

## 実行方法

アプリケーションを実行するには、以下のコマンドを使用します：

```bash
dotnet run
```

または開発環境では：

```bash
dotnet watch
```

アプリケーションは `https://localhost:5001` にてアクセスできます。

## 開発

### プロジェクト設定

設定ファイルは以下の通りです：

- `appsettings.json` - 本番環境設定
- `appsettings.Development.json` - 開発環境設定

### ビルド

```bash
dotnet build
```

### 公開

```bash
dotnet publish -c Release
```

## ファイル構成

- `Program.cs` - アプリケーションエントリーポイント
- `JankenGame.csproj` - プロジェクトファイル
- `JankenGame.sln` - ソリューションファイル
- `wwwroot/` - 静的アセット

## ライセンス

このプロジェクトは自由に利用できます。

## 作成者

@fdshg693
