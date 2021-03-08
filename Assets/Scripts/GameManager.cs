using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PPPaintable;
using PPFlyable;

namespace PPGameManager
{
    enum GameState {PLAY, PAUSE, END_GAME};
	public class GameManager : MonoBehaviour
    {
        // Game
        public GameObject m_BrushRed;
        public GameObject m_BrushBlue;
        public GameObject m_player;
        public float m_BrushSize = 0.1f;
        public float m_fireRate = 4.0f;
        public int m_quantityOfBrushToRenderTextures = 20;

        // UI
        public TextMeshProUGUI m_percentageTextBox;
        public TextMeshProUGUI m_countdownBox;
        public TextMeshProUGUI m_VictoryText;

        private List<GameObject> m_paintableObjects = new List<GameObject>();
        private List<GameObject> m_brushes = new List<GameObject>();
        private float m_fireCoolDown = 0.0f;
        private Vector3 m_clicPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        private GameState m_gameState;
        private float m_countdown;
        private int m_round = 0;

        // Start is called before the first frame update
        void Start()
	    {
            m_gameState = GameState.PAUSE;
            m_countdown = 3.0f;
            m_countdownBox.text = $"{formatTimer(m_countdown)}";
            m_player.GetComponent<Flyable>().setFlyghtStatus(false);
            m_player.GetComponent<Flyable>().resetSpeed();
            m_VictoryText.enabled = false;
        }
	
	    // Update is called once per frame
	    void Update()
	    {
            float deltaTime = Time.deltaTime;

            m_countdown -= deltaTime;
            m_countdownBox.text = $"{formatTimer(m_countdown)}";

            // GAMESTATE MANAGEMENT
            if (m_countdown <= 0.0f)
            {
                if(m_gameState == GameState.PAUSE)
                {
                    m_round++;
                    m_gameState = GameState.PLAY;
                    m_countdown = 61.0f;

                    m_player.transform.position = new Vector3(1000.0f, 200.0f, 0.0f);
                    Quaternion rot = Quaternion.identity;
                    rot.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
                    m_player.transform.rotation = rot;

                    m_player.GetComponent<Flyable>().setFlyghtStatus(true);
                    m_player.GetComponent<Flyable>().resetSpeed();
                }
                else if(m_gameState == GameState.PLAY)
                {
                    m_player.GetComponent<Flyable>().setFlyghtStatus(false);

                    m_player.transform.position = new Vector3(1000.0f, 500.0f, 500.0f);
                    Quaternion rot = Quaternion.identity;
                    rot.eulerAngles = new Vector3(20.0f, -120.0f, 0.0f);
                    m_player.transform.rotation = rot;

                    if (m_round == 4)
                    {
                        m_gameState = GameState.END_GAME;
                        m_countdown = 5.0f;
                        m_VictoryText.enabled = true;
                    }
                    else
                    {
                        m_gameState = GameState.PAUSE;
                        m_countdown = 2.0f;
                    }

                    DrawNewTextures();
                }
                else if(m_gameState == GameState.END_GAME)
                {
                    m_countdown = 0.0f;
                    Application.Quit(); // does not work in the editor so
                    // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            }

            // PLAY
            if(m_gameState == GameState.PLAY)
            {
                if (m_fireCoolDown > 0)
                {
                    m_fireCoolDown -= deltaTime;
                    return;
                }

                if (Input.GetMouseButton(0))
                {
                    SpawnPaintBrush(m_clicPoint, m_round % 2 == 0 ? m_BrushBlue : m_BrushRed);
                }
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

            CalculateColorPercentages();
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
                if(m_brushes.Count == m_quantityOfBrushToRenderTextures)
                {
                    DrawNewTextures();
                }
            }
        }

        public void CalculateColorPercentages()
        {
            int redPixelCount = 0, bluePixelCount = 0, totalPixelCount = 0;

            foreach (Paintable go in Object.FindObjectsOfType<Paintable>())
            {
                redPixelCount += go.redPixelCount();
                bluePixelCount += go.bluePixelCount();
                totalPixelCount += go.totalPixelCount();
            }

            int redPercentage = Mathf.FloorToInt(100 * ((float)redPixelCount / totalPixelCount));
            int bluePercentage = Mathf.FloorToInt(100 * ((float)bluePixelCount / totalPixelCount));
            m_percentageTextBox.text = $"{redPercentage}% vs {bluePercentage}%";
            m_VictoryText.text = ((redPercentage > bluePercentage) ? "Red" : "Blue") + " won";
        }



        void kill(GameObject go)
        {
            Destroy(go);
        }

        string formatTimer(float timer)
        {
            float seconds = Mathf.RoundToInt(timer % 60.0f);
            string minute = (Mathf.Floor(timer / 60.0f) >= 10) ? "" : "0";
            minute += Mathf.Floor(timer / 60.0f);
            string second = (seconds >= 10) ? seconds.ToString() : "0" + seconds.ToString();
            return minute + ":" + second;
        }
    }

}
