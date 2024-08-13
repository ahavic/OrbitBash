using UnityEngine;
using UnityEditor;
using Scripts.Projectiles;
using SyntaxTree.VisualStudio.Unity.Bridge;

public class ProjectileDesignWindow : EditorWindow
{
    Texture2D headerSectionTexture;
    Texture2D BombSectionTexture;
    Texture2D LaserSectionTexture;
    Texture2D MissileSectionTexture;

    Color headerSectionColor = Color.cyan;
    Color bombSectionColor = Color.white;
    Color laserSectionColor = Color.green;
    Color missileSectionColor = Color.red;

    Rect headerSection;
    Rect bombSection;
    Rect missileSection;
    Rect laserSection;

    static ProjectileData missileData;
    static ProjectileData laserData;
    static BombData bombData;

    public static ProjectileData MissileData { get => missileData; }
    public static ProjectileData LaserData { get => laserData; }
    public static ProjectileData BombData { get => bombData; }

    [MenuItem("Window/Projectile Designer")]
    static void OpenWindow()
    {
        ProjectileDesignWindow window = (ProjectileDesignWindow)GetWindow(typeof(ProjectileDesignWindow));
        window.minSize = new Vector2(600, 300);
        window.Show();
    }

    private void OnEnable()
    {
        InitTextures();
        InitData();
    }

    public static void InitData()
    {
        missileData = (ProjectileData)ScriptableObject.CreateInstance(typeof(ProjectileData));
        laserData = (ProjectileData)ScriptableObject.CreateInstance(typeof(ProjectileData));
        bombData = (BombData)ScriptableObject.CreateInstance(typeof(BombData));
    }

    void InitTextures()
    {
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        BombSectionTexture = new Texture2D(1, 1);
        BombSectionTexture.SetPixel(0, 0, bombSectionColor);
        BombSectionTexture.Apply();

        LaserSectionTexture = new Texture2D(1, 1);
        LaserSectionTexture.SetPixel(0, 0, laserSectionColor);
        LaserSectionTexture.Apply();

        MissileSectionTexture = new Texture2D(1, 1);
        MissileSectionTexture.SetPixel(0, 0, missileSectionColor);
        MissileSectionTexture.Apply();
    }

    private void OnGUI()
    {
        DrawLayouts();
        DrawHeader();
        DrawBombSettings();
        DrawMissileSettings();
        DrawLaserSettings();
        
    }

    void DrawLayouts()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = position.width;
        headerSection.height = 50;

        bombSection.x = 0;
        bombSection.y = 50;
        bombSection.width = position.width / 3f;
        bombSection.height = Screen.height - 50 ;

        laserSection.x = position.width / 3f;
        laserSection.y = 50;
        laserSection.width = position.width / 3f;
        laserSection.height = Screen.height - 50;

        missileSection.x = (position.width / 3f) * 2;
        missileSection.y = 50;
        missileSection.width = position.width / 3f;
        missileSection.height = Screen.height - 50;

        GUI.DrawTexture(headerSection, headerSectionTexture);
        GUI.DrawTexture(bombSection, BombSectionTexture);
        GUI.DrawTexture(laserSection, LaserSectionTexture);
        GUI.DrawTexture(missileSection, MissileSectionTexture);
    }

    void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);

        GUILayout.Label("Projectile Designer");


        GUILayout.EndArea();
    }

    void DrawBombSettings()
    {
        GUILayout.BeginArea(bombSection);

        GUILayout.Label("Bomb");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Damage");
        bombData.damage = (float)EditorGUILayout.FloatField(bombData.damage);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Speed");
        bombData.speed = (float)EditorGUILayout.FloatField(bombData.speed);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Damage");
        bombData.explosion = (GameObject)EditorGUILayout.ObjectField(bombData.explosion, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();


        if (GUILayout.Button("Create", GUILayout.Height(40)))
        {
            GeneralSettings.OpenWindow(GeneralSettings.SettingsType.bomb);
        }

        GUILayout.EndArea();
    }

    void DrawMissileSettings()
    {
        GUILayout.BeginArea(missileSection);

        GUILayout.Label("Missile");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Damage");
        missileData.damage = (float)EditorGUILayout.FloatField(missileData.damage);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Speed");
        missileData.speed = (float)EditorGUILayout.FloatField(missileData.speed);
        EditorGUILayout.EndHorizontal();


        if (GUILayout.Button("Create", GUILayout.Height(40)))
        {
            GeneralSettings.OpenWindow(GeneralSettings.SettingsType.missile);
        }

        GUILayout.EndArea();
    }

    void DrawLaserSettings()
    {
        GUILayout.BeginArea(laserSection);

        GUILayout.Label("Laser");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Damage");
        laserData.damage = (float)EditorGUILayout.FloatField(laserData.damage);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Speed");
        laserData.speed = (float)EditorGUILayout.FloatField(laserData.speed);
        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Create", GUILayout.Height(40)))
        {
            GeneralSettings.OpenWindow(GeneralSettings.SettingsType.laser);
        }

        GUILayout.EndArea();
    }
}

public class GeneralSettings : EditorWindow
{
    public enum SettingsType
    {
        missile,
        laser,
        bomb
    }

    static SettingsType dataSetting;
    static GeneralSettings window;

    public static void OpenWindow(SettingsType setting)
    {
        dataSetting = setting;
        window = (GeneralSettings)GetWindow(typeof(GeneralSettings));
        window.minSize = new Vector2(250, 200);
        window.Show();
    }
}