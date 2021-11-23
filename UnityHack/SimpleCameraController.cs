using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHack
{
	public class SimpleCameraController : MonoBehaviour
	{
        [SerializeField] private float m_MoveSpeed = 10;
        [SerializeField] private float m_RotateSpeed = 2;

        private int m_SpeedModifier = 5;

        private Vector3 m_Eulers = Vector3.zero;

        private string m_HorizontalAxisName = "Debug Horizontal";
        private string m_VerticalAxisName = "Debug Vertical";

        private string m_MouseHAxisName = "MouseX";
        private string m_MouseVAxisName = "MouseY";

        private void Update()
        {
            UpdadePosition();
            UpdateRotation();
        }

        private void UpdadePosition()
        {
            Vector3 translation = Vector3.zero;
            
            translation.x = Input.GetAxis(m_HorizontalAxisName);
            translation.z = Input.GetAxis(m_VerticalAxisName);

            float speed = Input.GetKey(KeyCode.RightShift) ? m_SpeedModifier * m_MoveSpeed : m_MoveSpeed;

            //transform.Translate(translation * m_MoveSpeed * Time.deltaTime);

            Vector3 dir = transform.TransformDirection(translation);
            transform.position += dir.normalized * speed * Time.deltaTime;
        }

        private void UpdateRotation()
        {
            m_Eulers.x -= Input.GetAxis(m_MouseVAxisName) * m_RotateSpeed;
            m_Eulers.y += Input.GetAxis(m_MouseHAxisName) * m_RotateSpeed;

            m_Eulers.x = Mathf.Clamp(m_Eulers.x, -90, 90);
            m_Eulers.y %= 360;

            transform.rotation = Quaternion.Euler(m_Eulers);
        }

        public void SetEulers(Vector3 eulers)
		{
            m_Eulers = eulers;
        }

        public void SetEulers(float x, float y)
		{
            SetEulers(new Vector3(x, y, 0));
		}

        private void OnGUI()
		{
            GUILayout.Box(transform.position.ToString());
            GUILayout.Box(transform.eulerAngles.ToString());
        }
    }
}
