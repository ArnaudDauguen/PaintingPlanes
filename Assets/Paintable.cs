using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Paintable : MonoBehaviour
{

    public GameObject m_BrushRed;
    public GameObject m_BrushBlue;
    public float m_BrushSize = 0.1f;
    public Camera m_cam;

    private RenderTexture m_RenderTexture;
    private Texture2D m_texture2D;
    private Material m_material;
    private List<GameObject> brushes = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        m_RenderTexture = new RenderTexture(m_cam.pixelWidth, m_cam.pixelHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        m_RenderTexture.antiAliasing = 1;

        m_cam.targetTexture = m_RenderTexture;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            //cast a ray to the plane
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(Ray, out hit))
            {
                //instanciate a BrushRed
                var go = Instantiate(m_BrushRed, hit.point + Vector3.up * 0.1f, Quaternion.identity, transform);
                go.transform.localScale = Vector3.one * m_BrushSize;
                brushes.Add(go);
            }

        }
        if (Input.GetMouseButton(1))
        {
            //cast a ray to the plane
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(Ray, out hit))
            {
                //instanciate a BrushBlue
                var go = Instantiate(m_BrushBlue, hit.point + Vector3.up * 0.1f, Quaternion.identity, transform);
                go.transform.localScale = Vector3.one * m_BrushSize;
                brushes.Add(go);
            }

        }
    }

    public void RenderTextures()
    {
        Debug.Log("Rendering");

        //convert rendering texture to texture2D
        m_texture2D = new Texture2D(m_RenderTexture.width, m_RenderTexture.height, TextureFormat.ARGB32, false);

        //set active texture
        m_cam.Render();
        RenderTexture.active = m_RenderTexture;


        //pickup datas and create new texture
        m_texture2D.ReadPixels(new Rect(0, 0, m_RenderTexture.width, m_RenderTexture.height), 0, 0);
        m_texture2D.Apply();
        //create material to allow us tu apply 'Unlit' shader (no lights)
        m_material = new Material(Shader.Find("Unlit/Texture"));
        m_material.mainTexture = (Texture)m_texture2D;

        //apply material
        GetComponent<Renderer>().material = m_material;


        //clear
        brushes.ForEach(kill);
        brushes.Clear();
    }

    /*private IEnumerator CoRenderTextures()
    {
        //wait for rendering
        yield return new WaitForEndOfFrame();
        
    }*/

    void kill(GameObject go)
    {
        Destroy(go);
    }
}

