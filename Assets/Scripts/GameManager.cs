using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PPPaintable;

namespace PPGameManager
{
	public class GameManager : MonoBehaviour
    {
        public GameObject m_BrushRed;
        public GameObject m_BrushBlue;
        public float m_BrushSize = 0.1f;
        public float m_fireRate = 4.0f;
        public int m_quantityOfBrushToRenderTextures = 20;

        private List<GameObject> m_paintableObjects = new List<GameObject>();
        private List<GameObject> m_brushes = new List<GameObject>();
        private float m_fireCoolDown = 0.0f;
        private Vector3 m_clicPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
            if(m_fireCoolDown > 0)
            {
                m_fireCoolDown -= Time.deltaTime;
                return;
            }

            if (Input.GetMouseButton(0))
            {
                SpawnPaintBrush(m_clicPoint, m_BrushRed);
            }
            else if (Input.GetMouseButton(1))
            {
                SpawnPaintBrush(m_clicPoint, m_BrushBlue);
            }
        }

        public void DrawNewTextures()
        {
            foreach (Paintable go in Object.FindObjectsOfType<Paintable>())
            {
                go.DrawNewTextures();
            }

            //clear
            m_brushes.ForEach(kill);
            m_brushes.Clear();
        }

        void SpawnPaintBrush(Vector3 clicPoint, GameObject brush)
        {
            // cast a ray to something
            Ray Ray = Camera.main.ScreenPointToRay(clicPoint);
            RaycastHit hit;
            // If hit something, instanciate a Brush
            if (Physics.Raycast(Ray, out hit))
            {
                GameObject go = Instantiate(brush, hit.point + hit.normal * 0.01f, Quaternion.identity, transform);
                // Rotate brush to hit surface
                go.transform.up = hit.normal;
                go.transform.localScale = Vector3.one * m_BrushSize;
                m_brushes.Add(go);

                // Reset fire cooldown
                m_fireCoolDown = 1.0f / m_fireRate;

                // Render Textures if too many
                if(m_brushes.Count >= m_quantityOfBrushToRenderTextures)
                {
                    DrawNewTextures();
                }
            }
        }



        void kill(GameObject go)
        {
            Destroy(go);
        }
    }
}
