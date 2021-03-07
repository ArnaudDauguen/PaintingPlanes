using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PPFlyable
{
	public class Flyable : MonoBehaviour
	{
		[Tooltip("m/s")]
		public float m_currentSpeed = 150.0f;
		[Tooltip("m/s")]
		public float m_maxSpeed = 700.0f;
		[Tooltip("°/s, up/down")]
		public float m_pitchSpeed = 90.0f;
		[Tooltip("°/s vertical left/right")]
		public float m_rollSpeed = 180.0f;
		public TextMeshProUGUI m_TMP_speed;
		public TextMeshProUGUI m_TMP_heigh;
		public TextMeshProUGUI m_TMP_enginePower;

		private float m_currentEnginePowerPercentage = 1.0f;

		private float m_fakeGravityFactor = 10.0f;
		// private float m_airVolumicMass = 1.225f;

		public float m_mass = 8000.0f;
		public float m_enginePower = 40300.0f;
		public float m_wingSurface = 23.0f;
		// [Tooltip("Should be between 0.3 and 3.0")]
		// public float m_liftCoefficient = 0.3f;
		// [Tooltip("Should be between 0.3 and 0.7")]
		// public float m_trailCoefficient = 0.3f;

	    // Start is called before the first frame update
	    void Start()
	    {
			
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			float deltatime = Time.deltaTime;

			// Rotations
			float pitch = Input.GetAxis("Vertical") * m_pitchSpeed * deltatime;
			float roll = Input.GetAxis("Horizontal") * m_rollSpeed * deltatime;
			if(pitch != 0.0f || roll != 0.0f)
            {
				gameObject.transform.Rotate(new Vector3(pitch, 0, roll));
            }


			// Engine Power management
			float percentageChange = Input.GetAxis("Mouse ScrollWheel");
			m_currentEnginePowerPercentage = Mathf.Clamp(m_currentEnginePowerPercentage + percentageChange, 0.0f, 1.0f);


			// Acceleration
			float angle = Mathf.Sin(-this.gameObject.transform.rotation.eulerAngles.x * (Mathf.PI / 180.0f));
			float acceleration = m_currentEnginePowerPercentage * m_enginePower / m_mass;
			acceleration += (angle > 0) // apply percentage of fakeGravity according to plane's angle
				? -2 * angle * m_fakeGravityFactor
				: (-angle) * m_fakeGravityFactor; 
			acceleration *= deltatime;
			m_currentSpeed += acceleration;


			//Other forces
			// float relativeWind = Mathf.Pow(m_currentSpeed + 0.0f, 2.0f); //TODO Add wind strength in speed direction to currentSpeed and square
			// Vector3 traction = this.gameObject.transform.rotation.eulerAngles * m_enginePower * m_currentEnginePowerPercentage;
			// 40kN
			// float trail = -0.5f * m_airVolumicMass * relativeWind * m_wingSurface * m_trailCoefficient;
			// 7kN
			// Vector3 weigh = new Vector3(0, -9.8f * m_mass, 0);
			// 80kN
			// float lift = 0.5f * m_airVolumicMass * relativeWind * m_wingSurface * m_liftCoefficient;
			// 36kN


			// Move
			gameObject.transform.Translate(new Vector3(0, 0, m_currentSpeed * deltatime));


			// Update HUD
			m_TMP_speed.text = $"{Mathf.RoundToInt(m_currentSpeed * 3.6f)}";
			m_TMP_heigh.text = $"{Mathf.RoundToInt(gameObject.transform.position.y)}";
			m_TMP_enginePower.text = $"{Mathf.RoundToInt(100 * m_currentEnginePowerPercentage)}";
		}
	}
}
