using System.Collections;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    private const float camMaxSize = 5, camMinSize = 4.7f;

    private const float offsetDefaultY = 0.02f, fallOffset = 0.12f;

    [SerializeField] private float offsetY;
    public float OffsetY { get { return offsetY; } set { offsetY = value; } }

    [SerializeField] private Camera cam;

    [SerializeField] private Transform target;
    private Player player;

    [SerializeField] private Vector3 offset;
    [SerializeField] private float offsetDefaultX;
    [SerializeField] private Vector2 speed = Vector2.one;
    [SerializeField] private Rect cameraLimits;

    [SerializeField] private float camSize;

    private void Start()
    {
        cam = gameObject.GetComponent<Camera>();

        target = GameObject.FindWithTag("Player").GetComponent<Transform>();
        player = target.gameObject.GetComponent<Player>();

        offsetDefaultX = offset.x;
        offsetY = offsetDefaultY;
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 newPos;

            if (player.CurrentVelocity.y < -20) speed.y = 0.1f;
            else speed.y = 0.3f;

            if (player.CurrentVelocity.x > 0 || target.rotation.y == 0)
            {
                newPos.x = target.position.x + offset.x;
                offset.x = offsetDefaultX;
            }
            else if (player.CurrentVelocity.x < 0 || target.rotation.y != 0)
            {
                newPos.x = target.position.x - offsetDefaultX;
                offset.x = -offsetDefaultX;
            }
            else
                newPos.x = target.position.x + offset.x;

            if (player.CurrentVelocity.y > 0)
                newPos.y = target.position.y + (target.position.y * offsetY);
            else if (player.CurrentVelocity.y < -15)
                newPos.y = target.position.y - (target.position.y * fallOffset);
            else
                newPos.y = target.position.y + (target.position.y * offsetY);
                
            newPos.z = transform.position.z;

            Vector3 delta = newPos - transform.position;

            newPos.x = transform.position.x + delta.x * Time.deltaTime / speed.x;
            newPos.y = transform.position.y + delta.y * Time.deltaTime / speed.y;

            if (newPos.x > cameraLimits.xMax) newPos.x = cameraLimits.xMax;
            else if (newPos.x < cameraLimits.xMin) newPos.x = cameraLimits.xMin;

            if (newPos.y > cameraLimits.yMax) newPos.y = cameraLimits.yMax;
            else if (newPos.y < cameraLimits.yMin) newPos.y = cameraLimits.yMin;

            transform.position = newPos;
        }
    }

    public IEnumerator Lock()
    {
        camSize = camMaxSize;

        do
        {
            camSize -= 0.005f;
            cam.orthographicSize = camSize;
            
            yield return null;
        }
        while (camSize >= camMinSize);
    }

    public IEnumerator Unlock(float timeToWait = 0)
    {
        if (timeToWait != 0)
            yield return new WaitForSeconds(timeToWait);

        camSize = camMinSize;

        do
        {
            camSize += 0.005f;
            cam.orthographicSize = camSize;

            yield return null;
        }
        while (camSize <= camMaxSize);
    }

    public void OffsetReset() => offsetY = offsetDefaultY;

    private void OnDrawGizmos()
    {
        Camera camera = GetComponent<Camera>();
        float  height = camera.orthographicSize;
        float  width = height * camera.aspect;

        Vector3 p1 = new Vector3(
            cameraLimits.xMin - width, cameraLimits.yMin - height, 0);
        Vector3 p2 = new Vector3(
            cameraLimits.xMax + width, cameraLimits.yMin - height, 0);
        Vector3 p3 = new Vector3(
            cameraLimits.xMax + width, cameraLimits.yMax + height, 0);
        Vector3 p4 = new Vector3(
            cameraLimits.xMin - width, cameraLimits.yMax + height, 0);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }
}
