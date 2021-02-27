﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PPPaintable
{

    public class Paintable : MonoBehaviour
    {
        public Camera m_cam;

        private RenderTexture m_RenderTexture;
        private Texture2D m_texture2D;
        private Material m_material;

        // Use this for initialization
        void Start()
        {
            m_cam.aspect = 1;
            m_RenderTexture = new RenderTexture(m_cam.pixelWidth, m_cam.pixelHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            m_RenderTexture.antiAliasing = 1;

            m_cam.targetTexture = m_RenderTexture;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DrawNewTextures()
        {
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
        }
    }
}

