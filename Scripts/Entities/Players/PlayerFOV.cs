using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Entities.Players
{
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;
    }
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;
    }
    [Serializable]
    public struct FOVInfo
    {
        [Range(0, 360)] public float viewAngle;
        public float viewRadius;
        public LayerMask _enemyMask;
        public LayerMask _obstacleMask;
    }
    public class PlayerFOV : MonoBehaviour
    {
        public float _enemyFindDelay;
        public float _meshResolution;
        public int _edgeResolveIterations;
        public float _edgeDistanceThreshold;

        public List<Transform> visibleTargets = new();
        public FOVInfo[] fovInfos;
        public Vector3 DirFromAngle(float degree, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
                degree += transform.eulerAngles.y;
            float rad = degree * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
        }
        private List<MeshFilter> _viewMeshFilter;
        private Mesh[] _viewMesh;

        private void Awake()
        {
            // 구현에 따라 ViewVisual은 없어도 된다.
            _viewMeshFilter = new List<MeshFilter>(fovInfos.Length);
            foreach (Transform item in transform)
            {
                _viewMeshFilter.Add(item.GetComponent<MeshFilter>());
            }
            _viewMesh = new Mesh[_viewMeshFilter.Count];
            for (int i = 0; i < _viewMeshFilter.Count; i++)
            {
                _viewMesh[i] = new Mesh();
                _viewMesh[i].name = "View Mesh";
                _viewMeshFilter[i].mesh = _viewMesh[i];
            }
        }

        private void Start()
        {
        }
        private void LateUpdate()
        {
            for (int i = 0; i < fovInfos.Length; i++)
            {

                DrawFieldOfView(fovInfos[i], _viewMesh[i]);
            }
        }
        private EdgeInfo FindEdge(FOVInfo fovInfo, ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;

            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < _edgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) * 0.5f;
                var castInfo = ViewCast(fovInfo, angle);

                bool edgeDistanceThresholdExceeded =
                    Mathf.Abs(minViewCast.distance - castInfo.distance) > _edgeDistanceThreshold;
                if (castInfo.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = castInfo.point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = castInfo.point;
                }
            }
            return new EdgeInfo { pointA = minPoint, pointB = maxPoint };
        }

        private void DrawFieldOfView(FOVInfo fovInfo, Mesh mesh)
        {
            int stepCount = Mathf.RoundToInt(fovInfo.viewAngle * _meshResolution);
            float stepAngleSize = fovInfo.viewAngle / stepCount;
            Vector3 pos = transform.position;
            List<Vector3> viewPoints = new List<Vector3>();
            var oldViewCastInfo = new ViewCastInfo();
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - fovInfo.viewAngle * 0.5f + stepAngleSize * i;
                //Debug.DrawLine(pos, pos + DirFromAngle(angle, true) * viewRadius, Color.red);
                var info = ViewCast(fovInfo, angle);
                if (i > 0)
                {
                    bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCastInfo.distance - info.distance) > _edgeDistanceThreshold;
                    if (oldViewCastInfo.hit != info.hit || (oldViewCastInfo.hit && info.hit && edgeDistanceThresholdExceeded))
                    {
                        var edge = FindEdge(fovInfo, oldViewCastInfo, info);
                        if (edge.pointA != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointA);
                        }
                        if (edge.pointB != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointB);
                        }
                    }
                }
                oldViewCastInfo = info;
                viewPoints.Add(info.point);
            }

            int vertCount = viewPoints.Count + 1; //원점이 하나 더 필요하고
            Vector3[] vertices = new Vector3[vertCount];
            int[] triangles = new int[(vertCount - 2) * 3];  // (정점의 갯수 -2) X 3 가 삼각형의 갯수

            vertices[0] = Vector3.zero; //원점 입력
            for (int i = 0; i < vertCount - 1; i++)
            {
                //정점 넣고
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertCount - 2) // i가 vertexCount -1 인 시점에서는 그릴 필요 없으니까 
                {
                    //트라이앵글 연결
                    int tIndex = i * 3;
                    triangles[tIndex + 0] = 0;
                    triangles[tIndex + 1] = i + 1;
                    triangles[tIndex + 2] = i + 2;
                }
            }
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        private ViewCastInfo ViewCast(FOVInfo fovInfo, float globalAngle)
        {
            Vector3 dir = DirFromAngle(globalAngle, true);
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, fovInfo.viewRadius, fovInfo._obstacleMask))
            {
                return new ViewCastInfo { hit = true, point = hit.point, distance = hit.distance, angle = globalAngle };
            }
            else
            {
                return new ViewCastInfo { hit = false, point = transform.position + dir * fovInfo.viewRadius, distance = fovInfo.viewRadius, angle = globalAngle };
            }
        }

        private IEnumerator FindEnemyWithDelay()
        {
            var time = new WaitForSeconds(_enemyFindDelay);
            while (true)
            {
                yield return time;
                visibleTargets.ForEach(item => before.Add(item));
                visibleTargets.Clear();
                foreach (var item in fovInfos)
                    FindVisibleEnemies(item);
                ClearBefore();
            }
        }
        private Coroutine find;
        public void SetEnable(bool val)
        {
            if (!val)
            {
                visibleTargets.ForEach(item => before.Add(item));
                visibleTargets.Clear();
                ClearBefore();
                if (find != null)
                    StopCoroutine(find);
            }
            else
            {
                find = StartCoroutine(FindEnemyWithDelay());
            }
            gameObject.SetActive(val);
        }
        private void ClearBefore()
        {
            foreach (var enemy in before)
            {
                if (enemy != null && enemy.TryGetComponent(out IFindable findable))
                    if (--findable.sightCount == 0)
                        findable.Escape();
            }
            before.Clear();
        }

        private HashSet<Transform> before = new();
        private void FindVisibleEnemies(FOVInfo fovInfo)
        {
            Collider[] enemiesInView = new Collider[6];
            int cnt = Physics.OverlapSphereNonAlloc(transform.position, fovInfo.viewRadius, enemiesInView, fovInfo._enemyMask);
            for (int i = 0; i < cnt; i++)
            {
                Transform enemy = enemiesInView[i].transform;
                if (visibleTargets.Contains(enemy))
                    return;
                Vector3 dir = enemy.position - transform.position;
                Vector3 dirToEnemy = dir.normalized;
                if (Vector3.Angle(transform.forward, dirToEnemy) < fovInfo.viewAngle * 0.5f)
                {
                    if (!Physics.Raycast(transform.position, dirToEnemy, dir.magnitude, fovInfo._obstacleMask))
                    {
                        visibleTargets.Add(enemy);
                        if (!before.Contains(enemy) && enemy.TryGetComponent(out IFindable findable))
                        {
                            if (++findable.sightCount == 1)
                                findable.Founded();
                        }
                        else if (before.Contains(enemy))
                            before.Remove(enemy);
                    }
                }
            }
        }
    }
}
