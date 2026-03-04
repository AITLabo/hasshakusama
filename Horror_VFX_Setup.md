# Hachishakusama Unity: Horror VFX Setup

不気味な雰囲気を最大限に引き出すための、URP Post-Processing（ポストプロセス）の設定手順です。

## 1. Post-Processing の基本設定
1. **Global Volume** を作成: `GameObject -> Volume -> Global Volume`
2. **Profile** を新規作成 (`New` ボタンを押す)。
3. **Add Override** で以下の効果を追加します。

## 2. 推奨される効果 (Overrides)

### A. Vignette (周辺減光)
- **State**: `All` をチェック。
- **Intensity**: `0.35` 程度。
- **Smoothness**: `0.5` 程度。
- **効果**: 画面の隅を暗くし、プレイヤーの視線を中央に集中させるとともに、閉塞感を演出します。

### B. Film Grain (フィルムノイズ)
- **Type**: `Medium / Large`
- **Intensity**: `0.15` 程度。
- **効果**: 画面にザラつきを加え、低照度環境でのリアリティと恐怖感を高めます。

### C. Chromatic Aberration (色収差)
- **Intensity**: 通常時は `0.1` 程度、逃走時などは `0.5` 以上。
- **効果**: 画面の端で色が滲む効果です。精神的な不安定さや焦りを表現するのに最適です。

### D. Color Adjustments
- **Post Exposure**: `-0.5` (少し暗くする)。
- **Contrast**: `20` (コントラストを強めて影を深くする)。

## 3. Fog (霧) の設定
1. **Window -> Rendering -> Lighting** を開く。
2. **Environment** タブの `Fog` にチェック。
3. **Color**: 暗い紺色または黒に近い色へ。
4. **Mode**: `Exponential Squared`
5. **Density**: `0.05` 〜 `0.1` (視界が数メートル先までになるように調整)。

---

> [!TIP]
> 八尺様が近づいたときに、スクリプトから Chromatic Aberration の強度を上げることで、プレイヤーへの警告（精神的ダメージ表現）として機能させることができます。
