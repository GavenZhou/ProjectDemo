using UnityEngine;

public static class LayerManager {

    static int defaultMask = 1 << LayerMask.NameToLayer("Default");
    static int transparentFxMask = 1 << LayerMask.NameToLayer("TransparentFX");
    static int ignoreMask = 1 << LayerMask.NameToLayer("Ignore Raycast");

    static int groundMask = 1 << LayerMask.NameToLayer("Ground");
    static int wallMask = 1 << LayerMask.NameToLayer("Wall");
    static int obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");
    static int interactiveMask = 1 << LayerMask.NameToLayer("Interactive");
    static int mobMask = 1 << LayerMask.NameToLayer("Mob");
    static int playerMask = 1 << LayerMask.NameToLayer("Player");
    static int sfxMask = 1 << LayerMask.NameToLayer("SFX");

    public static int CameraColliderMask {
        get { return groundMask | wallMask; }
    }

    public static int SceneColliderObjMask {
        get { return obstacleMask; }
    }

    public static int MobMask {
        get { return mobMask; }
    }

    public static int GroundMask {
        get { return groundMask; }
    }

    public static int ObstacleTest {
        get { return wallMask | obstacleMask; }
    }
}
