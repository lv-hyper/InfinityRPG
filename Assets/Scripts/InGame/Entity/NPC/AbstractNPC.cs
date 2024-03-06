using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;



#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShaderGraph;
#endif

namespace InGame.Entity
{
    [Serializable]
    public abstract class AbstractPathElement{
        [SerializeField] protected float time;
        public float GetTime(){return time;}
        public abstract IEnumerator Action(AbstractNPC npc);
    }

    [Serializable]
    public class MoveToPositionElement : AbstractPathElement{
        [SerializeField] Vector3 position;
        public Vector3 GetPosition(){return position;}
        public override IEnumerator Action(AbstractNPC npc)
        {
            npc.GetComponent<Animator>().SetBool("isIdle",false);

            float eta = time;
            
            Vector3 startPos = npc.transform.position;
            
            while(eta > 0)
            {
                yield return new WaitUntil(()=>{return !npc.isPaused;});

                eta-=Time.deltaTime;
                if(eta < 0) eta=0;

                Vector3 newPosition = Vector3.Lerp(startPos, position, (time-eta)/time);
                Vector3 diff = newPosition - npc.transform.position;

                npc.GetComponent<Animator>().SetFloat("deltaX", diff.x);
                npc.GetComponent<Animator>().SetFloat("deltaY", diff.y);
                npc.GetComponent<Animator>().SetFloat("speed", Vector3.Magnitude(diff) * (1.0f/Time.deltaTime));

                npc.transform.Translate(diff);
                
                yield return null;
            }
        }
    }

    [Serializable]
    public class WaitElement : AbstractPathElement{
        public override  IEnumerator Action(AbstractNPC npc)
        {
            npc.GetComponent<Animator>().SetBool("isIdle",true);

            yield return new WaitForSeconds(time);
        }
    }

    public abstract class AbstractNPC : MonoBehaviour
    {
        [SerializeReference] List<AbstractPathElement> path;
        public bool isPaused{get; private set;}
        
        public abstract void OnAction();
        public abstract void OnFinishAction();

        void Awake()
        {
            isPaused = false;
            StartCoroutine(MovePath());
        }

        IEnumerator MovePath()
        {
            while(true)
            {
                foreach(var pathElement in path)
                {
                    yield return pathElement.Action(this);
                }
                yield return null;
            }
        }

        public void PauseMovement(){
            if(GetComponent<Animator>() != null)
            {
                GetComponent<Animator>().SetBool("isIdle",true);
                GetComponent<Animator>().SetFloat("speed", 0);
            }
            isPaused = true;
        }

        public void UnpauseMovement(){
            if(GetComponent<Animator>() != null)
            {
                GetComponent<Animator>().SetBool("isIdle",false);
            }
            isPaused = false;
        }

        public List<AbstractPathElement> GetPath(){return path;}

        public void AddAbstractPathElement(AbstractPathElement element){path.Add(element);}
    }

    #if UNITY_EDITOR

    [CustomEditor(typeof(AbstractNPC), true)]
    public class NPCEditor : Editor{
        public override void OnInspectorGUI()
        {

            AbstractNPC npc = (AbstractNPC)target;

            if(GUILayout.Button("Add Move to Position"))
            {
                npc.AddAbstractPathElement(new MoveToPositionElement());
            }
            
            if(GUILayout.Button("Add Wait"))
            {
                npc.AddAbstractPathElement(new WaitElement());
            }
            base.OnInspectorGUI();
        }

        void OnSceneGUI()
        {

            AbstractNPC npc = (AbstractNPC)target;

            if(npc == null || npc.GetPath().Count==0) return;

            var filteredPath = npc.GetPath().Where(x=>x.GetType() == typeof(MoveToPositionElement)).ToList();
            if(filteredPath.Count==0) return;


            for(int i=0; i<=filteredPath.Count; ++i)
            {
                MoveToPositionElement moveToPositionElement = null;

                if(i!=filteredPath.Count)
                    moveToPositionElement = (MoveToPositionElement)filteredPath[i];
                else
                    moveToPositionElement = (MoveToPositionElement)filteredPath[0];

                Vector3 from, to;

                if(i==0)
                {
                    from = npc.transform.position;
                }
                else
                {
                    from = ((MoveToPositionElement)filteredPath[i-1]).GetPosition();
                }

                if(i==filteredPath.Count)
                {
                    to = ((MoveToPositionElement)filteredPath[0]).GetPosition();
                }
                else
                {
                    to = ((MoveToPositionElement)filteredPath[i]).GetPosition();
                }
                
                Handles.color = Color.green;

                Handles.DrawLine( from, to );

                var directionVector = (to-from).normalized / 10;

                Handles.DrawAAConvexPolygon(new Vector3[3]{
                    to + directionVector,
                    to + Quaternion.Euler(0,0,120) * directionVector,
                    to + Quaternion.Euler(0,0,-120) * directionVector
                });
                
                if(moveToPositionElement != null)
                    Handles.Label(Vector3.Lerp(from, to, 0.9f)+(Vector3.down/10), moveToPositionElement.GetTime()+"s");

            }
        }
    }

    #endif
}
