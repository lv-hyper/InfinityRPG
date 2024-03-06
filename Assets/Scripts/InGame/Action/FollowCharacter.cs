using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace InGame.Action
{
    public class FollowCharacter : MonoBehaviour
    {
        [SerializeField] GameObject character;
        //[SerializeField] Tilemap tilemap;
        [SerializeField] float speed;

        bool funcionality = false;

        // Update is called once per frame
        void Update()
        {
            if (funcionality)
            {
                MoveCamera();
            }
        }

        void MoveCamera()
        {
            /*Vector3 v = transform.position - character.transform.position;
            
            Vector2 cameraMin =
            GetComponent<Camera>().ViewportToWorldPoint(GetComponent<Camera>().rect.min);

            Vector2 cameraMax =
            GetComponent<Camera>().ViewportToWorldPoint(GetComponent<Camera>().rect.max);

            // cameraMin += new Vector2(transform.position.x, transform.position.y);
            // cameraMax += new Vector2(transform.position.x, transform.position.y);

            if (v.sqrMagnitude >= 1)
            {
                v.Normalize();
                v.z = 0;
            }
            else v = Vector3.zero;

            //Vector2 tmpV;
            //tmpV.x = v.x * Time.deltaTime * speed;
            //tmpV.y = v.y * Time.deltaTime * speed;

            /*
            if (tilemap.localBounds.min.x > (cameraMin.x - tmpV.x) || (cameraMax.x - tmpV.x) > tilemap.localBounds.max.x)
            {
                v.x = 0;
            }
            if (tilemap.localBounds.min.y > (cameraMin.y - tmpV.y) || (cameraMax.y - tmpV.y) > tilemap.localBounds.max.y)
            {
                v.y = 0;
            }
            */

            //transform.Translate(-v * Time.deltaTime * speed);

            var newPosition = Vector3.Lerp(transform.position, character.transform.position, Time.deltaTime * speed);
            newPosition.z = transform.position.z;

            transform.position = newPosition;
        }

        void ResetCameraPos()
        {
            ResetCameraPos(Vector3.zero);
        }

        void ResetCameraPos(Vector3 pos)
        {
            Vector3 v = character.transform.position;
            if(pos != Vector3.zero)
            {
                v = pos;
            }
            v.z = transform.position.z;

            Vector2 cameraMin =
            GetComponent<Camera>().ViewportToWorldPoint(GetComponent<Camera>().rect.min);

            Vector2 cameraMax =
            GetComponent<Camera>().ViewportToWorldPoint(GetComponent<Camera>().rect.max);

            Vector2 cameraSize = cameraMax - cameraMin;

            /*
            if (v.x < tilemap.localBounds.min.x + (cameraSize.x/2))
            {
                v.x = tilemap.localBounds.min.x + (cameraSize.x/2);
            }
            else if (v.x > tilemap.localBounds.max.x - (cameraSize.x/2))
            {
                v.x = tilemap.localBounds.max.x - (cameraSize.x/2);
            }

            if (v.y < tilemap.localBounds.min.y + (cameraSize.y/2))
            {
                v.y = tilemap.localBounds.min.y + (cameraSize.y/2);
            }
            else if (v.y > tilemap.localBounds.max.y - (cameraSize.y/2))
            {
                v.y = tilemap.localBounds.max.y - (cameraSize.y/2);
            }
            */

            transform.position = v;
        }

        public void SuspendFunction()
        {
            //tilemap = null;
            funcionality = false;
        }
        public void RestartFunction(Tilemap tilemap, Vector3 pos)
        {
            //this.tilemap = tilemap;
            //this.tilemap.CompressBounds();
            ResetCameraPos(pos);
            
            if(LoadingLayer.GetInstance() != null)
            {
                LoadingLayer.GetInstance().gameObject.SetActive(false);
            }

            funcionality = true;
        }
    }
}
