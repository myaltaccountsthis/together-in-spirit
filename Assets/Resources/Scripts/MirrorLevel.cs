using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MirrorLevel : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform mirrorFolder, wallFolder;
    public GameObject wall;

    private new EdgeCollider2D collider;
    private Vector3Int origin;
    private Vector3Int target;
    [SerializeField] private Vector3Int initialDirection;
    private Mirror[,] mirrorMatrix;
    private bool[,] impasssable;
    private Vector3Int bottomLeft, topRight;
    private Vector3Int boundsSize;

    private Vector3Int Vector3ToVector3IntFloored(Vector3 vector) {
        return new(Mathf.RoundToInt(vector.x - .5f), Mathf.RoundToInt(vector.y - .5f));
    }

    void Awake() {
        collider = lineRenderer.GetComponent<EdgeCollider2D>();
        Vector3 originPos = transform.Find("Origin").position;
        Vector3 targetPos = transform.Find("Target").position;
        // Find bounds
        bottomLeft = Vector3Int.Min(Vector3ToVector3IntFloored(originPos), Vector3ToVector3IntFloored(targetPos));
        topRight = Vector3Int.Max(Vector3ToVector3IntFloored(originPos), Vector3ToVector3IntFloored(targetPos));
        foreach (Transform transform in mirrorFolder) {
            Vector3Int localPos = Vector3ToVector3IntFloored(transform.position);
            bottomLeft = Vector3Int.Min(bottomLeft, localPos);
            topRight = Vector3Int.Max(topRight, localPos);
        }
        foreach (Transform transform in wallFolder) {
            Vector3Int localPos = Vector3ToVector3IntFloored(transform.position);
            bottomLeft = Vector3Int.Min(bottomLeft, localPos);
            topRight = Vector3Int.Max(topRight, localPos);
        }
        topRight = new(topRight.x + 1, topRight.y + 1);
        boundsSize = new(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
        // Initialize origin, mirror matrix
        origin = Vector3ToVector3IntFloored(originPos - bottomLeft);
        target = Vector3ToVector3IntFloored(targetPos - bottomLeft);
        mirrorMatrix = new Mirror[boundsSize.x, boundsSize.y];
        foreach (Transform mirrorTransform in mirrorFolder) {
            Mirror mirror = mirrorTransform.GetComponent<Mirror>();
            mirror.lineReflector = this;
            Vector3Int localPos = GetLocalPosition(Vector3ToVector3IntFloored(mirrorTransform.position));
            mirrorMatrix[localPos.x, localPos.y] = mirror;
        }
        impasssable = new bool[boundsSize.x, boundsSize.y];
        foreach (Transform wall in wallFolder) {
            Vector3Int localPos = GetLocalPosition(Vector3ToVector3IntFloored(wall.position));
            impasssable[localPos.x, localPos.y] = true;
        }
    }

    void Start() {
        UpdateLine();
    }

    private Vector3Int GetLocalPosition(Vector3Int position) {
        return position - bottomLeft;
    }

    private void ChangeDirection(ref Vector3Int direction, bool rotated) {
        if (rotated)
            direction = new(direction.y, direction.x);
        else
            direction = new(-direction.y, -direction.x);
    }

    /// <summary>
    /// Checks if a local position is in bounds
    /// </summary>
    private bool InBounds(Vector3Int position) {
        return position.x >= 0 && position.y >= 0 && position.x < boundsSize.x && position.y < boundsSize.y;
    }

    public void UpdateLine() {
        List<Vector3Int> localPoints = new() { origin };
        Vector3Int direction = initialDirection;
        Vector3Int prevPoint = localPoints[0];
        bool reachedTarget = false;
        // Generate the key points on the path
        while (InBounds(prevPoint) && !impasssable[prevPoint.x, prevPoint.y]) {
            if (prevPoint.Equals(target)) {
                reachedTarget = true;
                break;
            }
            Mirror mirror = mirrorMatrix[prevPoint.x, prevPoint.y];
            if (mirror != null) {
                ChangeDirection(ref direction, mirror.Rotated);
                localPoints.Add(prevPoint);
            }
            prevPoint += direction;
        }
        localPoints.Add(prevPoint);
        List<Vector2> pointsList = localPoints.ConvertAll(point => new Vector2(point.x + .5f + bottomLeft.x - transform.position.x, point.y + .5f + bottomLeft.y - transform.position.y));
        Vector3[] points = pointsList.Select(point => (Vector3) point).ToArray();
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
        collider.SetPoints(pointsList);
        wall.SetActive(!reachedTarget);
    }
}
