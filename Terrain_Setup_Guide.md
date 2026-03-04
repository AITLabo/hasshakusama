# Unity Terrain & NavMesh Setup Guide

八尺様プロジェクトの舞台となる「不気味な山道」をUnity上で構築する手順です。

## 1. Terrain（地形）の作成
1. **GameObject -> 3D Object -> Terrain** を選択。
2. **Terrain Settings** (歯車アイコン) で `Terrain Width/Length` を **500 x 500** 程度に設定。
3. **Paint Terrain** ツールを選択し、`Raise or Lower Terrain` で大きな高低差（山）を作る。
   - **Tip**: ブラシの `Opacity` を低めにして、なだらかな坂を作ると歩きやすくなります。
4. **Layers**: `Create Layer` で「土」「草」「岩」のテクスチャを割り当てる。

## 2. NavMesh（AIナビゲーション）の構築
八尺様が地形に沿って追跡するために必須の設定です。

1. **TerrainをStaticにする**: 
   - Inspector上部の `Static` チェックボックスをオン（または `Navigation Static` を選択）。
2. **Navigation ウィンドウを開く**: `Window -> AI -> Navigation`
3. **Bake タブ**:
   - `Agent Radius`: 0.5
   - `Agent Height`: 2.4 (八尺様の高さに合わせる)
   - `Max Slope`: 45
4. **Bake ボタン** を押す。
   - 地形の上に青いメッシュが表示されれば、AIが移動可能な領域として認識されています。

## 3. 八尺様の設定
1. 空の GameObject `Hachisha_AI` を作成。
2. `NavMeshAgent` コンポーネントを追加。
3. 先ほど作成した [Hachisha_StalkingAI.cs](file:///c:/xampp/htdocs/brain/projects/games/Hachishakusama_Unity/_Scripts/Hachisha_StalkingAI.cs) をアタッチ。
4. **Player** スロットに `FirstPersonPlayer` をドラッグ＆ドロップ。

---

> [!IMPORTANT]
> 地形を変更した後は、必ず **NavMesh の再 Bake** を行ってください。そうしないと八尺様が山の斜面で動けなくなります。
