# GitHub Copilot Instructions

このドキュメントは、JankenGameプロジェクトでGitHub Copilotを使用する際のガイドラインと推奨事項を記載しています。

## プロジェクト概要

JankenGameは、Blazor Interactive Server Renderingを使用したASP.NET Core 9.0のウェブアプリケーションです。ジャンケンとブラックジャックの2つのゲームが実装されています。

## コーディングスタイル

### C#

- **命名規則**
  - クラス名: PascalCase（例: `JankenPlayer`）
  - メソッド名: PascalCase（例: `GetHandResult`）
  - プロパティ名: PascalCase（例: `PlayerScore`）
  - ローカル変数: camelCase（例: `playerHand`）
  - 定数: UPPER_SNAKE_CASE（例: `MAX_CARDS`）

- **Null安全性**: Nullable参照型を有効にしているため、適切にnull許容型（`?`）を使用してください
- **言語機能**: C# 13の最新機能を積極的に使用してください

### Razor Components

- **ファイル名**: PascalCase（例: `Janken.razor`）
- **コンポーネント構造**
  - `@page` ディレクティブでルートを定義
  - `@code` ブロックで論理を実装
  - ステートフルなロジックはモデルクラスに分離
  - UIロジックはコンポーネントに記述

### CSS

- **スタイルシート**: 各Razorコンポーネントに対応した`*.razor.css`ファイルを使用
- **クラス名**: kebab-case（例: `game-board`）
- **レスポンシブ設計**: Bootstrap を利用

## ディレクトリ構成のルール

```
Components/
├── Pages/          # ルーティング対象のページコンポーネント
├── Layout/         # レイアウトコンポーネント
└── _Imports.razor  # グローバルインポート

Models/
├── Janken/         # ジャンケン関連モデル
└── BlackJack/      # ブラックジャック関連モデル
```

新機能追加時は、適切なディレクトリに配置してください。

## コード作成時の注意点

### 新機能追加

1. **モデル層**
   - `Models/` ディレクトリに対応するモデルクラスを作成
   - 単一責任原則に従う
   - ビジネスロジックをカプセル化

2. **Razorコンポーネント**
   - `Components/Pages/` に新しいゲームページを追加
   - `@page` でルートを定義
   - ナビゲーションメニューに自動的に反映されるよう設計

3. **スタイリング**
   - `*.razor.css` でコンポーネント固有のスタイルを定義
   - グローバルスタイルは `wwwroot/app.css` に記述

### ゲーム実装テンプレート

新しいゲームを追加する際は、以下の構造を参考にしてください：

```csharp
// Models/[GameName]/[GameName]Logic.cs
public class [GameName]Logic
{
    public bool ValidateMove(/* parameters */) { }
    public [GameName]Result ExecuteMove(/* parameters */) { }
}

// Components/Pages/[GameName].razor
@page "/[game-name]"
<PageTitle>[Game Name]</PageTitle>

@code {
    private [GameName]Logic gameLogic = new();
    // コンポーネントロジック
}
```

## アーキテクチャの原則

- **関心の分離**: UI、ロジック、データモデルを分離
- **再利用性**: 共通ロジックはコンポーネント化またはサービス化
- **テスト容易性**: ビジネスロジックは独立してテスト可能に

## パフォーマンス

- **レンダリング最適化**: 必要に応じて `@key` ディレクティブを使用
- **イベントハンドリング**: 不要なイベントバインディングは避ける
- **状態管理**: コンポーネント間の通信は親コンポーネント経由で

## セキュリティ

- **入力検証**: すべてのユーザー入力を検証
- **Antiforgery**: フォーム送信には `@antiforgery` を使用
- **機密情報**: 秘密鍵やAPIキーは `appsettings.json` に保存しない

## テスト推奨事項

- ビジネスロジッククラスはユニットテストを実装
- ゲームルールのエッジケースをテスト
- `Models/` 内のクラスは独立してテスト可能に設計

## ドキュメンテーション

- **XML コメント**: Publicなクラス、メソッドに記述
- **README**: 新機能追加時は更新
- **インラインコメント**: 複雑なロジックのみ記述

### XML コメント例

```csharp
/// <summary>
/// ジャンケンの手とコンピューターの手から結果を判定します
/// </summary>
/// <param name="playerHand">プレイヤーの手</param>
/// <param name="computerHand">コンピューターの手</param>
/// <returns>ゲーム結果</returns>
public JankenResult DetermineWinner(JankenHand playerHand, JankenHand computerHand)
{
    // 実装
}
```

## 参考リソース

- [Blazor ドキュメント](https://docs.microsoft.com/aspnet/core/blazor)
- [ASP.NET Core ドキュメント](https://docs.microsoft.com/aspnet/core)
- [C# ドキュメント](https://docs.microsoft.com/dotnet/csharp/)

## よくあるパターン

### ゲーム状態の管理

```csharp
@code {
    private GameState currentState = GameState.WaitingForInput;
    private GameResult? lastResult = null;

    private void OnPlayerMove(PlayerMove move)
    {
        // 入力検証
        // ゲームロジック実行
        // UI更新（StateHasChangedなど）
    }
}
```

### ゲーム結果の表示

- 結果を専用のコンポーネント内で表示
- スコアボードはコンポーネント間で共有可能な構造に
- 履歴管理は `JankenRecord` のような専用クラスで実装
