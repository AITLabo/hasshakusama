# 八尺様：レベルレイアウト・ガイドライン

広大で恐ろしい森を構築するための推奨レイアウトです。

## 1. 基本構成
- **フィールドサイズ**: 500x500 程度を推奨。
- **プレイヤー初期位置**: マップの端（例：250, 0, 0）
- **目的地（祠 - Shrine）**: マップの対角線上の端（例：-200, 0, -200）
  - すぐに目的地に到達できないよう、障害物や曲がりくねった道を配置します。

## 2. 森の密度と視認性
- **中央部（濃い森）**: Scatter Tool を使って木々を密集させます。
  - AIの視認ロジック（LOS）が実装されているため、木々の間を縫うように逃げることが有効になります。
- **散在する地蔵（Jizo）**: 迷いやすい森の中で、目的地への微かな道標として配置します。
  - 地蔵の向きが祠の方を向いている、といったギミックが効果的です。

## 3. 安全地帯（SafeZone / 祠）
- マップ上に数点、鳥居（Torii Gate）などで囲まれた「安全地帯」を配置します。
- `Hachisha_SafeZone.cs` をアタッチしたトリガー内にいる間、八尺様は接近できなくなります。

## 4. 八尺様の出現
- プロジェクトセットアップツール（Setup All）を実行すると、八尺様はプレイヤーから40m離れた位置に自動配置されます。
- 彼女は NavMesh を使用するため、Terrainに対して必ず **NavMeshのベイク（Bake）** を行ってください。

## 5. 推奨アセット配置フロー（実際のアセット使用版）

### ステップ1: 基本オブジェクトを生成
```
hasshakusama → Setup All
```
プレイヤー・AI・NPC・GameManager・ライティングが一括生成されます。

### ステップ2: Shrine Pack で安全地帯を自動生成
```
hasshakusama → Setup Shrine Zones
```
- **鳥居（Torii）**・**石灯籠（StoneLantern）**・**石畳（PavingStone）** つきの SafeZone が3箇所生成されます。
- `Hachisha_ProjectSetup.cs` の冒頭パス定数が正しいか確認してください。

> [!NOTE]
> パスが間違っている場合は Cube プレースホルダーが表示されます（Console に警告あり）。

### ステップ3: European Forest で森を配置

**方法A — Scatter Tool で一括配置（推奨）**
```
hasshakusama → Scatter European Forest
```
- Oak / Birch / Dead Tree / Bush が自動的にマップ全体に散布されます。
- Shrine Zone 内には木が置かれません（自動回避）。

**方法B — Terrain Paint Trees で手動ペイント**
```
hasshakusama → Register European Forest Trees
```
Terrain に Tree Prototype を登録後、Paint Trees ブラシで密度を調整しながら手描きできます。

### ステップ4: NavMesh を Bake
```
Window → AI → Navigation → Bake
```
木を配置した後に必ず Bake し直してください。

### ステップ5: 動作確認
- **Play** を押して懐中電灯をつけると八尺様の探知範囲が2倍になることを確認。
- Shrine Zone に入ると八尺様が追跡停止・姿を消すことを確認。
