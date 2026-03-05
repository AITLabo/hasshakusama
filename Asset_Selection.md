# Hachishakusama Unity: Asset Selection

プロジェクトで実際に使用しているアセットのリストです。

---

## 1. 地形・自然 ✅ インポート済み

### European Forest（欧州の森林）
- **用途**: マップ全体を覆う木々・茂みの配置
- **プレハブ格納先 (例)**: `Assets/EuropeanForest/Prefabs/Trees/`
- **種類**:
  - `Tree_Oak_01`    — どんぐりの木（メイン森林）
  - `Tree_Birch_01`  — 白樺（明暗のコントラストに有効）
  - `Tree_Dead_01`   — 枯れ木（恐怖演出に最適）
  - `Bush_01`        — 低木・藪（視線を遮る障害物）

> [!TIP]
> **配置方法**: Unityメニュー `hasshakusama → Scatter European Forest` を実行すると、
> 木々と低木を一括配置します。Shrine Zone 内には自動的に木が置かれません。
>
> **Terrain Paint Trees でも使用可能**: メニュー `hasshakusama → Register European Forest Trees` で
> Terrain の Tree Prototype に自動登録されます。

---

## 2. 建築物・神社 ✅ インポート済み

### Shrine Pack（鳥居・石灯籠・石畳）
- **用途**: 安全地帯（SafeZone）のビジュアル構築
- **プレハブ格納先 (例)**: `Assets/ShrinePackage/Prefabs/`
- **種類**:
  - `Torii`         — 鳥居（安全地帯の入口目印）
  - `StoneLantern`  — 石灯籠（灯籠ライトと組み合わせてポイントライト演出）
  - `PavingStone`   — 石畳（参道の足元）

> [!TIP]
> **配置方法**: メニュー `hasshakusama → Setup Shrine Zones` を実行すると、
> マップ上に3箇所の SafeZone が鳥居・石灯籠・石畳つきで自動生成されます。

> [!IMPORTANT]
> **パスの更新が必要**: `Hachisha_ProjectSetup.cs` と `Hachisha_ScatterObjects.cs` の
> 冒頭にある定数 (`SHRINE_*_PREFAB`, `EURO_*_PREFAB`) を、
> 実際にインポートされたプレハブのパスに書き換えてください。
> パスが間違っている場合は **プレースホルダー Cube** が代わりに配置されます（Console に警告が表示されます）。

---

## 3. キャラクター (Hachishakusama)

- **仮素材**: 白いEmissionを持つ細長いCapsule/Cube（`Hachisha_ProjectSetup` で自動生成）
- **推奨本番素材**: VRoid Studio で白ワンピースモデルを作成 → Unity にエクスポート

---

## 4. VFX & ポストプロセス

詳細は `Horror_VFX_Setup.md` を参照。

- **URP Post-Processing**: Vignette / Film Grain / Chromatic Aberration / Color Adjustments
- **Fog**: `Exponential Squared`, Density `0.05〜0.1`, 暗い紺色
- 八尺様接近時に Chromatic Aberration 強度をスクリプトから上昇させることで警告演出が可能

---

## 5. 音響 (Audio)

- **BGM**: 環境音（風の音・木の葉のざわめき）ループ素材
- **SE**: 「ポポポ」音（`Hachisha_AudioManager` で3D Spatial Soundとして再生）
  - 近づくほどピッチが上がる（距離連動）
