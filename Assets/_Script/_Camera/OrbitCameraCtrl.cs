using UnityEngine;
using System.Collections;


public class OrbitCameraCtrl : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

    static Vector3[] camDetectOffsets = new Vector3[5] {
        new Vector3 (  0.0f,  0.0f, 0.0f ),
        new Vector3 (  2.0f,  0.0f, 0.0f ),
        new Vector3 ( -2.0f,  0.0f, 0.0f ),
        new Vector3 (  0.0f,  2.0f, 0.0f ),
        new Vector3 (  0.0f, -2.0f, 0.0f ),
    };
    public static OrbitCameraCtrl instance;

    ///////////////////////////////////////////////////////////////////////////////
    // serialize
    ///////////////////////////////////////////////////////////////////////////////

    public bool allowRotate = true;
    public bool allowZoom = true;
    public bool allowYaw = true;
    public bool allowPitch = true;

    public float moveDampingDuration = 0.1f;
    public float rotDampingDuration = 0.1f;
    public float zoomDamping = 10.0f;
    public float zoomInDamping = 20.0f;
    public float zoomOutDamping = 2.0f;
    public float maxDistance = 10.0f;
    public float minDistance = 2.0f;

    public bool acceptInput = false;

    public Transform traceTarget;
    public Transform cameraAnchor;
    public Vector3 offset = Vector3.zero;
    public Camera mainCamera;

    [System.NonSerialized]
    public Animation cameraAnim = null;


    ///////////////////////////////////////////////////////////////////////////////
    // non-serialize
    ///////////////////////////////////////////////////////////////////////////////

    float cameraRotSide;
    float cameraRotUp;
    float curZoomDamping = 10.0f;
    bool collided = false;
    float distanceNoCollision;
    float distanceWithCollision;


    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    void Awake() {
        instance = this;
    }

    void Start() {
        cameraAnim = cameraAnchor.animation;
        transform.localPosition = GetLookAtPoint();
        transform.localRotation = mainCamera.transform.rotation;
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        cameraAnchor.transform.localPosition = new Vector3(0, 0, -1 * distance);
        mainCamera.transform.parent = cameraAnchor.transform;
        mainCamera.transform.localPosition = Vector3.zero;
        mainCamera.transform.localRotation = Quaternion.identity;

        cameraRotUp = transform.eulerAngles.x;
        cameraRotSide = transform.eulerAngles.y;
        distanceNoCollision = -cameraAnchor.localPosition.z;
    }


    ///////////////////////////////////////////////////////////////////////////////
    // public function
    ///////////////////////////////////////////////////////////////////////////////

    public Vector3 GetLookAtPoint() { return traceTarget.position + offset; }

    public void Apply() {
        transform.position = GetLookAtPoint();
        transform.rotation = Quaternion.Euler(cameraRotUp, cameraRotSide, 0);
        cameraAnchor.localPosition = -Vector3.forward * distanceNoCollision;

        curCameraRotUpVel = 0.0f;
        curCameraRotSideVel = 0.0f;
        curXVel = 0.0f;
        curYVel = 0.0f;
        curZVel = 0.0f;
        curZoomVel = 0.0f;
    }

    public void ShakeCamera(float _startInSeconds) {
        StartCoroutine("ShakeCamera_CO", new ShakeCameraParam(_startInSeconds));
    }

    class ShakeCameraParam
    {
        public AnimationClip shakeClip;
        public float startInSeconds;

        public ShakeCameraParam(float _startInSeconds) {
            startInSeconds = _startInSeconds;
        }
    }

    IEnumerator ShakeCamera_CO(ShakeCameraParam _params) {
        yield return new WaitForSeconds(_params.startInSeconds);

        //AnimationState state = cameraAnim["shake"];
        //if (state != null) {
        //    cameraAnim.Stop();
        //    AnimationClip oldClip = state.clip;
        //    cameraAnim.RemoveClip("shake");
        //    DestroyImmediate(oldClip);
        //}
        //cameraAnim.AddClip(_params.shakeClip, "shake");
        AnimationState animState = cameraAnim["shake"];
        animState.blendMode = AnimationBlendMode.Additive;
        animState.wrapMode = WrapMode.Once;
        cameraAnim.Play("shake");
    }

    //public void SetCameraRotUpByPos(Vector3 _pos) {
    //    Vector3 lookatPoint = GetLookAtPoint();
    //    Vector3 delta = lookatPoint - _pos;

    //    //
    //    Vector3 dir = delta;
    //    dir.Normalize();

    //    Quaternion quat = Quaternion.identity;
    //    quat.SetLookRotation(dir);
    //    Vector3 eulerAngles = quat.eulerAngles;

    //    cameraRotUp = eulerAngles.x;
    //    if (eulerAngles.x >= 180.0f)
    //        cameraRotUp -= 360.0f;
    //}

    //public void MoveTo(Vector3 _pos) {
    //    Vector3 lookatPoint = GetLookAtPoint();
    //    Vector3 delta = lookatPoint - _pos;

    //    //
    //    Vector3 dir = delta;
    //    dir.Normalize();

    //    Quaternion quat = Quaternion.identity;
    //    quat.SetLookRotation(dir);
    //    Vector3 eulerAngles = quat.eulerAngles;
    //    cameraRotSide = eulerAngles.y;

    //    cameraRotUp = eulerAngles.x;
    //    if (eulerAngles.x >= 180.0f)
    //        cameraRotUp -= 360.0f;

    //    distanceNoCollision = delta.magnitude;
    //}

    //public void SetCamera(float _cameraRotUp, float _cameraRotSide, float _distance) {
    //    cameraRotUp = _cameraRotUp;
    //    cameraRotSide = _cameraRotSide;
    //    distanceNoCollision = Mathf.Clamp(_distance, minDistance, maxDistance);
    //}



    void Update() {
        HandleInput();
        UpdateTransform();
    }

    void HandleInput() {
        if (acceptInput) {
            if (Input.GetMouseButton(1) && allowRotate) {
                float axisX = Input.GetAxis("Mouse X") * 5;
                float axisY = Input.GetAxis("Mouse Y") * 5;
                if (allowYaw)
                    cameraRotSide += axisX;
                if (allowPitch)
                    cameraRotUp -= axisY;
            }

            if (cameraRotUp >= 180.0f)
                cameraRotUp -= 360.0f;

            if (allowPitch)
                cameraRotUp = Mathf.Clamp(cameraRotUp, -50.0f, 80.0f);

            float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(zoomDelta) >= 0.01f && allowZoom) {
                curZoomDamping = 0.3f;
                if (collided) {
                    if (zoomDelta > 0.0f) {
                        if (collided) {
                            distanceNoCollision = distanceWithCollision;
                            distanceNoCollision *= (1.0f - zoomDelta);
                            distanceWithCollision = distanceNoCollision;
                        }
                    }
                } else {
                    distanceNoCollision *= (1.0f - zoomDelta);
                }
                distanceNoCollision = Mathf.Clamp(distanceNoCollision, minDistance, maxDistance);
            }
        }
    }

    float curCameraRotUpVel = 0.0f;
    float curCameraRotSideVel = 0.0f;
    float curXVel = 0.0f;
    float curYVel = 0.0f;
    float curZVel = 0.0f;
    float curZoomVel = 0.0f;

    void UpdateTransform() {

        Vector3 eulerAngles = transform.eulerAngles;
        if (float.IsNaN(curCameraRotUpVel)) curCameraRotUpVel = 0.0f;
        if (float.IsNaN(curCameraRotSideVel)) curCameraRotSideVel = 0.0f;

        float newCameraRotUp = Mathf.SmoothDampAngle(eulerAngles.x, cameraRotUp, ref curCameraRotUpVel, rotDampingDuration);
        float newCameraRotSide = Mathf.SmoothDampAngle(eulerAngles.y, cameraRotSide, ref curCameraRotSideVel, rotDampingDuration);
        float distance = distanceNoCollision;

        if (traceTarget) {
            Vector3 cameraSrcPos = transform.position - transform.forward * distanceNoCollision;
            Vector3 lookatPoint = GetLookAtPoint();

            bool hasCollision = false;
            for (int i = 0; i < camDetectOffsets.Length; ++i) {
                Vector3 camOffset = camDetectOffsets[i];

                Vector3 camOffsetPos = cameraSrcPos + cameraAnchor.rotation * camOffset;
                Vector3 targetToCamera = camOffsetPos - lookatPoint;

                RaycastHit hit;
                if (Physics.Raycast(lookatPoint, targetToCamera.normalized, out hit, distanceNoCollision, LayerManager.CameraColliderMask)) {
                    if (collided == false) {
                        collided = true;
                        curZoomDamping = 0.1f;
                        curZoomVel = 0.0f;
                    }
                    distanceWithCollision = (lookatPoint - hit.point).magnitude;
                    if (distanceWithCollision < distance) {
                        distance = distanceWithCollision;
                    }
                    hasCollision = true;
                }
            }

            if (hasCollision == false) {
                if (collided) {
                    collided = false;
                    curZoomDamping = 1.0f;
                    curZoomVel = 0.0f;
                }
            }

            transform.position = new Vector3(
                Mathf.SmoothDamp(transform.position.x, lookatPoint.x, ref curXVel, moveDampingDuration),
                Mathf.SmoothDamp(transform.position.y, lookatPoint.y, ref curYVel, moveDampingDuration),
                Mathf.SmoothDamp(transform.position.z, lookatPoint.z, ref curZVel, moveDampingDuration)
            );
        }
        transform.rotation = Quaternion.Euler(newCameraRotUp, newCameraRotSide, 0);

        float dist = Mathf.SmoothDamp(-cameraAnchor.transform.localPosition.z, distance, ref curZoomVel, curZoomDamping);
        cameraAnchor.localPosition = -Vector3.forward * dist;
    }
}

