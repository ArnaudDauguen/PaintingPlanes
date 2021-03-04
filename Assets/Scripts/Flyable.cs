using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PPFlyable
{
	public class Flyable : MonoBehaviour
	{
		[Tooltip("m/s")]
		public float m_currentSpeed = 500.0f;
		[Tooltip("m/s")]
		public float m_maxSpeed = 700.0f;
		[Tooltip("°/s, up/down")]
		public float m_pitchSpeed = 90.0f;
		[Tooltip("°/s vertical left/right")]
		public float m_rollSpeed = 90.0f;
		public TextMeshProUGUI m_TMP_speed;
		public TextMeshProUGUI m_TMP_heigh;

	    // Start is called before the first frame update
	    void Start()
	    {
			
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			float deltatime = Time.deltaTime;


			// Rotate
			float pitch = Input.GetAxis("Vertical") * m_pitchSpeed * deltatime;
			float roll = Input.GetAxis("Horizontal") * m_rollSpeed * deltatime;
			if(pitch != 0.0f || roll != 0.0f)
            {
				this.gameObject.transform.Rotate(new Vector3(pitch, 0, roll));
            }

			// Move
			this.gameObject.transform.Translate(new Vector3(0, 0, m_currentSpeed * deltatime));

			// Update HUD
			this.m_TMP_speed.text = $"{this.m_currentSpeed}";
			this.m_TMP_heigh.text = $"{this.gameObject.transform.position.y}";
		}
	}
}
