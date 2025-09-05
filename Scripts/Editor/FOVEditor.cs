using Scripts.Entities.Players;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    [CustomEditor(typeof(PlayerFOV))]
    public class FOVEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var pFOV = (PlayerFOV)target;
            var pos = pFOV.transform.position;
            foreach (var item in pFOV.fovInfos)
            {

                Handles.color = Color.white;
                Handles.DrawWireArc(pos, Vector3.up, Vector3.forward, 360f, item.viewRadius);
                Vector3 viewAngleA = pFOV.DirFromAngle(-item.viewAngle * 0.5f, false);
                Vector3 viewAngleB = pFOV.DirFromAngle(item.viewAngle * 0.5f, false);

                Handles.DrawLine(pos, pos + viewAngleA * item.viewRadius);
                Handles.DrawLine(pos, pos + viewAngleB * item.viewRadius);
                Handles.color = Color.red;

                foreach (var trm in pFOV.visibleTargets)
                {
                    Handles.DrawLine(pos, trm.position);
                }
            }

        }
    }

}
