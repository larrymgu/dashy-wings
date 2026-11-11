using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Connect the score sound after compilation without discarding unsaved scene work.
[InitializeOnLoad]
public static class ScoreSoundSetup
{
    private const string SetupKey = "MousePlatformer.ScoreSoundSetup.v1";

    static ScoreSoundSetup()
    {
        EditorApplication.delayCall += Configure;
    }

    [MenuItem("Tools/Mouse Platformer/Assign Score Sound")]
    private static void Configure()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(
            "Assets/All Of The Lights (mp3cut.net).mp3");
        if (clip == null) return;

        bool configured = false;
        foreach (ShipController ship in Object.FindObjectsByType<ShipController>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!ship.gameObject.scene.IsValid() || !ship.gameObject.scene.isLoaded)
                continue;

            GameSounds sounds = ship.GetComponent<GameSounds>();
            if (SessionState.GetBool(SetupKey, false) && sounds != null &&
                sounds.enemyDefeatedSound == clip) continue;

            if (sounds == null)
                sounds = Undo.AddComponent<GameSounds>(ship.gameObject);
            Undo.RecordObject(sounds, "Assign score sound");
            sounds.enemyDefeatedSound = clip;
            AudioSource source = ship.GetComponent<AudioSource>();
            Undo.RecordObject(source, "Configure score audio");
            source.playOnAwake = false;
            source.loop = false;
            source.clip = null;
            source.spatialBlend = 0f;
            EditorUtility.SetDirty(sounds);
            EditorUtility.SetDirty(source);
            EditorSceneManager.MarkSceneDirty(ship.gameObject.scene);
            configured = true;
        }

        if (configured)
        {
            SessionState.SetBool(SetupKey, true);
            Debug.Log("Score MP3 assigned to ship. Save the scene to keep the assignment.");
        }
    }
}
