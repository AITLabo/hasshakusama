# 八尺様：ホラー環境演出（Atmosphere）ガイド [NEW]

Unity上で「八尺様」の恐怖を引き立てる、不気味な環境（夜の森）を構築する手順です。

## 1. ライティング（Lighting）
1. **Directional Light** を選択し、`Intensity` を **0.1** 程度に下げる。
2. `Color` を **濃い紺色（#000B22）** に設定。
3. `Shadow Type` を **Soft Shadows** に設定。

## 2. スカイボックス（Skybox）
1. **Window -> Rendering -> Lighting** を開く。
2. **Environment** タブを選択。
3. `Skybox Material` に、デフォルトの素材ではなく **深夜の空（暗いグレーや黒）** の素材を割り当てる。
   - 素材がない場合は、新規 Material を作成し `Shader: Skybox/Procedural` で `Atmosphere Thickness` を上げるか `Exposure` を下げて暗くします。

## 3. フォグ（Fog）
不気味な距離感を演出するために必須です。

1. **Lighting ウィンドウ -> Environment** タブの下部にある **Fog** にチェック。
2. `Color` を **黒（#000000）** または **深い霧のグレー** に設定。
3. `Mode` を **Exponential Squared** に。
4. `Density` を **0.05 〜 0.1** 程度に調整。（視界が制限されるくらいがベストです）

## 4. プレイヤーの懐中電灯（Flashlight）
1. **FirstPersonPlayer -> Main Camera** の子オブジェクトとして **Spot Light** を追加。
2. `Range` を **20**, `Spot Angle` を **40** 程度に設定。
3. [Hachisha_PlayerController.cs](file:///c:/xampp/htdocs/brain/projects/games/Hachishakusama_Unity/_Scripts/Hachisha_PlayerController.cs) がアタッチされている場合、この Spot Light を `flashlight` スロットに割り当てることで FキーでON/OFFできるようになります。

---

> [!TIP]
> **Post-Processing** (URP使用時) で **Vignette** (周囲を暗くする) や **Film Grain** (ノイズ) を加えると、さらにホラー映画のような雰囲気になります。
