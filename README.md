# Empty Project to Unity

The project is a template when you use Unity develop.

Now support Unity version is 2022.3.15 LTS ([download 22.3.15f1](https://unity.com/releases/editor/archive#download-archive-2022)).

## Include

The project include:

- Resources
- Prefabs
  - AlertPrefab
  - ProgressBarPrefab
- Scenes
  - Main: all user interface in there, use `*Panel` assembled, suggest use `UIManager` control show/hide.
- Scripts
  - GameManager: control user/config data and related logical
  - UIManager: control user interface when to global logical
  - SoundManager: control music and effect sound
  - ADManager: control advertisement logical

and `.gitignore` / `README.md` file, set manager "Script Execution Order".

## Dependence

- [穿山甲广告/CSJ Advertisement](https://www.csjplatform.com/) using 5.6.0.0 version.
- [Lean Localization/精益本地化](https://carloswilkes.com/Documentation/LeanLocalization) using 2.0.3 version.

## TODO list

- SoundManager not active.
- ADManager not active.
- Prefab Text must replace use TMP_Text (WTH of Unity UI upgrade...more and more).