# Hachishakusama Unity: Asset Selection

プロジェクトの雰囲気を決定づける推奨アセットリストです。

## 1. 地形・自然 (Forest & Terrain)
- **Unity Starter Assets (First Person)**: プレイヤーコントローラーのベースとして（現在のスクリプトで代用可能ですが、モデルが必要な場合に便利）。
- **Textures.com (Standard Materials)**: 岩、泥、濡れた地面のテクスチャ。
- **Free Trees (SpeedTree or Sample Assets)**: 枯れ木や針葉樹などの不気味な森用アセット。

## 2. 建築物 (Shrines & Temples)
- **Japanese Shrine Assets**: Asset Storeで「Japanese」「Shrine」で検索される無料アセット。
- **Low Poly Japanese Temple**: 軽量で扱いやすいお堂モデル。

## 3. キャラクター (Hachishakusama)
- **Custom Model**: 
  - 白いワンピースを模したシンプルな人型モデル（VRoid Studio等で作成しエクスポート可能）。
  - 仮素材としては、白いEmission（発光）を持たせた細長いCube/Capsuleにしています。

## 4. エフェクト (VFX & Post-Processing)
- **URP Post-Processing**:
  - `Vignette` (周辺減光): 視界を狭め、圧迫感を出します。
  - `Film Grain` (ノイズ): 古いフィルムのような質感を出し、ホラー感を高めます。
  - `Chromatic Aberration` (色収差): 逃走時などに揺らすと効果的です。

## 5. 音響 (Audio)
- **BGM/SE**:
  - 風の音、木の葉のざわめき（ループ素材）。
  - 「ポポポ」という八尺様固有の音（自作または合成音が必要）。

---

> [!TIP]
> アセットをインポートする際は、`_Prefabs` や `_Textures` フォルダに整理して格納してください。
